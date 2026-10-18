using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;
using GameScrubsV2.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using static GameScrubsV2.Endpoints.Placement.PlacementEndpoints.SetPlacementScoreResult;

namespace GameScrubsV2.Endpoints.Placement;

public static partial class PlacementEndpoints
{
	private const string IsDouble = "double";

	/// <summary>
	/// Grand finals reset match slots: the winners bracket player (r1) against the losers bracket player (r2).
	/// </summary>
	private const string ResetPlayer1 = "r1";

	private const string ResetPlayer2 = "r2";

	public static void SetPlacementScore(this RouteGroupBuilder group) =>
		group.MapPost("/score/{placementId:int}/{lockCode?}", async (
				[FromRoute] int bracketId,
				[FromRoute] int placementId,
				[FromRoute] int? lockCode,
				[FromServices] GameScrubsV2DbContext dbContext,
				[FromServices] IBracketHubService bracketHubService,
				BracketRepository bracketRepository,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("PlacementEndpoints");
				logger.LogDebug("Setting placement score for placement id {PlacementId}", placementId);

				var winner = await dbContext.Placements
					.FirstOrDefaultAsync(placement => placement.Id == placementId, cancellationToken);

				if (winner is null)
				{
					return Results.NotFound(new MessageResponse("Placement not found"));
				}

				var bracket = await bracketRepository.GetByIdAsync(bracketId, cancellationToken);

				if (bracket is null)
				{
					return Results.NotFound(new MessageResponse("Bracket not found"));
				}

				if (bracket.LockCode is not null && bracket.LockCode != lockCode)
				{
					return Results.BadRequest(new MessageResponse("Invalid lock code, lock code must match"));
				}

				if (bracket.Status != BracketStatus.Started)
				{
					return Results.BadRequest(new MessageResponse("Bracket is not in started status"));
				}

				if (winner.BracketId != bracketId)
				{
					logger.LogError("Placement {PlacementId} does not belong to bracket {BracketId}", placementId, bracketId);
					return Results.BadRequest(new MessageResponse("Placement does not belong to bracket"));
				}

				var isDouble = bracket.Type
					.ToString()
					.ToLower()
					.Contains(IsDouble);

				var bracketPositions = await dbContext.BracketPositions
					.Where(position => position.Type == bracket.Type)
					.ToListAsync(cancellationToken);

				if (bracketPositions.Count == 0)
				{
					return Results.InternalServerError(new MessageResponse("Bracket positions not found"));
				}

				var position = bracketPositions
					.SingleOrDefault(position => position.Player1 == winner.BracketPlace || position.Player2 == winner.BracketPlace);

				if (position == null)
				{
					return Results.Ok();
				}

				var isPlayer1 = position.Player1 == winner.BracketPlace;

				var resetPosition = bracketPositions.SingleOrDefault(bracketPosition => bracketPosition.Player1 == ResetPlayer1);

				var isGrandFinalsWithReset = bracket.GrandFinalReset
											&& resetPosition is not null
											&& position != resetPosition
											&& position.WinLocation == resetPosition.WinLocation;

				if (isGrandFinalsWithReset)
				{
					var response = await SetGrandFinalsWinner(
						dbContext,
						winner,
						bracket,
						position,
						isPlayer1,
						logger,
						cancellationToken);

					if (response != Success)
					{
						return Results.InternalServerError(new MessageResponse(response.ToString()));
					}

					await bracketHubService.NotifyMatchScoreUpdated(bracketId, winner.Id);

					return Results.NoContent();
				}

				var isEdit = await dbContext.Placements
					.AnyAsync(placement => placement.BracketId == bracket.Id &&
										   placement.BracketPlace == position.WinLocation,
					cancellationToken);

				var winnerScore = await dbContext.BracketScores
					.SingleOrDefaultAsync(score => score.BracketPlace == position.WinLocation
												   && score.Type == bracket.Type, cancellationToken)
								  ?? new BracketScore { BracketPlace = position.WinLocation, Score = 0 };

				var loserScore = await dbContext.BracketScores
					.SingleOrDefaultAsync(score => score.BracketPlace == position.LoseLocation
												   && score.Type == bracket.Type, cancellationToken)
								 ?? new BracketScore { BracketPlace = position.LoseLocation, Score = 0 };

				if (!isEdit)
				{
					var response = await InsertPlacement(
						dbContext,
						winner,
						bracket,
						isDouble,
						position,
						isPlayer1,
						winnerScore,
						loserScore,
						logger,
						cancellationToken);

					if (response != Success)
					{
						return Results.InternalServerError(new MessageResponse(response.ToString()));
					}
				}
				else
				{
					var response = await UpdatePlacement(
						dbContext,
						winner,
						bracket,
						isDouble,
						position,
						isPlayer1,
						winnerScore,
						loserScore,
						logger,
						cancellationToken);

					if (response == DatabaseError)
					{
						return Results.InternalServerError(new MessageResponse(response.ToString()));
					}
				}

				await bracketHubService.NotifyMatchScoreUpdated(bracketId, winner.Id);

				return Results.NoContent();

			}).WithName("SetPlacementScore")
			.AllowAnonymous();

	private static async Task<SetPlacementScoreResult> InsertPlacement(
		GameScrubsV2DbContext dbContext,
		Models.Placement winner,
		Models.Bracket bracket,
		bool isDouble,
		BracketPosition position,
		bool isPlayer1,
		BracketScore winnerScore,
		BracketScore loserScore,
		ILogger logger,
		CancellationToken cancellationToken)
	{
		try
		{
			winner.Status = PlacementStatus.Winner;

			dbContext.Placements.Add(new Models.Placement
			{
				BracketId = bracket.Id,
				PlayerName = winner.PlayerName,
				BracketPlace = position.WinLocation,
				Score = winnerScore.Score,
				Status = PlacementStatus.Default,
				PreviousBracketPlace = winner.BracketPlace,
				IsTop = isPlayer1,
			});

			var loser = await dbContext.Placements
				.FirstOrDefaultAsync(placement => placement.BracketId == bracket.Id && placement.BracketPlace == (isPlayer1
					                                   ? position.Player2
					                                   : position.Player1), cancellationToken);

			// Update existing status
			loser?.Status = PlacementStatus.Loser;

			if (isDouble && !string.IsNullOrEmpty(position.LoseLocation))
			{
				// Create new placement
				dbContext.Placements.Add(new Models.Placement
				{
					BracketId = bracket.Id,
					PlayerName = loser!.PlayerName,
					BracketPlace = position.LoseLocation,
					Score = loserScore.Score,
					Status = PlacementStatus.Default,
					PreviousBracketPlace = loser.BracketPlace,
				});
			}

			await dbContext.SaveChangesAsync(cancellationToken);

			return Success;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error inserting placement for bracket {BracketId}", bracket.Id);
			return DatabaseError;
		}
	}

	private static async Task<SetPlacementScoreResult> UpdatePlacement(
		GameScrubsV2DbContext dbContext,
		Models.Placement winner,
		Models.Bracket bracket,
		bool isDouble,
		BracketPosition position,
		bool isPlayer1,
		BracketScore winnerScore,
		BracketScore loserScore,
		ILogger logger,
		CancellationToken cancellationToken)
	{
		winner.Status = PlacementStatus.Winner;
		var winnerExistingPlacement = await dbContext.Placements
			.SingleOrDefaultAsync(placement => placement.BracketId == bracket.Id
											   && placement.BracketPlace == position.WinLocation,
				cancellationToken);

		if (winnerExistingPlacement is null)
		{
			logger.LogError("Winner placement not found for bracket position {@BracketPosition}", position);
			return MissingWinnerPlacement;
		}

		winnerExistingPlacement.PlayerName = winner.PlayerName;
		winnerExistingPlacement.Score = winnerScore.Score;
		winnerExistingPlacement.Status = PlacementStatus.Winner;
		winnerExistingPlacement.PreviousBracketPlace = winner.BracketPlace;
		winnerExistingPlacement.IsTop = isPlayer1;

		var loser = await dbContext.Placements
			.SingleAsync(placement => placement.BracketId == bracket.Id
			                                   && placement.BracketPlace == (isPlayer1
				                                   ? position.Player2
				                                   : position.Player1),
				cancellationToken);


		loser.Status = PlacementStatus.Loser;

		if (isDouble)
		{


			var loserExistingPlacement = await dbContext.Placements
				.SingleOrDefaultAsync(placement => placement.BracketId == bracket.Id
												   && placement.BracketPlace == position.LoseLocation,
					cancellationToken);

			if (loserExistingPlacement != null)
			{
				loserExistingPlacement.PlayerName = loser.PlayerName;
				loserExistingPlacement.Score = loserScore.Score;
				loserExistingPlacement.Status = PlacementStatus.Loser;
				loserExistingPlacement.PreviousBracketPlace = loser.BracketPlace;
				loserExistingPlacement.IsTop = false;
			}

		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return Success;
	}

	/// <summary>
	/// Sets the grand finals winner for a bracket with a grand final reset.
	/// If the winners bracket player wins they are the champion. If the losers bracket player wins,
	/// both players have one loss so they move to the reset match, whose winner is the champion.
	/// Anything already decided after the grand finals is cleared, so the result can be changed.
	/// </summary>
	private static async Task<SetPlacementScoreResult> SetGrandFinalsWinner(
		GameScrubsV2DbContext dbContext,
		Models.Placement winner,
		Models.Bracket bracket,
		BracketPosition position,
		bool isPlayer1,
		ILogger logger,
		CancellationToken cancellationToken)
	{
		try
		{
			var loser = await dbContext.Placements
				.SingleOrDefaultAsync(placement => placement.BracketId == bracket.Id
				                                   && placement.BracketPlace == (isPlayer1
					                                   ? position.Player2
					                                   : position.Player1),
					cancellationToken);

			if (loser is null)
			{
				logger.LogError("Loser placement not found for bracket position {@BracketPosition}", position);
				return MissingLoserPlacement;
			}

			winner.Status = PlacementStatus.Winner;
			loser.Status = PlacementStatus.Loser;

			var placementsAfterGrandFinals = await dbContext.Placements
				.Where(placement => placement.BracketId == bracket.Id
				                    && (placement.BracketPlace == position.WinLocation
				                        || placement.BracketPlace == ResetPlayer1
				                        || placement.BracketPlace == ResetPlayer2))
				.ToListAsync(cancellationToken);

			dbContext.Placements.RemoveRange(placementsAfterGrandFinals);

			var scores = await dbContext.BracketScores
				.Where(score => score.Type == bracket.Type
				                && (score.BracketPlace == position.WinLocation
				                    || score.BracketPlace == ResetPlayer1
				                    || score.BracketPlace == ResetPlayer2))
				.ToDictionaryAsync(score => score.BracketPlace!, score => score.Score, cancellationToken);

			if (isPlayer1)
			{
				dbContext.Placements.Add(new Models.Placement
				{
					BracketId = bracket.Id,
					PlayerName = winner.PlayerName,
					BracketPlace = position.WinLocation,
					Score = scores.GetValueOrDefault(position.WinLocation!),
					Status = PlacementStatus.Default,
					PreviousBracketPlace = winner.BracketPlace,
					IsTop = true,
				});
			}
			else
			{
				dbContext.Placements.Add(new Models.Placement
				{
					BracketId = bracket.Id,
					PlayerName = loser.PlayerName,
					BracketPlace = ResetPlayer1,
					Score = scores.GetValueOrDefault(ResetPlayer1),
					Status = PlacementStatus.Default,
					PreviousBracketPlace = loser.BracketPlace,
					IsTop = true,
				});

				dbContext.Placements.Add(new Models.Placement
				{
					BracketId = bracket.Id,
					PlayerName = winner.PlayerName,
					BracketPlace = ResetPlayer2,
					Score = scores.GetValueOrDefault(ResetPlayer2),
					Status = PlacementStatus.Default,
					PreviousBracketPlace = winner.BracketPlace,
					IsTop = false,
				});
			}

			await dbContext.SaveChangesAsync(cancellationToken);

			return Success;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error setting grand finals winner for bracket {BracketId}", bracket.Id);
			return DatabaseError;
		}
	}

	public enum SetPlacementScoreResult
	{
		Success,
		MissingWinnerPlacement,
		MissingLoserPlacement,
		DatabaseError
	}
}
using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using static GameScrubsV2.Endpoints.Placement.PlacementEndpoints.SetPlacementScoreResult;

namespace GameScrubsV2.Endpoints.Placement;

public static partial class PlacementEndpoints
{
	private const string IsDouble = "double";

	public static void SetPlacementScore(this RouteGroupBuilder group) =>
		group.MapPost("/score/{placementId:int}/{lockCode?}", async (
				[FromRoute] int placementId,
				[FromRoute] string? lockCode,
				[FromServices] GameScrubsV2DbContext dbContext,
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

				var bracket = await bracketRepository.GetByIdAsync(winner.BracketId, cancellationToken);

				if (bracket is null)
				{
					return Results.NotFound(new MessageResponse("Bracket not found"));
				}

				if (bracket.Status != BracketStatus.Started)
				{
					return Results.BadRequest(new MessageResponse("Bracket is not in started status"));
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

				var isEdit = await dbContext.Placements
					.AnyAsync(placement => placement.BracketId == bracket.Id &&
					                       placement.BracketPlace == position.WinLocation,
					cancellationToken);

				var isPlayer1 = position.Player1 == winner.BracketPlace;

				var winnerScore = await dbContext.BracketScores
					.SingleOrDefaultAsync(score => score.BracketPlace == position.WinLocation
					                               && score.Type == bracket.Type, cancellationToken)
				                  ?? new BracketScore { BracketPlace = position.WinLocation, Score = 0 };

				var loserScore = await dbContext.BracketScores
					.SingleOrDefaultAsync(score => score.BracketPlace == position.Loselocation
					                               && score.Type == bracket.Type, cancellationToken)
				                 ?? new BracketScore { BracketPlace = position.Loselocation, Score = 0 };

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
		dbContext.Placements.Add(new Models.Placement
		{
			BracketId = bracket.Id,
			PlayerName = winner.PlayerName,
			BracketPlace = position.WinLocation,
			Score = winnerScore.Score,
			Status = PlacementStatus.Winner,
			PreviousBracketPlace = winner.BracketPlace,
			IsTop = isPlayer1,
		});

		if (isDouble && !string.IsNullOrEmpty(position.Loselocation))
		{
			var loser = await dbContext.Placements
				.SingleOrDefaultAsync(placement => placement.BracketPlace != position.WinLocation
				                                   && placement.BracketPlace == (isPlayer1
					                                   ? position.Player2
					                                   : position.Player1),
					cancellationToken);

			if (loser != null)
			{
				dbContext.Placements.Add(new Models.Placement
				{
					BracketId = bracket.Id,
					PlayerName = loser.PlayerName,
					BracketPlace = position.Loselocation,
					Score = loserScore.Score,
					Status = PlacementStatus.Loser,
					PreviousBracketPlace = loser.BracketPlace,
				});
			}
			else
			{
				logger.LogError("Losers bracket placement not found for bracket position {@BracketPosition}", position);
				return MissingLoserPlacement;
			}
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return Success;
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

		if (isDouble)
		{
			var loser = await dbContext.Placements
				.SingleOrDefaultAsync(placement => placement.BracketId == bracket.Id
				                                   && placement.BracketPlace == (isPlayer1
					                                   ? position.Player2
					                                   : position.Player1),
					cancellationToken);

			if (loser is null)
			{
				logger.LogError("Losers placement not found for bracket position {@BracketPosition}", position);
				return MissingLoserPlacement;
			}

			var loserExistingPlacement = await dbContext.Placements
				.SingleOrDefaultAsync(placement => placement.BracketId == bracket.Id
				                                   && placement.BracketPlace == position.Loselocation,
					cancellationToken);

			if (loserExistingPlacement is null)
			{
				logger.LogError("Losers placement not found for bracket position {@BracketPosition}", position);
				return MissingLoserPlacement;
			}

			loserExistingPlacement.PlayerName = loser.PlayerName;
			loserExistingPlacement.Score = loserScore.Score;
			loserExistingPlacement.Status = PlacementStatus.Winner;
			loserExistingPlacement.PreviousBracketPlace = loser.BracketPlace;
			loserExistingPlacement.IsTop = false;
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return Success;
	}

	public enum SetPlacementScoreResult
	{
		Success,
		MissingWinnerPlacement,
		MissingLoserPlacement,
		DatabaseError
	}
}
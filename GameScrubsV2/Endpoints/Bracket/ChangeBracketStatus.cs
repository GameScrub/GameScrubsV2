using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;
using GameScrubsV2.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameScrubsV2.Endpoints.Bracket;

public static partial class BracketEndpoints
{
	public static void ChangeBracketStatus(this RouteGroupBuilder group) =>
		group.MapPost("/change-status/{lockcode?}", async (
				[FromRoute] int? lockCode,
				[FromBody] ChangeBracketStatusRequest request,
				[FromServices] BracketRepository bracketRepository,
				[FromServices] IBracketHubService bracketHubService,
				GameScrubsV2DbContext dbContext,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("BracketEndpoints");

				logger.LogDebug("Changing bracket status with request {@Request}", request);

				try
				{
					var bracket = await bracketRepository.GetByIdAsync(request.BracketId, cancellationToken);

					if (bracket is null)
					{
						return Results.NotFound();
					}

					if (bracket.LockCode is not null && bracket.LockCode != lockCode)
					{
						return Results.BadRequest(new MessageResponse("Invalid lock code, failed to update bracket"));
					}

					var placements = await dbContext.Placements
						.Where(placement => placement.BracketId == bracket.Id)
						.ToListAsync(cancellationToken);

					var players = await dbContext.PlayerLists
						.Where(playerList => playerList.BracketId == bracket.Id)
						.OrderBy(playerList => playerList.Seed)
						.ToListAsync(cancellationToken);

					switch (request.Status)
					{
						case BracketStatus.Setup:
							dbContext.Placements.RemoveRange(placements);
							bracket.Status = request.Status;
							await bracketRepository.UpdateAsync(bracket, cancellationToken);
							break;

						case BracketStatus.Started:
							if (bracket.Placements.Count != 0)
							{
								logger.LogError("Bracket {bracketId} already started", bracket.Id);
								return Results.BadRequest(new MessageResponse("Bracket already started"));
							}

							var positions = await GetStartPositions(dbContext, bracket.Type, cancellationToken);

							for (var seed = 0; seed < positions.Count; seed++)
							{
								if (players.Exists(player => player.Seed == seed))
								{
									dbContext.Placements.Add(new Models.Placement
									{
										BracketPlace = positions.ElementAt(seed).Key,
										Score = positions.ElementAt(seed).Value,
										PlayerName = players.Single(player => player.Seed == seed).PlayerName,
										BracketId = bracket.Id,
										Status = PlacementStatus.Default,
									});
								}
								else
								{
									bracket.Placements.Add(new Models.Placement
									{
										BracketPlace = positions.ElementAt(seed).Key,
										Score = positions.ElementAt(seed).Value,
										PlayerName = Constants.DefaultPlayerName,
										BracketId = bracket.Id,
										Status = PlacementStatus.Default
									});
								}
							}
							await dbContext.SaveChangesAsync(cancellationToken);

							bracket.Status = request.Status;
							await bracketRepository.UpdateAsync(bracket, cancellationToken);

							break;

						case BracketStatus.OnHold:
						case BracketStatus.Completed:
							bracket.Status = request.Status;
							await bracketRepository.UpdateAsync(bracket, cancellationToken);
							break;

						default:
							logger.LogError("Invalid bracket status {@BracketStatus}", request.Status);
							return Results.BadRequest(new MessageResponse("Invalid bracket status"));
					}

					// Notify clients via SignalR that the bracket status has changed
					await bracketHubService.NotifyBracketStatusChanged(bracket.Id, bracket.Status.ToString());

					return Results.Ok(ChangeBracketStatusResponse.ToResponseModel(bracket));
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error updating bracket with request {@BracketRequest}", request);
					return Results.InternalServerError(new MessageResponse("Error updating bracket"));
				}
			})
			.WithName("ChangeBracketStatus")
			.RequireRateLimiting("BracketUpdates")
			.AllowAnonymous();

	private static async Task<Dictionary<string, int>> GetStartPositions(
		GameScrubsV2DbContext dbContext,
		BracketType type,
		CancellationToken cancellationToken)
	{

		var positions = new Dictionary<string, int>();

		var totalPlayers = int.Parse(type.ToString().Split('_').Last());

		for (var i = 1; i <= totalPlayers; i++)
		{
			var position = "w" + i;

			var bracketScore = await dbContext.BracketScores
				.Where(bracketScores => bracketScores.Type == type && bracketScores.BracketPlace == position)
				.SingleOrDefaultAsync(cancellationToken);

			var score = bracketScore?.Score ?? 0;

			positions.Add(position, score);
		}

		return positions;
	}

	public sealed record ChangeBracketStatusRequest
	{
		public required int BracketId { get; init; }
		public required BracketStatus Status { get; init; }
	}

	public sealed record ChangeBracketStatusResponse
	{
		public required int Id { get; init; }
		public required string? Name { get; init; }
		public required string? Game { get; init; }
		public required bool IsLocked { get; init; }
		public required BracketType Type { get; init; }
		public required BracketStatus Status { get; init; }
		public required CompetitionType Competition { get; init; }
		public required DateTime StartDate { get; init; }
		public required DateTime CreatedDate { get; init; }

		public static ChangeBracketStatusResponse ToResponseModel(Models.Bracket data) => new()
		{
			Id = data.Id,
			Name = data.Name,
			Game = data.Game,
			IsLocked = data.IsLocked,
			Type = data.Type,
			Status = data.Status,
			Competition = data.Competition,
			StartDate = data.StartDate,
			CreatedDate = data.CreatedDate
		};
	}
}
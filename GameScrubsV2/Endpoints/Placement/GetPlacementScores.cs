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

	public static void GetPlacementScores(this RouteGroupBuilder group) =>
		group.MapGet("/{bracketId:int}/score", async (
				[FromRoute] int bracketId,
				[FromServices] GameScrubsV2DbContext dbContext,
				BracketRepository bracketRepository,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("PlacementEndpoints");
				logger.LogDebug("Getting placement scores for bracket id {BracketId}", bracketId);

				var playerScores = new Dictionary<string, int>();

				var bracket = await bracketRepository.GetByIdAsync(bracketId, cancellationToken);

				if (bracket is null)
				{
					return Results.NotFound(new MessageResponse("Bracket not found"));
				}

				var placements = await dbContext.Placements
					.Where(placement => placement.BracketId == bracket.Id && placement.Score > 0)
					.OrderBy(placement => placement.Score)
					.ThenByDescending(placement => placement.Id)
					.ToListAsync(cancellationToken);

				foreach (var position in placements.Where(
					         position => !playerScores.ContainsKey(position.PlayerName!)
					                     && position.PlayerName!.ToLower() != Constants.DefaultPlayerName))
				{
					playerScores.Add(position.PlayerName!, position.Score);
				}

				return Results.Ok(playerScores);

			}).WithName("GetPlacementScores")
			.AllowAnonymous();
}
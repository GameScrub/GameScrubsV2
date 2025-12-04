using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameScrubsV2.Endpoints.Placement;

public static partial class PlacementEndpoints
{
	public static void GetBracketPlacement(this RouteGroupBuilder group) =>
		group.MapGet("/{bracketId:int}", async (
				[FromRoute] int bracketId,
				[FromServices] GameScrubsV2DbContext dbContext,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("PlacementEndpoints");
				logger.LogDebug("Getting bracket placements for bracket id {BracketId}", bracketId);

				var result = await dbContext.Placements
					.Where(placement => placement.BracketId == bracketId)
					.ToListAsync(cancellationToken);

				return Results.Ok(result.Select(PlacementResponse.ToResponseModel));

			}).WithName("GetBracketPlacement")
			.AllowAnonymous();

	public sealed record PlacementResponse
	{
		public required int Id { get; init; }
		public required string? PlayerName { get; init; }
		public required string? BracketPlace { get; init; }
		public required int BracketId { get; init; }
		public required int Score { get; init; }
		public required PlacementStatus Status { get; init; }
		public required string? PreviousBracketPlace { get; init; }
		public required bool IsTop { get; init; }

		public static PlacementResponse ToResponseModel(Models.Placement data) => new()
		{
			Id = data.Id,
			BracketId = data.BracketId,
			PlayerName = data.PlayerName,
			Score = data.Score,
			Status = data.Status,
			PreviousBracketPlace = data.PreviousBracketPlace,
			IsTop = data.IsTop,
			BracketPlace = data.BracketPlace
		};
	}
}
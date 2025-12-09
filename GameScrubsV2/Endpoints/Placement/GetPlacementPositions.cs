using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Placement;

public static partial class PlacementEndpoints
{

	public static void GetPlacementPositions(this RouteGroupBuilder group) =>
		group.MapGet("/positions", async (
				[FromRoute] int bracketId,
				[FromServices] GameScrubsV2DbContext dbContext,
				BracketRepository bracketRepository,
				BracketPositionsRepository bracketPositionsRepository,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("PlacementEndpoints");
				logger.LogDebug("Getting placement positions for bracket id {BracketId}", bracketId);

				var bracket = await bracketRepository.GetByIdAsync(bracketId, cancellationToken);

				if (bracket is null)
				{
					return Results.NotFound(new MessageResponse("Bracket not found"));
				}

				var positions = await bracketPositionsRepository.GetByTypeAsync(bracket.Type, cancellationToken);

				if (positions is null)
				{
					return Results.NotFound(new MessageResponse("Bracket positions not found"));
				}

				return Results.Ok(positions.Select(GetPlacementPositionsResponse.ToResponseModel));

			}).WithName("GetPlacementPositions")
			.AllowAnonymous();

	public sealed record GetPlacementPositionsResponse
	{
		public required int Id { get; init; }
		public required BracketType Type { get; init; }
		public required string? Player1 { get; init; }
		public required string? Player2 { get; init; }
		public required string? WinLocation { get; init; }
		public required string? LoseLocation { get; init; }

		public static GetPlacementPositionsResponse ToResponseModel(BracketPosition position) => new()
		{
			Id = position.Id,
			Type = position.Type,
			Player1 = position.Player1,
			Player2 = position.Player2,
			WinLocation = position.WinLocation,
			LoseLocation = position.LoseLocation
		};
	}
}
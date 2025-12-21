using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Bracket;

public static partial class BracketEndpoints
{

	public static void GetBracketPositions(this RouteGroupBuilder group) =>
		group.MapGet("/positions/{bracketType}", async (
				[FromRoute] BracketType bracketType,
				BracketPositionsRepository bracketPositionsRepository,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("PlacementEndpoints");

				logger.LogDebug("Getting bracket positions for bracket type {BracketType}", bracketType);

				var positions = await bracketPositionsRepository.GetByTypeAsync(bracketType, cancellationToken);

				if (positions is null)
				{
					return Results.NotFound(new MessageResponse("Bracket positions not found"));
				}

				return Results.Ok(positions.Select(GetBracketPositionsResponse.ToResponseModel));

			}).WithName("GetBracketPositions")
			.AllowAnonymous();

	public sealed record GetBracketPositionsResponse
	{
		public required int Id { get; init; }
		public required BracketType Type { get; init; }
		public required string? Player1 { get; init; }
		public required string? Player2 { get; init; }
		public required string? WinLocation { get; init; }
		public required string? LoseLocation { get; init; }
		public required int? MarkerPosition { get; init; }

		public static GetBracketPositionsResponse ToResponseModel(BracketPosition position) => new()
		{
			Id = position.Id,
			Type = position.Type,
			Player1 = position.Player1,
			Player2 = position.Player2,
			WinLocation = position.WinLocation,
			LoseLocation = position.LoseLocation,
			MarkerPosition = position.MarkerPosition
		};
	}
}
using GameScrubsV2.Common;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameScrubsV2.Endpoints.Player;

public static partial class PlayerEndpoints
{
	public static void GetAllPlayers(this RouteGroupBuilder group) =>
		group.MapGet("/{bracketId:int}", async (
				[FromRoute] int bracketId,
				[FromServices] BracketRepository bracketRepository,
				GameScrubsV2DbContext dbContext,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
		{
			var logger = loggerFactory.GetLogger("PlayerEndpoints");

			logger.LogDebug("Get all player from specified bracket with request");

			var bracket = await bracketRepository.GetByIdAsync(bracketId, cancellationToken);

			if (bracket is null)
			{
				return Results.NotFound(new MessageResponse("Bracket not found"));
			}

			var players = await dbContext.PlayerLists
				.Where(player => player.BracketId == bracketId)
				.OrderBy(player => player.Seed)
				.Select(player => GetAllPlayersResponse.ToResponseModel(player))
				.ToListAsync(cancellationToken);

			return Results.Ok(players);
		})
		.WithName("GetAllPlayers")
		.AllowAnonymous();

		public sealed record GetAllPlayersResponse
		{
			public required int Id { get; init; }
			public required int BracketId { get; init; }
			public required string? PlayerName { get; init; }
			public required int Seed { get; init; }
			public required int Score { get; init; }

			public static GetAllPlayersResponse ToResponseModel(PlayerList data) => new()
			{
				Id = data.Id,
				PlayerName = data.PlayerName,
				BracketId = data.BracketId,
				Seed = data.Seed,
				Score = data.Score
			};
		}
}
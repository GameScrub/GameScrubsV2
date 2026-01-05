using System.ComponentModel.DataAnnotations;

using GameScrubsV2.Common;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameScrubsV2.Endpoints.Player;

public static partial class PlayerEndpoints
{
	public static void AddPlayer(this RouteGroupBuilder group) =>
		group.MapPost("/{lockcode?}", async (
				[FromRoute] int? lockCode,
				[FromBody] AddPlayerRequest request,
				[FromServices] BracketRepository bracketRepository,
				GameScrubsV2DbContext dbContext,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
		{
			var logger = loggerFactory.GetLogger("BracketEndpoints");

			logger.LogDebug("Add player to bracket with request {@Request}", request);

			var bracket = await bracketRepository.GetByIdAsync(request.BracketId, cancellationToken);

			if (bracket is null)
			{
				return Results.NotFound();
			}

			if (bracket.LockCode is not null && bracket.LockCode != lockCode)
			{
				return Results.BadRequest(new MessageResponse("Invalid lock code, failed to add player to bracket"));
			}

			var playerName = request.PlayerName.ToLower().Trim();

			var players = await dbContext.PlayerLists
				.Where(players => players.BracketId == request.BracketId)
				.ToListAsync(cancellationToken);

			if (players.Any(player => player.PlayerName?.ToLower() == playerName))
			{
				return Results.BadRequest(new MessageResponse("Player already exists in bracket"));
			}

			if (playerName == Constants.DefaultPlayerName)
			{
				return Results.BadRequest(new MessageResponse("Player name cannot be the reserved name of --"));
			}

			var newPlayer = new PlayerList
			{
				BracketId = request.BracketId,
				PlayerName = request.PlayerName.Trim(),
				Score = 0,
				Seed = 0
			};

			if (!int.TryParse(bracket.Type.ToString().Split('_').Last(), out var maxAmountOfPlayers))
			{
				return Results.InternalServerError(
					new MessageResponse($"Error calculating max number of players for bracket type: {bracket.Type}"));
			}

			if (players.Count == maxAmountOfPlayers)
			{
				return Results.BadRequest(new MessageResponse("Bracket is full"));
			}

			for (var seed = 0; seed < maxAmountOfPlayers; seed++)
			{
				if (players.Exists(player => player.Seed == seed))
				{
					continue;
				}

				newPlayer.Seed = seed;
				break;
			}

			dbContext.PlayerLists.Add(newPlayer);

			await dbContext.SaveChangesAsync(cancellationToken);

			return Results.NoContent();
		})
		.WithName("AddPlayer")
		.AllowAnonymous();

	public sealed record AddPlayerRequest
	{
		public required int BracketId { get; init; }

		[Min(1)]
		[MaxLength(25)]
		public required string PlayerName { get; init; }

		[Min(0)]
		public required int Seed { get; init; }
	}
}
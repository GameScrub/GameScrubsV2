using GameScrubsV2.Common;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameScrubsV2.Endpoints.Player;

public static partial class PlayerEndpoints
{
	public static void ReorderPlayers(this RouteGroupBuilder group) =>
		group.MapPost("/reorder/{lockcode?}", async (
				[FromRoute] string? lockCode,
				[FromBody] ReorderPlayersRequest request,
				[FromServices] BracketRepository bracketRepository,
				GameScrubsV2DbContext dbContext,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
		{
			var logger = loggerFactory.GetLogger("PlayerEndpoints");

			logger.LogDebug("Reordering players for bracket id {BracketId}", request.BracketId);

			var bracket = await bracketRepository.GetByIdAsync(request.BracketId, cancellationToken);

			if (bracket is null)
			{
				return Results.NotFound();
			}

			if(!int.TryParse(bracket.Type.ToString().Split('_').Last(), out var maxAmountOfPlayers))
			{
				return Results.InternalServerError(
					new ErrorResponse($"Error calculating max number of players for bracket type: {bracket.Type}"));
			}

			if (request.PlayerIds.Length > maxAmountOfPlayers)
			{
				return Results.BadRequest(new ErrorResponse("Too many players"));
			}

			if (bracket.LockCode is not null && bracket.LockCode != lockCode)
			{
				return Results.BadRequest(new ErrorResponse("Invalid lock code, failed to add player to bracket"));
			}

			var players = await dbContext.PlayerLists
				.Where(player => player.BracketId == request.BracketId &&
				                 request.PlayerIds.AsEnumerable().Contains(player.Id))
				.ToListAsync(cancellationToken);

			if (players.Count != request.PlayerIds.Length)
			{
				return Results.BadRequest(new ErrorResponse("Invalid player ids"));
			}

			for(var i = 0; i < request.PlayerIds.Length; i++)
			{
				if (request.PlayerIds[i] == 0)
				{
					continue;
				}

				var currentPlayer = players.First(player => player.Id == request.PlayerIds[i]);
				currentPlayer.Seed = i + 1;
			}

			await dbContext.SaveChangesAsync(cancellationToken);

			return Results.NoContent();
		})
		.WithName("ReorderPlayers")
		.AllowAnonymous();

	public sealed record ReorderPlayersRequest
	{
		public required int BracketId { get; init; }
		public required int[] PlayerIds { get; init; }
	}
}
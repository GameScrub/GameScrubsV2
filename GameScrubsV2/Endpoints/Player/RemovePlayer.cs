using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameScrubsV2.Endpoints.Player;

public static partial class PlayerEndpoints
{
	public static void RemovePlayer(this RouteGroupBuilder group) =>
		group.MapDelete("/{lockcode?}", async (
				[FromRoute] int? lockCode,
				[FromBody] RemovePlayersRequest request,
				[FromServices] BracketRepository bracketRepository,
				GameScrubsV2DbContext dbContext,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
		{
			var logger = loggerFactory.GetLogger("PlayerEndpoints");

			logger.LogDebug("Removing player for bracket id {BracketId}", request.BracketId);

			var bracket = await bracketRepository.GetByIdAsync(request.BracketId, cancellationToken);

			if (bracket is null)
			{
				return Results.NotFound();
			}

			if (bracket.LockCode is not null && bracket.LockCode != lockCode)
			{
				return Results.BadRequest(new MessageResponse("Invalid lock code, failed to remove player to bracket"));
			}

			if (bracket.Status != BracketStatus.Setup)
			{
				return Results.BadRequest(new MessageResponse("Bracket is not in setup status"));
			}

			var player = await dbContext.PlayerLists
				.FirstOrDefaultAsync(player => player.Id == request.PlayerId, cancellationToken);

			if (player is null)
			{
				return Results.NotFound(new MessageResponse("Player not found"));
			}

			dbContext.PlayerLists.Remove(player);

			await dbContext.SaveChangesAsync(cancellationToken);

			return Results.NoContent();
		})
		.WithName("RemovePlayer")
		.AllowAnonymous();

	public sealed record RemovePlayersRequest
	{
		public required int BracketId { get; init; }
		public required int PlayerId { get; init; }
	}
}
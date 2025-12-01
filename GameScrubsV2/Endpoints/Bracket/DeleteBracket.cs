using System.ComponentModel.DataAnnotations;

using GameScrubsV2.Common;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Bracket;

public static partial class BracketEndpoints
{
	public static void DeleteBracket(this RouteGroupBuilder group) =>
		group.MapDelete("/", async (
				[FromBody] DeleteBracketRequest request,
				[FromServices] BracketRepository bracketRepository,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("BracketEndpoints");

				logger.LogDebug("Deleting bracket with request {@BracketRequest}", request);

				try
				{
					var bracket = await bracketRepository.GetByIdAsync(request.Id, cancellationToken);

					if (bracket is null)
					{
						return Results.NotFound();
					}

					if (bracket.LockCode is not null && bracket.LockCode != request.LockCode)
					{
						return Results.BadRequest(new ErrorResponse("Invalid lock code"));
					}

					await bracketRepository.DeleteAsync(request.Id, cancellationToken);

					return Results.NoContent();
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error updating bracket with request {@BracketRequest}", request);
					return Results.InternalServerError(new ErrorResponse("Error updating bracket"));
				}
			})
			.WithName("DeleteBracket")
			.AllowAnonymous();

	public sealed record DeleteBracketRequest
	{
		[Required]
		public required int Id { get; init; }

		public string? LockCode { get; init; }
	}
}
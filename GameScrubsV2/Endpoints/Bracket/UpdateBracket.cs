using System.ComponentModel.DataAnnotations;

using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Bracket;

public static partial class BracketEndpoints
{
	public static void UpdateBracket(this RouteGroupBuilder group) =>
		group.MapPut("/{lockcode?}", async (
				[FromRoute] int? lockCode,
				[FromBody] UpdateBracketRequest request,
				[FromServices] BracketRepository bracketRepository,
				TimeProvider timeProvider,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("BracketEndpoints");

				logger.LogDebug("Updating bracket with request {@BracketRequest}", request);

				try
				{
					var bracket = await bracketRepository.GetByIdAsync(request.Id, cancellationToken);

					if (bracket is null)
					{
						return Results.NotFound();
					}

					if (bracket.LockCode is not null && bracket.LockCode != lockCode)
					{
						return Results.BadRequest(new MessageResponse("Invalid lock code, failed to update bracket"));
					}

					var startDate = DateTime.Parse(request.StartDate.ToString());

					if (startDate.Date < timeProvider.GetUtcNow().DateTime.Date)
					{
						return Results.BadRequest(new MessageResponse("Start date must be in the future"));
					}

					bracket.Name = request.Name;
					bracket.IsLocked = request.LockCode != null;
					bracket.LockCode = request.LockCode;
					bracket.Type = request.Type;
					bracket.Competition = request.Competition;
					bracket.Game = request.Game;
					bracket.StartDate = startDate;

					await bracketRepository.UpdateAsync(bracket, cancellationToken);

					return Results.Ok(UpdateBracketResponse.ToResponseModel(bracket));
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error updating bracket with request {@BracketRequest}", request);
					return Results.InternalServerError(new MessageResponse("Error updating bracket"));
				}
			})
			.WithName("UpdateBracket")
			.RequireRateLimiting("BracketUpdates")
			.AllowAnonymous();

	public sealed record UpdateBracketRequest
	{
		[Required]
		public required int Id { get; init; }

		[Required]
		[MaxLength(100)]
		[MinLength(5)]
		public required string Name { get; init; }

		[Required]
		[MaxLength(100)]
		[MinLength(5)]
		public required string Game { get; init; }

		[Range(0, 99999)]
		public int? LockCode { get; init; }

		public required BracketType Type { get; init; }
		public required CompetitionType Competition { get; init; }
		public required DateOnly StartDate { get; init; }
	}

	public sealed record UpdateBracketResponse
	{
		public required int Id { get; init; }
		public required string? Name { get; init; }
		public required string? Game { get; init; }
		public required bool IsLocked { get; init; }
		public required BracketType Type { get; init; }
		public required CompetitionType Competition { get; init; }
		public required DateTime StartDate { get; init; }
		public required DateTime CreatedDate { get; init; }

		public static UpdateBracketResponse ToResponseModel(Models.Bracket data) => new()
		{
			Id = data.Id,
			Name = data.Name,
			Game = data.Game,
			IsLocked = data.IsLocked,
			Type = data.Type,
			Competition = data.Competition,
			StartDate = data.StartDate,
			CreatedDate = data.CreatedDate
		};
	}
}
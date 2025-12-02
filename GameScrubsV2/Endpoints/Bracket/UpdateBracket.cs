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
				[FromRoute] string? lockCode,
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
						return Results.BadRequest(new ErrorResponse("Invalid lock code, failed to update bracket"));
					}

					var startDate = DateTime.Parse(request.StartDate.ToString());

					if (startDate.Date < timeProvider.GetUtcNow().DateTime.Date)
					{
						return Results.BadRequest(new ErrorResponse("Start date must be in the future"));
					}

					bracket.Name = request.Name;
					bracket.Url = request.Url;
					bracket.IsLocked = !string.IsNullOrEmpty(request.LockCode);
					bracket.LockCode = request.LockCode;
					bracket.Type = request.Type;
					bracket.Competition = request.Competition;
					bracket.Status = request.Status;
					bracket.Game = request.Game;
					bracket.Email = request.Email;
					bracket.StartDate = startDate;

					await bracketRepository.UpdateAsync(bracket, cancellationToken);

					return Results.Ok(UpdateBracketResponse.ToResponseModel(bracket));
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error updating bracket with request {@BracketRequest}", request);
					return Results.InternalServerError(new ErrorResponse("Error updating bracket"));
				}
			})
			.WithName("UpdateBracket")
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

		[EmailAddress]
		public string? Email { get; init; }

		[Url]
		public string? Url { get; init; }

		public string? LockCode { get; init; }
		public required BracketStatus Status { get; init; }
		public required BracketType Type { get; init; }
		public required CompetitionType Competition { get; init; }
		public required DateOnly StartDate { get; init; }
	}

	public sealed record UpdateBracketResponse
	{
		public required int Id { get; init; }
		public required string? Name { get; init; }
		public required string? Game { get; init; }
		public required string? Url { get; init; }
		public required bool IsLocked { get; init; }
		public required BracketType Type { get; init; }
		public required BracketStatus Status { get; init; }
		public required CompetitionType Competition { get; init; }
		public required DateTime StartDate { get; init; }
		public required DateTime CreatedDate { get; init; }

		public static UpdateBracketResponse ToResponseModel(Models.Bracket data) => new()
		{
			Id = data.Id,
			Name = data.Name,
			Game = data.Game,
			Url = data.Url,
			IsLocked = data.IsLocked,
			Type = data.Type,
			Status = data.Status,
			Competition = data.Competition,
			StartDate = data.StartDate,
			CreatedDate = data.CreatedDate
		};
	}
}
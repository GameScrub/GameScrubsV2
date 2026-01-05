using System.ComponentModel.DataAnnotations;

using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Bracket;

public static partial class BracketEndpoints
{
	public static void CreateBracket(this RouteGroupBuilder group) =>
		group.MapPost("/", async (
				[FromBody] CreateBracketRequest request,
				[FromServices] BracketRepository bracketRepository,
				TimeProvider timeProvider,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("BracketEndpoints");

				logger.LogDebug("Creating new bracket");

				try
				{
					var startDate = DateTime.Parse(request.StartDate.ToString());

					if (startDate.Date < timeProvider.GetUtcNow().DateTime.Date)
					{
						return Results.BadRequest(new MessageResponse("Start date must be in the future"));
					}

					var bracket = new Models.Bracket
					{
						Name = request.Name,
						IsLocked = request.LockCode != null,
						LockCode = request.LockCode,
						Type = request.Type,
						Competition = request.Competition,
						Status = BracketStatus.Setup,
						Game = request.Game,
						StartDate = startDate,
						CreatedDate = timeProvider.GetUtcNow().DateTime
					};

					await bracketRepository.InsertAsync(bracket, cancellationToken);

					return Results.CreatedAtRoute("GetBracketById", new { id = bracket.Id },
						CreateBracketResponse.ToResponseModel(bracket));
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error creating bracket");
					return Results.InternalServerError(new MessageResponse("Error creating bracket"));
				}
			})
			.WithName("CreateBracket")
			.AllowAnonymous();

	public sealed record CreateBracketRequest
	{
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

	public sealed record CreateBracketResponse
	{
		public required int Id { get; init; }
		public required string? Name { get; init; }
		public required string? Game { get; init; }
		public required bool IsLocked { get; init; }
		public required BracketType Type { get; init; }
		public required BracketStatus Status { get; init; }
		public required CompetitionType Competition { get; init; }
		public required DateTime StartDate { get; init; }
		public required DateTime CreatedDate { get; init; }

		public static CreateBracketResponse ToResponseModel(Models.Bracket data) => new()
		{
			Id = data.Id,
			Name = data.Name,
			Game = data.Game,
			IsLocked = data.IsLocked,
			Type = data.Type,
			Status = data.Status,
			Competition = data.Competition,
			StartDate = data.StartDate,
			CreatedDate = data.CreatedDate
		};
	}
}
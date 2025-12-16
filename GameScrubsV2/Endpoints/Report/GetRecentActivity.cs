using GameScrubsV2.Enums;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Report;

public static partial class DashboardEndpoints
{
	public static void GetRecentActivity(this RouteGroupBuilder group) =>
		group.MapGet("/recent-activity", async (
				[FromServices] BracketRepository bracketRepository,
				CancellationToken cancellationToken) =>
			{
				var brackets = await bracketRepository.GetAllAsync(cancellationToken);

				var groupedBrackets = brackets?
					.OrderBy(bracket => bracket.CreatedDate)
					.GroupBy(bracket => bracket.Status)
					.Select(bracket => new { bracket.Key, Brackets = bracket
						.Select(GetBracketByIdResponse.ToResponseModel)
						.Take(3)
						.ToList() });


				return groupedBrackets is null
					? Results.NotFound()
					: Results.Ok(groupedBrackets);
			})
			.WithName("GetRecentActivity")
			.AllowAnonymous();

	public sealed record GetBracketByIdResponse
	{
		public required int Id { get; init; }
		public required string? Name { get; init; }
		public required string? Game { get; init; }
		public required bool IsLocked { get; init; }
		public required BracketType Type { get; init; }
		public required BracketStatus Status { get; init; }
		public required CompetitionType Competition { get; init; }
		public required DateTime StartDate { get; init; }

		public static GetBracketByIdResponse ToResponseModel(Models.Bracket data) => new()
		{
			Id = data.Id,
			Name = data.Name,
			Game = data.Game,
			IsLocked = data.IsLocked,
			Type = data.Type,
			Status = data.Status,
			Competition = data.Competition,
			StartDate = data.StartDate,
		};
	}
}
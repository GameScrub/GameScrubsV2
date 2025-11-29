using GameScrubsV2.Enums;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Bracket;

public static partial class BracketEndpoints
{
	public static void GetBracketById(this RouteGroupBuilder group) =>
		group.MapGet("/{id:int}", async (
				int id,
				[FromServices] BracketRepository bracketRepository,
				CancellationToken cancellationToken) =>
			{
				var bracket = await bracketRepository.GetByIdAsync(id, cancellationToken);

				return bracket is null
					? Results.NotFound()
					: Results.Ok(GetBracketByIdResponse.ToResponseModel(bracket));
			})
			.WithName("GetBracketById")
			.AllowAnonymous();

	public sealed record GetBracketByIdResponse
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

		public static GetBracketByIdResponse ToResponseModel(Models.Bracket data) => new()
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
using GameScrubsV2.Common;
using GameScrubsV2.Enums;
using GameScrubsV2.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Bracket;

public static partial class BracketEndpoints
{
	public static void GetAllBrackets(this RouteGroupBuilder group) =>
		group.MapGet("/", async (
				[FromBody] GetAllBracketsRequest? request,
				[FromServices] BracketRepository bracketRepository,
				ILoggerFactory loggerFactory,
				CancellationToken cancellationToken) =>
			{
				var logger = loggerFactory.GetLogger("BracketEndpoints");
				logger.LogDebug("Getting all brackets");

				var data = await bracketRepository.GetAllAsync(cancellationToken);

				if (data is null)
				{
					return Results.NotFound(new ErrorResponse("No brackets found"));
				}

				var query = data.AsQueryable();

				if (request?.Status is not null)
				{
					query = query.Where(bracket => bracket.Status == request.Status);
				}

				if (request?.Type is not null)
				{
					query = query.Where(bracket => bracket.Type == request.Type);
				}

				if (request?.Competition is not null)
				{
					query = query.Where(bracket => bracket.Competition == request.Competition);
				}

				if (request?.IsLocked is not null)
				{
					query = query.Where(bracket => bracket.IsLocked == request.IsLocked);
				}

				var pagination = request?.GetPagination() ?? Pagination.Empty;

				var result = query
				   .OrderByDescending(bracket => bracket.StartDate)
				   .Paginate(pagination)
				   .ToList()
				   .Select(BracketResponse.ToResponseModel);

				return Results.Ok(result);
			}).WithName("GetAllBrackets")
			.AllowAnonymous();

	public sealed record GetAllBracketsRequest
	{
		[Min(0)]
		public int? PageSize { get; init; }
		[Min(0)]
		public int? PageNumber { get; init; }
		public bool? IsLocked { get; init; }

		public BracketStatus? Status { get; init; }
		public BracketType? Type { get; init; }
		public CompetitionType? Competition { get; init; }
		public string? Name { get; init; }

		public Pagination GetPagination() => new(PageSize ?? 0, PageNumber ?? 0);
	}

	public sealed record GetAllBracketsResponse
	{
		public GetAllBracketsResponse(List<BracketResponse> brackets)
		{
			Brackets = brackets;
		}

		public required IReadOnlyList<BracketResponse> Brackets { get; init; }
		public required int PageNumber { get; init; }
		public required int PageSize { get; init; }
		public required int TotalCount { get; set; }
		public required int TotalPages { get; set; }
		public required bool HasPreviousPage { get; set; }
		public required bool HasNextPage { get; set; }
	}

	public sealed record BracketResponse
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

		public static BracketResponse ToResponseModel(Models.Bracket data) => new()
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
using GameScrubsV2.Enums;
using GameScrubsV2.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GameScrubsV2.Repositories;

public sealed class BracketPositionsRepository
{
	private readonly GameScrubsV2DbContext _dbContext;
	private readonly IMemoryCache _cache;
	private readonly ILogger<BracketPositionsRepository> _logger;

	public BracketPositionsRepository(
		GameScrubsV2DbContext dbContext,
		IMemoryCache cache,
		ILogger<BracketPositionsRepository> logger)
	{
		_dbContext = dbContext;
		_cache = cache;
		_logger = logger;
	}

	private const string CacheKeyPrefix = "bracketPositions";
	private static string GetCacheKey(BracketType bracketType) => $"{CacheKeyPrefix}:{bracketType}";

	public async Task<List<BracketPosition>?> GetByTypeAsync(BracketType bracketType, CancellationToken cancellationToken)
	{
		var positions = await _cache.GetOrCreateAsync(GetCacheKey(bracketType), async _ =>
			await _dbContext.BracketPositions
				.AsNoTracking()
				.Where(dbBracket => dbBracket.Type == bracketType)
				.ToListAsync(cancellationToken));

		if (positions is null)
		{
			_logger.LogError("There is no bracket positions with type {BracketType}", bracketType);
		}

		return positions;
	}

	private void ExpireCache(BracketType bracketType)
		=> _cache.Remove(GetCacheKey(bracketType));
}
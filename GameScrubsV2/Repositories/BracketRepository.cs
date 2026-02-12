using GameScrubsV2.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GameScrubsV2.Repositories;

public sealed class BracketRepository
{
	private readonly GameScrubsV2DbContext _dbContext;
	private readonly IMemoryCache _cache;
	private readonly ILogger<BracketRepository> _logger;

	public BracketRepository(
		GameScrubsV2DbContext dbContext,
		IMemoryCache cache,
		ILogger<BracketRepository> logger)
	{
		_dbContext = dbContext;
		_cache = cache;
		_logger = logger;
	}

	private const string CacheKeyPrefix = "bracket";
	private const string AllCacheKey = $"{CacheKeyPrefix}:all";
	private static string GetCacheKey(int id) => $"{CacheKeyPrefix}:{id}";

	public async Task<IEnumerable<Bracket>?> GetAllAsync(CancellationToken cancellationToken) =>
		await _cache.GetOrCreateAsync(AllCacheKey, async _ => await _dbContext.Brackets
			.AsNoTracking()
			.ToArrayAsync(cancellationToken));

	public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
		(await GetByIdAsync(id, cancellationToken)) is not null;

	public async Task<Bracket?> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		var bracket = await _cache.GetOrCreateAsync(GetCacheKey(id), async _ => await _dbContext.Brackets
				.AsNoTracking()
				.FirstOrDefaultAsync(dbBracket => dbBracket.Id == id, cancellationToken));

		if (bracket is null)
		{
			_logger.LogDebug("There is no bracket with id {Id}", id);
		}

		return bracket;
	}

	public async Task<Bracket> InsertAsync(Bracket bracket, CancellationToken cancellationToken)
	{
		_logger.LogDebug("Inserting bracket {@Bracket}", bracket);

		_dbContext.Brackets.Add(bracket);
		await _dbContext.SaveChangesAsync(cancellationToken);
		_dbContext.Entry(bracket).State = EntityState.Detached;

		ExpireCache();

		return bracket;
	}

	public async Task UpdateAsync(Bracket bracket, CancellationToken cancellationToken)
	{
		_logger.LogDebug("Updating bracket {@Bracket}", bracket);

		_dbContext.Brackets.Update(bracket);
		await _dbContext.SaveChangesAsync(cancellationToken);
		_dbContext.Entry(bracket).State = EntityState.Detached;

		ExpireCache(bracket.Id);
	}

	public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
	{
		_logger.LogDebug("Deleting bracket {bracketId}", id);

		var result = await _dbContext.Brackets
			.Where(season => season.Id == id)
			.ExecuteDeleteAsync(cancellationToken);

		if (result == 0)
		{
			_logger.LogDebug("Delete bracket {Id} command returned 0 rows deleted", id);
			return false;
		}

		if (result > 1)
		{
			_logger.LogInformation("Delete bracket {Id} command returned {RowsDeleted} rows deleted", id, result);
		}

		ExpireCache(id);
		return true;
	}

	private void ExpireCache() =>
		_cache.Remove(AllCacheKey);

	private void ExpireCache(int id)
	{
		_cache.Remove(AllCacheKey);
		_cache.Remove(GetCacheKey(id));
	}
}
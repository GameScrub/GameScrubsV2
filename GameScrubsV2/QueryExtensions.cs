using System.Linq.Expressions;

namespace GameScrubsV2;

public static class QueryableExtensions
{
	public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition,
		Expression<Func<TSource, bool>> predicate) =>
		condition ? source.Where(predicate) : source;
}

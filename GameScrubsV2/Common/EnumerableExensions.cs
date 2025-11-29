namespace GameScrubsV2.Common;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
	/// <param name="source">The <see cref="IEnumerable{T}"/> to check for emptiness.</param>
	/// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
	extension<T>(IEnumerable<T> source)
	{
		/// <summary>
		/// Determines whether a sequence has no elements.
		/// </summary>
		/// <returns><see langword="true"/> if the source is empty; otherwise <see langword="false"/>.</returns>
		public bool IsEmpty() => !source.Any();

		/// <summary>
		/// Determines whether no elements of a sequence satisfy a condition.
		/// </summary>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <returns>
		/// <see langword="true"/> if the source sequence is empty or none of its elements pass the test
		/// in the specified predicate; <see langword="false"/> otherwise.
		/// </returns>
		public bool None(Func<T, bool> predicate) => !source.Any(predicate);

		/// <summary>
		/// Determines whether a sequence doesn't contain a specified element.
		/// </summary>
		/// <param name="value">The value to locate in the sequence.</param>
		/// <returns>
		/// <see langword="true"/> if the source sequence doesn't contain the specified value;
		/// otherwise <see langword="false"/>.
		/// </returns>
		public bool DoesNotContain(T value) => !source.Contains(value);

		/// <summary>
		/// Determines whether a sequence doesn't contain a specified element.
		/// </summary>
		/// <param name="value">The value to locate in the sequence.</param>
		/// <param name="comparer">An equality comparer to compare values.</param>
		/// <returns>
		/// <see langword="true"/> if the source sequence doesn't contain the specified value;
		/// otherwise <see langword="false"/>.
		/// </returns>
		public bool DoesNotContain(T value, IEqualityComparer<T> comparer) =>
			!source.Contains(value, comparer);

		/// <summary>
		/// Paginates a sequence.
		/// </summary>
		/// <param name="pagination">The pagination information.</param>
		/// <returns>The paginated data.</returns>
		/// <remarks>
		/// If <paramref name="pagination"/> is empty, then no pagination occurs and the sequence is returned unaltered.
		/// </remarks>
		public IEnumerable<T> Paginate(Pagination pagination) =>
			pagination.IsEmpty ? source : source.Skip(pagination.SkippedCount).Take(pagination.PageSize);

		///  <summary>
		///  Splits a sequence into two new sequences based on a predicate. All elements of the sequence that pass
		///  the predicate are grouped into one sequence, and the elements that fail the predicate are grouped into the
		///  other.
		///  </summary>
		///  <param name="predicate">The predicate function.</param>
		///  <returns>
		///  A tuple of <see cref="IEnumerable{T}"/> where the first element is a sequence of the elements of
		///  <paramref name="source"/> for which calling <paramref name="predicate"/> on each of them returns
		///  <see langword="true"/>. The second element is the sequence for which calling <paramref name="predicate"/>
		///  returns <see langword="false"/>.
		///  </returns>
		///  <example>
		///  <code>
		/// 		var digits = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		/// 		var splitByIsEven = digits.SplitBy(n => n % 2 == 0);
		/// 		// splitByIsEven.Pass is { 0, 2, 4, 6, 8 }
		/// 		// splitByIsEven.Fail is { 1, 3, 5, 7, 9 }
		///  </code>
		///  </example>
		public (IEnumerable<T> Pass, IEnumerable<T> Fail) SplitBy(Func<T, bool> predicate)
		{
			var lookup = source.ToLookup(predicate);
			return (lookup[true], lookup[false]);
		}
	}
}
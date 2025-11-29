namespace GameScrubsV2.Common;

/// <summary>
/// A value that represents pagination data.
/// </summary>
public readonly record struct Pagination
{
	public int PageSize { get; }
	public int PageNumber { get; }

	/// <summary>
	/// Creates a new pagination object.
	/// </summary>
	/// <param name="pageSize">The number of elements per page.</param>
	/// <param name="pageNumber">The page number of the current pagination value.</param>
	public Pagination(int pageSize, int pageNumber)
	{
		if (pageSize < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size cannot be negative");
		}

		if (pageNumber < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "Page size cannot be negative");
		}

		PageSize = pageSize;
		PageNumber = pageNumber;
	}

	/// <summary>
	/// The number of elements that have been skipped to return the page associated with this pagination value.
	/// </summary>
	public int SkippedCount => IsEmpty ? 0 : PageSize * (PageNumber - 1);

	/// <summary>
	/// Indicates whether this value is an empty pagination, meaning that no pagination will occur.
	/// </summary>
	public bool IsEmpty => PageSize == 0 || PageNumber == 0;

	/// <summary>
	/// Creates an empty pagination.
	/// </summary>
	public static Pagination Empty => new(0, 0);
};
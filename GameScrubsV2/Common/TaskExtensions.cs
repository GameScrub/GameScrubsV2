namespace GameScrubsV2.Common;

public static class TaskExtensions
{
	/// <summary>
	/// Maps a value into an asynchronous function that accepts a parameter of that type
	/// and returns a <see cref="Task{TResult}"/>.
	/// </summary>
	/// <param name="source">The value to map.</param>
	/// <param name="func">The asynchronous function to map the value onto.</param>
	/// <typeparam name="TSource">The type of the mapped value.</typeparam>
	/// <typeparam name="TResult">The type of the result of <paramref name="func"/>.</typeparam>
	/// <returns>The result of calling <paramref name="func"/> with <paramref name="source"/>.</returns>
	public static async Task<TResult> MapAsync<TSource, TResult>(this TSource source, Func<TSource, Task<TResult>> func) =>
		await func.Invoke(source);

	/// <summary>
	/// Maps a <see cref="Task{TSource}"/> into an asynchronous function that accepts a parameter of that type
	/// and returns a <see cref="Task{TResult}"/>.
	/// </summary>
	/// <param name="source">The value to map.</param>
	/// <param name="func">The asynchronous function to map the value onto.</param>
	/// <typeparam name="TSource">The type of the mapped value.</typeparam>
	/// <typeparam name="TResult">The type of the result of <paramref name="func"/>.</typeparam>
	/// <returns>The result of calling <paramref name="func"/> with <paramref name="source"/>.</returns>
	public static async Task<TResult>
		MapAsync<TSource, TResult>(this Task<TSource> source, Func<TSource, TResult> func) =>
		func.Invoke(await source);

	/// <summary>
	/// Binds an asynchronous function to the result of a given task, returning a new task
	/// that resolves to the result of the bound function.
	/// </summary>
	/// <param name="source">The source task whose result will be passed to the function.</param>
	/// <param name="func">The asynchronous function to invoke with the result of the source task.</param>
	/// <typeparam name="TSource">The type source task's result.</typeparam>
	/// <typeparam name="TResult">The type bound function's result.</typeparam>
	/// <returns>
	/// A new task that resolves to the result of the asynchronous function when provided
	/// <paramref name="source"/> as its argument.
	/// </returns>
	public static async Task<TResult> BindAsync<TSource, TResult>(
		this Task<TSource> source,
		Func<TSource, Task<TResult>> func) =>
		await func.Invoke(await source);
}
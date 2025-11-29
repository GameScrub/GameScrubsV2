namespace GameScrubsV2.Common;

/// <summary>
/// A wrapper for the result of an operation when the type of the result is different
/// depending on whether the operation was a success or a failure.
/// </summary>
/// <typeparam name="TSuccess">The type of the result in the success case.</typeparam>
/// <typeparam name="TFailure">The type of the result in the failure case.</typeparam>
public sealed class Result<TSuccess, TFailure>
{
	/// <summary>
	/// The result of the operation if it succeeded. If the operation failed,
	/// the value of this property is undefined and should not be used.
	/// </summary>
	public TSuccess SuccessValue { get; }

	/// <summary>
	/// The result of the operation if it failed. If the operation succeeded,
	/// the value of this property is undefined and should not be used.
	/// </summary>
	public TFailure FailureValue { get; }

	/// <summary>
	/// A flag indicating whether this result indicates a successful operation (if <see langword="true"/>),
	/// or a failed one (if <see langword="false"/>).
	/// </summary>
	public bool IsSuccess { get; }

	/// <summary>
	/// A flag indicating whether this result indicates a failed operation (if <see langword="true"/>),
	/// or a successful one (if <see langword="false"/>).
	/// </summary>
	public bool IsFailure => !IsSuccess;

	/// <summary>
	/// Create a success result.
	/// </summary>
	/// <param name="successValue">The result of the successful operation.</param>
	public Result(TSuccess successValue)
	{
		SuccessValue = successValue;
		FailureValue = default!;
		IsSuccess = true;
	}

	/// <summary>
	/// Create a fail result.
	/// </summary>
	/// <param name="failureValue">The result of the failed operation.</param>
	private Result(TFailure failureValue)
	{
		SuccessValue = default!;
		FailureValue = failureValue;
		IsSuccess = false;
	}

	/// <summary>
	/// Gets the success value from the result.
	/// </summary>
	/// <param name="value">
	/// When this method returns, contains the success value if the result was a success; otherwise,
	/// the default value for the type of the value parameter. This parameter is passed uninitialized.
	/// </param>
	/// <returns>true if the <see cref="Result{TSuccess, TFailure}"/> is a success; otherwise, false.</returns>
	public bool TryGet(out TSuccess value)
	{
		value = SuccessValue;
		return IsSuccess;
	}

	/// <summary>
	/// If this result is a success, then this method passes the success value into the given function and returns
	/// the output of that function wrapped as a successful <see cref="Result{TResult,TFailure}"/>. Otherwise, the
	/// failure value of this result is returned.
	/// </summary>
	/// <param name="func">A function that will be called with the success value if this result is a success.</param>
	/// <typeparam name="TResult">The type of the output of <paramref name="func"/>.</typeparam>
	/// <returns>
	/// The result of calling <paramref name="func"/> if this result is a success;
	/// this result's failure value otherwise.
	/// </returns>
	public Result<TResult, TFailure> Map<TResult>(Func<TSuccess, TResult> func) =>
		IsFailure ? FailureValue : func.Invoke(SuccessValue);

	/// <summary>
	/// If this result is a success, then this method passes the success value into the given asynchronous function
	/// and returns the output of that function wrapped as a successful <see cref="Result{TResult, TFailure}"/>.
	/// Otherwise, the failure value of this result is returned.
	/// </summary>
	/// <param name="func">
	/// An asynchronous function that will be called with the success value if this result is a success.
	/// </param>
	/// <typeparam name="TResult">The type of the output of <paramref name="func"/>.</typeparam>
	/// <returns>
	/// The result of calling <paramref name="func"/> if this result is a success;
	/// this result's failure value otherwise.
	/// </returns>
	public async Task<Result<TResult, TFailure>> MapAsync<TResult>(Func<TSuccess, Task<TResult>> func)
	{
		if (IsFailure)
		{
			return FailureValue;
		}

		return await SuccessValue.MapAsync(func);
	}

	/// <summary>
	/// If this result is a success, then this method passes the success value into the given function and returns
	/// the output of that function. Otherwise, the failure value of this result is returned.
	/// </summary>
	/// <param name="func">A function that will be called with the success value if this result is a success.</param>
	/// <typeparam name="TResult">The type of a successful result of <paramref name="func"/>.</typeparam>
	/// <returns>
	/// The result of calling <paramref name="func"/> if this result is a success;
	/// this result's failure value otherwise.
	/// </returns>
	public Result<TResult, TFailure> Bind<TResult>(Func<TSuccess, Result<TResult, TFailure>> func) =>
		IsFailure ? FailureValue : func.Invoke(SuccessValue);

	/// <summary>
	/// If this result is a success, then this method passes the success value into the given asynchronous function and
	/// returns the output of that function. Otherwise, the failure value of this result is returned.
	/// </summary>
	/// <param name="func">
	/// An asynchronous function that will be called with the success value if this result is a success.
	/// </param>
	/// <typeparam name="TResult">The type of a successful result of <paramref name="func"/>.</typeparam>
	/// <returns>
	/// The result of calling <paramref name="func"/> if this result is a success;
	/// this result's failure value otherwise.
	/// </returns>
	public async Task<Result<TResult, TFailure>> BindAsync<TResult>(Func<TSuccess, Task<Result<TResult, TFailure>>> func)
	{
		if (IsFailure)
		{
			return FailureValue;
		}

		return await SuccessValue.MapAsync(func);
	}

	public static implicit operator Result<TSuccess, TFailure>(TSuccess successValue) => new(successValue);
	public static implicit operator Result<TSuccess, TFailure>(TFailure failureValue) => new(failureValue);
}
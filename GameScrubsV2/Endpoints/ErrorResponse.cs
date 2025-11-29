namespace GameScrubsV2.Endpoints;

public record ErrorResponse
{
	public ErrorResponse()
	{
		Errors = Array.Empty<string>();
	}

	public ErrorResponse(string error)
	{
		Errors = [error];
	}

	public ErrorResponse(List<string> errors)
	{
		Errors = errors;
	}

	public IEnumerable<string> Errors { get; init; }
}
namespace GameScrubsV2.Endpoints;

public record MessageResponse
{
	public MessageResponse()
	{
		Messages = Array.Empty<string>();
	}

	public MessageResponse(string error)
	{
		Messages = [error];
	}

	public MessageResponse(List<string> messages)
	{
		Messages = messages;
	}

	public IEnumerable<string> Messages { get; init; }
}
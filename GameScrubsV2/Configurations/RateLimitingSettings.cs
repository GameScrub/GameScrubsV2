namespace GameScrubsV2.Configurations;

public class RateLimitingSettings
{
	public const string Key = "RateLimiting";

	public bool Enabled { get; init; } = true;
}
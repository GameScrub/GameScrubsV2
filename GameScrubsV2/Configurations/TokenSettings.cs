using System.ComponentModel.DataAnnotations;

namespace GameScrubsV2.Configurations;

public class TokenSettings
{
	public const string Key = "TokenSettings";

	[Required, MinLength(32)]
	public required string SecretKey { get; init; } = string.Empty;

	[Required]
	public required string Issuer { get; init; } = string.Empty;

	[Required]
	public required string Audience { get; init; } = string.Empty;

	[Range(1, 1440)] // 1 minute to 24 hours
	public required int ExpirationInMinutes { get; init; } = 60;
}
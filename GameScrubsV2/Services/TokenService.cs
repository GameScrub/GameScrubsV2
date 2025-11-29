using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using GameScrubsV2.Common;
using GameScrubsV2.Configurations;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using static GameScrubsV2.Services.TokenServiceFailureResult;

namespace GameScrubsV2.Services;

public sealed class TokenService
{
	private readonly TokenSettings _tokenSettings;
	private readonly ILogger<TokenService> _logger;

	public TokenService(IOptions<TokenSettings> tokenSettings, ILogger<TokenService> logger)
	{
		_tokenSettings = tokenSettings.Value;
		_logger = logger;
	}

	public Result<TokenServiceResponse, TokenServiceFailureResult> GenerateToken(TokenServiceRequest request)
	{
		try
		{
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, request.User.Id),
				new(ClaimTypes.Email, request.User.Email!),
				new(ClaimTypes.Name, request.User.UserName!),
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			claims.AddRange(request.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _tokenSettings.Issuer,
				audience: _tokenSettings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_tokenSettings.ExpirationInMinutes),
				signingCredentials: credentials
			);

			return new TokenServiceResponse { Token = new JwtSecurityTokenHandler().WriteToken(token) };
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error generating auth token");
			return DatabaseError;
		}
	}
}

public enum TokenServiceFailureResult
{
	DatabaseError,
}

public sealed record TokenServiceRequest
{
	public required IdentityUser User { get; init; }
	public required IReadOnlyList<string> Roles { get; init; }
}

public sealed record TokenServiceResponse
{
	public required string Token { get; init; }
}
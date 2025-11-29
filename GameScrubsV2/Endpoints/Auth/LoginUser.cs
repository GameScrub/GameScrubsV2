using GameScrubsV2.Common;
using GameScrubsV2.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Auth;

public static partial class AuthEndpoints
{
	public static void LoginUser(this RouteGroupBuilder group) =>
		group.MapPost("/login", async (
			[FromBody] LoginRequest request,
			[FromServices] UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager,
			TokenService tokenService,
			ILoggerFactory loggerFactory) =>
		{
			var logger = loggerFactory.GetLogger("AuthEndpoints");

			try
			{
				var user = await userManager.FindByEmailAsync(request.Email);

				if (user == null)
				{
					return Results.Unauthorized();
				}

				var result = await signInManager
					.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

				if (!result.Succeeded)
				{
					return result.IsLockedOut
						? Results.BadRequest(new ErrorResponse("Account locked out"))
						: Results.Unauthorized();
				}

				var roles = await userManager.GetRolesAsync(user);

				var tokenResult = tokenService.GenerateToken(new TokenServiceRequest
				{
					User = user,
					Roles = roles.ToList()
				});

				return !tokenResult.TryGet(out var tokenData)
					? Results.InternalServerError(new ErrorResponse(tokenResult.FailureValue.ToString()))
					: Results.Ok(new LoginResponse(tokenData.Token));

			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error logging in user");
				return Results.InternalServerError(new ErrorResponse("Error logging in user"));
			}
		}).WithName("Login");


	public sealed record LoginRequest(string Email, string Password);
	public sealed record LoginResponse(string Token);
}
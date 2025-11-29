using GameScrubsV2.Common;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Auth;

public static partial class AuthEndpoints
{
	public static void LogoutUser(this RouteGroupBuilder group) =>
		group.MapPost("/logout", async (
				[FromServices] SignInManager<IdentityUser> signInManager,
				ILoggerFactory loggerFactory) =>
			{
				var logger = loggerFactory.GetLogger("AuthEndpoints");
				await signInManager.SignOutAsync();

				logger.LogDebug("User logged out");
				return Results.Ok(new { message = "Logout successful" });
			}).WithName("Logout")
			.RequireAuthorization();
}
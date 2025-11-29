using GameScrubsV2.Common;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Auth;

public static partial class AuthEndpoints
{
	public static void UserDetails(this RouteGroupBuilder group) =>
		group.MapGet("/me", async (
				 HttpContext httpContext,
				 [FromServices] UserManager<IdentityUser> userManager,
				ILoggerFactory loggerFactory) =>
			{
				var logger = loggerFactory.GetLogger("AuthEndpoints");

				logger.LogDebug("Getting current user details");
				var user = await userManager.GetUserAsync(httpContext.User);

				return user == null
					? Results.Unauthorized()
					: Results.Ok(new GetCurrentUserResponse(user.Id, user.Email, user.UserName));
			})
			.WithName("GetCurrentUser")
			.RequireAuthorization();

	public sealed record GetCurrentUserResponse(string UserId, string? Email, string? UserName);
}
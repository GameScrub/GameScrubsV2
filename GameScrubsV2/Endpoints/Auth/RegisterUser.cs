using GameScrubsV2.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameScrubsV2.Endpoints.Auth;

public static partial class AuthEndpoints
{
	public static void RegisterUser(this RouteGroupBuilder group) =>
		group.MapPost("/register", async (
				[FromBody] RegisterRequest request,
				[FromServices] UserManager<IdentityUser> userManager,
				TokenService tokenService) =>
			{
				var existingUser = await userManager.FindByEmailAsync(request.Email);

				if (existingUser != null)
				{
					return Results.BadRequest(new { message = "User with this email already exists" });
				}

				var user = new IdentityUser
				{
					UserName = request.Email,
					Email = request.Email,
				};

				var result = await userManager.CreateAsync(user, request.Password);

				if (result.Succeeded)
				{
					var roles = await userManager.GetRolesAsync(user);
					var tokenResult = tokenService.GenerateToken(new TokenServiceRequest
					{
						User = user,
						Roles = roles.ToList()
					});

					if (!tokenResult.TryGet(out var tokenData))
					{
						return Results.InternalServerError(new MessageResponse(tokenResult.FailureValue.ToString()));
					}

					return Results.Ok(new RegisterResponse("User registered successfully", user.Id, user.Email, tokenData.Token));
				}

				return Results.BadRequest(new MessageResponse(result.Errors.Select(e => e.Description).ToList()));
			})
			.WithName("Register");

	public sealed record RegisterRequest(string Email, string Password);
	public sealed record RegisterResponse(string Message, string UserId, string Email, string Token);
}
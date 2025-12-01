using GameScrubsV2.Endpoints.Auth;
using GameScrubsV2.Endpoints.Bracket;

namespace GameScrubsV2.Endpoints;

public static class ServiceRegistration
{
	extension(IEndpointRouteBuilder app)
	{
		public void MapAuthEndpoints()
		{
			var authGroup = app.MapGroup("/api/auth").WithTags("Authentication");

			authGroup.RegisterUser();
			authGroup.LoginUser();
			authGroup.LogoutUser();
			authGroup.UserDetails();
		}

		public void MapBracketEndpoints()
		{
			var bracketGroup = app.MapGroup("/api/brackets").WithTags("Bracket");
			bracketGroup.GetAllBrackets();
			bracketGroup.GetBracketById();
			bracketGroup.CreateBracket();
			bracketGroup.UpdateBracket();
			bracketGroup.DeleteBracket();
		}
	}
}
using GameScrubsV2.Endpoints.Auth;
using GameScrubsV2.Endpoints.Bracket;
using GameScrubsV2.Endpoints.Player;

namespace GameScrubsV2.Endpoints;

public static class EndpointRegistration
{
	extension(IEndpointRouteBuilder app)
	{
		public IEndpointRouteBuilder MapEndpoints() =>
			app
				.MapAuthEndpoints()
				.MapBracketEndpoints()
				.MapPlayerEndpoints();

		private IEndpointRouteBuilder MapAuthEndpoints()
		{
			var authGroup = app.MapGroup("/api/auth").WithTags("Authentication");

			authGroup.RegisterUser();
			authGroup.LoginUser();
			authGroup.LogoutUser();
			authGroup.UserDetails();

			return app;
		}

		private IEndpointRouteBuilder MapBracketEndpoints()
		{
			var bracketGroup = app.MapGroup("/api/brackets").WithTags("Brackets");

			bracketGroup.GetAllBrackets();
			bracketGroup.GetBracketById();
			bracketGroup.CreateBracket();
			bracketGroup.UpdateBracket();
			bracketGroup.DeleteBracket();

			return app;
		}

		private IEndpointRouteBuilder MapPlayerEndpoints()
		{
			var bracketGroup = app.MapGroup("/api/players").WithTags("Players");

			bracketGroup.AddPlayer();
			bracketGroup.GetAllPlayers();
			bracketGroup.ReorderPlayers();
			bracketGroup.RemovePlayer();

			return app;
		}
	}
}
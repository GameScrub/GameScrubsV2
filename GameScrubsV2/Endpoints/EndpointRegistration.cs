using GameScrubsV2.Endpoints.Auth;
using GameScrubsV2.Endpoints.Bracket;
using GameScrubsV2.Endpoints.Placement;
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
				.MapPlayerEndpoints()
				.MapBracketPlacementEndpoints();

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
			var group = app.MapGroup("/api/brackets").WithTags("Brackets");

			group.GetAllBrackets();
			group.GetBracketById();
			group.CreateBracket();
			group.UpdateBracket();
			group.DeleteBracket();
			group.ChangeBracketStatus();

			return app;
		}

		private IEndpointRouteBuilder MapPlayerEndpoints()
		{
			var group = app.MapGroup("/api/players").WithTags("Players");

			group.AddPlayer();
			group.GetAllPlayers();
			group.ReorderPlayers();
			group.RemovePlayer();

			return app;
		}

		private IEndpointRouteBuilder MapBracketPlacementEndpoints()
		{
			var group = app.MapGroup("/api/placements").WithTags("Placements");

			group.GetBracketPlacement();
			group.SetPlacementScore();
			group.GetPlacementScores();

			return app;
		}
	}
}
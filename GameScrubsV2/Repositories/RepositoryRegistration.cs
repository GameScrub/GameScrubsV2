namespace GameScrubsV2.Repositories;

public static class RepositoryRegistration
{
	extension(WebApplicationBuilder builder)
	{
		public WebApplicationBuilder RegisterRepositories()
		{
			builder.Services.AddTransient<BracketRepository>();
			builder.Services.AddTransient<BracketPositionsRepository>();

			return builder;
		}
	}
}
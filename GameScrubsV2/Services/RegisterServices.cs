namespace GameScrubsV2.Services;

public static class ServiceRegistration
{
	extension(WebApplicationBuilder builder)
	{
		public WebApplicationBuilder RegisterServices()
		{
			builder.Services.AddTransient<TokenService>();

			return builder;
		}
	}
}
namespace GameScrubsV2.Configurations;

public static class Configuration
{
	extension(WebApplicationBuilder builder)
	{
		public WebApplicationBuilder RegisterConfigurations()
		{
			builder.InitializeSettings<TokenSettings>(TokenSettings.Key);

			return builder;
		}

		private WebApplicationBuilder InitializeSettings<TSettings>(string key)
			where TSettings : class
		{
			builder.Services
				.AddOptions<TSettings>()
				.BindConfiguration(key)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			return builder;
		}
	}
}
namespace GameScrubsV2.Common;

public static class Logger
{
	public static ILogger GetLogger(this ILoggerFactory loggerFactory, string categoryName) =>
		loggerFactory.CreateLogger(categoryName);
}
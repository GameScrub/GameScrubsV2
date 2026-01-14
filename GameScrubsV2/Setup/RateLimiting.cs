using System.Threading.RateLimiting;

using GameScrubsV2.Configurations;

namespace GameScrubsV2.Setup;

public static class RateLimiting
{
 extension(WebApplicationBuilder webApplicationBuilder)
 {
	 public void ConfigureRateLimiting() =>
		 webApplicationBuilder.Services.AddRateLimiter(options =>
		 {
			const string unknown = "Unknown";
			const string rateLimiting = "RateLimiting";

			var rateLimitingSettings = webApplicationBuilder.Configuration
				.GetSection(RateLimitingSettings.Key)
				.Get<RateLimitingSettings>();

			if (!rateLimitingSettings!.Enabled)
			{
				options.AddPolicy("CreateBracket", context => RateLimitPartition.GetNoLimiter("test"));
				options.AddPolicy("PlayerOperations", context => RateLimitPartition.GetNoLimiter("test"));
				options.AddPolicy("BracketUpdates", context => RateLimitPartition.GetNoLimiter("test"));
				options.AddPolicy("SearchOperations", context => RateLimitPartition.GetNoLimiter("test"));
			}
			else {

			 // Global rate limiter - applies to all requests
			 options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
				 RateLimitPartition.GetFixedWindowLimiter(
					 partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? unknown,
					 factory: _ => new FixedWindowRateLimiterOptions
					 {
						 PermitLimit = 100,
						 Window = TimeSpan.FromMinutes(1),
						 QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						 QueueLimit = 0
					 }));

			 // Strict policy for bracket creation (prevents spam)
			 options.AddPolicy("CreateBracket", context =>
				 RateLimitPartition.GetFixedWindowLimiter(
					 partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? unknown,
					 factory: _ => new FixedWindowRateLimiterOptions
					 {
						 PermitLimit = 5,
						 Window = TimeSpan.FromHours(1),
						 QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						 QueueLimit = 0
					 }));

			 // Policy for player operations (adding/removing players)
			 options.AddPolicy("PlayerOperations", context =>
				 RateLimitPartition.GetFixedWindowLimiter(
					 partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? unknown,
					 factory: _ => new FixedWindowRateLimiterOptions
					 {
						 PermitLimit = 30,
						 Window = TimeSpan.FromMinutes(1),
						 QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						 QueueLimit = 0

					 }));

			 // Policy for bracket updates (prevents excessive status changes)
			 options.AddPolicy("BracketUpdates", context =>
				 RateLimitPartition.GetFixedWindowLimiter(
					 partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? unknown,
					 factory: _ => new FixedWindowRateLimiterOptions
					 {
						 PermitLimit = 20,
						 Window = TimeSpan.FromMinutes(1),
						 QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						 QueueLimit = 0
					 }));

			 // Policy for search operations (prevents search spam)
			 options.AddPolicy("SearchOperations", context =>
				 RateLimitPartition.GetFixedWindowLimiter(
					 partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? unknown,
					 factory: _ => new FixedWindowRateLimiterOptions
					 {
						 PermitLimit = 30,
						 Window = TimeSpan.FromMinutes(1),
						 QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						 QueueLimit = 0
					 }));

			 options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
			 options.OnRejected = async (context, cancellationToken) =>
			 {
				 var httpContext = context.HttpContext;
				 var logger = httpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(rateLimiting);

				 var endpoint = httpContext.GetEndpoint()?.DisplayName ?? unknown;
				 var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? unknown;
				 var path = httpContext.Request.Path;
				 var method = httpContext.Request.Method;

				 logger.LogWarning(
					 "Rate limit exceeded for {Method} {Path} from IP {IpAddress} (Endpoint: {Endpoint})",
					 method,
					 path,
					 ipAddress,
					 endpoint);

				 httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
				 httpContext.Response.ContentType = "application/json";

				 await httpContext.Response.WriteAsJsonAsync(new
				 {
					 StatusCode = 429,
					 Message = "Too many requests. Please try again later.",
					 RetryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
						 ? retryAfter.TotalSeconds
						 : (double?)null
				 }, cancellationToken);
			 };
			}
		 });
 }
}
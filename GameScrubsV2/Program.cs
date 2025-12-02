using System.Text;
using System.Text.Json;

using GameScrubsV2.Configurations;
using GameScrubsV2.Endpoints;
using GameScrubsV2.Models;
using GameScrubsV2.Repositories;
using GameScrubsV2.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Serilog;
using Serilog.Events;

using static GameScrubsV2.Common.Json.SerializerOptions;

// Create the logger BEFORE building the app
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
	.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
	.MinimumLevel.Override("System", LogEventLevel.Warning)
	.Enrich.FromLogContext()
	.Enrich.WithEnvironmentName()
	.Enrich.WithMachineName()
	.Enrich.WithThreadId()
	.WriteTo.Console(
		outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}")
	.WriteTo.File(
		path: "Logs/log-.txt",
		rollingInterval: RollingInterval.Day,
		outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
	.CreateLogger();

Log.Information("Starting web application");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddDbContext<GameScrubsV2DbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();

builder
	.RegisterConfigurations()
	.RegisterServices()
	.RegisterRepositories();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
	{
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireUppercase = true;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequiredLength = 6;
	})
	.AddEntityFrameworkStores<GameScrubsV2DbContext>()
	.AddDefaultTokenProviders();

var tokenSettings = builder.Configuration.GetRequiredSection(TokenSettings.Key).Get<TokenSettings>()!;

builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = tokenSettings.Issuer,
		ValidAudience = tokenSettings.Audience,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey)),
		ClockSkew = TimeSpan.Zero
	});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
	foreach (var converter in DefaultJsonSerializerOptions.Converters)
	{
		options.SerializerOptions.Converters.Add(converter);
	}
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
	{
		var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
		var exception = exceptionHandlerPathFeature?.Error;

		if (exception is BadHttpRequestException badHttpEx && badHttpEx.InnerException is JsonException jsonEx)
		{
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			context.Response.ContentType = "application/json";

			await context.Response.WriteAsJsonAsync(new
			{
				StatusCode = 400,
				Message = "Invalid request data",
				Error = "The request contains invalid enum values or malformed JSON"
			});
		}
		else
		{
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			await context.Response.WriteAsJsonAsync(new
			{
				StatusCode = 500,
				Message = "An internal server error occurred"
			});
		}
	}));

app.UseSerilogRequestLogging(options =>
{
	options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
	options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
	{
		diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
		diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
	};
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
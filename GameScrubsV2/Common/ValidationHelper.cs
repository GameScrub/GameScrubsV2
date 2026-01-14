using System.ComponentModel.DataAnnotations;

namespace GameScrubsV2.Common;

/// <summary>
/// Endpoint filter that automatically validates request models with data annotations
/// </summary>
public class ValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Find all [FromBody] parameters and validate them
        for (var i = 0; i < context.Arguments.Count; i++)
        {
            var argument = context.Arguments[i];
            if (argument != null && ShouldValidate(argument))
            {
                var validationResult = ValidateModel(argument);
                if (validationResult != null)
                {
                    return validationResult;
                }
            }
        }

        return await next(context);
    }

    private static bool ShouldValidate(object model)
    {
        var type = model.GetType();

        // Only validate classes that have validation attributes
        return type.IsClass &&
               type != typeof(string) &&
               HasValidationAttributes(type);
    }

    private static bool HasValidationAttributes(Type type) =>
	    type.GetProperties()
		    .SelectMany(p => p.GetCustomAttributes(typeof(ValidationAttribute), true))
		    .Any();

    private static IResult? ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);

        if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
        {
            var errors = validationResults
                .GroupBy(vr => vr.MemberNames.FirstOrDefault() ?? "validation")
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(vr => vr.ErrorMessage ?? "Validation error").ToArray()
                );

            return Results.ValidationProblem(errors);
        }

        return null;
    }
}

/// <summary>
/// Extension methods for adding validation to route groups
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Adds automatic model validation to all endpoints in the route group
    /// </summary>
    public static RouteGroupBuilder WithValidation(this RouteGroupBuilder builder) =>
	    builder.AddEndpointFilter<ValidationFilter>();
}



/// <summary>
/// Static helper for manual validation (keeping for backward compatibility)
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates a request model using data annotations and returns a validation problem result if invalid.
    /// </summary>
    /// <typeparam name="T">The type of the request model to validate</typeparam>
    /// <param name="model">The request model instance to validate</param>
    /// <returns>A validation problem result if validation fails, otherwise null</returns>
    public static IResult? ValidateModel<T>(T model) where T : class
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);

        if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
        {
            var errors = validationResults
                .GroupBy(vr => vr.MemberNames.FirstOrDefault() ?? "validation")
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(vr => vr.ErrorMessage ?? "Validation error").ToArray()
                );

            return Results.ValidationProblem(errors);
        }

        return null;
    }
}
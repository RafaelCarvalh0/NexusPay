using FluentValidation;
using FluentValidation.Results;
using NexusPay.Shared.Models.Error;

namespace NexusPay.Api.Extensions
{
    public static class ValidationExtension
    {
        public static RouteHandlerBuilder WithValidation<TRequest>(this RouteHandlerBuilder builder)
            where TRequest : class
            => builder.AddEndpointFilter<ValidationFilter<TRequest>>();

        public class ValidationFilter<TRequest> : IEndpointFilter where TRequest : class
        {
            public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
            {
                TRequest? request = context.Arguments.OfType<TRequest>().FirstOrDefault();

                if (request is null)
                {
                    return Results.BadRequest(new ErrorResponse
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Error = "Bad Request",
                        Messages = ["Invalid request payload."],
                        Path = context.HttpContext.Request.Path.Value!
                    });
                }

                IValidator<TRequest>? validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();

                if (validator is null)
                    throw new InvalidOperationException(
                        $"No validator registered for {typeof(TRequest).Name}. " +
                        $"Register IValidator<{typeof(TRequest).Name}> in the DI container.");

                ValidationResult validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                    return Results.Json(new ErrorResponse
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Error = "Validation Failed",
                        Messages = validationResult.Errors
                       .Select(e => e.ErrorMessage)
                       .ToArray(),
                        Path = context.HttpContext.Request.Path.Value!
                    }, statusCode: StatusCodes.Status400BadRequest);
                }

                return await next(context);
            }
        }
    }
}

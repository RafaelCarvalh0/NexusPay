using NexusPay.Api.Endpoints;
using NexusPay.Api.Middlewares;

namespace NexusPay.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        extension(WebApplication app)
        {
            public WebApplication MapEndpoints()
            {
                app.MapUserEndpoints();
                app.MapAuthEndpoints();
                //app.MapPaymentEndpoints();

                return app;
            }

            public WebApplication UseGlobalMiddlewares()
            {
                app.UseCors("NexusPayPolicy");
                app.UseExceptionHandler();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseMiddleware<TokenValidationMiddleware>();

                return app;
            }
        }
    }
}

using NexusPay.Api.Endpoints;

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
                app.UseExceptionHandler();

                return app;
            }
        }
    }
}

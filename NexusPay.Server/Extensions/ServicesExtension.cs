using NexusPay.Server.Services;

namespace NexusPay.Server.Extensions
{
    public static class ServicesExtension
    {
        extension(WebApplication app)
        {
            public WebApplication ApplyServices()
            {
                app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

                app.MapGrpcService<AuthGrpcService>();
                app.MapGrpcService<TransactionGrpcService>();
                app.MapGrpcReflectionService();

                return app;
            }
        }
    }
}

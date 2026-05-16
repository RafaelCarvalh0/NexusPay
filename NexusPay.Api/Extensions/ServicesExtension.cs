using NexusPay.Client.Services;

namespace NexusPay.Api.Extensions
{
    public static class ServicesExtension
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection ApplyServiceConfiguration(IConfiguration configuration)
            {
                services
                    .AddValidators()
                    .AddServices(configuration.GetSection("GrpcServer").GetSection("BaseUrl").Value!);

                return services;
            }

            private IServiceCollection AddValidators()
            {
                //services.AddScoped<IValidator<PaymentRequest>, PaymentRequestValidator>();
                return services;
            }

            private IServiceCollection AddServices(string baseUrl)
            {
                services.AddScoped<IAuthGrpcClient, AuthGrpcClient>(provider => new AuthGrpcClient(baseUrl, provider.GetRequiredService<ILogger<AuthGrpcClient>>()));

                return services;
            }
        }
    }
}

using FluentValidation;
using Grpc.Net.Client;
using NexusPay.Api.Middlewares;
using NexusPay.Client.Services;
using NexusPay.Shared.Models.Auth;
using NexusPay.Shared.Models.Auth.Validators;
using NexusPay.Shared.Models.User;
using NexusPay.Shared.Models.User.Validators;

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
                    .AddExceptionHandler<GlobalExceptionHandler>()
                    .AddProblemDetails()
                    .AddGrpcClientServices(configuration.GetSection("GrpcServer").GetSection("BaseUrl").Value!);

                return services;
            }

            private IServiceCollection AddValidators()
            {
                services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
                services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
                
                return services;
            }

            private IServiceCollection AddGrpcClientServices(string baseUrl)
            {
                services.AddSingleton(provider =>
                {
                    return GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions
                    {
                        HttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        }
                    });
                });

                services.AddScoped<IUserGrpcClient, UserGrpcClient>();
                services.AddScoped<IAuthGrpcClient, AuthGrpcClient>();

                return services;
            }
        }
    }
}

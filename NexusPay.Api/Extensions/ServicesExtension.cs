using FluentValidation;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NexusPay.Api.Middlewares;
using NexusPay.Client.Services;
using NexusPay.Client.Services.Interfaces;
using NexusPay.Shared.Jwt;
using NexusPay.Shared.Markers;
using System.Text;

namespace NexusPay.Api.Extensions
{
    public static class ServicesExtension
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection ApplyServiceConfiguration(IConfiguration configuration)
            {
                services
                    .AddSecurity()
                    .AddAuthConfiguration(configuration.GetSection("Jwt"))
                    .AddValidators()
                    .AddExceptionHandler<GlobalExceptionHandler>()
                    .AddProblemDetails()
                    .AddGrpcClientServices(configuration.GetSection("GrpcServer").GetSection("BaseUrl").Value!);

                return services;
            }

            private IServiceCollection AddSecurity()
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("NexusPayPolicy", policy =>
                    {
                        policy.WithOrigins(
                                  "https://nexuspay.com.br",       // prod
                                  "http://localhost:3000",         // frontend dev
                                  "http://127.0.0.1:5500"          // from file system (e.g., Live Server)
                              )
                              .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                              .WithHeaders("Content-Type", "Authorization");
                    });
                });

                return services;
            }

            private IServiceCollection AddAuthConfiguration(IConfiguration jwtConfiguration)
            {
                services.Configure<JwtSettings>(jwtConfiguration);
                JwtSettings jwtSettings = jwtConfiguration.Get<JwtSettings>()!;

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.MapInboundClaims = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                                           Encoding.UTF8.GetBytes(jwtSettings.Key)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Admin", policy =>
                    {
                        policy.RequireRole("Admin");
                    });
                });

                return services;
            }

            private IServiceCollection AddValidators()
            {
                // Registers all validators in the assembly containing SharedAssemblyMarker
                services.AddValidatorsFromAssembly(typeof(SharedAssemblyMarker).Assembly);

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
                services.AddScoped<ITenantGrpcClient, TenantGrpcClient>();

                return services;
            }
        }
    }
}

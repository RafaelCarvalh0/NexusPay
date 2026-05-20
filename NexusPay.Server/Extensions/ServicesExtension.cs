using NexusPay.Data.Configuration;
using NexusPay.Data.Repositories;
using NexusPay.Server.Helper.Jwt;
using NexusPay.Server.Helper.Redis;
using NexusPay.Server.Interceptors;
using NexusPay.Shared.Models.Jwt;
using StackExchange.Redis;

namespace NexusPay.Server.Extensions
{
    public static class ServicesExtension
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection ApplyConfigurations(IConfiguration configuration)
            {            
                services
                    .AddServices(configuration.GetSection("Jwt"))
                    .AddGlobalException()
                    .AddRepositories()
                    .AddInfrasctructure(configuration);

                return services;
            }

            private IServiceCollection AddServices(IConfigurationSection jwtSection)
            {
                services.Configure<JwtSettings>(jwtSection);
                services.AddScoped<IJwtService, JwtService>();

                return services;
            }

            private IServiceCollection AddGlobalException()
            {
                services.AddGrpc(options => { options.Interceptors.Add<GlobalExceptionInterceptor>(); });
                services.AddScoped<GlobalExceptionInterceptor>();

                return services;
            }

            private IServiceCollection AddRepositories()
            {
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IAuthRepository, AuthRepository>();
                //services.AddScoped<ITransactionRepository, TransactionRepository>();

                return services;
            }

            private IServiceCollection AddInfrasctructure(IConfiguration configuration)
            {
                var connectionString = configuration.GetConnectionString("SQL") ?? throw new InvalidOperationException("Connection string 'SQL' not found.");
                var redisConnectionString = configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Connection string 'Redis' not found.");

                services.AddTransient<IUniversal, Universal>(provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<Universal>>();
                    return new Universal(connectionString, logger);
                });

                services.AddSingleton<IConnectionMultiplexer>(
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

                services.AddScoped<IRedisService, RedisService>();

                return services;
            }
        }
    }
}

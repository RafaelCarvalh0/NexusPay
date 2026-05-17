using NexusPay.Data.Configuration;
using NexusPay.Data.Repositories;
using NexusPay.Server.Interceptors;

namespace NexusPay.Server.Extensions
{
    public static class ServicesExtension
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection ApplyConfigurations(IConfiguration configuration)
            {
                string connectionString = configuration.GetConnectionString("SQL") ?? throw new InvalidOperationException("Connection string 'SQL' not found.");

                services
                    .AddGlobalException()
                    .AddRepositories()
                    .AddInfrasctructure(connectionString);

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

            private IServiceCollection AddInfrasctructure(string connectionString)
            {
                services.AddTransient<IUniversal, Universal>(provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<Universal>>();
                    return new Universal(connectionString, logger);
                });

                return services;
            }
        }
    }
}

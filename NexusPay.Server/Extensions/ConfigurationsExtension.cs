
using NexusPay.Data.Configuration;

namespace NexusPay.Server.Extensions
{
    public static class ConfigurationsExtension
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection ApplyConfigurations(IConfiguration configuration)
            {
                string connectionString = configuration.GetConnectionString("SQL") ?? throw new InvalidOperationException("Connection string 'SQL' not found.");

                services
                    .AddRepositories()
                    .AddInfrasctructure(connectionString);

                return services;
            }

            private IServiceCollection AddRepositories()
            {
                //services.AddScoped<IAccountRepository, AccountRepository>();
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

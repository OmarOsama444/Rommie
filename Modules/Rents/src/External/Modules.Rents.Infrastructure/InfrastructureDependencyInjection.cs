using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Modules.Rents.Infrastructure.Data;
using Modules.Rents.Application.Abstractions;
using Modules.Rents.Application.Repositories;
using Modules.Rents.Infrastructure.Inbox;
using Modules.Rents.Infrastructure.Outbox;
using Modules.Rents.Infrastructure.Repositories;

namespace Modules.Rents.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        string dbConnectionString = configuration.GetConnectionString("RommieDb")!;
        services.AddDbContext<RentsDbContext>((sp, options) =>
        {
            options
                .UseNpgsql(dbConnectionString, op =>
                {
                    op.MigrationsAssembly(AssemblyRefrence.Assembly);
                    op.UseNetTopologySuite();
                })
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<PublishOutboxMessagesInterceptor>());
        });
        services.AddScoped<PublishOutboxMessagesInterceptor>();
        services.Configure<OutBoxOptions>(configuration.GetSection("Rents:OutBox"));
        services.Configure<InBoxOptions>(configuration.GetSection("Rents:InBox"));
        services.AddScoped<IDbConnectionFactory>(x => new DbConnectionFactory(dbConnectionString));
        services.AddScoped<RepositoryFactory>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<RentsDbContext>());
        // adding quartz for background jobs 
        services.AddQuartz();
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        services.ConfigureOptions<ConfigureProcessOutboxJob>();
        services.ConfigureOptions<ConfigureProcessInboxJob>();
        return services;
    }
}

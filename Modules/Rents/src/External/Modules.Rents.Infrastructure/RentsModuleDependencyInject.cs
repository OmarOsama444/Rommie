using Common.Application.DomainEvents.Extensions;
using Common.Application.IntegrationEvents;
using Common.Domain.IntragationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Modules.Rents.Application;
using Modules.Rents.Infrastructure.Inbox;
using Modules.Rents.Infrastructure.Outbox;
using Modules.Rents.Presentation;
using Rebus.Handlers;

namespace Modules.Rents.Infrastructure
{
    public static class RentsModuleDependencyInject
    {
        public static IServiceCollection AddRentsModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplication();
            services.AddInfrastructure(configuration);
            services.AddPresentation();

            #region Integration Events Subscription
            services.AddTransient<IHandleMessages<UserCreatedIntegrationEvent>, BaseIngtegrationEventHandler<UserCreatedIntegrationEvent>>();
            #endregion

            services.AddIntegrationEventHandlers();

            services.AddDomainEventHandlerDecorators(
                cfg =>
                    cfg.AddAssemblies(Application.AssemblyRefrence.Assembly)
                    .AddPipeline(typeof(OutboxIdempotentDomainEventHandlerDecorator<>)));

            return services;
        }

        private static void AddIntegrationEventHandlers(this IServiceCollection services)
        {
            Type[] integrationEventHandlers = Rents.Presentation.AssemblyRefrence.Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
                .ToArray();

            foreach (Type integrationEventHandler in integrationEventHandlers)
            {
                services.TryAddScoped(integrationEventHandler);

                Type integrationEvent = integrationEventHandler
                    .GetInterfaces()
                    .Single(i => i.IsGenericType)
                    .GetGenericArguments()
                    .Single();

                Type closedIdempotentHandler =
                    typeof(InboxIdempotentIntegrationEventHandlerDecorator<>).MakeGenericType(integrationEvent);

                services.Decorate(integrationEventHandler, closedIdempotentHandler);
            }
        }
    }
}

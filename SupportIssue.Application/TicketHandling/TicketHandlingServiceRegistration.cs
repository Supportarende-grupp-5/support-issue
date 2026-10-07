using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SupportIssue.Application.TicketHandling;

public static class TicketHandlingServiceRegistration
{
    public static IServiceCollection AddTicketHandlingApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddTransient<ITicketHandlingService, TicketHandlingService>();
        return services;
    }
}

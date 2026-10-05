using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SupportIssue.Application.TicketHandling;

namespace SupportIssue.Infrastructure.TicketHandling;

public static class TicketHandlingRepositoryRegistration
{
    public static IServiceCollection AddTicketHandlingInfrastructure(
        this IServiceCollection services, string ticketFilePath)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(ticketFilePath);
        var fullPath = Path.GetFullPath(ticketFilePath);
        services.TryAddSingleton<ITicketHandlingRepository>(
            _ => new JsonFileTicketHandlingRepository(fullPath));
        return services;
    }
}

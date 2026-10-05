using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SupportIssue.Presentation.ViewModels;
using SupportIssue.Presentation.Views;

namespace SupportIssue.Presentation.TicketHandling;

public static class TicketHandlingPresentationRegistration
{
    public static IServiceCollection AddTicketHandlingPresentation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddTransient<TicketDetailsViewModel>();
        services.TryAddTransient<TicketDetailsPage>();
        return services;
    }
}

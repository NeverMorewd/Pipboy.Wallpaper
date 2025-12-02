using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;

namespace Pipboy.Wallpaper.Framework;

public static class WpfApplicationLifetimeExtensions
{

#if NET6_0_OR_GREATER
    public static IServiceCollection AddWpfApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services)
#else
    public static IServiceCollection AddWpfApplication<TApplication>(this IServiceCollection services)
#endif
        where TApplication : System.Windows.Application =>
        services
            .AddSingleton(provider =>
            {
                var instance = ActivatorUtilities.CreateInstance<TApplication>(provider);
                (instance as IComponentConnector)?.InitializeComponent();
                return instance;
            })
            .AddSingleton<IHostLifetime, WpfApplicationLifetime<TApplication>>();

    public static void RunWpfApplication<TApplication>(this IHost host)
        where TApplication : System.Windows.Application
    {
        _ = host ?? throw new ArgumentNullException(nameof(host));
        var application = host.Services.GetRequiredService<TApplication>();
        var hostTask = host.RunAsync();
        Environment.ExitCode = application.Run();
        hostTask.GetAwaiter().GetResult();
    }
}

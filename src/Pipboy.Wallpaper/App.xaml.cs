using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pipboy.Wallpaper.ViewModels;
using ReactiveUI;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Pipboy.Wallpaper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IObserver<Exception>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    public App(IServiceProvider serviceProvider)
    {
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        PlatformRegistrationManager.SetRegistrationNamespaces(RegistrationNamespace.Wpf);
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetRequiredService<ILogger<App>>();

        RxApp.DefaultExceptionHandler = this;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        DispatcherUnhandledException += Dispatcher_UnhandledException;
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        _logger.LogDebug("starting...");
        base.OnStartup(e);
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        Current.MainWindow = mainWindow;
        Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        mainWindow.Show();
        _logger.LogDebug("started");
    }
    #region Exception Handlers
    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        _logger.LogError(e.Exception, "Unobserved task exception");
    }

    private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        _logger.LogError(e?.Exception, "Dispatcher unhandled exception");
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        _logger.LogError(e.ExceptionObject as Exception, $"Current domain unhandled exception,IsTerminating:{e.IsTerminating}");
    }

    public void OnCompleted()
    {
        _logger.LogInformation("ReactiveUI UnhandledErrorStream was Completed");
    }

    public void OnError(Exception error)
    {
        _logger.LogError(error, "ReactiveUI unhandled exception");
    }

    public void OnNext(Exception value)
    {
        _logger.LogError(value, "ReactiveUI unhandled exception");
    }
    #endregion
}

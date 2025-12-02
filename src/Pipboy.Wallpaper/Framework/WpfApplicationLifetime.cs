using Microsoft.Extensions.Hosting;

namespace Pipboy.Wallpaper.Framework;

public sealed class WpfApplicationLifetime<TApplication> : IHostLifetime
            where TApplication : System.Windows.Application
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly TaskCompletionSource<object> _applicationExited = new();
    private readonly TApplication _application;

    public WpfApplicationLifetime(IHostApplicationLifetime applicationLifetime, TApplication application)
    {
        _applicationLifetime = applicationLifetime;
        _application = application;
    }

    public async Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        var ready = new TaskCompletionSource<object>();
        using var registration = cancellationToken.Register(() => ready.TrySetCanceled(cancellationToken));
        _application.Startup += (_, _) => ready.TrySetResult(null!);
        _application.Exit += (_, _) =>
        {
            _applicationExited.TrySetResult(null!);
            _applicationLifetime.StopApplication();
        };
        var c = SynchronizationContext.Current;
        await ready.Task.ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _application.Dispatcher.BeginInvoke(() => _application.Shutdown());
        return _applicationExited.Task;
    }
}

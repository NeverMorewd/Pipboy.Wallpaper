using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pipboy.Wallpaper.Abstractions;
using Pipboy.Wallpaper.Framework;
using Pipboy.Wallpaper.Models;
using Pipboy.Wallpaper.Services;
using Serilog;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Pipboy.Wallpaper;

public static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        if (File.Exists(AppDataContext.ConfigFileName))
        {
            File.Copy(AppDataContext.ConfigFileName, Path.Combine(AppDataContext.Current.AppTempDirectory, "crt_config.json"), true);
        }


        AttachConsole(ATTACH_PARENT_PROCESS);
        var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile(Path.Combine(AppDataContext.Current.AppTempDirectory,"crt_config.json"), optional: true, reloadOnChange: true)
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .Build();

        string logsDirectory = Path.Combine(AppDataContext.Current.AppTempDirectory, "Logs");
        Directory.CreateDirectory(logsDirectory);

        string logFilePath = Path.Combine(logsDirectory, "log_.txt");

        bool enableDebug = config.GetValue<bool>("AppSettings:EnableDebugMode");
        Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Is(enableDebug ? Serilog.Events.LogEventLevel.Debug : Serilog.Events.LogEventLevel.Information)
             .Enrich.FromLogContext()
             .Enrich.WithProcessId()
             .Enrich.WithThreadId()
             .WriteTo.Console(outputTemplate:
                 "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ProcessId}] [{ThreadId}] {Message:lj}{NewLine}{Exception}")
             .WriteTo.File(
                 path: logFilePath,
                 fileSizeLimitBytes: 10 * 1024 * 1024,
                 rollOnFileSizeLimit: true,
                 rollingInterval: RollingInterval.Day,
                 retainedFileCountLimit: 14,
                 encoding: Encoding.UTF8,
                 outputTemplate:
                 "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ProcessId}] [{ThreadId}] {Message:lj}{NewLine}{Exception}")
             .CreateLogger();
        try
        {
            var host = CreateHostBuilder(args, config, enableDebug).Build();

            Run(host);
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly.");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
            FreeConsole();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration, bool enableDebug)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddConfiguration(configuration);

                Log.Information($"command params:{string.Join(" ", args)}");

                if (enableDebug)
                {
                    var fullConfig = configBuilder.Build();
                    var sb = new StringBuilder();
                    sb.AppendLine("Configuration:");
                    sb.AppendLine("====Begin====");
                    foreach (var kv in fullConfig.AsEnumerable().OrderBy(kv => kv.Key))
                    {
                        sb.AppendLine($"  {kv.Key}: {kv.Value}");
                    }
                    sb.AppendLine("====End====");
                    Log.Debug(sb.ToString());
                }
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<CrtOptionsDto>(context.Configuration.GetSection("CrtOptions"));
                services.Configure<TextOptionsDto>(context.Configuration.GetSection("TextOptions"));
                services.AddSingleton<IHostLifetime, WpfApplicationLifetime<App>>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<ICrtSettingsServiceFacade, CrtSettingsServiceFacade>()
                        .AddTransient<IScanlineSettingsService, ScanlineSettingsService>()
                        .AddTransient<IScanBeamSettingsService, ScanBeamSettingsService>()
                        .AddTransient<INoiseSettingsService, NoiseSettingsService>()
                        .AddSingleton<TextDataContext>()
                        .AddSingleton<CrtDataContext>();

                services.AddSingleton(provider =>
                {
                    var app = new App(provider);
                    app.InitializeComponent();
                    return app;
                });
            })
            .UseSerilog();

        return hostBuilder;
    }


    private static void Run(IHost host)
    {
        host.RunWpfApplication<App>();
    }

    internal const uint ATTACH_PARENT_PROCESS = 0xFFFFFFFF;

    [DllImport("kernel32.dll")]
    static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll")]
    static extern bool FreeConsole();
}
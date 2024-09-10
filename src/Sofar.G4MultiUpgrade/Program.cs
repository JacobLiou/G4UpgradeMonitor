using Serilog;

namespace Sofar.G4MultiUpgrade;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        string logTemplate =
            "[{Timestamp:yy/MM/dd HH:mm:ss.fff zzz}] [{Level:u3}] [PID:{ProcessId}] [TID:{ThreadId}] {Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .WriteTo.File(path: @"Logs/log_.log", rollingInterval: RollingInterval.Day, outputTemplate: logTemplate)
                .CreateLogger();

        ApplicationConfiguration.Initialize();
        Application.Run(new FrmMain());
    }
}
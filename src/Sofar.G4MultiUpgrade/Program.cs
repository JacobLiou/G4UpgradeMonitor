using Microsoft.VisualBasic.Logging;
using Serilog;
using System.Text;
using Log = Serilog.Log;

namespace Sofar.G4MultiUpgrade
{
    internal static class Program
    {
        private static FrmMain _frmMain;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
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

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            ApplicationConfiguration.Initialize();

            _frmMain = new FrmMain();
            Application.Run(_frmMain);
        }



        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            string msg = GetExceptionMessage(e.Exception, e.ToString());
            MessageBox.Show(msg, $"Running Error({_frmMain.Text})", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Serilog.Log.Error(msg);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string msg = GetExceptionMessage(e.ExceptionObject as Exception, e.ToString());
            MessageBox.Show(msg, $"Running Error({_frmMain.Text})", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Serilog.Log.Error(msg);
        }


        private static string GetExceptionMessage(Exception? ex, string? message)
        {
            StringBuilder sb = new StringBuilder();
            if (ex != null)
            {
                sb.AppendLine($"¡¾Type¡¿£º{ex.GetType().Name}");
                sb.AppendLine($"¡¾Message¡¿£º{ex.Message}");
                sb.AppendLine($"¡¾StackTrace¡¿£º{ex.StackTrace}");
            }
            else
            {
                sb.AppendLine($"¡¾Unhandled¡¿£º{message}");
            }

            return sb.ToString();
        }

    }
}
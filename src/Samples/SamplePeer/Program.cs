using System;
using System.Windows.Forms;

namespace SamplePeer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            var name = args.Length > 0 ? args[0] : null;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Run(new MainForm(name));
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = (Exception)e.ExceptionObject;
                var msg = $"Unhandled exception {ex.Message}";
                Console.WriteLine(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Unhandled exception {ex.Message}";
                Console.WriteLine(msg);
            }
        }
    }
}

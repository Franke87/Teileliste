using System;
using System.Threading;
using System.Windows;
using TeileListe.Classes;
using TeileListe.Common.View;

namespace TeileListe
{
    public partial class App
    {
        // ReSharper disable once NotAccessedField.Local
        private static Mutex _mutex;

        [STAThread]
        static void main()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        private void CheckForMutex(object sender, StartupEventArgs e)
        {
            const string appName = "TeileListe";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application  
                Shutdown();
            }

            if (!PluginManager.InitPlugins())
            {
                Shutdown();
            }
            else if(PluginManager.DbManager.KonvertierungErforderlich())
            {
                MainWindow = new UpdateWindow.View.UpdateWindow();
                var result = MainWindow.ShowDialog();

                if(result.HasValue && result.Value)
                {
                    MainWindow = new Teileliste.View.TeilelisteView();
                    MainWindow.ShowDialog();
                }
                else
                {
                    var message = "Das Update der Datenbank ist fehlgeschlagen.";
                    message += Environment.NewLine + Environment.NewLine;
                    message += "Die Anwendung beendet sich jetzt.";

                    MainWindow = new MyMessageBox("Teileliste", message, true);
                    MainWindow.ShowInTaskbar = true;
                    MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    MainWindow.ShowDialog();
                }
            }
            else
            {
                MainWindow = new Teileliste.View.TeilelisteView();
                MainWindow.ShowDialog();
            }

            Shutdown();
        }  
    }
}

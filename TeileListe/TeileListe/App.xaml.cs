﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using TeileListe.Classes;
using TeileListe.Common.View;
using TeileListe.Teileliste.View;

namespace TeileListe
{
    public partial class App
    {
        // ReSharper disable once NotAccessedField.Local
        private static Mutex _mutex;

        [DllImport("dwmapi.dll")]
        public static extern IntPtr DwmIsCompositionEnabled(out bool pfEnabled);

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

            _mutex = new Mutex(true, appName, out bool createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application  
                Shutdown();
            }

            if (DwmIsCompositionEnabled(out bool aeroEnabled) == IntPtr.Zero)
            {
                if (aeroEnabled)
                {
                    Current.Resources.Clear();
                }
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
                    MainWindow = new TeilelisteView();
                    MainWindow.ShowDialog();
                }
                else
                {
                    var message = "Das Update der Datenbank ist fehlgeschlagen.";
                    message += Environment.NewLine + Environment.NewLine;
                    message += "Die Anwendung beendet sich jetzt.";

                    MainWindow = new MyMessageBox("Teileliste", message, true)
                    {
                        ShowInTaskbar = true,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };
                    MainWindow.ShowDialog();
                }
            }
            else
            {
                MainWindow = new TeilelisteView();
                MainWindow.ShowDialog();
            }

            Shutdown();
        }  
    }
}

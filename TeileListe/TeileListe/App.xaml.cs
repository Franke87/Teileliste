using System.Threading;
using System.Windows;
using TeileListe.Classes;

namespace TeileListe
{
    public partial class App
    {
// ReSharper disable once NotAccessedField.Local
        private static Mutex _mutex;

        private void CheckForMutex(object sender, StartupEventArgs e)
        {
            const string appName = "TeileListe";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application  
                Current.Shutdown();
            }

            if (!PluginManager.InitPlugins())
            {
                Current.Shutdown();
            }
        }  
    }
}

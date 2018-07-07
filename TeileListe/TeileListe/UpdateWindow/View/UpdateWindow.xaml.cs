using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using TeileListe.Classes;

namespace TeileListe.UpdateWindow.View
{
    public partial class UpdateWindow : Window
    {
        private readonly BackgroundWorker _worker;
        public string WaitText { get; set; }

        public UpdateWindow()
        {
            InitializeComponent();
            DataContext = this;

            WaitText = "Update der Datenbank wird durchgeführt...";

            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = (bool?)e.Result;
            Close();
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = PluginManager.DbManager.Konvertiere();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(_worker.IsBusy)
            {
                e.Cancel = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                new Action(() => _worker.RunWorkerAsync()), DispatcherPriority.ApplicationIdle);
        }
    }
}

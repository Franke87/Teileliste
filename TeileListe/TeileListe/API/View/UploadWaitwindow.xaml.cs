using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using TeileListe.API.Classes;
using TeileListe.API.Helper;
using TeileListe.API.PostClasses;
using TeileListe.MessungHochladen.Dto;

namespace TeileListe.API.View
{
    public partial class UploadWaitwindow
    {
        private readonly BackgroundWorker _worker;
        public bool Success { get; set; }
        public string WaitText { get; set; }
        public string ResultProduktId { get; set; }
        public string ResultProduktUrl { get; set; }
        public string ErrorText;
        private readonly UploadApiEventArgs _eventArgs;

        public UploadWaitwindow(string datenbank, 
                                string apiToken, 
                                MessungHochladenDto messung,
                                ProduktHochladenDto produkt)
        {
            InitializeComponent();

            DataContext = this;

            WaitText = "Die Informationen werden an " + datenbank + " übertragen...";
            ErrorText = string.Empty;

            _eventArgs = new UploadApiEventArgs
            {
                ApiToken = apiToken, 
                Datenbank = datenbank, 
                Messung = messung, 
                Produkt = produkt
            };

             _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Success = (bool) e.Result;
            Close();
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var eventArgs = e.Argument as UploadApiEventArgs;
                if (eventArgs != null)
                {
                    var apiHandler = new ApiHandler();

                    if (eventArgs.Messung != null)
                    {
                        var result = apiHandler.SendMessung(eventArgs.Datenbank,
                                                            eventArgs.ApiToken,
                                                            eventArgs.Messung.ProduktId,
                                                            eventArgs.Messung.Gewicht,
                                                            eventArgs.Messung.ImageBase64);

                        if (result.Status == "OK")
                        {
                            e.Result = true;
                        }
                    }
                    else if (eventArgs.Produkt != null)
                    {
                        var dto = new AddProduktDto
                        {
                            Kategorie = eventArgs.Produkt.Kategorie,
                            Hersteller = eventArgs.Produkt.Hersteller,
                            Beschreibung = eventArgs.Produkt.Beschreibung,
                            Gewicht = eventArgs.Produkt.Gewicht,
                            GewichtHersteller = eventArgs.Produkt.GewichtHersteller,
                            Groesse = eventArgs.Produkt.Groesse,
                            ImageBase64 = eventArgs.Produkt.ImageBase64,
                            Jahr = eventArgs.Produkt.Jahr,
                            Kommentar = eventArgs.Produkt.Kommentar,
                            Link = eventArgs.Produkt.Link
                        };
                        var result = apiHandler.SendProdukt(eventArgs.Datenbank,
                                                            eventArgs.ApiToken,
                                                            dto);
                        if (result.Status == "OK")
                        {
                            ResultProduktId = String.Format("{0}", (int) result.Data.Produkt.ProduktId);
                            ResultProduktUrl = result.Data.Produkt.ProduktUrl;
                            e.Result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                e.Result = false;
                ErrorText = ex.Message;
            }
        }

        private void WaitWindowClosing(object sender, CancelEventArgs e)
        {
            if (_worker.IsBusy)
            {
                e.Cancel = true;
            }
        }

        private void WaitWindowLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                        new Action(() => _worker.RunWorkerAsync(_eventArgs)), DispatcherPriority.ApplicationIdle);
        }
    }
}
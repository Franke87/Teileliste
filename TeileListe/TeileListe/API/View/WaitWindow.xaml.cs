using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using TeileListe.API.Classes;
using TeileListe.API.Helper;
using TeileListe.API.ResponseClasses;
using TeileListe.Gewichtsdatenbanken.ViewModel;
using TeileListe.NeuesEinzelteil.ViewModel;

namespace TeileListe.API.View
{
    public partial class WaitWindow
    {
        private readonly BackgroundWorker _worker;
        public bool Success { get; set; }
        public string WaitText { get; set; }
        public ResponseHerstellerBaseDto ResultHerstellerDto { get; set; }
        public List<KategorienViewModel> ResultKategorienList { get; set; }
        public List<DatenbankteilAuswahlViewModel> ResultList { get; set; }
        public string ResultProduktString { get; set; }
        public string ResultProduktLink { get; set; }
        public string ErrorText;
        private readonly ApiEventArgs _eventArgs;

        public WaitWindow(string datenbank, 
                            string apiToken, 
                            string idHersteller, 
                            string idKategorie, 
                            string idProdukt)
        {
            InitializeComponent();

            DataContext = this;

            WaitText = "Die Informationen werden von " + datenbank + " abgerufen...";
            ResultHerstellerDto = new ResponseHerstellerBaseDto();
            ResultKategorienList = new List<KategorienViewModel>();
            ResultList = new List<DatenbankteilAuswahlViewModel>();
            ResultProduktString = "";
            ResultProduktLink = "";
            ErrorText = string.Empty;

            _eventArgs = new ApiEventArgs
            {
                ApiToken = apiToken, 
                Datenbank = datenbank, 
                HerstellerId = idHersteller, 
                KategorieId = idKategorie, 
                ProduktId = idProdukt
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
                var eventArgs = e.Argument as ApiEventArgs;
                if (eventArgs != null)
                {
                    var apiHandler = new ApiHandler();

                    if (!String.IsNullOrWhiteSpace(eventArgs.ProduktId))
                    {
                        var produkt = apiHandler.SucheEinzelnenArtikel(eventArgs.Datenbank, 
                                                                        eventArgs.ApiToken, 
                                                                        eventArgs.ProduktId);
                        ResultProduktString += produkt.Data.Produkt.Kategorie.Title.Trim() + Environment.NewLine;
                        ResultProduktString += produkt.Data.Produkt.Manufacturer.Name.Trim() + Environment.NewLine;
                        ResultProduktString += produkt.Data.Produkt.Name.Trim() + Environment.NewLine;
                        if (!string.IsNullOrWhiteSpace(produkt.Data.Produkt.Groesse))
                        {
                            ResultProduktString += produkt.Data.Produkt.Groesse.Trim() + Environment.NewLine;
                        }
                        else
                        {
                            ResultProduktString += Environment.NewLine;
                        }
                        if (!string.IsNullOrWhiteSpace(produkt.Data.Produkt.Jahr))
                        {
                            ResultProduktString += produkt.Data.Produkt.Jahr.Trim() + Environment.NewLine;
                        }
                        else
                        {
                            ResultProduktString += Environment.NewLine;
                        }
                        var converter = new Converter.IntToWeightConverter();
                        ResultProduktString += converter.Convert((int)produkt.Data.Produkt.Gewicht, null, null, null);

                        ResultProduktLink = produkt.Data.Produkt.Url.Trim();

                        e.Result = true;
                    }
                    else if (string.IsNullOrWhiteSpace(eventArgs.HerstellerId)
                        && string.IsNullOrWhiteSpace(eventArgs.KategorieId))
                    {
                        ResultHerstellerDto = apiHandler.GetHerstellerListe(eventArgs.Datenbank, eventArgs.ApiToken);
                        var resultDto = apiHandler.GetKategorienListe(eventArgs.Datenbank, eventArgs.ApiToken);
                        KonvertiereKategorien(resultDto);
                        e.Result = true;
                    }
                    else
                    {
                        bool herstellerMitnehmen = !string.IsNullOrWhiteSpace(eventArgs.HerstellerId);
                        bool kategorienMitnehmen = !string.IsNullOrWhiteSpace(eventArgs.KategorieId);
                        var produkteHersteller = new ResponseProduktListeDto();
                        var produkteKategorie = new ResponseProduktListeDto();
                        if (herstellerMitnehmen)
                        {
                            produkteHersteller = apiHandler.SucheArtikel(eventArgs.Datenbank,
                                                                                eventArgs.ApiToken,
                                                                                eventArgs.HerstellerId,
                                                                                true);
                        }
                        if (kategorienMitnehmen && !herstellerMitnehmen)
                        {
                            produkteKategorie = apiHandler.SucheArtikel(eventArgs.Datenbank,
                                eventArgs.ApiToken,
                                eventArgs.KategorieId,
                                false);
                        }

                        if (herstellerMitnehmen && kategorienMitnehmen)
                        {
                            foreach (var item in produkteHersteller.Data.Produkte)
                            {
                                if (item.Produkt.Kategorie.Id == eventArgs.KategorieId)
                                {
                                    var viewModel = new DatenbankteilAuswahlViewModel
                                    {
                                        Komponente = item.Produkt.Kategorie.Title,
                                        Hersteller = item.Produkt.Manufacturer.Name,
                                        DatenbankLink = item.Produkt.Url,
                                        DatenbankId = eventArgs.Datenbank + ":" + item.Produkt.Id,
                                        Beschreibung = item.Produkt.Name,
                                        Gewicht = (int) item.Produkt.Gewicht,
                                        Groesse = item.Produkt.Groesse,
                                        Jahr = item.Produkt.Jahr
                                    };
                                    ResultList.Add(viewModel);
                                }
                            }
                        }
                        else if (herstellerMitnehmen)
                        {
                            foreach (var item in produkteHersteller.Data.Produkte)
                            {
                                var viewModel = new DatenbankteilAuswahlViewModel
                                {
                                    Komponente = item.Produkt.Kategorie.Title,
                                    Hersteller = item.Produkt.Manufacturer.Name,
                                    DatenbankLink = item.Produkt.Url,
                                    DatenbankId = eventArgs.Datenbank + ":" + item.Produkt.Id,
                                    Beschreibung = item.Produkt.Name,
                                    Gewicht = (int) item.Produkt.Gewicht,
                                    Groesse = item.Produkt.Groesse,
                                    Jahr = item.Produkt.Jahr
                                };
                                ResultList.Add(viewModel);
                            }
                        }
                        else
                        {
                            foreach (var item in produkteKategorie.Data.Produkte)
                            {
                                var viewModel = new DatenbankteilAuswahlViewModel
                                {
                                    Komponente = item.Produkt.Kategorie.Title,
                                    Hersteller = item.Produkt.Manufacturer.Name,
                                    DatenbankLink = item.Produkt.Url,
                                    DatenbankId = eventArgs.Datenbank + ":" + item.Produkt.Id,
                                    Beschreibung = item.Produkt.Name,
                                    Gewicht = (int)item.Produkt.Gewicht,
                                    Groesse = item.Produkt.Groesse,
                                    Jahr = item.Produkt.Jahr
                                };
                                ResultList.Add(viewModel);
                            }
                        }
                        
                    
                        e.Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                e.Result = false;
                ErrorText = ex.Message;
            }
        }

        private void KonvertiereKategorien(ResponseKategorieBaseDto resultDto)
        {
            foreach (var item in resultDto.KategorienListe)
            {
                var viewModel = new KategorienViewModel
                {
                    Name = item.Title,
                    Id = item.KategorieId,
                    EnthaeltProdukte = item.EnthaeltProdukte, 
                    AnzahlProdukte = (int)item.AnzahlProdukte,
                    UnterKategorien = new ObservableCollection<KategorienViewModel>()
                };
                if (item.Unterkategorien.Count > 0)
                {
                    AddKategorie(ref viewModel, item);
                }
                ResultKategorienList.Add(viewModel);
            }
        }

        private void AddKategorie(ref KategorienViewModel viewModel, ResponseKategorieDto kategorie)
        {
            foreach (var subKategorie in kategorie.Unterkategorien)
            {
                var subViewModel = new KategorienViewModel
                {
                    Name = subKategorie.Title,
                    Id = subKategorie.KategorieId,
                    EnthaeltProdukte = subKategorie.EnthaeltProdukte, 
                    AnzahlProdukte = (int)subKategorie.AnzahlProdukte,
                    UnterKategorien = new ObservableCollection<KategorienViewModel>()
                };
                if (subKategorie.Unterkategorien.Count > 0)
                {
                    AddKategorie(ref subViewModel, subKategorie);
                }
                viewModel.UnterKategorien.Add(subViewModel);
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Enums;
using TeileListe.NeuesEinzelteil.View;
using TeileListe.NeuesEinzelteil.ViewModel;

namespace TeileListe.Restekiste.ViewModel
{
    internal class RestekisteViewModel : INotifyPropertyChanged
    {
        #region Commands

        public MyParameterCommand<Window> ExportCommand { get; set; }
        public MyCommand ZuruecksetzenCommand { get; set; }
        public MyCommand SichernCommand { get; set; }
        public MyParameterCommand<Window> HinzufuegenCommand { get; set; }

        #endregion

        #region Properties

        private readonly List<string> _deletedItems;

        private ObservableCollection<RestteilViewModel> _resteListe;
        public ObservableCollection<RestteilViewModel> ResteListe
        {
            get { return _resteListe; }
            set { SetRestekisteCollectionProperty("ResteListe", ref _resteListe, value); }
        }

        private bool _exportformatCsv;
        public bool ExportformatCsv
        {
            get { return _exportformatCsv; }
            set { SetRestekisteBoolProperty("ExportformatCsv", ref _exportformatCsv, value); }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { SetRestekisteBoolProperty("IsDirty", ref _isDirty, value); }
        }

        public string CustomExportKuerzel
        {
            get { return PluginManager.ExportManager.GetKuerzel(); }
        }

        #endregion

        #region Konstruktor

        internal RestekisteViewModel()
        {
            ResteListe = new ObservableCollection<RestteilViewModel>();
            _deletedItems = new List<string>();

            ExportformatCsv = true;

            ExportCommand = new MyParameterCommand<Window>(OnExport);
            ZuruecksetzenCommand = new MyCommand(Zureucksetzen);
            SichernCommand = new MyCommand(Sichern);
            HinzufuegenCommand = new MyParameterCommand<Window>(Hinzufuegen);

            IsDirty = false;

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(Zureucksetzen),
                                                        DispatcherPriority.ApplicationIdle);
        }

        #endregion

        #region Commandfunktionen

        private void OnExport(Window window)
        {
            if (ExportformatCsv)
            {
                try
                {
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var file = Path.Combine(path, "Restekiste.csv");
                    var i = 1;
                    while (File.Exists(file))
                    {
                        file = Path.Combine(path, string.Format("Restekiste ({0}).csv", i++));
                    }
                    using (var formatter = new CsvFormatter())
                    {
                        using (var sw = new StreamWriter(file,
                                                            false,
                                                            Encoding.Default))
                        {
                            sw.Write(formatter.GetFormattetRestekiste(ResteListe));
                        }
                        Process.Start(new ProcessStartInfo("explorer.exe")
                        {
                            Arguments = "/select, \"" + file + "\""
                        });
                    }
                }
                catch (IOException ex)
                {
                    HilfsFunktionen.ShowMessageBox(window, ex.Message, "Restekiste", true);
                }
            }
            else
            {
                var liste = ResteListe.Select(item => new EinzelteilExportDto
                                                    {
                                                        Guid = item.Guid,
                                                        Komponente = item.Komponente,
                                                        Hersteller = item.Hersteller,
                                                        Beschreibung = item.Beschreibung,
                                                        Groesse = item.Groesse,
                                                        Jahr = item.Jahr,
                                                        DatenbankId = item.DatenbankId,
                                                        DatenbankLink = item.DatenbankLink,
                                                        Preis = item.Preis,
                                                        Gewicht = item.Gewicht, 
                                                        DokumentenListe = new List<DateiDto>()
                                                    }).ToList();

                foreach (var item in liste)
                {
                    var dateiListe = new List<DateiDto>();
                    PluginManager.DbManager.GetDateiInfos(item.Guid, ref dateiListe);
                    item.DokumentenListe.AddRange(dateiListe);
                }

                var csvExport = "";

                using (var formatter = new CsvFormatter())
                {
                    csvExport = formatter.GetFormattetRestekiste(ResteListe);

                }

                PluginManager.ExportManager.ExportKomponenten(new WindowInteropHelper(window).Handle,
                                                                "Restekiste",
                                                                csvExport,
                                                                liste);
            }
        }

        public void Zureucksetzen()
        {
            ResteListe.Clear();
            _deletedItems.Clear();

            var teileliste = new List<RestteilDto>();
            PluginManager.DbManager.GetEinzelteile(ref teileliste);
            foreach (var item in teileliste)
            {
                var viewModel = new RestteilViewModel(item)
                {
                    LoeschenAction = Loeschen,
                    NachObenAction = NachObenSortieren,
                    NachUntenAction = NachUntenSortieren
                };
                viewModel.PropertyChanged += ContentPropertyChanged;
                ResteListe.Add(viewModel);
            }

            IsDirty = false;
        }

        public void Sichern()
        {
            PluginManager.DbManager.DeleteEinzelteile(_deletedItems);
            _deletedItems.Clear();
            PluginManager.DbManager.SaveEinzelteile(ResteListe.Select(item => new RestteilDto
                                    {
                                        Guid = item.Guid,
                                        Komponente = item.Komponente, 
                                        Hersteller = item.Hersteller,
                                        Beschreibung = item.Beschreibung, 
                                        Groesse = item.Groesse,
                                        Jahr = item.Jahr,
                                        DatenbankId = item.DatenbankId,
                                        DatenbankLink = item.DatenbankLink,
                                        Preis = item.Preis,
                                        Gewicht = item.Gewicht
                                    }).ToList());

            IsDirty = false;
        }

        private void Hinzufuegen(Window window)
        {
            var dialog = new NeuesEinzelteilDialog
            {
                Top = window.Top + 40,
                Left = window.Left + (window.ActualWidth - 600)/2,
                Owner = window
            };
            var viewModel = new NeuesEinzelteilViewModel(EinzelteilBearbeitenEnum.Restteil, 
                                                            new List<EinzelteilAuswahlViewModel>(), 
                                                            new List<WunschteilAuswahlViewModel>())
            {
                CloseAction = dialog.Close
            };
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (viewModel.IsOk)
            {
                switch (viewModel.Auswahl)
                {
                    case SourceEnum.NeuesEinzelteil:
                    {
                        var neuesEinzelteil = new RestteilViewModel(new RestteilDto
                        {
                            Guid = Guid.NewGuid().ToString(),
                            Komponente = viewModel.NeuViewModel.Komponente,
                            Hersteller = viewModel.NeuViewModel.Hersteller,
                            Beschreibung = viewModel.NeuViewModel.Beschreibung,
                            Groesse = viewModel.NeuViewModel.Groesse,
                            Jahr = viewModel.NeuViewModel.Jahr,
                            DatenbankId = "",
                            DatenbankLink = "",
                            Preis = viewModel.NeuViewModel.Preis,
                            Gewicht = viewModel.NeuViewModel.Gewicht
                        })
                        {
                            NachObenAction = NachObenSortieren,
                            NachUntenAction = NachUntenSortieren,
                            LoeschenAction = Loeschen
                        };
                        neuesEinzelteil.PropertyChanged += ContentPropertyChanged;
                        ResteListe.Add(neuesEinzelteil);

                        break;
                    }
                    case SourceEnum.AusDatei:
                    {
                        var importer = new TeileImporter();
                        foreach (var item in importer.ImportEinzelteile(viewModel.DateiViewModel.Datei))
                        {
                            var neuesEinzelteil = new RestteilViewModel(item)
                            {
                                NachObenAction = NachObenSortieren,
                                NachUntenAction = NachUntenSortieren,
                                LoeschenAction = Loeschen,
                            };
                            neuesEinzelteil.PropertyChanged += ContentPropertyChanged;
                            ResteListe.Add(neuesEinzelteil);
                        }

                        break;
                    }
                    case SourceEnum.AusGewichtsdatenbank:
                    {
                        foreach (var teil in viewModel.DatenbankViewModel.Datenbankteile.Where(teil => teil.IsChecked).ToList())
                        {
                            if (teil.IsChecked)
                            {
                                var neuesEinzelteil = new RestteilViewModel(new RestteilDto
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    Komponente = teil.Komponente,
                                    Hersteller = teil.Hersteller,
                                    Beschreibung = teil.Beschreibung,
                                    Groesse = teil.Groesse,
                                    Jahr = teil.Jahr,
                                    DatenbankId = teil.DatenbankId,
                                    DatenbankLink = teil.DatenbankLink,
                                    Preis = 0,
                                    Gewicht = teil.Gewicht
                                })
                                {
                                    NachObenAction = NachObenSortieren,
                                    NachUntenAction = NachUntenSortieren,
                                    LoeschenAction = Loeschen
                                };
                                neuesEinzelteil.PropertyChanged += ContentPropertyChanged;
                                ResteListe.Add(neuesEinzelteil);
                            }
                        }
                        break;
                    }
                }

                IsDirty = true;
            }
        }

        #endregion

        #region Actionfunktionen

        private void Loeschen(string guid)
        {
            var item = ResteListe.First(teil => teil.Guid == guid);
            _deletedItems.Add(item.Guid);
            ResteListe.Remove(item);
            IsDirty = true;
        }

        public void NachObenSortieren(string guid)
        {
            var teil1 = ResteListe.First(teil => teil.Guid == guid);
            if (teil1 != null && ResteListe.IndexOf(teil1) + 1 > 1)
            {
                var teil2 = ResteListe[ResteListe.IndexOf(teil1) - 1];

                if (teil2 != null)
                {
                    ResteListe.Move(ResteListe.IndexOf(teil1), ResteListe.IndexOf(teil2));
                    IsDirty = true;
                }
            }
        }

        public void NachUntenSortieren(string guid)
        {
            var teil1 = ResteListe.First(teil => teil.Guid == guid);

            if (teil1 != null && ResteListe.IndexOf(teil1) + 1 < ResteListe.Count)
            {
                var teil2 = ResteListe[ResteListe.IndexOf(teil1) + 1];

                if (teil2 != null)
                {
                    ResteListe.Move(ResteListe.IndexOf(teil1), ResteListe.IndexOf(teil2));
                    IsDirty = true;
                }
            }
        }

        #endregion

        #region Hilfsfunktionen

        public void OnClosing(object sender, CancelEventArgs e)
        {
            if (IsDirty)
            {
                var window = sender as Window;
                var owner = window ?? Application.Current.MainWindow;
                if (HilfsFunktionen.ShowQuestionBox(owner, "Restekiste"))
                {
                    Sichern();
                }
            }
        }

        public void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetRestekisteIntProperty(string propertyName, ref int backingField, int newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        internal void SetRestekisteCollectionProperty(string propertyName, 
                                                        ref ObservableCollection<RestteilViewModel> backingField, 
                                                        ObservableCollection<RestteilViewModel> newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        internal void SetRestekisteBoolProperty(string propertyName, ref bool backingField, bool newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        #endregion
    }
}

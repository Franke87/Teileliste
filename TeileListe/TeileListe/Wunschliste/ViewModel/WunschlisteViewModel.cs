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
using TeileListe.Common.Interface;
using TeileListe.Enums;
using TeileListe.NeuesEinzelteil.View;
using TeileListe.NeuesEinzelteil.ViewModel;

namespace TeileListe.Wunschliste.ViewModel
{
    internal class WunschlisteViewModel : INotifyPropertyChanged
    {
        #region Commands

        public MyParameterCommand<Window> ExportCommand { get; set; }
        public MyCommand ZuruecksetzenCommand { get; set; }
        public MyCommand SichernCommand { get; set; }
        public MyParameterCommand<Window> HinzufuegenCommand { get; set; }

        #endregion

        #region Properties

        private readonly List<LoeschenDto> _deletedItems;

        private ObservableCollection<WunschteilViewModel> _wunschliste;
        public ObservableCollection<WunschteilViewModel> Wunschliste
        {
            get { return _wunschliste; }
            set { SetWunschlisteCollectionProperty("Wunschliste", ref _wunschliste, value); }
        }

        private bool _exportformatCsv;
        public bool ExportformatCsv
        {
            get { return _exportformatCsv; }
            set { SetWunschlisteBoolProperty("ExportformatCsv", ref _exportformatCsv, value); }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { SetWunschlisteBoolProperty("IsDirty", ref _isDirty, value); }
        }

        public string CustomExportKuerzel
        {
            get { return PluginManager.ExportManager.GetKuerzel(); }
        }

        #endregion

        #region Konstruktor

        internal WunschlisteViewModel()
        {
            Wunschliste = new ObservableCollection<WunschteilViewModel>();
            _deletedItems = new List<LoeschenDto>();

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
                    var file = Path.Combine(path, "Wunschliste.csv");
                    var i = 1;
                    while (File.Exists(file))
                    {
                        file = Path.Combine(path, string.Format("Wunschliste ({0}).csv", i++));
                    }
                    using (var formatter = new CsvFormatter())
                    {
                        using (var sw = new StreamWriter(file,
                                                            false,
                                                            Encoding.Default))
                        {
                            sw.Write(formatter.GetFormattetWunschliste(Wunschliste));
                        }
                        Process.Start(new ProcessStartInfo("explorer.exe")
                        {
                            Arguments = "/select, \"" + file + "\""
                        });
                    }
                }
                catch (IOException ex)
                {
                    HilfsFunktionen.ShowMessageBox(window, ex.Message, "Wunschliste", true);
                }
            }
            else
            {
                var liste = Wunschliste.Select(item => new EinzelteilExportDto
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
                    csvExport = formatter.GetFormattetWunschliste(Wunschliste);
                }

                PluginManager.ExportManager.ExportKomponenten(new WindowInteropHelper(window).Handle,
                                                                "Wunschliste",
                                                                csvExport,
                                                                liste);
            }
        }

        public void Zureucksetzen()
        {
            Wunschliste.Clear();
            _deletedItems.Clear();

            var teileliste = new List<WunschteilDto>();

            PluginManager.DbManager.GetWunschteile(ref teileliste);

            foreach (var item in teileliste)
            {
                var viewModel = new WunschteilViewModel(item)
                {
                    LoeschenAction = Loeschen,
                    NachObenAction = NachObenSortieren,
                    NachUntenAction = NachUntenSortieren
                };
                viewModel.PropertyChanged += ContentPropertyChanged;
                Wunschliste.Add(viewModel);
            }

            IsDirty = false;
        }

        public void Sichern()
        {
            PluginManager.DbManager.DeleteWunschteile(_deletedItems);
            _deletedItems.Clear();
            PluginManager.DbManager.SaveWunschteile(Wunschliste.Select(item => new WunschteilDto
                                    {
                                        Guid = item.Guid,
                                        Komponente = item.Komponente, 
                                        Hersteller = item.Hersteller,
                                        Beschreibung = item.Beschreibung, 
                                        Groesse = item.Groesse,
                                        Jahr = item.Jahr,
                                        Shop = item.Shop, 
                                        Link = item.Link, 
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
                Left = window.Left + (window.ActualWidth - 600) / 2,
                Owner = window
            };
            var viewModel = new NeuesEinzelteilViewModel(EinzelteilBearbeitenEnum.Wunschteil, 
                                                            new List<EinzelteilAuswahlViewModel>(), 
                                                            new List<WunschteilAuswahlViewModel>())
            {
                CloseAction = dialog.Close
            };
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (viewModel.IsOk)
            {
                switch(viewModel.Auswahl)
                {
                    case SourceEnum.NeuesEinzelteil:
                    {
                        var neuesWunschteil = new WunschteilViewModel(new WunschteilDto
                        {
                            Guid = Guid.NewGuid().ToString(),
                            Komponente = viewModel.NeuViewModel.Komponente,
                            Hersteller = viewModel.NeuViewModel.Hersteller,
                            Beschreibung = viewModel.NeuViewModel.Beschreibung,
                            Groesse = viewModel.NeuViewModel.Groesse,
                            Jahr = viewModel.NeuViewModel.Jahr,
                            Shop = viewModel.NeuViewModel.Shop,
                            Link = viewModel.NeuViewModel.Link,
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
                        neuesWunschteil.PropertyChanged += ContentPropertyChanged;
                        Wunschliste.Add(neuesWunschteil);
                        break;
                    }
                    case SourceEnum.AusDatei:
                    {
                        var importer = new TeileImporter();
                        foreach (var item in importer.ImportWunschteile(viewModel.DateiViewModel.Datei))
                        {
                            var neuesWusnchteil = new WunschteilViewModel(item)
                            {
                                NachObenAction = NachObenSortieren,
                                NachUntenAction = NachUntenSortieren,
                                LoeschenAction = Loeschen, 
                            };
                            neuesWusnchteil.PropertyChanged += ContentPropertyChanged;
                            Wunschliste.Add(neuesWusnchteil);
                        }
                        break;
                    }
                    case SourceEnum.AusGewichtsdatenbank:
                    {
                        foreach (var teil in viewModel.DatenbankViewModel.Datenbankteile.Where(teil => teil.IsChecked).ToList())
                        {
                            if (teil.IsChecked)
                            {
                                var neuesWusnchteil = new WunschteilViewModel(new WunschteilDto
                                {
                                    Guid = Guid.NewGuid().ToString(),
                                    Komponente = teil.Komponente,
                                    Hersteller = teil.Hersteller,
                                    Beschreibung = teil.Beschreibung,
                                    Groesse = teil.Groesse,
                                    Jahr = teil.Jahr,
                                    Shop = "", 
                                    Link = "",
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
                                neuesWusnchteil.PropertyChanged += ContentPropertyChanged;
                                Wunschliste.Add(neuesWusnchteil);
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
            var item = Wunschliste.First(teil => teil.Guid == guid);
            _deletedItems.Add(new LoeschenDto { Guid = item.Guid, DokumenteLoeschen = true });
            Wunschliste.Remove(item);
            IsDirty = true;
        }

        public void NachObenSortieren(string guid)
        {
            var teil1 = Wunschliste.First(teil => teil.Guid == guid);
            if (teil1 != null && Wunschliste.IndexOf(teil1) + 1 > 1)
            {
                var teil2 = Wunschliste[Wunschliste.IndexOf(teil1) - 1];

                if (teil2 != null)
                {
                    Wunschliste.Move(Wunschliste.IndexOf(teil1), Wunschliste.IndexOf(teil2));
                    IsDirty = true;
                }
            }
        }

        public void NachUntenSortieren(string guid)
        {
            var teil1 = Wunschliste.First(teil => teil.Guid == guid);

            if (teil1 != null && Wunschliste.IndexOf(teil1) + 1 < Wunschliste.Count)
            {
                var teil2 = Wunschliste[Wunschliste.IndexOf(teil1) + 1];

                if (teil2 != null)
                {
                    Wunschliste.Move(Wunschliste.IndexOf(teil1), Wunschliste.IndexOf(teil2));
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
                if (HilfsFunktionen.ShowQuestionBox(owner, "Wunschliste"))
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

        internal void SetWunschlisteIntProperty(string propertyName, ref int backingField, int newValue)
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

        internal void SetWunschlisteCollectionProperty(string propertyName,
                                                        ref ObservableCollection<WunschteilViewModel> backingField,
                                                        ObservableCollection<WunschteilViewModel> newValue)
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

        internal void SetWunschlisteBoolProperty(string propertyName, ref bool backingField, bool newValue)
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

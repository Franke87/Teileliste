﻿using System;
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
    internal class WunschlisteViewModel : MyCommonViewModel
    {
        #region Commands

        public MyParameterCommand<Window> ExportCommand { get; set; }
        public MyCommand ZuruecksetzenCommand { get; set; }
        public MyCommand SichernCommand { get; set; }
        public MyParameterCommand<Window> HinzufuegenCommand { get; set; }

        #endregion

        #region Properties

        private readonly List<LoeschenDto> _deletedItems;
        private readonly List<Tuple<string, List<DateiDto>>> _dateiCache;

        private ObservableCollection<WunschteilViewModel> _wunschliste;
        public ObservableCollection<WunschteilViewModel> Wunschliste
        {
            get { return _wunschliste; }
            set { SetProperty("Wunschliste", ref _wunschliste, value); }
        }

        private bool _exportformatCsv;
        public bool ExportformatCsv
        {
            get { return _exportformatCsv; }
            set { SetProperty("ExportformatCsv", ref _exportformatCsv, value); }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { SetProperty("IsDirty", ref _isDirty, value); }
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
            _dateiCache = new List<Tuple<string, List<DateiDto>>>();

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

                    using (var sw = new StreamWriter(file, false, Encoding.Default))
                    {
                        sw.Write(CsvFormatter.GetFormattetWunschliste(Wunschliste));
                    }

                    Process.Start(new ProcessStartInfo("explorer.exe")
                    {
                        Arguments = "/select, \"" + file + "\""
                    });
                }
                catch (IOException ex)
                {
                    HilfsFunktionen.ShowMessageBox(window, "Wunschliste", ex.Message, true);
                }
            }
            else
            {
                var liste = new List<EinzelteilExportDto>();
                
                foreach (var item in Wunschliste)
                {
                    var wunschteil = new EinzelteilExportDto
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
                    };

                    if (item.IsNeueKomponente)
                    {
                        var cachedItem = _dateiCache.FirstOrDefault(teil => teil.Item1 == wunschteil.Guid);
                        if (cachedItem != null)
                        {
                            wunschteil.DokumentenListe.AddRange(cachedItem.Item2);
                        }
                    }
                    else
                    {
                        var dateiListe = new List<DateiDto>();
                        PluginManager.DbManager.GetDateiInfos(item.Guid, ref dateiListe);
                        wunschteil.DokumentenListe.AddRange(dateiListe);
                    }

                    liste.Add(wunschteil);
                }

                PluginManager.ExportManager.ExportKomponenten(new WindowInteropHelper(window).Handle,
                                                                "Wunschliste",
                                                                CsvFormatter.GetFormattetWunschliste(Wunschliste),
                                                                liste);
            }
        }

        public void Zureucksetzen()
        {
            Wunschliste.Clear();
            _deletedItems.Clear();

            foreach (var item in _dateiCache)
            {
                foreach (var file in item.Item2)
                {
                    try
                    {
                        File.Delete(Path.Combine("Daten", "Temp", file.Guid + "." + file.Dateiendung));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            _dateiCache.Clear();

            var teileliste = new List<WunschteilDto>();

            PluginManager.DbManager.GetWunschteile(ref teileliste);

            foreach (var item in teileliste)
            {
                var viewModel = new WunschteilViewModel(item)
                {
                    LoeschenAction = Loeschen,
                    NachObenAction = NachObenSortieren,
                    NachUntenAction = NachUntenSortieren,
                    GetDateiCacheFunc = GetDateiCache,
                    SaveDateiCache = AktualisiereDateiCache
                };
                viewModel.PropertyChanged += ContentPropertyChanged;
                Wunschliste.Add(viewModel);
            }

            IsDirty = false;
        }

        public void Sichern()
        {
            PluginManager.DbManager.DeleteTeile(_deletedItems);
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

            foreach (var item in _dateiCache)
            {
                PluginManager.DbManager.SaveDateiInfos(item.Item1, item.Item2);
            }

            foreach (var item in Wunschliste)
            {
                item.IsNeueKomponente = false;
            }

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
                            LoeschenAction = Loeschen,
                            GetDateiCacheFunc = GetDateiCache,
                            SaveDateiCache = AktualisiereDateiCache
                        };
                        neuesWunschteil.PropertyChanged += ContentPropertyChanged;
                        neuesWunschteil.IsNeueKomponente = true;
                        Wunschliste.Add(neuesWunschteil);
                        
                        break;
                    }
                    case SourceEnum.AusDatei:
                    {
                        var importer = new TeileImporter();
                        foreach (var item in importer.ImportWunschteile(viewModel.DateiViewModel.Datei))
                        {
                            var neuesWunschteil = new WunschteilViewModel(item)
                            {
                                NachObenAction = NachObenSortieren,
                                NachUntenAction = NachUntenSortieren,
                                LoeschenAction = Loeschen,
                                GetDateiCacheFunc = GetDateiCache,
                                SaveDateiCache = AktualisiereDateiCache
                            };
                            neuesWunschteil.PropertyChanged += ContentPropertyChanged;
                            neuesWunschteil.IsNeueKomponente = true;
                            Wunschliste.Add(neuesWunschteil);
                        }
                        _dateiCache.AddRange(importer.DateiCache);

                        break;
                    }
                    case SourceEnum.AusGewichtsdatenbank:
                    {
                        foreach (var teil in viewModel.DatenbankViewModel.Datenbankteile.Where(teil => teil.IsChecked).ToList())
                        {
                            if (teil.IsChecked)
                            {
                                var neuesWunschteil = new WunschteilViewModel(new WunschteilDto
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
                                    LoeschenAction = Loeschen,
                                    GetDateiCacheFunc = GetDateiCache,
                                    SaveDateiCache = AktualisiereDateiCache
                                };
                                neuesWunschteil.PropertyChanged += ContentPropertyChanged;
                                neuesWunschteil.IsNeueKomponente = true;
                                Wunschliste.Add(neuesWunschteil);
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
            var datei = _dateiCache.FirstOrDefault(teil => teil.Item1 == guid);
            if (datei != null)
            {
                foreach (var file in datei.Item2)
                {
                    try
                    {
                        File.Delete(Path.Combine("Daten", "Temp", file.Guid + "." + file.Dateiendung));
                    }
                    catch (Exception)
                    {
                    }
                }
                _dateiCache.Remove(datei);
            }

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

        public void AktualisiereDateiCache(string guid, List<DateiDto> cache)
        {
            var item = _dateiCache.FirstOrDefault(komponente => komponente.Item1 == guid);

            if (item != null)
            {
                item.Item2.Clear();
                item.Item2.AddRange(cache);
            }
            else
            {
                _dateiCache.Add(new Tuple<string, List<DateiDto>>(guid, cache));
            }
        }

        public List<DateiDto> GetDateiCache(string guid)
        {
            var cache = _dateiCache.FirstOrDefault(item => item.Item1 == guid);

            if (cache != null)
            {
                return cache.Item2;
            }

            return new List<DateiDto>();
        }

        #endregion

        #region Hilfsfunktionen

        public void OnClosing(object sender, CancelEventArgs e)
        {
            if (IsDirty)
            {
                var window = sender as Window;
                var owner = window ?? Application.Current.MainWindow;
                if (HilfsFunktionen.ShowCloseQuestionBox(owner, "Wunschliste"))
                {
                    Sichern();
                }
                else
                {
                    foreach (var item in _dateiCache)
                    {
                        foreach (var file in item.Item2)
                        {
                            try
                            {
                                File.Delete(Path.Combine("Daten", "Temp", file.Guid + "." + file.Dateiendung));
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
        }

        public void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }

        #endregion

    }
}

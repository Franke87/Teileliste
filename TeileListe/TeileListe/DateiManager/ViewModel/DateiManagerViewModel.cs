﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.DateiManager.View;

namespace TeileListe.DateiManager.ViewModel
{
    internal class DateiManagerViewModel : MyCommonViewModel
    {
        #region Properties

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

        private ObservableCollection<DokumentViewModel> _dateiListe;
        public ObservableCollection<DokumentViewModel> DateiListe
        {
            get { return _dateiListe; }
            set { SetProperty("DateiListe", ref _dateiListe, value); }
        }

        private readonly bool _isCachedKomponente;
        public List<DateiDto> DateiCache { get; set; }

        private string _komponenteGuid;
        private string _komponenteKomponente;
        private string _komponenteHersteller;
        private string _komponenteBeschreibung;

        private readonly List<string> _deletedItems;
        
        public MyParameterCommand<Window> SichernCommand { get; set; }
        public MyParameterCommand<Window> HinzufuegenCommand { get; set; }
        public MyParameterCommand<Window> ExportCommand { get; set; }
        public MyCommand ZuruecksetzenCommand { get; set; }

        #endregion

        #region Konstruktor

        internal DateiManagerViewModel(string guid,
            string komponente,
            string hersteller,
            string beschreibung,
            bool isCachedKomponente,
            List<DateiDto> dateiListe)
        {
            DateiListe = new ObservableCollection<DokumentViewModel>();
            _deletedItems = new List<string>();

            SichernCommand = new MyParameterCommand<Window>(SichernVoid);
            HinzufuegenCommand = new MyParameterCommand<Window>(OnHinzufuegen);
            ExportCommand = new MyParameterCommand<Window>(OnExport);
            ZuruecksetzenCommand = new MyCommand(Zuruecksetzen);

            _komponenteGuid = guid;
            _komponenteKomponente = komponente;
            _komponenteHersteller = hersteller;
            _komponenteBeschreibung = beschreibung;

            _isCachedKomponente = isCachedKomponente;

            if (_isCachedKomponente)
            {
                DateiCache = new List<DateiDto>(dateiListe);

                foreach (var item in DateiCache)
                {
                    var viewModel = new DokumentViewModel(_komponenteGuid, item)
                    {
                        NachObenAction = NachObenSortieren,
                        NachUntenAction = NachUntenSortieren,
                        LoeschenAction = Loeschen
                    };
                    viewModel.PropertyChanged += ContentPropertyChanged;

                    DateiListe.Add(viewModel);
                }
            }
            else
            {
                DateiCache = new List<DateiDto>();

                if (Directory.Exists(Path.Combine("Daten", _komponenteGuid)))
                {
                    var liste = new List<DateiDto>();
                    PluginManager.DbManager.GetDateiInfos(_komponenteGuid, ref liste);

                    foreach (var datei in liste)
                    {
                        var viewModel = new DokumentViewModel(_komponenteGuid, datei)
                        {
                            NachObenAction = NachObenSortieren,
                            NachUntenAction = NachUntenSortieren,
                            LoeschenAction = Loeschen
                        };
                        viewModel.PropertyChanged += ContentPropertyChanged;

                        DateiListe.Add(viewModel);
                    }
                }
            }

            IsDirty = false;
        }

        #endregion

        #region Funktionen

        private void OnExport(Window window)
        {
            var item = new EinzelteilExportDto();
            item.Komponente = _komponenteKomponente;
            item.Hersteller = _komponenteHersteller;
            item.Beschreibung = _komponenteBeschreibung;
            item.Guid = _komponenteGuid;
            item.DokumentenListe = new List<DateiDto>();

            foreach(var file in DateiListe)
            {
                item.DokumentenListe.Add(new DateiDto
                                            {
                                                Guid = file.Guid,
                                                Beschreibung = file.Beschreibung,
                                                Dateiendung = file.Dateiendung,
                                                Kategorie = file.Kategorie
                                            });
            }

            PluginManager.ExportManager.ExportKomponenten(new WindowInteropHelper(window).Handle,
                                                                _komponenteKomponente,
                                                                "",
                                                                new List<EinzelteilExportDto>() { item });
        }

        private void OnHinzufuegen(Window window)
        {
            var dialog = new NeuesDokumentDialog()
            {
                Top = window.Top + 40,
                Left = window.Left + (window.ActualWidth - 505) / 2,
                Owner = window
            };

            var viewModel = new DokumentBearbeitenViewModel("", true)
            {
                CloseAction = dialog.Close
            };
            
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (viewModel.IsOk)
            {
                try
                {
                    var datei = viewModel.DateiViewModel.Datei;
                    var dateiendung = Path.GetExtension(datei);
                    if(!string.IsNullOrWhiteSpace(dateiendung))
                    {
                        if(dateiendung.Substring(0,1) == ".")
                        {
                            dateiendung = dateiendung.Substring(1);
                        }
                    }

                    dateiendung = dateiendung.ToLower();

                    var guid = Guid.NewGuid().ToString();

                    File.Copy(datei, "Daten\\Temp\\" + guid + "." + dateiendung);

                    var newItem = new DokumentViewModel(_komponenteGuid, new DateiDto()
                    {
                        Guid = guid,
                        Beschreibung = viewModel.Beschreibung,
                        Kategorie = viewModel.SelectedKategorie,
                        Dateiendung = dateiendung
                    })
                    {
                        NachObenAction = NachObenSortieren,
                        NachUntenAction = NachUntenSortieren,
                        LoeschenAction = Loeschen
                    };
                    newItem.PropertyChanged += ContentPropertyChanged;

                    DateiListe.Add(newItem);

                    IsDirty = true;
                }
                catch(Exception ex)
                {
                    var message = "Die Datei konnte nicht kopiert werden.";

                    if (!string.IsNullOrWhiteSpace(ex.Message))
                    {
                        message += Environment.NewLine + Environment.NewLine;
                        message += ex.Message;
                    }

                    HilfsFunktionen.ShowMessageBox(window, "Dateimanager", message, true);
                }
            }
        }

        public void Zuruecksetzen()
        {
            DateiListe.Clear();
            _deletedItems.Clear();

            var teileliste = new List<DateiDto>();
            if (_isCachedKomponente)
            {
                foreach (var item in DateiCache)
                {
                    var viewModel = new DokumentViewModel(_komponenteGuid, item)
                    {
                        NachObenAction = NachObenSortieren,
                        NachUntenAction = NachUntenSortieren,
                        LoeschenAction = Loeschen
                    };
                    viewModel.PropertyChanged += ContentPropertyChanged;

                    DateiListe.Add(viewModel);
                }
            }
            else
            {
                PluginManager.DbManager.GetDateiInfos(_komponenteGuid, ref teileliste);
                foreach (var item in teileliste)
                {
                    var viewModel = new DokumentViewModel(_komponenteGuid, item)
                    {
                        LoeschenAction = Loeschen,
                        NachObenAction = NachObenSortieren,
                        NachUntenAction = NachUntenSortieren
                    };
                    viewModel.PropertyChanged += ContentPropertyChanged;
                    DateiListe.Add(viewModel);
                }
            }

            IsDirty = false;
        }

        public bool Sichern(Window window)
        {
            var bReturn = true;

            try
            {
                if (_isCachedKomponente)
                {
                    foreach(var item in _deletedItems)
                    {
                        var deletedItem = DateiCache.FirstOrDefault(teil => teil.Guid == item);
                        if(deletedItem != null)
                        {
                            try
                            {
                                File.Delete(Path.Combine("Daten", "Temp", deletedItem.Guid + "." + deletedItem.Dateiendung));
                            }
                            catch(Exception)
                            {
                            }
                        }
                    }
                    DateiCache.Clear();

                    foreach(var item in DateiListe)
                    {
                        DateiCache.Add(new DateiDto
                                        {
                                            Guid = item.Guid,
                                            Kategorie = item.Kategorie,
                                            Beschreibung = item.Beschreibung,
                                            Dateiendung = item.Dateiendung
                                        });
                    }
                }
                else
                {
                    PluginManager.DbManager.DeleteDateiInfos(_komponenteGuid, _deletedItems);
                    _deletedItems.Clear();
                    PluginManager.DbManager.SaveDateiInfos(_komponenteGuid, DateiListe.Select(item => new DateiDto
                    {
                        Guid = item.Guid,
                        Kategorie = item.Kategorie,
                        Beschreibung = item.Beschreibung,
                        Dateiendung = item.Dateiendung
                    }).ToList());
                }

                IsDirty = false;
            }
            catch (Exception ex)
            {
                var message = "Beim Speichern der Dateien ist ein Fehler aufgetreten.";

                if(!string.IsNullOrWhiteSpace(ex.Message))
                {
                    message += Environment.NewLine + Environment.NewLine;
                    message += ex.Message;
                }

                HilfsFunktionen.ShowMessageBox(window, "Dateimanager", message, true);

                bReturn = false;
            }

            return bReturn;
        }

        #endregion

        #region Actionfunktionen

        private void Loeschen(Window window, string guid)
        {
            var item = DateiListe.First(teil => teil.Guid == guid);

            try
            {
                var dateiName = Path.Combine("Daten", _komponenteGuid, item.Guid + "." + item.Dateiendung);
                if (File.Exists(dateiName))
                {
                    using (var file = File.Open(dateiName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        file.Close();
                    }
                }
                else
                {
                    dateiName = Path.Combine("Daten", "Temp", item.Guid + "." + item.Dateiendung);
                    if (File.Exists(dateiName))
                    {
                        using (var file = File.Open(dateiName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            file.Close();
                        }
                    }
                }

                _deletedItems.Add(item.Guid + "." + item.Dateiendung);
                DateiListe.Remove(item);
                IsDirty = true;
            }
            catch(Exception ex)
            {
                var message = "Die Datei konnte nicht entfernt werden.";
                message += Environment.NewLine + Environment.NewLine;
                message += "Stellen Sie sicher, dass die Datei nicht geöffnet ist und versuchen Sie es dann erneut.";

                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    message += Environment.NewLine + Environment.NewLine;
                    message += ex.Message;
                }

                HilfsFunktionen.ShowMessageBox(window, "Dateimanager", message, true);
            }
        }

        public void NachObenSortieren(string guid)
        {
            var teil1 = DateiListe.First(teil => teil.Guid == guid);
            if (teil1 != null && DateiListe.IndexOf(teil1) + 1 > 1)
            {
                var teil2 = DateiListe[DateiListe.IndexOf(teil1) - 1];

                if (teil2 != null)
                {
                    DateiListe.Move(DateiListe.IndexOf(teil1), DateiListe.IndexOf(teil2));
                    IsDirty = true;
                }
            }
        }

        public void NachUntenSortieren(string guid)
        {
            var teil1 = DateiListe.First(teil => teil.Guid == guid);

            if (teil1 != null && DateiListe.IndexOf(teil1) + 1 < DateiListe.Count)
            {
                var teil2 = DateiListe[DateiListe.IndexOf(teil1) + 1];

                if (teil2 != null)
                {
                    DateiListe.Move(DateiListe.IndexOf(teil1), DateiListe.IndexOf(teil2));
                    IsDirty = true;
                }
            }
        }

        #endregion

        #region Hilfsfunktionen

        private void SichernVoid(Window window)
        {
            Sichern(window);
        }

        public void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            if (IsDirty)
            {
                var window = sender as Window;
                var owner = window ?? Application.Current.MainWindow;
                if (HilfsFunktionen.ShowCloseQuestionBox(owner, "Dateimanager"))
                {
                    if(!Sichern(owner))
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
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
            get { return Classes.PluginManager.ExportManager.GetKuerzel(); }
        }

        private ObservableCollection<DokumentViewModel> _dateiListe;
        public ObservableCollection<DokumentViewModel> DateiListe
        {
            get { return _dateiListe; }
            set { SetProperty("DateiListe", ref _dateiListe, value); }
        }

        private string _komponenteGuid;

        private readonly List<string> _deletedItems;

        public MyParameterCommand<Window> SichernCommand { get; set; }
        public MyParameterCommand<Window> HinzufuegenCommand { get; set; }
        public MyCommand ZuruecksetzenCommand { get; set; }

        #endregion

        #region Konstruktor

        internal DateiManagerViewModel(string guid)
        {
            DateiListe = new ObservableCollection<DokumentViewModel>();
            _deletedItems = new List<string>();

            SichernCommand = new MyParameterCommand<Window>(SichernVoid);
            HinzufuegenCommand = new MyParameterCommand<Window>(OnHinzufuegen);
            ZuruecksetzenCommand = new MyCommand(Zuruecksetzen);

            _komponenteGuid = guid;

            if (Directory.Exists(Path.Combine("Daten", _komponenteGuid)))
            {
                var liste = new List<DateiDto>();
                Classes.PluginManager.DbManager.GetDateiInfos(_komponenteGuid, ref liste);

                foreach(var datei in liste)
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

            IsDirty = false;
        }

        #endregion

        #region Funktionen

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
                    var dateieindung = Path.GetExtension(datei);
                    if(!string.IsNullOrWhiteSpace(dateieindung))
                    {
                        if(dateieindung.Substring(0,1) == ".")
                        {
                            dateieindung = dateieindung.Substring(1);
                        }
                    }
                    var guid = Guid.NewGuid().ToString();

                    File.Copy(datei, "Daten\\Temp\\" + guid + "." + dateieindung);

                    var newItem = new DokumentViewModel(_komponenteGuid, new DateiDto()
                    {
                        Guid = guid,
                        Beschreibung = viewModel.Beschreibung,
                        Kategorie = viewModel.SelectedKategorie,
                        Dateiendung = dateieindung
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
                    var message = "Die Datei kann nicht kopiert werden.";

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
            Classes.PluginManager.DbManager.GetDateiInfos(_komponenteGuid, ref teileliste);
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

            IsDirty = false;
        }

        public bool Sichern(Window window)
        {
            var bReturn = true;

            try
            {
                Classes.PluginManager.DbManager.DeleteDateiInfos(_komponenteGuid, _deletedItems);
                _deletedItems.Clear();
                Classes.PluginManager.DbManager.SaveDateiInfos(_komponenteGuid, DateiListe.Select(item => new DateiDto
                {
                    Guid = item.Guid,
                    Kategorie = item.Kategorie,
                    Beschreibung = item.Beschreibung,
                    Dateiendung = item.Dateiendung
                }).ToList());

                IsDirty = false;
            }
            catch (Exception ex)
            {
                var message = "Fehler beim Speichern der Dateien";

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
                var message = "Das Dokument kann nicht entfernt werden.";
                message += Environment.NewLine + Environment.NewLine;
                message += "Stellen Sie sicher, dass das Dokument nicht geöffnet ist und versuchen Sie es dann erneut.";

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
                if (HilfsFunktionen.ShowQuestionBox(owner, "Dateimanager"))
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

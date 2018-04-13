using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.EinzelteilBearbeiten.View;

namespace TeileListe.DateiManager.ViewModel
{
    internal class DokumentViewModel : MyCommonViewModel
    {
        #region Properties

        private string _guid;
        public string Guid
        {
            get { return _guid; }
            set { SetProperty("Guid", ref _guid, value); }
        }

        private string _kategorie;
        public string Kategorie
        {
            get { return _kategorie; }
            set { SetProperty("Kategorie", ref _kategorie, value); }
        }

        private string _beschreibung;
        public string Beschreibung
        {
            get { return _beschreibung; }
            set { SetProperty("Beschreibung", ref _beschreibung, value); }
        }

        private string _groesse;
        public string Groesse
        {
            get { return _groesse; }
            set { SetProperty("Groesse", ref _groesse, value); }
        }

        public string Dateiendung { get; set; }
        public string KomponenteGuid { get; set; }

        #endregion

        #region Commands

        public MyParameterCommand<Window> ChangeCommand { get; set; }
        public MyParameterCommand<Window> FileCommand { get; set; }
        public MyParameterCommand<Window> LoeschenCommand { get; set; }
        public MyCommand NachObenCommand { get; set; }
        public MyCommand NachUntenCommand { get; set; }

        #endregion

        #region Actions

        public Action<string> NachObenAction { get; set; }
        public Action<string> NachUntenAction { get; set; }
        public Action<Window, string> LoeschenAction { get; set; }

        #endregion

        internal DokumentViewModel(string komponenteGuid, DateiDto datei)
        {
            ChangeCommand = new MyParameterCommand<Window>(OnChange);
            FileCommand = new MyParameterCommand<Window>(OnFileManager);
            LoeschenCommand = new MyParameterCommand<Window>(OnLoeschen);
            NachObenCommand = new MyCommand(NachOben);
            NachUntenCommand = new MyCommand(NachUnten);

            Guid = datei.Guid;
            Kategorie = datei.Kategorie;
            Beschreibung = datei.Beschreibung;
            Dateiendung = datei.Dateiendung;
            KomponenteGuid = komponenteGuid;

            var dateiName = Path.Combine("Daten", KomponenteGuid, Guid + "." + Dateiendung);

            if (File.Exists(dateiName))
            {
                var dateiInfo = new FileInfo(dateiName);
                Groesse = GetDateiGroesse(dateiInfo.Length);
            }
            else
            {
                dateiName = Path.Combine("Daten", "Temp", Guid + "." + Dateiendung);
                if (File.Exists(dateiName))
                {
                    var dateiInfo = new FileInfo(dateiName);
                    Groesse = GetDateiGroesse(dateiInfo.Length);
                }
                else
                {
                    Groesse = "0 Bytes";
                }
            }
        }

        private void OnFileManager(Window window)
        {
            var dateiName = Path.Combine("Daten", KomponenteGuid, Guid + "." + Dateiendung);

            if (!string.IsNullOrWhiteSpace(dateiName))
            {
                try
                {
                    if (File.Exists(dateiName))
                    {
                        Process.Start(dateiName);
                    }
                    else
                    {
                        dateiName = Path.Combine("Daten", "Temp", Guid + "." + Dateiendung);
                        if (File.Exists(dateiName))
                        {
                            Process.Start(dateiName);
                        }
                    }
                }
                catch (Exception e)
                {
                    var message = "Das Dokument konnte nicht geöffnet werden"
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + e.Message;
                    HilfsFunktionen.ShowMessageBox(window,
                                                            "Dateimanager",
                                                            message,
                                                            true);
                }
            }
        }

        private void NachOben()
        {
            NachObenAction(Guid);
        }

        private void NachUnten()
        {
            NachUntenAction(Guid);
        }

        private void OnChange(Window window)
        {
            var dialog = new EinzelteilBearbeitenDialog(false);
            var viewModel = new DokumentBearbeitenViewModel(Kategorie, false)
            {
                Beschreibung = Beschreibung, 
                CloseAction = dialog.Close
            };

            dialog.Owner = window;
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (viewModel.IsOk)
            {
                Beschreibung = viewModel.Beschreibung;
                Kategorie = viewModel.SelectedKategorie;
            }
        }

        private void OnLoeschen(Window window)
        {
            LoeschenAction(window, Guid);
        }

        internal string GetDateiGroesse(long dateiGroesse)
        {
            decimal groesse = dateiGroesse;

            if(groesse > 1024)
            {
                groesse = groesse / 1024;
                if (groesse > 1024)
                {
                    groesse = groesse / 1024;
                    if (groesse > 1024)
                    {
                        return string.Format("{0:0.0} GB", groesse);
                    }
                    else
                    {
                        return string.Format("{0:0.0} MB", groesse);
                    }
                }
                else
                {
                    return string.Format("{0:0.0} KB", groesse);
                }
            }
            else
            {
                return string.Format("{0:0} B", groesse);
            }
        }
    }
}

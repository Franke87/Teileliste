using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using TeileListe.API.View;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Common.ViewModel;
using TeileListe.Enums;
using TeileListe.MessungHochladen.Dto;

namespace TeileListe.MessungHochladen.ViewModel
{
    internal class MessungHochladenViewModel : MyCommonViewModel
    {
        #region Properties

        public string TitelText { get; set; }
        public string Artikeltext { get; set; }
        public string Datenbank { get; set; }
        public string ProduktId { get; set; }
        public string DatenbankLink { get; set; }
        private string _guid { get; set; }
        public bool AuswahlEnabled { get; set; }

        public CommonDateiViewModel DateiViewModel { get; set; }

        public MyParameterCommand<Window> ArtikelInfosAbrufenCommand { get; set; }
        public MyParameterCommand<Window> OnHochladenCommand { get; set; }
        public MyParameterCommand<Window> ArtikelAufrufenCommand { get; set; }
        public Action CloseAction { get; set; }
        public Action<DateiDto> SaveDateiAction { get; set; }

        private string _datenbankInfos;
        public string DatenbankInfos
        {
            get { return _datenbankInfos; }
            set { SetProperty("DatenbankInfos", ref _datenbankInfos, value); }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private bool _neuesAusgewaehlt;
        public bool NeuesAusgewaehlt
        {
            get { return _neuesAusgewaehlt; }
            set
            {
                SetProperty("NeuesAusgewaehlt", ref _neuesAusgewaehlt, value);
                HasError = CheckForError();
            }
        }

        private decimal _gewicht;
        public decimal Gewicht
        {
            get { return _gewicht; }
            set
            {
                if (SetProperty("Gewicht", ref _gewicht, value))
                {
                    HasError = CheckForError();
                }
            }
        }

        private ObservableCollection<DateiAuswahlViewModel> _dateiListe;
        public ObservableCollection<DateiAuswahlViewModel> DateiListe
        {
            get { return _dateiListe; }
            set
            {
                SetProperty("DateiListe", ref _dateiListe, value);
                HasError = CheckForError();
            }
        }

        private DateiAuswahlViewModel _selectedDatei;
        public DateiAuswahlViewModel SelectedDatei
        {
            get { return _selectedDatei; }
            set
            {
                if (SetProperty("SelectedDatei", ref _selectedDatei, value))
                {
                    HasError = CheckForError();
                }
            }
        }

        #endregion

        #region Konstruktor

        internal MessungHochladenViewModel(KomponenteDto einzelteil, List<DateiDto> listeDateien, EinzelteilBearbeitenEnum typ)
        {
            DatenbankInfos = "";

            switch (typ)
            {
                case EinzelteilBearbeitenEnum.Komponente:
                {
                    TitelText = "Teileliste";
                    break;
                }
                case EinzelteilBearbeitenEnum.Restteil:
                {
                    TitelText = "Restekiste";
                    break;
                }
                case EinzelteilBearbeitenEnum.Wunschteil:
                {
                    TitelText = "Wunschliste";
                    break;
                }
            }

            DatenbankLink = einzelteil.DatenbankLink;

            if (!string.IsNullOrWhiteSpace(einzelteil.DatenbankId))
            {
                var index = einzelteil.DatenbankId.IndexOf(':');
                if (index > 0)
                {
                    Datenbank =  einzelteil.DatenbankId.Substring(0, index);
                    ProduktId = einzelteil.DatenbankId.Substring(index + 1);
                }
            }

            var converter = new Converter.IntToWeightConverter();
            Artikeltext = einzelteil.Komponente + " "
                + HilfsFunktionen.GetAnzeigeName(einzelteil.Hersteller,
                                                    einzelteil.Beschreibung,
                                                    einzelteil.Groesse,
                                                    einzelteil.Jahr)
                + " " + converter.Convert(einzelteil.Gewicht, null, null, null);

            OnHochladenCommand = new MyParameterCommand<Window>(OnHochladen);
            ArtikelInfosAbrufenCommand = new MyParameterCommand<Window>(OnArtikelInfosAbrufen);
            ArtikelAufrufenCommand = new MyParameterCommand<Window>(OnArtikelAufrufen);

            DateiViewModel = new CommonDateiViewModel(DateiOeffnenEnum.Image);
            DateiViewModel.PropertyChanged += ContentPropertyChanged;

            Gewicht = einzelteil.Gewicht;
            _guid = einzelteil.Guid;

            var liste = new List<DateiDto>(listeDateien);
            liste.RemoveAll(item => item.Kategorie != "Gewichtsmessung");
            liste.RemoveAll(item => !(item.Dateiendung.ToLower() == "png"
                                    || item.Dateiendung.ToLower() == "jpg"
                                    || item.Dateiendung.ToLower() == "jpeg"));

            NeuesAusgewaehlt = liste.Count == 0;
            AuswahlEnabled = liste.Count > 0;

            DateiListe = new ObservableCollection<DateiAuswahlViewModel>();

            foreach(var item in liste)
            {
                DateiListe.Add(new DateiAuswahlViewModel(_guid, TitelText, item));
            }

            SelectedDatei = DateiListe.FirstOrDefault();

            HasError = CheckForError();
        }

        #endregion

        #region Funktionen

        private void OnHochladen(Window window)
        {
            var dateiName = string.Empty;

            if (_neuesAusgewaehlt)
            {
                dateiName = DateiViewModel.Datei;
            }
            else
            {
                dateiName = Path.Combine("Daten", _guid, SelectedDatei.Guid + "." + SelectedDatei.Dateiendung);

                if (!File.Exists(dateiName))
                {
                    dateiName = Path.Combine("Daten", "Temp", SelectedDatei.Guid + "." + SelectedDatei.Dateiendung);
                }
            }

            string base64ImageRepresentation;

            try
            {
                byte[] imageArray = File.ReadAllBytes(dateiName);
                base64ImageRepresentation = Convert.ToBase64String(imageArray);
            }
            catch(Exception ex)
            {
                var message = "Die Datei konnte nicht geöffnet werden." 
                            + Environment.NewLine 
                            + Environment.NewLine 
                            + ex.Message;

                HilfsFunktionen.ShowMessageBox(window,
                                                TitelText,
                                                message,
                                                true);
                return;
            }

            var datenbanken = new List<DatenbankDto>
            {
                new DatenbankDto {Datenbank = Datenbank}
            };
            
            PluginManager.DbManager.GetDatenbankDaten(ref datenbanken);

            var progressWindow = new UploadWaitwindow(Datenbank,
                                                        datenbanken[0].ApiToken,
                                                        new MessungHochladenDto
                                                        {
                                                            Gewicht = Gewicht, 
                                                            ProduktId = ProduktId,
                                                            ImageBase64 = base64ImageRepresentation
                                                        }, 
                                                        null)
            {
                Owner = window
            };

            progressWindow.ShowDialog();

            if (progressWindow.Success)
            {
                var message = "Messung erfolgreich hochgeladen";

                if (_neuesAusgewaehlt)
                {
                    try
                    {
                        var datei = DateiViewModel.Datei;
                        var guid = Guid.NewGuid().ToString();
                        var dateiendung = Path.GetExtension(datei);
                        if(dateiendung.StartsWith("."))
                        {
                            dateiendung = dateiendung.Substring(1);
                        }

                        dateiendung = dateiendung.ToLower();

                        File.Copy(datei, "Daten\\Temp\\" + guid + "." + dateiendung);

                        SaveDateiAction(new DateiDto
                        {
                            Guid = guid,
                            Kategorie = "Gewichtsmessung",
                            Beschreibung = Path.GetFileNameWithoutExtension(datei),
                            Dateiendung = dateiendung
                        });
                    }
                    catch (Exception ex)
                    {
                        message += Environment.NewLine + Environment.NewLine;

                        message += "Die Datei kann nicht kopiert werden.";

                        if (!string.IsNullOrWhiteSpace(ex.Message))
                        {
                            message += Environment.NewLine + Environment.NewLine;
                            message += ex.Message;
                        }
                    }
                }

                HilfsFunktionen.ShowMessageBox(window,
                                                TitelText,
                                                message,
                                                false);
            }
            else
            {
                HilfsFunktionen.ShowMessageBox(window,
                                                TitelText,
                                                progressWindow.ErrorText,
                                                true);
            }

            CloseAction();
        }

        private void OnArtikelAufrufen(Window window)
        {
            if (!string.IsNullOrWhiteSpace(DatenbankLink))
            {
                try
                {
                    Process.Start(DatenbankLink);
                }
                catch (Exception e)
                {
                    var message = "Der Link konnte nicht geöffnet werden" 
                                    + Environment.NewLine 
                                    + Environment.NewLine 
                                    + e.Message;
                    HilfsFunktionen.ShowMessageBox(window,
                                                    TitelText,
                                                    message,
                                                    true);
                }
            }
        }

        private void OnArtikelInfosAbrufen(Window window)
        {
            var errorText = string.Empty;

            var datenbanken = new List<DatenbankDto>
            {
                new DatenbankDto {Datenbank = Datenbank}
            };
            
            PluginManager.DbManager.GetDatenbankDaten(ref datenbanken);

            var dialog = new WaitWindow(Datenbank,
                                        datenbanken[0].ApiToken,
                                        "",
                                        "",
                                        ProduktId) { Owner = window };
            dialog.ShowDialog();
            if (dialog.Success)
            {
                DatenbankInfos = dialog.ResultProduktString;
            }
            else
            {
                errorText = dialog.ErrorText;
            }

            if (!string.IsNullOrWhiteSpace(errorText))
            {
                HilfsFunktionen.ShowMessageBox(window,
                                                TitelText,
                                                errorText,
                                                true);
            }
        }

        void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasError = CheckForError();
        }

        bool CheckForError()
        {
            return Gewicht == 0 || NeuesAusgewaehlt ? DateiViewModel.HasError : SelectedDatei == null ;
        }

        #endregion

    }
}

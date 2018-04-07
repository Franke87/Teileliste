using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using TeileListe.API.View;
using TeileListe.Classes;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;
using TeileListe.Common.ViewModel;
using TeileListe.Enums;
using TeileListe.MessungHochladen.Dto;

namespace TeileListe.MessungHochladen.ViewModel
{
    internal class MessungHochladenViewModel : CommonViewModel
    {
        #region Properties

        public string TitelText { get; set; }
        public string Artikeltext { get; set; }
        public string Datenbank { get; set; }
        public string ProduktId { get; set; }
        public string DatenbankLink { get; set; }

        public CommonDateiViewModel DateiViewModel { get; set; }

        public MyParameterCommand<Window> ArtikelInfosAbrufenCommand { get; set; }
        public MyParameterCommand<Window> OnHochladenCommand { get; set; }
        public MyParameterCommand<Window> ArtikelAufrufenCommand { get; set; }
        public Action CloseAction { get; set; }

        private string _datenbankInfos;
        public string DatenbankInfos
        {
            get { return _datenbankInfos; }
            set { SetCommonStringProperty("DatenbankInfos", ref _datenbankInfos, value); }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetCommonBoolProperty("HasError", ref _hasError, value); }
        }

        private decimal _gewicht;
        public decimal Gewicht
        {
            get { return _gewicht; }
            set
            {
                SetDecimalProperty("Gewicht", ref _gewicht, value);
                HasError = DateiViewModel.HasError || Gewicht == 0;
            }
        }

        #endregion

        #region Konstruktor

        internal MessungHochladenViewModel(KomponenteDto einzelteil, EinzelteilBearbeitenEnum typ)
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

            HasError = true;
        }

        #endregion

        #region Funktionen

        private void OnHochladen(Window window)
        {
            byte[] imageArray = File.ReadAllBytes(DateiViewModel.Datei);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

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
                HilfsFunktionen.ShowMessageBox(window,
                                                TitelText,
                                                "Messung erfolgreich hochgeladen",
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
            HasError = DateiViewModel.HasError || Gewicht == 0;
        }

        #endregion

    }
}

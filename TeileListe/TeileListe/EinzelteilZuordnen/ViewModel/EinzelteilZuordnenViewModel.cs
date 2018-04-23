using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using TeileListe.API.View;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;
using TeileListe.Common.ViewModel;
using TeileListe.Enums;
using TeileListe.MessungHochladen.Dto;

namespace TeileListe.EinzelteilZuordnen.ViewModel
{
    public class EinzelteilZuordnenViewModel : INotifyPropertyChanged
    {
        private bool _bestehendSuchen;
        public bool BestehendSuchen
        {
            get { return _bestehendSuchen; }
            set
            {
                SetNeuesEinzelteilBoolProperty("BestehendSuchen", ref _bestehendSuchen, value);
                HasError = BestehendSuchen ? DatenbankViewModel.HasError : AnlegenViewModel.HasError;
            }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetNeuesEinzelteilBoolProperty("HasError", ref _hasError, value); }
        }

        public bool IsOk { get; set; }

        public string TitelText { get; set; }
        public string Artikeltext { get; set; }
        public string ResultDatenbankLink { get; set; }
        public string ResultDatenbankId { get; set; }

        public WebAuswahlViewModel DatenbankViewModel { get; set; }
        public ArtikelAnlegenViewModel AnlegenViewModel { get; set; }

        public MyParameterCommand<Window> OnOkCommand { get; set; }
        public Action CloseAction { get; set; }

        public EinzelteilZuordnenViewModel(KomponenteDto einzelteil, EinzelteilBearbeitenEnum typ)
        {
            
            IsOk = false;

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

            var converter = new Converter.IntToWeightConverter();

            Artikeltext = einzelteil.Komponente + " " 
                + HilfsFunktionen.GetAnzeigeName(einzelteil.Hersteller,
                                                    einzelteil.Beschreibung,
                                                    einzelteil.Groesse,
                                                    einzelteil.Jahr)
                + " " + converter.Convert(einzelteil.Gewicht, null, null, null);

            ResultDatenbankLink = "";
            ResultDatenbankId = "";

            var datenbanken = new List<DatenbankDto>
            {
                new DatenbankDto { Datenbank = "mtb-news.de"}, 
                new DatenbankDto { Datenbank = "rennrad-news.de"}
            };

            PluginManager.DbManager.GetDatenbankDaten(ref datenbanken);

            DatenbankViewModel = new WebAuswahlViewModel(datenbanken, true);
            DatenbankViewModel.PropertyChanged += ContentPropertyChanged;

            AnlegenViewModel = new ArtikelAnlegenViewModel(datenbanken, einzelteil);
            AnlegenViewModel.PropertyChanged += ContentPropertyChanged;

            BestehendSuchen = true;

            OnOkCommand = new MyParameterCommand<Window>(OnOkFunc);

            HasError = true;
        }

        public void OnOkFunc(Window window)
        {
            if (!BestehendSuchen)
            {
                var datenbanken = new List<DatenbankDto>
                {
                    new DatenbankDto {Datenbank = AnlegenViewModel.AusgewaelteDatenbank}
                };
                
                PluginManager.DbManager.GetDatenbankDaten(ref datenbanken);

                byte[] imageArray = File.ReadAllBytes(AnlegenViewModel.DateiViewModel.Datei);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                var progressWindow = new UploadWaitwindow(AnlegenViewModel.AusgewaelteDatenbank,
                                                            datenbanken[0].ApiToken,
                                                            null,
                                                            new ProduktHochladenDto
                                                            {
                                                                Beschreibung = AnlegenViewModel.Beschreibung,
                                                                Gewicht = AnlegenViewModel.Gewicht,
                                                                GewichtHersteller = AnlegenViewModel.GewichtHersteller,
                                                                Groesse = AnlegenViewModel.Groesse,
                                                                Hersteller = AnlegenViewModel.SelectedHersteller.Key,
                                                                ImageBase64 = base64ImageRepresentation,
                                                                Jahr = AnlegenViewModel.Jahr,
                                                                Kategorie = AnlegenViewModel.GetSelectedKategorieId(),
                                                                Kommentar = AnlegenViewModel.Kommentar,
                                                                Link = AnlegenViewModel.Link
                                                            })
                {
                    Owner = window
                };
                progressWindow.ShowDialog();

                if (progressWindow.Success)
                {
                    ResultDatenbankLink = progressWindow.ResultProduktUrl;
                    ResultDatenbankId = AnlegenViewModel.AusgewaelteDatenbank + ":" + progressWindow.ResultProduktId;

                    HilfsFunktionen.ShowMessageBox(window,
                                                    TitelText,
                                                    "Messung erfolgreich hochgeladen",
                                                    false);
                    IsOk = true;
                    CloseAction();
                }
                else
                {
                    HilfsFunktionen.ShowMessageBox(window,
                                                    TitelText,
                                                    progressWindow.ErrorText,
                                                    true);
                }
            }
            else
            {
                ResultDatenbankLink = DatenbankViewModel.SelectedItem.DatenbankLink;
                ResultDatenbankId = DatenbankViewModel.SelectedItem.DatenbankId;
                IsOk = true;
                CloseAction();
            }
        }

        void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasError = BestehendSuchen ? DatenbankViewModel.HasError : AnlegenViewModel.HasError;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetNeuesEinzelteilBoolProperty(string propertyName, 
                                                    ref bool backingField, 
                                                    bool newValue)
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

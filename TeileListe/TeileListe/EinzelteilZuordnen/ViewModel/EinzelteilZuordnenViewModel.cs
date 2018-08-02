using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using TeileListe.API.View;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Enums;
using TeileListe.Common.ViewModel;
using TeileListe.MessungHochladen.Dto;

namespace TeileListe.EinzelteilZuordnen.ViewModel
{
    public class EinzelteilZuordnenViewModel : MyCommonViewModel
    {
        private bool _bestehendSuchen;
        public bool BestehendSuchen
        {
            get { return _bestehendSuchen; }
            set
            {
                SetProperty("BestehendSuchen", ref _bestehendSuchen, value);
                HasError = BestehendSuchen ? DatenbankViewModel.HasError : AnlegenViewModel.HasError;
            }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
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
        public Action<DateiDto> SaveDateiAction { get; set; }

        public EinzelteilZuordnenViewModel(KomponenteDto einzelteil, List<DateiDto> listeDateien, EinzelteilBearbeitenEnum typ)
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

            DatenbankViewModel = new WebAuswahlViewModel(datenbanken, DatenbankModus.SingleSelection);
            DatenbankViewModel.PropertyChanged += ContentPropertyChanged;

            AnlegenViewModel = new ArtikelAnlegenViewModel(datenbanken, listeDateien, einzelteil);
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
                    new DatenbankDto {Datenbank = AnlegenViewModel.DatenbankViewModel.AusgewaelteDatenbank}
                };
                
                PluginManager.DbManager.GetDatenbankDaten(ref datenbanken);

                var dateiName = string.Empty;

                if (AnlegenViewModel.NeuesAusgewaehlt)
                {
                    dateiName = AnlegenViewModel.DateiViewModel.Datei;
                }
                else
                {
                    dateiName = Path.Combine("Daten", AnlegenViewModel.Guid, AnlegenViewModel.SelectedDatei.Guid + "." + AnlegenViewModel.SelectedDatei.Dateiendung);

                    if (!File.Exists(dateiName))
                    {
                        dateiName = Path.Combine("Daten", "Temp", AnlegenViewModel.SelectedDatei.Guid + "." + AnlegenViewModel.SelectedDatei.Dateiendung);
                    }
                }

                string base64ImageRepresentation;

                try
                {
                    byte[] imageArray = File.ReadAllBytes(dateiName);
                    base64ImageRepresentation = Convert.ToBase64String(imageArray);
                }
                catch (Exception ex)
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

                var progressWindow = new UploadWaitwindow(AnlegenViewModel.DatenbankViewModel.AusgewaelteDatenbank,
                                                            datenbanken[0].ApiToken,
                                                            null,
                                                            new ProduktHochladenDto
                                                            {
                                                                Beschreibung = AnlegenViewModel.Beschreibung,
                                                                Gewicht = AnlegenViewModel.Gewicht,
                                                                GewichtHersteller = AnlegenViewModel.GewichtHersteller,
                                                                Groesse = AnlegenViewModel.Groesse,
                                                                Hersteller = AnlegenViewModel.DatenbankViewModel.SelectedHersteller.Key,
                                                                ImageBase64 = base64ImageRepresentation,
                                                                Jahr = AnlegenViewModel.Jahr,
                                                                Kategorie = AnlegenViewModel.DatenbankViewModel.GetSelectedKategorieId(),
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
                    ResultDatenbankId = AnlegenViewModel.DatenbankViewModel.AusgewaelteDatenbank + ":" + progressWindow.ResultProduktId;

                    var message = "Die Messung wurde erfolgreich hochgeladen.";

                    if (AnlegenViewModel.NeuesAusgewaehlt)
                    {
                        try
                        {
                            var datei = AnlegenViewModel.DateiViewModel.Datei;
                            var guid = Guid.NewGuid().ToString();
                            var dateiendung = Path.GetExtension(datei);
                            if (dateiendung.StartsWith("."))
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

                            message += "Die Datei konnte nicht kopiert werden.";

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
    }
}

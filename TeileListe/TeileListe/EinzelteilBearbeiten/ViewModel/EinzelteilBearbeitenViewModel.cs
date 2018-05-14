using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using TeileListe.API.View;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Enums;

namespace TeileListe.EinzelteilBearbeiten.ViewModel
{
    internal class EinzelteilBearbeitenViewModel : MyCommonViewModel
    {
        #region Properties

        public bool IsOk { get; set; }

        public EinzelteilBearbeitenEnum Typ { get; set; }

        public string TitelText { get; set; }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private string _komponente;
        public string Komponente
        {
            get { return _komponente; }
            set
            {
                SetProperty("Komponente", ref _komponente, value);
                HasError = string.IsNullOrWhiteSpace(Komponente);
            }
        }

        private string _hersteller;
        public string Hersteller
        {
            get { return _hersteller; }
            set { SetProperty("Hersteller", ref _hersteller, value); }
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

        private string _jahr;
        public string Jahr
        {
            get { return _jahr; }
            set { SetProperty("Jahr", ref _jahr, value); }
        }

        private string _shop;
        public string Shop
        {
            get { return _shop; }
            set { SetProperty("Shop", ref _shop, value); }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { SetProperty("Link", ref _link, value); }
        }

        private string _datenbankId;
        public string DatenbankId
        {
            get { return _datenbankId; }
            set { SetProperty("DatenbankId", ref _datenbankId, value); }
        }

        public string DatenbankAnzeigeString
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(DatenbankId))
                {
                    var index = DatenbankId.IndexOf(':');
                    if (index > 0)
                    {
                        return DatenbankId.Substring(0, index);
                    }
                }
                return string.Empty;
            }
        }

        private string _datenbankLink;
        public string DatenbankLink
        {
            get { return _datenbankLink; }
            set { SetProperty("DatenbankLink", ref _datenbankLink, value); }
        }

        private int _preis;
        public int Preis
        {
            get { return _preis; }
            set { SetProperty("Preis", ref _preis, value); }
        }

        private int _gewicht;
        public int Gewicht
        {
            get { return _gewicht; }
            set { SetProperty("Gewicht", ref _gewicht, value); }
        }

        private bool _gekauft;
        public bool Gekauft
        {
            get { return _gekauft; }
            set { SetProperty("Gekauft", ref _gekauft, value); }
        }

        private bool _gewogen;
        public bool Gewogen
        {
            get { return _gewogen; }
            set { SetProperty("Gewogen", ref _gewogen, value); }
        }

        private string _datenbankInfos;
        public string DatenbankInfos
        {
            get { return _datenbankInfos; }
            set { SetProperty("DatenbankInfos", ref _datenbankInfos, value); }
        }

        public MyCommand OnOkCommand { get; set; }
        public Action CloseAction { get; set; }
        public MyCommand VerknuepfungEntfernenCommand { get; set; }
        public MyParameterCommand<Window> ArtikelAufrufenCommand { get; set; }
        public MyParameterCommand<Window> ArtikelInfosAbrufenCommand { get; set; }

        #endregion

        internal EinzelteilBearbeitenViewModel(KomponenteDto einzelteil, EinzelteilBearbeitenEnum typ)
        {
            HasError = false;
            IsOk = false;
            Typ = typ;

            switch (Typ)
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

            DatenbankInfos = string.Empty;

            Komponente = einzelteil.Komponente;
            Hersteller = einzelteil.Hersteller;
            Beschreibung = einzelteil.Beschreibung;
            Groesse = einzelteil.Groesse;
            Jahr = einzelteil.Jahr;
            Shop = einzelteil.Shop;
            Link = einzelteil.Link;
            DatenbankId = einzelteil.DatenbankId;
            DatenbankLink = einzelteil.DatenbankLink;
            Preis = einzelteil.Preis;
            Gewicht = einzelteil.Gewicht;
            Gekauft = einzelteil.Gekauft;
            Gewogen = einzelteil.Gewogen;

            OnOkCommand = new MyCommand(OnOkFunc);
            VerknuepfungEntfernenCommand = new MyCommand(OnVerknuepfungEntfernen);
            ArtikelAufrufenCommand = new MyParameterCommand<Window>(OnArtikelAufrufen);
            ArtikelInfosAbrufenCommand = new MyParameterCommand<Window>(OnArtikelInfosAbrufen);
        }

        #region Funktionen

        private void OnArtikelInfosAbrufen(Window window)
        {
            var errorText = string.Empty;

            if (!string.IsNullOrWhiteSpace(DatenbankId))
            {
                var index = DatenbankId.IndexOf(':');
                if (index > 0)
                {
                    var produktId = DatenbankId.Substring(index + 1);

                    if (!string.IsNullOrWhiteSpace(produktId))
                    {
                        var datenbanken = new List<DatenbankDto>
                        {
                            new DatenbankDto {Datenbank = DatenbankAnzeigeString}
                        };
                        
                        PluginManager.DbManager.GetDatenbankDaten(ref datenbanken);

                        var dialog = new WaitWindow(DatenbankAnzeigeString,
                                                    datenbanken[0].ApiToken,
                                                    "",
                                                    "",
                                                    produktId) {Owner = window};
                        dialog.ShowDialog();
                        if (dialog.Success)
                        {
                            DatenbankInfos = dialog.ResultProduktString;
                        }
                        else
                        {
                            errorText = dialog.ErrorText;
                        }
                    }
                    else
                    {
                        errorText = "Verknüpfung zur Gewichtsdatenbank fehlerhaft.";
                    }
                }
                else
                {
                    errorText = "Verknüpfung zur Gewichtsdatenbank fehlerhaft.";
                }
            }
            else
            {
                errorText = "Verknüpfung zur Gewichtsdatenbank fehlerhaft.";
            }

            if(!string.IsNullOrWhiteSpace(errorText))
            {
                HilfsFunktionen.ShowMessageBox(window, "Teileliste", errorText, true);
            }
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

        private void OnVerknuepfungEntfernen()
        {
            DatenbankId = "";
            DatenbankLink = "";
        }

        public void OnOkFunc()
        {
            IsOk = true;
            CloseAction();
        }

        #endregion
    }
}

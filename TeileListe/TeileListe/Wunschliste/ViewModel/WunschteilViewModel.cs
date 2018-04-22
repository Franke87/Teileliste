using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.DateiManager.View;
using TeileListe.DateiManager.ViewModel;
using TeileListe.EinzelteilBearbeiten.View;
using TeileListe.EinzelteilBearbeiten.ViewModel;
using TeileListe.EinzelteilZuordnen.View;
using TeileListe.EinzelteilZuordnen.ViewModel;
using TeileListe.Enums;
using TeileListe.MessungHochladen.View;
using TeileListe.MessungHochladen.ViewModel;

namespace TeileListe.Wunschliste.ViewModel
{
    class WunschteilViewModel : MyCommonViewModel
    {
        #region Commands

        public MyParameterCommand<Window> ChangeCommand { get; set; }
        public MyParameterCommand<Window> DatenbankCommand { get; set; }
        public MyParameterCommand<Window> ShopCommand { get; set; }
        public MyParameterCommand<Window> FileCommand { get; set; }
        public MyCommand ClearCommand { get; set; }
        public MyCommand NachObenCommand { get; set; }
        public MyCommand NachUntenCommand { get; set; }
        public MyCommand LoeschenCommand { get; set; }
        
        #endregion

        #region Actions

        public Action<string> NachObenAction { get; set; }
        public Action<string> NachUntenAction { get; set; }
        public Action<string> LoeschenAction { get; set; }
        public Func<string, List<DateiDto>> GetDateiCacheFunc { get; set; }
        public Action<string, List<DateiDto>> SaveDateiCache { get; set; }

        #endregion

        #region Properties

        private string _komponente;
        private string _hersteller;
        private string _beschreibung;
        private string _groesse;
        private string _jahr;
        private string _shop;
        private string _link;
        private string _datenbankId;
        private string _datenbankLink;
        private int _preis;
        private int _gewicht;

        public string Guid { get; set; }

        public string Komponente
        {
            get { return _komponente; }
            set { SetProperty("Komponente", ref _komponente, value); }
        }

        public string Hersteller
        {
            get { return _hersteller; }
            set
            {
                SetProperty("Hersteller", ref _hersteller, value);
                UpdateProperty("AnzeigeName");
            }
        }

        public string Beschreibung
        {
            get { return _beschreibung; }
            set
            {
                SetProperty("Beschreibung", ref _beschreibung, value);
                UpdateProperty("AnzeigeName");
            }
        }

        public string Groesse
        {
            get { return _groesse; }
            set
            {
                SetProperty("Groesse", ref _groesse, value);
                UpdateProperty("AnzeigeName");
            }
        }

        public string Jahr
        {
            get { return _jahr; }
            set
            {
                SetProperty("Jahr", ref _jahr, value);
                UpdateProperty("AnzeigeName");
            }
        }

        public string AnzeigeName
        {
            get { return HilfsFunktionen.GetAnzeigeName(Hersteller, Beschreibung, Groesse, Jahr); }
        }

        public string Shop
        {
            get { return _shop; }
            set { SetProperty("Shop", ref _shop, value); }
        }

        public string Link
        {
            get { return _link; }
            set { SetProperty("Link", ref _link, value); }
        }

        public string DatenbankId
        {
            get { return _datenbankId; }
            set { SetProperty("DatenbankId", ref _datenbankId, value); }
        }

        public string DatenbankLink
        {
            get { return _datenbankLink; }
            set { SetProperty("DatenbankLink", ref _datenbankLink, value); }
        }

        public int Preis
        {
            get { return _preis; }
            set { SetProperty("Preis", ref _preis, value); }
        }

        public int Gewicht
        {
            get { return _gewicht; }
            set { SetProperty("Gewicht", ref _gewicht, value); }
        }

        public bool IsNeueKomponente { get; set; }

        #endregion

        internal WunschteilViewModel(WunschteilDto wunschteil)
        {
            Guid = wunschteil.Guid;
            Hersteller = wunschteil.Hersteller;
            Beschreibung = wunschteil.Beschreibung;
            Groesse = wunschteil.Groesse;
            Jahr = wunschteil.Jahr;
            Shop = wunschteil.Shop;
            Link = wunschteil.Link;
            DatenbankId = wunschteil.DatenbankId;
            DatenbankLink = wunschteil.DatenbankLink;
            Komponente = wunschteil.Komponente;
            Preis = wunschteil.Preis;
            Gewicht = wunschteil.Gewicht;

            IsNeueKomponente = false;

            ChangeCommand = new MyParameterCommand<Window>(OnChange);
            DatenbankCommand = new MyParameterCommand<Window>(OnDatenbank);
            ShopCommand = new MyParameterCommand<Window>(OnShopClick);
            FileCommand = new MyParameterCommand<Window>(OnFileManager);
            ClearCommand = new MyCommand(OnClear);
            LoeschenCommand = new MyCommand(OnLoeschen);
            NachObenCommand = new MyCommand(NachOben);
            NachUntenCommand = new MyCommand(NachUnten);
        }

        #region Commandfunktionen

        private void OnFileManager(Window window)
        {
            var dialog = new DateiManagerView(window);
            var cache = IsNeueKomponente ? GetDateiCacheFunc(Guid) : new List<DateiDto>();
            var viewModel = new DateiManagerViewModel(Guid, Komponente, Hersteller, Beschreibung, IsNeueKomponente, cache);
            dialog.DataContext = viewModel;
            dialog.Closing += viewModel.OnClosing;
            dialog.ShowDialog();
            dialog.Closing -= viewModel.OnClosing;

            if (IsNeueKomponente)
            {
                SaveDateiCache(Guid, viewModel.DateiCache);
            }
        }

        private void OnDatenbank(Window window)
        {
            if (string.IsNullOrWhiteSpace(DatenbankId))
            {
                var dialog = new EinzelteilZuordnenDialog
                {
                    Top = window.Top + 40,
                    Left = window.Left + (window.ActualWidth - 750) / 2,
                    Owner = window
                };

                var viewModel = new EinzelteilZuordnenViewModel(new KomponenteDto
                                                                {
                                                                    Komponente = Komponente,
                                                                    Hersteller = Hersteller,
                                                                    Beschreibung = Beschreibung,
                                                                    Groesse = Groesse,
                                                                    Jahr = Jahr,
                                                                    Gewicht = Gewicht
                                                                },
                                                                EinzelteilBearbeitenEnum.Wunschteil)
                {
                    CloseAction = dialog.Close
                };
                dialog.DataContext = viewModel;
                dialog.ShowDialog();

                if (viewModel.IsOk)
                {
                    DatenbankId = viewModel.ResultDatenbankId;
                    DatenbankLink = viewModel.ResultDatenbankLink;
                }
            }
            else
            {
                var dialog = new MessungHochladenDialog
                {
                    Top = window.Top + 40,
                    Left = window.Left + (window.ActualWidth - 600) / 2,
                    Owner = window
                };
                var viewModel = new MessungHochladenViewModel(new KomponenteDto
                                                                {
                                                                    Guid = Guid,
                                                                    Komponente = Komponente,
                                                                    Hersteller = Hersteller,
                                                                    Beschreibung = Beschreibung,
                                                                    Groesse = Groesse,
                                                                    Jahr = Jahr,
                                                                    DatenbankId = DatenbankId,
                                                                    DatenbankLink = DatenbankLink,
                                                                    Gewicht = Gewicht
                                                                },
                                                                EinzelteilBearbeitenEnum.Wunschteil)
                {
                    CloseAction = dialog.Close
                };

                dialog.DataContext = viewModel;
                dialog.ShowDialog();
            }
        }

        internal void OnShopClick(Window window)
        {
            if (!string.IsNullOrWhiteSpace(Link))
            {
                try
                {
                    Process.Start(Link);
                }
                catch (Exception e)
                {
                    var message = "Der Link konnte nicht geöffnet werden" 
                                    + Environment.NewLine 
                                    + Environment.NewLine 
                                    + e.Message;
                    HilfsFunktionen.ShowMessageBox(window, 
                                                    "Wunschliste", 
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
            var dialog = new EinzelteilBearbeitenDialog(true);
            var viewModel = new EinzelteilBearbeitenViewModel(new KomponenteDto
                                                                {
                                                                    Komponente = Komponente,
                                                                    Hersteller = Hersteller,
                                                                    Beschreibung = Beschreibung,
                                                                    Groesse = Groesse,
                                                                    Jahr = Jahr,
                                                                    Shop = Shop,
                                                                    Link = Link,
                                                                    DatenbankId = DatenbankId,
                                                                    DatenbankLink = DatenbankLink,
                                                                    Preis = Preis,
                                                                    Gewicht = Gewicht,
                                                                    Gekauft = false,
                                                                    Gewogen = false,
                                                                },
                                                                EinzelteilBearbeitenEnum.Wunschteil)
            {
                CloseAction = dialog.Close
            };
            dialog.Owner = window;
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (viewModel.IsOk)
            {
                Komponente = viewModel.Komponente;
                Hersteller = viewModel.Hersteller;
                Beschreibung = viewModel.Beschreibung;
                Groesse = viewModel.Groesse;
                Jahr = viewModel.Jahr;
                Shop = viewModel.Shop;
                Link = viewModel.Link;
                DatenbankId = viewModel.DatenbankId;
                DatenbankLink = viewModel.DatenbankLink;
                Preis = viewModel.Preis;
                Gewicht = viewModel.Gewicht;
            }
        }

        private void OnLoeschen()
        {
            LoeschenAction(Guid);
        }

        private void OnClear()
        {
            Hersteller = "";
            Beschreibung = "";
            Groesse = "";
            Jahr = "";
            Shop = "";
            Link = "";
            DatenbankId = "";
            DatenbankLink = "";
            Preis = 0;
            Gewicht = 0;
        }

        #endregion

    }
}

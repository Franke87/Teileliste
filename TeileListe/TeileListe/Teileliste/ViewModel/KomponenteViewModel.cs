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

namespace TeileListe.Teileliste.ViewModel
{
    internal class KomponenteViewModel : INotifyPropertyChanged
    {
        #region Commands

        public MyParameterCommand<Window> ChangeCommand { get; set; }
        public MyParameterCommand<Window> DatenbankCommand { get; set; }
        public MyParameterCommand<Window> ShopCommand { get; set; }
        public MyParameterCommand<Window> FileCommand { get; set; }
        public MyCommand AusbauenCommand { get; set; }
        public MyCommand AlternativenCommand { get; set; }
        public MyCommand ClearCommand { get; set; }
        public MyCommand NachObenCommand { get; set; }
        public MyCommand NachUntenCommand { get; set; }
        public MyCommand LoeschenCommand { get; set; }

        #endregion

        #region Properties

        private string _guid;
        public string Guid
        {
            get { return _guid; }
            set { SetKomponenteStringProperty("Guid", ref _guid, value); }
        }

        private List<AlternativeViewModel> _alternativen;
        public List<AlternativeViewModel> Alternativen
        {
            get { return _alternativen; }
            set { SetKomponenteAlternativeListProperty("Alternativen", ref _alternativen, value); }
        }

        private int _gewicht;
        public int Gewicht
        {
            get { return _gewicht; }
            set { SetKomponenteIntProperty("Gewicht", ref _gewicht, value); }
        }

        private int _preis;
        public int Preis
        {
            get { return _preis; }
            set { SetKomponenteIntProperty("Preis", ref _preis, value); }
        }

        private string _komponente;
        public string Komponente
        {
            get { return _komponente; }
            set { SetKomponenteStringProperty("Komponente", ref _komponente, value); }
        }

        private string _hersteller;
        public string Hersteller
        {
            get { return _hersteller; }
            set
            {
                SetKomponenteStringProperty("Hersteller", ref _hersteller, value);
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("AnzeigeName"));
                }
            }
        }

        private string _beschreibung;
        public string Beschreibung
        {
            get { return _beschreibung; }
            set
            {
                SetKomponenteStringProperty("Beschreibung", ref _beschreibung, value);
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("AnzeigeName"));
                }
            }
        }

        private string _groesse;
        public string Groesse
        {
            get { return _groesse; }
            set
            {
                SetKomponenteStringProperty("Groesse", ref _groesse, value);
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("AnzeigeName"));
                }
            }
        }

        private string _jahr;
        public string Jahr
        {
            get { return _jahr; }
            set
            {
                SetKomponenteStringProperty("Jahr", ref _jahr, value);
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("AnzeigeName"));
                }
            }
        }

        public string AnzeigeName
        {
            get { return HilfsFunktionen.GetAnzeigeName(Hersteller, Beschreibung, Groesse, Jahr); }
        }

        private string _shop;
        public string Shop
        {
            get { return _shop; }
            set { SetKomponenteStringProperty("Shop", ref _shop, value); }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { SetKomponenteStringProperty("Link", ref _link, value); }
        }

        private string _datenbankId;
        public string DatenbankId
        {
            get { return _datenbankId; }
            set { SetKomponenteStringProperty("DatenbankId", ref _datenbankId, value); }
        }

        private string _datenbankLink;
        public string DatenbankLink
        {
            get { return _datenbankLink; }
            set { SetKomponenteStringProperty("DatenbankLink", ref _datenbankLink, value); }
        }

        private bool _gekauft;
        public bool Gekauft
        {
            get { return _gekauft; }
            set { SetKomponenteBoolProperty("Gekauft", ref _gekauft, value); }
        }

        private bool _gewogen;
        public bool Gewogen
        {
            get { return _gewogen; }
            set { SetKomponenteBoolProperty("Gewogen", ref _gewogen, value); }
        }

        private bool _alternativenAnzeigen;
        public bool AlternativenAnzeigen
        {
            get { return _alternativenAnzeigen; }
            set { SetKomponenteBoolProperty("AlternativenAnzeigen", ref _alternativenAnzeigen, value); }
        }

        #endregion
        
        #region Actions

        public Action<string> AusbauenAction { get; set; }
        public Action<string> NachObenAction { get; set; }
        public Action<string> NachUntenAction { get; set; }
        public Action<string> LoeschenAction { get; set; }

        #endregion

        #region Constructor

        internal KomponenteViewModel(KomponenteDto dto)
        {
            ChangeCommand = new MyParameterCommand<Window>(OnChange);
            DatenbankCommand = new MyParameterCommand<Window>(OnDatenbank);
            ShopCommand = new MyParameterCommand<Window>(OnLinkClick);
            FileCommand = new MyParameterCommand<Window>(OnFileManager);
            AusbauenCommand = new MyCommand(OnAusbauen);
            AlternativenCommand = new MyCommand(OnAlternativen);
            ClearCommand = new MyCommand(OnClear);
            LoeschenCommand = new MyCommand(OnLoeschen);
            NachObenCommand = new MyCommand(NachOben);
            NachUntenCommand = new MyCommand(NachUnten);

            Guid = dto.Guid;
            Komponente = dto.Komponente;
            Hersteller = dto.Hersteller;
            Beschreibung = dto.Beschreibung;
            Groesse = dto.Groesse;
            Jahr = dto.Jahr;
            Shop = dto.Shop;
            Link = dto.Link;
            DatenbankId = dto.DatenbankId;
            DatenbankLink = dto.DatenbankLink;
            Preis = dto.Preis;
            Gewicht = dto.Gewicht;
            Gekauft = dto.Gekauft;
            Gewogen = dto.Gewogen;

            Alternativen = new List<AlternativeViewModel>();
            AlternativenAnzeigen = false;
        }

        #endregion

        #region Commandfunktionen

        private void OnFileManager(Window window)
        {
            var dialog = new DateiManagerView(window);
            var viewModel = new DateiManagerViewModel(Guid, Komponente, Hersteller, Beschreibung, false, new List<DateiDto>());
            dialog.DataContext = viewModel;
            dialog.Closing += viewModel.OnClosing;
            dialog.ShowDialog();
            dialog.Closing -= viewModel.OnClosing;
        }

        private void OnAlternativen()
        {
            AlternativenAnzeigen = !AlternativenAnzeigen;
        }

        private void OnAusbauen()
        {
            AusbauenAction(Guid);
        }

        private void NachOben()
        {
            NachObenAction(Guid);
        }

        private void NachUnten()
        {
            NachUntenAction(Guid);
        }

        private void OnLinkClick(Window window)
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
                                                    "Teileliste", 
                                                    message, 
                                                    true);
                }
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
                                                                EinzelteilBearbeitenEnum.Komponente)
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
                                                                EinzelteilBearbeitenEnum.Komponente)
                {
                    CloseAction = dialog.Close
                };

                dialog.DataContext = viewModel;
                dialog.ShowDialog();
            }
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
                                                                    Gekauft = Gekauft,
                                                                    Gewogen = Gewogen,
                                                                },
                                                                EinzelteilBearbeitenEnum.Komponente)
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
                Gekauft = viewModel.Gekauft;
                Gewogen = viewModel.Gewogen;
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
            Gekauft = false;
            Gewogen = false;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetKomponenteIntProperty(string propertyName, ref int backingField, int newValue)
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

        internal void SetKomponenteAlternativeListProperty(string propertyName, 
                                                        ref List<AlternativeViewModel> backingField, 
                                                        List<AlternativeViewModel> newValue)
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

        internal void SetKomponenteStringProperty(string propertyName, ref string backingField, string newValue)
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

        internal void SetKomponenteBoolProperty(string propertyName, ref bool backingField, bool newValue)
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

using System;
using System.ComponentModel;
using System.Windows;
using TeileListe.Classes;
using TeileListe.Common.Dto;
using TeileListe.EinzelteilBearbeiten.View;
using TeileListe.EinzelteilBearbeiten.ViewModel;
using TeileListe.EinzelteilZuordnen.View;
using TeileListe.EinzelteilZuordnen.ViewModel;
using TeileListe.Enums;
using TeileListe.MessungHochladen.View;
using TeileListe.MessungHochladen.ViewModel;

namespace TeileListe.Restekiste.ViewModel
{
    internal class RestteilViewModel : INotifyPropertyChanged
    {
        #region Commands

        public MyParameterCommand<Window> ChangeCommand { get; set; }
        public MyParameterCommand<Window> DatenbankCommand { get; set; }
        public MyCommand ClearCommand { get; set; }
        public MyCommand NachObenCommand { get; set; }
        public MyCommand NachUntenCommand { get; set; }
        public MyCommand LoeschenCommand { get; set; }
        
        #endregion

        #region Actions

        public Action<string> NachObenAction { get; set; }
        public Action<string> NachUntenAction { get; set; }
        public Action<string> LoeschenAction { get; set; }

        #endregion

        #region Properties

        private string _komponente;
        private string _beschreibung;
        private string _hersteller;
        private string _groesse;
        private string _jahr;
        private string _datenbankId;
        private string _datenbankLink;
        private int _preis;
        private int _gewicht;

        public string Guid { get; set; }

        public string Komponente
        {
            get { return _komponente; }
            set { SetEinzelteilStringProperty("Komponente", ref _komponente, value); }
        }

        public string Beschreibung
        {
            get { return _beschreibung; }
            set
            {
                SetEinzelteilStringProperty("Beschreibung", ref _beschreibung, value);
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("AnzeigeName"));
                }
            }
        }

        public string Hersteller
        {
            get { return _hersteller; }
            set
            {
                SetEinzelteilStringProperty("Hersteller", ref _hersteller, value);
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("AnzeigeName"));
                }
            }
        }

        public string Groesse
        {
            get { return _groesse; }
            set
            {
                SetEinzelteilStringProperty("Groesse", ref _groesse, value);
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("AnzeigeName"));
                }
            }
        }

        public string Jahr
        {
            get { return _jahr; }
            set
            {
                SetEinzelteilStringProperty("Jahr", ref _jahr, value);
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

        public string DatenbankId
        {
            get { return _datenbankId; }
            set { SetEinzelteilStringProperty("DatenbankId", ref _datenbankId, value); }
        }

        public string DatenbankLink
        {
            get { return _datenbankLink; }
            set { SetEinzelteilStringProperty("DatenbankLink", ref _datenbankLink, value); }
        }

        public int Preis
        {
            get { return _preis; }
            set { SetEinzelteilIntProperty("Preis", ref _preis, value); }
        }

        public int Gewicht
        {
            get { return _gewicht; }
            set { SetEinzelteilIntProperty("Gewicht", ref _gewicht, value); }
        }

        #endregion

        internal RestteilViewModel(RestteilDto restteil)
        {
            Guid = restteil.Guid;
            Komponente = restteil.Komponente;
            Hersteller = restteil.Hersteller;
            Beschreibung = restteil.Beschreibung;
            Groesse = restteil.Groesse;
            Jahr = restteil.Jahr;
            DatenbankId = restteil.DatenbankId;
            DatenbankLink = restteil.DatenbankLink;
            Preis = restteil.Preis;
            Gewicht = restteil.Gewicht;

            ChangeCommand = new MyParameterCommand<Window>(OnChange);
            DatenbankCommand = new MyParameterCommand<Window>(OnDatenbank);
            ClearCommand = new MyCommand(OnClear);
            LoeschenCommand = new MyCommand(OnLoeschen);
            NachObenCommand = new MyCommand(NachOben);
            NachUntenCommand = new MyCommand(NachUnten);
        }

        #region Commandfunktionen

        private void OnDatenbank(Window window)
        {
            if (string.IsNullOrWhiteSpace(DatenbankId))
            {
                var dialog = new EinzelteilZuordnenDialog
                {
                    Top = window.Top + 40,
                    Left = window.Left + (window.ActualWidth - 750)/2,
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
                                                                EinzelteilBearbeitenEnum.Restteil)
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
                                                                    Komponente = Komponente,
                                                                    Hersteller = Hersteller,
                                                                    Beschreibung = Beschreibung,
                                                                    Groesse = Groesse,
                                                                    Jahr = Jahr,
                                                                    DatenbankId = DatenbankId,
                                                                    DatenbankLink = DatenbankLink,
                                                                    Gewicht = Gewicht
                                                                },
                                                                EinzelteilBearbeitenEnum.Restteil)
                {
                    CloseAction = dialog.Close
                };

                dialog.DataContext = viewModel;
                dialog.ShowDialog();
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
            var dialog = new EinzelteilBearbeitenDialog();
            var viewModel = new EinzelteilBearbeitenViewModel(new KomponenteDto
                                                                {
                                                                    Komponente = Komponente, 
                                                                    Hersteller = Hersteller, 
                                                                    Beschreibung = Beschreibung,
                                                                    Groesse = Groesse,
                                                                    Jahr = Jahr,
                                                                    Shop = "",
                                                                    Link = "",
                                                                    DatenbankId = DatenbankId, 
                                                                    DatenbankLink = DatenbankLink, 
                                                                    Preis = Preis, 
                                                                    Gewicht = Gewicht,
                                                                    Gekauft = false,
                                                                    Gewogen = false,
                                                                }, 
                                                                EinzelteilBearbeitenEnum.Restteil) 
                                                                { CloseAction = dialog.Close };
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
            DatenbankId = "";
            DatenbankLink = "";
            Preis = 0;
            Gewicht = 0;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetEinzelteilIntProperty(string propertyName, ref int backingField, int newValue)
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

        internal void SetEinzelteilStringProperty(string propertyName, ref string backingField, string newValue)
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

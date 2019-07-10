using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TeileListe.Restekiste.ViewModel
{
    internal class RestteilViewModel : MyCommonViewModel
    {
        #region Commands

        public MyParameterCommand<Window> ChangeCommand { get; set; }
        public MyParameterCommand<Window> DatenbankCommand { get; set; }
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
            set { SetProperty("Komponente", ref _komponente, value); }
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

        public string Hersteller
        {
            get { return _hersteller; }
            set
            {
                SetProperty("Hersteller", ref _hersteller, value);
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

        internal RestteilViewModel(RestteilDto restteil)
        {
            IsNeueKomponente = false;

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
            var viewModel = new DateiManagerViewModel(Guid,
                                                        Komponente,
                                                        Hersteller,
                                                        Beschreibung,
                                                        IsNeueKomponente,
                                                        cache);
            dialog.DataContext = viewModel;
            dialog.Closing += viewModel.OnClosing;
            dialog.ShowDialog();
            dialog.Closing -= viewModel.OnClosing;
            if(IsNeueKomponente)
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
                    Left = window.Left + (window.ActualWidth - 750)/2,
                    Owner = window
                };

                var liste = new List<DateiDto>();
                if (IsNeueKomponente)
                {
                    liste.AddRange(GetDateiCacheFunc(Guid));
                }
                else
                {
                    PluginManager.DbManager.GetDateiInfos(Guid, ref liste);
                }
                var viewModel = new EinzelteilZuordnenViewModel(new KomponenteDto
                                                                {
                                                                    Guid = Guid,
                                                                    Komponente = Komponente,
                                                                    Hersteller = Hersteller,
                                                                    Beschreibung = Beschreibung,
                                                                    Groesse = Groesse,
                                                                    Jahr = Jahr,
                                                                    Gewicht = Gewicht
                                                                },
                                                                liste,
                                                                EinzelteilBearbeitenEnum.Restteil)
                {
                    CloseAction = dialog.Close,
                    SaveDateiAction = SaveDatei
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
                var liste = new List<DateiDto>();
                if(IsNeueKomponente)
                {
                    liste.AddRange(GetDateiCacheFunc(Guid));
                }
                else
                {
                    PluginManager.DbManager.GetDateiInfos(Guid, ref liste);
                }
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
                                                                liste,
                                                                EinzelteilBearbeitenEnum.Restteil)
                {
                    CloseAction = dialog.Close,
                    SaveDateiAction = SaveDatei
                };

                dialog.DataContext = viewModel;
                dialog.ShowDialog();
            }
        }

        private void SaveDatei(DateiDto datei)
        {
            var liste = new List<DateiDto>();
            if (IsNeueKomponente)
            {
                liste.AddRange(GetDateiCacheFunc(Guid));
                liste.Add(datei);
                SaveDateiCache(Guid, liste);
            }
            else
            {
                PluginManager.DbManager.GetDateiInfos(Guid, ref liste);
                liste.Add(datei);
                PluginManager.DbManager.SaveDateiInfos(Guid, liste);
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

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Enums;
using TeileListe.Gewichtsdatenbanken.ViewModel;
using TeileListe.Internal.ViewModel;

namespace TeileListe.NeuesEinzelteil.ViewModel
{
    class NeuesEinzelteilViewModel : MyCommonViewModel
    {
        #region Properties

        public bool IsOk { get; set; }

        public string TitelText { get; set; }

        public CommonDateiViewModel DateiViewModel { get; set; }
        public NeuesEinzelteilNeuViewModel NeuViewModel{ get; set; }
        public WebAuswahlViewModel DatenbankViewModel { get; set; }
        public RestekisteAuswahlViewModel RestekisteViewModel { get; set; }
        public WunschlisteAuswahlViewModel WunschlisteViewModel { get; set; }

        private SourceEnum _auswahl;
        public SourceEnum Auswahl
        {
            get { return _auswahl; }
            set
            {
                SetProperty("Auswahl", ref _auswahl, value);
                switch (value)
                {
                    case SourceEnum.AusDatei:
                        {
                            HasError = DateiViewModel.HasError;
                        }
                        break;
                    case SourceEnum.NeuesEinzelteil:
                        {
                            HasError = NeuViewModel.HasError;
                        }
                        break;
                    case SourceEnum.AusGewichtsdatenbank:
                        {
                            HasError = DatenbankViewModel.HasError;
                        }
                        break;
                    case SourceEnum.AusRestekiste:
                        {
                            HasError = RestekisteViewModel.HasError;
                        }
                        break;
                    case SourceEnum.AusWunschliste:
                        {
                            HasError = WunschlisteViewModel.HasError;
                        }
                        break;
                }
            }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        public MyCommand OnOkCommand { get; set; }
        public Action CloseAction { get; set; }

        #endregion

        #region Konstruktor

        public NeuesEinzelteilViewModel(EinzelteilBearbeitenEnum typ,
                                        List<EinzelteilAuswahlViewModel> listRestekiste,
                                        List<WunschteilAuswahlViewModel> listWunschliste)
        {
            HasError = false;
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

            DateiViewModel = new CommonDateiViewModel(DateiOeffnenEnum.Csv);
            DateiViewModel.PropertyChanged += ContentPropertyChanged;

            NeuViewModel = new NeuesEinzelteilNeuViewModel(typ);
            NeuViewModel.PropertyChanged += ContentPropertyChanged;

            RestekisteViewModel = new RestekisteAuswahlViewModel(listRestekiste);
            RestekisteViewModel.PropertyChanged += ContentPropertyChanged;

            WunschlisteViewModel = new WunschlisteAuswahlViewModel(listWunschliste);
            WunschlisteViewModel.PropertyChanged += ContentPropertyChanged;

            var datenbanken = new List<DatenbankDto>
            {
                new DatenbankDto { Datenbank = "mtb-news.de"}, 
                new DatenbankDto { Datenbank = "rennrad-news.de"}
            };
            
            PluginManager.DbManager.GetDatenbankDaten(ref datenbanken);

            DatenbankViewModel = new WebAuswahlViewModel(datenbanken, DatenbankModus.MultiSelection);
            DatenbankViewModel.PropertyChanged += ContentPropertyChanged;

            Auswahl = SourceEnum.NeuesEinzelteil;

            OnOkCommand = new MyCommand(OnOkFunc);
        }

        #endregion

        #region Funktionen

        public void OnOkFunc()
        {
            IsOk = true;
            CloseAction();
        }

        void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (Auswahl)
            {
                case SourceEnum.NeuesEinzelteil:
                {
                    HasError = NeuViewModel.HasError;
                    break;
                }
                case SourceEnum.AusDatei:
                {
                    HasError = DateiViewModel.HasError;
                    break;
                }
                case SourceEnum.AusGewichtsdatenbank:
                {
                    HasError = DatenbankViewModel.HasError;
                    break;
                }
                case SourceEnum.AusRestekiste:
                {
                    HasError = RestekisteViewModel.HasError;
                }
                break;
                case SourceEnum.AusWunschliste:
                {
                    HasError = WunschlisteViewModel.HasError;
                }
                break;
            }
        }

        #endregion
    }
}

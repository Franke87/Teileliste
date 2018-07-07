using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TeileListe.API.View;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;
using TeileListe.Common.ViewModel;
using TeileListe.Enums;
using TeileListe.MessungHochladen.ViewModel;

namespace TeileListe.EinzelteilZuordnen.ViewModel
{
    public class ArtikelAnlegenViewModel : MyCommonViewModel
    {
        #region Properties

        private string _beschreibung;
        public string Beschreibung
        {
            get { return _beschreibung; }
            set
            {
                SetProperty("Beschreibung", ref _beschreibung, value);
                HasError = HasValidationError();
            }
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
            set
            {
                SetProperty("Jahr", ref _jahr, value);
                HasError = HasValidationError();
            }
        }

        private decimal _gewicht;
        public decimal Gewicht
        {
            get { return _gewicht; }
            set 
            { 
                SetProperty("Gewicht", ref _gewicht, value);
                HasError = HasValidationError();
            }
        }

        private decimal _gewichtHersteller;
        public decimal GewichtHersteller
        {
            get { return _gewichtHersteller; }
            set { SetProperty("GewichtHersteller", ref _gewichtHersteller, value); }
        }

        private string _kommentar;
        public string Kommentar
        {
            get { return _kommentar; }
            set { SetProperty("Kommentar", ref _kommentar, value); }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { SetProperty("Link", ref _link, value); }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        public bool HerstellerKategorieOk
        {
            get
            {
                bool herstellerOk = true;
                bool kategorieOk = true;

                if (HerstellerList.Count == 0 || SelectedHersteller.Key == null)
                {
                    herstellerOk = false;
                }

                if (KategorienList.Count == 0)
                {
                    kategorieOk = false;
                }
                else
                {
                    foreach (var item in KategorienList)
                    {
                        if (item.IsSelected && item.EnthaeltProdukte)
                        {
                            break;
                        }

                        kategorieOk = IsAnySelected(item);

                        if (kategorieOk)
                        {
                            break;
                        }
                    }
                }

                return herstellerOk && kategorieOk;
            }
        }

        private readonly List<DatenbankDto> _datenbanken;

        public ObservableCollection<string> DatenbankQuellen { get; set; }

        private bool _apiTokenChanged;

        private string _userApiToken;

        public string UserApiToken
        {
            get { return _userApiToken; }
            set
            {
                if (SetProperty("UserApiToken", ref _userApiToken, value))
                {
                    _datenbanken.First(x => x.Datenbank == AusgewaelteDatenbank).ApiToken = UserApiToken;
                    _apiTokenChanged = true;
                }
            }
        }

        private ObservableCollection<KeyValuePair<string, string>> _herstellerList;

        public ObservableCollection<KeyValuePair<string, string>> HerstellerList
        {
            get { return _herstellerList; }
            set { SetProperty("HerstellerList", ref _herstellerList, value); }

        }

        private KeyValuePair<string, string> _selectedHersteller;

        public KeyValuePair<string, string> SelectedHersteller
        {
            get { return _selectedHersteller; }
            set
            {
                SetProperty("SelectedHersteller", ref _selectedHersteller, value);
                HasError = HasValidationError();
            }
        }

        private ObservableCollection<KategorienViewModel> _kategorienList;

        public ObservableCollection<KategorienViewModel> KategorienList
        {
            get { return _kategorienList; }
            set
            {
                if (SetProperty("KategorienList", ref _kategorienList, value))
                {
                    UpdateProperty("HerstellerKategorieOk");
                    HasError = HasValidationError();
                }
            }
        }

        private string _ausgewaelteDatenbank;

        public string AusgewaelteDatenbank
        {
            get { return _ausgewaelteDatenbank; }
            set
            {
                if (SetProperty("AusgewaelteDatenbank", ref _ausgewaelteDatenbank, value))
                {
                    UserApiToken = _datenbanken.First(x => x.Datenbank == AusgewaelteDatenbank).ApiToken;
                    _apiTokenChanged = false;
                    HerstellerList.Clear();
                    KategorienList.Clear();
                    SelectedHersteller = HerstellerList.FirstOrDefault();
                    UpdateProperty("HerstellerKategorieOk");
                    HasError = HasValidationError();
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
                HasError = HasValidationError();
            }
        }

        private bool _neuesAusgewaehlt;
        public bool NeuesAusgewaehlt
        {
            get { return _neuesAusgewaehlt; }
            set
            {
                SetProperty("NeuesAusgewaehlt", ref _neuesAusgewaehlt, value);
                HasError = HasValidationError();
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
                    HasError = HasValidationError();
                }
            }
        }

        public CommonDateiViewModel DateiViewModel { get; set; }

        public string Guid { get; set; }
        public bool AuswahlEnabled { get; set; }

        #endregion

        #region Commands

        public MyParameterCommand<Window> OnAbrufenCommand { get; set; }
        public MyCommand SelectedKategorieChangedCommand { get; set; }

        #endregion

        #region Konstruktor

        public ArtikelAnlegenViewModel(List<DatenbankDto> datenbanken, List<DateiDto> listeDateien, KomponenteDto einzelteil)
        {
            OnAbrufenCommand = new MyParameterCommand<Window>(OnAbrufen);
            SelectedKategorieChangedCommand = new MyCommand(OnKategorieChanged);

            HerstellerList = new ObservableCollection<KeyValuePair<string, string>>();
            KategorienList = new ObservableCollection<KategorienViewModel>();

            _datenbanken = datenbanken;

            DatenbankQuellen = new ObservableCollection<string>();

            AusgewaelteDatenbank = _datenbanken.First().Datenbank;
            UserApiToken = _datenbanken.First().ApiToken;

            foreach (var item in _datenbanken)
            {
                DatenbankQuellen.Add(item.Datenbank);
                if (item.IsDefault)
                {
                    AusgewaelteDatenbank = item.Datenbank;
                    UserApiToken = item.ApiToken;
                }
            }

            SelectedHersteller = HerstellerList.FirstOrDefault();

            DateiViewModel = new CommonDateiViewModel(DateiOeffnenEnum.Image);
            DateiViewModel.PropertyChanged += ContentPropertyChanged;

            Beschreibung = einzelteil.Beschreibung;
            Groesse = einzelteil.Groesse;
            Jahr = einzelteil.Jahr;
            Gewicht = einzelteil.Gewicht;
            GewichtHersteller = 0;
            Kommentar = "";
            Link = "";

            Guid = einzelteil.Guid;

            var liste = new List<DateiDto>(listeDateien);
            liste.RemoveAll(item => item.Kategorie != "Gewichtsmessung");
            liste.RemoveAll(item => !(item.Dateiendung.ToLower() == "png"
                                    || item.Dateiendung.ToLower() == "jpg"
                                    || item.Dateiendung.ToLower() == "jpeg"));

            NeuesAusgewaehlt = liste.Count == 0;
            AuswahlEnabled = liste.Count > 0;

            DateiListe = new ObservableCollection<DateiAuswahlViewModel>();

            foreach (var item in liste)
            {
                DateiListe.Add(new DateiAuswahlViewModel(Guid, "Teileliste", item));
            }

            SelectedDatei = DateiListe.FirstOrDefault();

            _apiTokenChanged = false;
            HasError = HasValidationError();
        }

        #endregion

        #region Funktionen

        private string GetSelectedKategorie(KategorienViewModel viewModel)
        {
            var retStr = string.Empty;

            if (viewModel.IsSelected && viewModel.EnthaeltProdukte)
            {
                retStr = viewModel.Id;
            }

            if (string.IsNullOrWhiteSpace(retStr))
            {
                foreach (var item in viewModel.UnterKategorien)
                {
                    retStr = GetSelectedKategorie(item);
                    if (!string.IsNullOrWhiteSpace(retStr))
                    {
                        break;
                    }
                }
            }

            return retStr;
        }

        public string GetSelectedKategorieId()
        {
            var selectedKategorie = string.Empty;

            foreach (var item in KategorienList)
            {
                selectedKategorie = GetSelectedKategorie(item);
                if (!string.IsNullOrWhiteSpace(selectedKategorie))
                {
                    break;
                }
            }

            return selectedKategorie;
        }

        void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasError = HasValidationError();
        }

        public bool HasValidationError()
        {
            bool hasError = !HerstellerKategorieOk
                            || string.IsNullOrWhiteSpace(Beschreibung)
                            || Gewicht == 0
                            || string.IsNullOrWhiteSpace(Jahr);

            if (!hasError)
            {
                hasError = NeuesAusgewaehlt ? DateiViewModel.HasError : SelectedDatei == null; 
            }

            return hasError;
        }

        public bool IsAnySelected(KategorienViewModel viewModel)
        {
            bool bReturn = viewModel.IsSelected && viewModel.EnthaeltProdukte;

            if (!bReturn)
            {
                foreach (var item in viewModel.UnterKategorien)
                {
                    bReturn = IsAnySelected(item);
                    if (bReturn)
                    {
                        break;
                    }
                }
            }

            return bReturn;
        }

        private void SetAction(KategorienViewModel item)
        {
            item.SelectionChanged = OnKategorieChanged;

            foreach (var subItem in item.UnterKategorien)
            {
                SetAction(subItem);
            }
        }

        private void SaveDatenbanken()
        {
            bool defaultChanged = false;

            foreach (var datenbank in _datenbanken)
            {
                if (datenbank.Datenbank == AusgewaelteDatenbank && !datenbank.IsDefault)
                {
                    defaultChanged = true;
                    break;
                }
            }

            if (_apiTokenChanged || defaultChanged)
            {
                _datenbanken.ForEach(item => item.IsDefault = false);
                _datenbanken.Find(item => item.Datenbank == AusgewaelteDatenbank).IsDefault = true;

                PluginManager.DbManager.SaveDatenbankDaten(_datenbanken);
                _apiTokenChanged = false;
            }
        }

        public void OnKategorieChanged()
        {
            UpdateProperty("HerstellerKategorieOk");
            HasError = HasValidationError();
        }

        public void OnAbrufen(Window window)
        {
            var progressWindow = new WaitWindow(AusgewaelteDatenbank,
                                                UserApiToken,
                                                "",
                                                "",
                                                "") { Owner = window };
            progressWindow.ShowDialog();

            if (progressWindow.Success)
            {
                SaveDatenbanken();
                HerstellerList.Clear();
                KategorienList.Clear();
                HerstellerList = new ObservableCollection<KeyValuePair<string, string>>(progressWindow.ResultHerstellerDto.Data);
                SelectedHersteller = HerstellerList.ElementAtOrDefault(-1);

                KategorienList = new ObservableCollection<KategorienViewModel>(progressWindow.ResultKategorienList);
                foreach (var item in KategorienList)
                {
                    SetAction(item);
                }

                UpdateProperty("HerstellerKategorieOk");

                HasError = HasValidationError();
            }
            else
            {
                HilfsFunktionen.ShowMessageBox(window, "Teileliste", progressWindow.ErrorText, true);
            }
        }

        #endregion

    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TeileListe.API.View;
using TeileListe.Classes;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;
using TeileListe.Common.ViewModel;
using TeileListe.Enums;

namespace TeileListe.EinzelteilZuordnen.ViewModel
{
    public class ArtikelAnlegenViewModel : INotifyPropertyChanged
    {
        #region Properties

        private string _beschreibung;
        public string Beschreibung
        {
            get { return _beschreibung; }
            set
            {
                SetArtikelAnlegenStringProperty("Beschreibung", ref _beschreibung, value);
                HasError = HasValidationError();
            }
        }

        private string _groesse;
        public string Groesse
        {
            get { return _groesse; }
            set { SetArtikelAnlegenStringProperty("Groesse", ref _groesse, value); }
        }

        private string _jahr;
        public string Jahr
        {
            get { return _jahr; }
            set
            {
                SetArtikelAnlegenStringProperty("Jahr", ref _jahr, value);
                HasError = HasValidationError();
            }
        }

        private decimal _gewicht;
        public decimal Gewicht
        {
            get { return _gewicht; }
            set 
            { 
                SetArtikelAnlegenDecimalValue("Gewicht", ref _gewicht, value);
                HasError = HasValidationError();
            }
        }

        private decimal _gewichtHersteller;
        public decimal GewichtHersteller
        {
            get { return _gewichtHersteller; }
            set { SetArtikelAnlegenDecimalValue("GewichtHersteller", ref _gewichtHersteller, value); }
        }

        private string _kommentar;
        public string Kommentar
        {
            get { return _kommentar; }
            set { SetArtikelAnlegenStringProperty("Kommentar", ref _kommentar, value); }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { SetArtikelAnlegenStringProperty("Link", ref _link, value); }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetArtikelAnlegenBoolProperty("HasError", ref _hasError, value); }
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
                if (SetArtikelAnlegenStringProperty("UserApiToken", ref _userApiToken, value))
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
            set { SetArtikelAnlegenCollectionHerstellerProperty("HerstellerList", ref _herstellerList, value); }

        }

        private KeyValuePair<string, string> _selectedHersteller;

        public KeyValuePair<string, string> SelectedHersteller
        {
            get { return _selectedHersteller; }
            set
            {
                SetArtikelAnlegenKeyValuePairProperty("SelectedHersteller", ref _selectedHersteller, value);
                HasError = HasValidationError();
            }
        }

        private ObservableCollection<KategorienViewModel> _kategorienList;

        public ObservableCollection<KategorienViewModel> KategorienList
        {
            get { return _kategorienList; }
            set
            {
                if (SetArtikelAnlegenCollectionProperty("KategorienList", ref _kategorienList, value))
                {
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("HerstellerKategorieOk"));
                    }
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
                if (SetArtikelAnlegenStringProperty("AusgewaelteDatenbank", ref _ausgewaelteDatenbank, value))
                {
                    UserApiToken = _datenbanken.First(x => x.Datenbank == AusgewaelteDatenbank).ApiToken;
                    _apiTokenChanged = false;
                    HerstellerList.Clear();
                    KategorienList.Clear();
                    SelectedHersteller = HerstellerList.FirstOrDefault();
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("HerstellerKategorieOk"));
                    }
                    HasError = HasValidationError();
                }
            }
        }

        public CommonDateiViewModel DateiViewModel { get; set; }

        #endregion

        #region Commands

        public MyParameterCommand<Window> OnAbrufenCommand { get; set; }
        public MyCommand SelectedKategorieChangedCommand { get; set; }

        #endregion

        #region Konstruktor

        public ArtikelAnlegenViewModel(List<DatenbankDto> datenbanken, KomponenteDto einzelteil)
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

            _apiTokenChanged = false;
            HasError = true;
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
            return !HerstellerKategorieOk 
                || DateiViewModel.HasError
                || string.IsNullOrWhiteSpace(Beschreibung)
                || Gewicht == 0
                || string.IsNullOrWhiteSpace(Jahr);
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
            _datenbanken.ForEach(item => item.IsDefault = false);
            _datenbanken.Find(item => item.Datenbank == AusgewaelteDatenbank).IsDefault = true;

            if (_apiTokenChanged)
            {
                PluginManager.DbManager.SaveDatenbankDaten(_datenbanken);
                _apiTokenChanged = false;
            }
            PluginManager.DbManager.SaveDefaultDatenbank(AusgewaelteDatenbank);
        }

        public void OnKategorieChanged()
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("HerstellerKategorieOk"));
            }
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
                SelectedHersteller = HerstellerList.ElementAtOrDefault(-1); // HerstellerList.FirstOrDefault();

                KategorienList = new ObservableCollection<KategorienViewModel>(progressWindow.ResultKategorienList);
                foreach (var item in KategorienList)
                {
                    SetAction(item);
                }

                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("HerstellerKategorieOk"));
                }
                HasError = HasValidationError();
            }
            else
            {
                HilfsFunktionen.ShowMessageBox(window, "Teileliste", progressWindow.ErrorText, true);
            }
        }


        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal bool SetArtikelAnlegenDecimalValue(string propertyName,
            ref decimal backingField,
            decimal newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                return true;
            }
            return false;
        }

        internal bool SetArtikelAnlegenCollectionProperty(string propertyName,
            ref ObservableCollection<KategorienViewModel> backingField,
            ObservableCollection<KategorienViewModel> newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                return true;
            }
            return false;
        }

        internal void SetArtikelAnlegenKeyValuePairProperty(string propertyName,
            ref KeyValuePair<string, string> backingField,
            KeyValuePair<string, string> newValue)
        {
            if (backingField.Key != newValue.Key || backingField.Value != newValue.Value)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        internal void SetArtikelAnlegenCollectionHerstellerProperty(string propertyName,
            ref ObservableCollection<KeyValuePair<string, string>> backingField,
            ObservableCollection<KeyValuePair<string, string>> newValue)
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

        internal bool SetArtikelAnlegenStringProperty(string propertyName,
                                                        ref string backingField,
                                                        string newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                return true;
            }
            return false;
        }

        internal void SetArtikelAnlegenBoolProperty(string propertyName,
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

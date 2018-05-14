﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TeileListe.API.View;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.NeuesEinzelteil.ViewModel;

namespace TeileListe.Common.ViewModel
{
    public class WebAuswahlViewModel : MyCommonViewModel
    {
        #region Properties

        public bool IsSingleSelection { get; set; }

        private DatenbankteilAuswahlViewModel _selectedItem;

        public DatenbankteilAuswahlViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty("SelectedItem", ref _selectedItem, value);
                HasError = IsSingleSelection ? SelectedItem == null : !Datenbankteile.Any(teil => teil.IsChecked);
            }
        }

        private ObservableCollection<DatenbankteilAuswahlViewModel> _datenbankteile;

        public ObservableCollection<DatenbankteilAuswahlViewModel> Datenbankteile
        {
            get { return _datenbankteile; }
            set
            {
                SetProperty("Datenbankteile", ref _datenbankteile, value);
                HasError = IsSingleSelection ? SelectedItem != null : !Datenbankteile.Any(teil => teil.IsChecked);
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
            set { SetProperty("SelectedHersteller", ref _selectedHersteller, value); }
        }

        private ObservableCollection<KategorienViewModel> _kategorienList;

        public ObservableCollection<KategorienViewModel> KategorienList
        {
            get { return _kategorienList; }
            set
            {
                if (SetProperty("KategorienList", ref _kategorienList, value))
                {
                    UpdateProperty("KannSuchen");
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
                    Datenbankteile.Clear();
                    SelectedItem = null;
                    UpdateProperty("KannSuchen");
                }
            }
        }

        private bool _herstellerSuchen;

        public bool HerstellerSuchen
        {
            get { return _herstellerSuchen; }
            set
            {
                SetProperty("HerstellerSuchen", ref _herstellerSuchen, value);
                UpdateProperty("KannSuchen");
            }
        }

        private bool _kategorieSuchen;

        public bool KategorieSuchen
        {
            get { return _kategorieSuchen; }
            set
            {
                SetProperty("KategorieSuchen", ref _kategorieSuchen, value);
                UpdateProperty("KannSuchen");
            }
        }

        public bool KannSuchen
        {
            get
            {
                bool herstellerOk = true;
                bool kategorieOk = true;

                if (HerstellerSuchen && HerstellerList.Count == 0)
                {
                    herstellerOk = false;
                }

                if (KategorieSuchen)
                {
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
                }

                return herstellerOk && kategorieOk && (KategorieSuchen || HerstellerSuchen);
            }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        #endregion

        #region Commands

        public MyParameterCommand<Window> OnAbrufenCommand { get; set; }
        public MyParameterCommand<Window> OnSuchenCommand { get; set; }
        public MyCommand SelectedKategorieChangedCommand { get; set; }

        #endregion

        #region Konstruktor

        public WebAuswahlViewModel(List<DatenbankDto> datenbanken, bool isSingleSelection)
        {
            IsSingleSelection = isSingleSelection;

            KategorieSuchen = true;
            HerstellerSuchen = true;

            OnAbrufenCommand = new MyParameterCommand<Window>(OnAbrufen);
            OnSuchenCommand = new MyParameterCommand<Window>(OnSuchen);
            SelectedKategorieChangedCommand = new MyCommand(OnKategorieChanged);

            _datenbanken = datenbanken;

            Datenbankteile = new ObservableCollection<DatenbankteilAuswahlViewModel>();
            KategorienList = new ObservableCollection<KategorienViewModel>();
            HerstellerList = new ObservableCollection<KeyValuePair<string, string>>();
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
            SelectedItem = null;

            _apiTokenChanged = false;
            HasError = true;
        }

        #endregion

        #region Private Funktionen

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

        public void OnKategorieChanged()
        {
            UpdateProperty("KannSuchen");
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

                HerstellerList = new ObservableCollection<KeyValuePair<string, string>>(progressWindow.ResultHerstellerDto.Data);
                SelectedHersteller = HerstellerList.FirstOrDefault();

                KategorienList = new ObservableCollection<KategorienViewModel>(progressWindow.ResultKategorienList);
                foreach (var item in KategorienList)
                {
                    SetAction(item);
                }

                UpdateProperty("KannSuchen");
            }
            else
            {
                HilfsFunktionen.ShowMessageBox(window,
                                                "Teileliste",
                                                progressWindow.ErrorText,
                                                true);
            }
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

        public void OnSuchen(Window window)
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
            var progressWindow = new WaitWindow(AusgewaelteDatenbank,
                                                UserApiToken,
                                                HerstellerSuchen ? SelectedHersteller.Key : "",
                                                KategorieSuchen ? selectedKategorie : "", 
                                                "") { Owner = window };
            progressWindow.ShowDialog();

            if (progressWindow.Success)
            {
                Datenbankteile.Clear();
                foreach (var item in progressWindow.ResultList)
                {
                    item.PropertyChanged += ContentPropertyChanged;
                    Datenbankteile.Add(item);
                }
                SelectedItem = null;
            }
            else
            {
                HilfsFunktionen.ShowMessageBox(window, "Teileliste", progressWindow.ErrorText, true);
            }
        }

        private void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasError = IsSingleSelection ? SelectedItem != null : !Datenbankteile.Any(item => item.IsChecked);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.ViewModel;
using TeileListe.KategorienVerwalten.View;
using TeileListe.KategorienVerwalten.ViewModel;

namespace TeileListe.DateiManager.ViewModel
{
    internal class DokumentBearbeitenViewModel : MyCommonViewModel
    {
        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private string _beschreibung;
        public string Beschreibung
        {
            get { return _beschreibung; }
            set
            {
                if (SetProperty("Beschreibung", ref _beschreibung, value))
                {
                    HasError = HasValidationError();
                }
            }
        }

        private string _selectedKategorie;
        public string SelectedKategorie
        {
            get { return _selectedKategorie; }
            set
            {
                if (SetProperty("SelectedKategorie", ref _selectedKategorie, value))
                {
                    HasError = HasValidationError();
                }
            }
        }

        private ObservableCollection<string> _kategorieList;
        public ObservableCollection<string> KategorieList
        {
            get { return _kategorieList; }
            set { SetProperty("KategorieList", ref _kategorieList, value); }
        }

        public CommonDateiViewModel DateiViewModel { get; set; }
        public bool IsOk { get; set; }

        public MyCommand OnOkCommand { get; set; }
        public MyParameterCommand<Window> KategorieBearbeitenCommand { get; set; }

        public Action CloseAction { get; set; }

        private bool MitDateiauswahl { get; set; }
        private string _originalKategorie;

        internal DokumentBearbeitenViewModel(string kategorie, bool mitDateiauswahl)
        {
            DateiViewModel = new CommonDateiViewModel(Enums.DateiOeffnenEnum.All);
            DateiViewModel.PropertyChanged += ContentPropertyChanged;

            OnOkCommand = new MyCommand(OnOkFunc);
            KategorieBearbeitenCommand = new MyParameterCommand<Window>(OnKategorieBearbeiten);

            MitDateiauswahl = mitDateiauswahl;
            IsOk = false;
            _originalKategorie = kategorie;

            var list = new List<string>();
            PluginManager.DbManager.GetDateiKategorien(ref list);
            KategorieList = new ObservableCollection<string>(list);

            if(!string.IsNullOrWhiteSpace(kategorie))
            {
                if (!KategorieList.Any(item => item == _originalKategorie))
                {
                    KategorieList.Add(_originalKategorie);
                }
                SelectedKategorie = KategorieList.FirstOrDefault(item => item == _originalKategorie);
            }
            else
            {
                SelectedKategorie = KategorieList.FirstOrDefault();
            }
        }

        public void OnOkFunc()
        {
            IsOk = true;
            CloseAction();
        }

        public void OnKategorieBearbeiten(Window window)
        {
            var selected = SelectedKategorie;

            var dialog = new KategorienVerwaltenView(window);
            var viewModel = new KategorienVerwaltenViewModel(_originalKategorie);
            dialog.DataContext = viewModel;
            dialog.Closing += viewModel.OnClosing;
            dialog.ShowDialog();
            dialog.Closing -= viewModel.OnClosing;

            var list = new List<string>();
            PluginManager.DbManager.GetDateiKategorien(ref list);
            KategorieList = new ObservableCollection<string>(list);

            if (!string.IsNullOrWhiteSpace(selected) && KategorieList.Any(item => item == selected))
            {
                SelectedKategorie = KategorieList.FirstOrDefault(item => item == selected);
            }
            else
            {
                SelectedKategorie = KategorieList.FirstOrDefault();
            }
        }

        void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(Beschreibung))
            {
                Beschreibung = Path.GetFileNameWithoutExtension(DateiViewModel.Datei);
            }
            HasError = HasValidationError();
        }

        public bool HasValidationError()
        {
            return (MitDateiauswahl && DateiViewModel.HasError)
                || string.IsNullOrWhiteSpace(Beschreibung)
                || string.IsNullOrWhiteSpace(SelectedKategorie);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.ViewModel;

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

        public Action CloseAction { get; set; }

        private bool MitDateiauswahl { get; set; }

        internal DokumentBearbeitenViewModel(string kategorie, bool mitDateiauswahl)
        {
            DateiViewModel = new CommonDateiViewModel(Enums.DateiOeffnenEnum.All);
            DateiViewModel.PropertyChanged += ContentPropertyChanged;

            OnOkCommand = new MyCommand(OnOkFunc);

            MitDateiauswahl = mitDateiauswahl;
            IsOk = false;

            var list = new List<string>();
            PluginManager.DbManager.GetDateiKategorien(ref list);
            KategorieList = new ObservableCollection<string>(list);


            if(!string.IsNullOrWhiteSpace(kategorie))
            {
                if (!KategorieList.Any(item => item == kategorie))
                {
                    KategorieList.Add(kategorie);
                }
                SelectedKategorie = KategorieList.FirstOrDefault(item => item == kategorie);
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

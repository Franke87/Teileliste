using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;

namespace TeileListe.Exporter.ViewModel
{
    internal class ExportManagerViewModel : MyCommonViewModel
    {
        private bool _mitCsv;
        public bool MitCsv
        {
            get { return _mitCsv; }
            set
            {
                if (SetProperty("MitCsv", ref _mitCsv, value))
                {
                    IsOk = Validate();
                }
            }
        }

        private bool _alleKategorien;
        public bool AlleKategorien
        {
            get { return _alleKategorien; }
            set { SetProperty("AlleKategorien", ref _alleKategorien, value); }
        }

        private bool _alleDateiendungen;
        public bool AlleDateiendungen
        {
            get { return _alleDateiendungen; }
            set { SetProperty("AlleDateiendungen", ref _alleDateiendungen, value); }
        }

        private bool _isOk;
        public bool IsOk
        {
            get { return _isOk; }
            set { SetProperty("IsOk", ref _isOk, value); }
        }

        private ObservableCollection<FilterViewModel> _listDateiendungen;
        public ObservableCollection<FilterViewModel> ListDateiendungen
        {
            get { return _listDateiendungen; }
            set { SetProperty("ListDateiendungen", ref _listDateiendungen, value); }
        }

        private ObservableCollection<FilterViewModel> _listKategorien;
        public ObservableCollection<FilterViewModel> ListKategorien
        {
            get { return _listKategorien; }
            set { SetProperty("ListKategorien", ref _listKategorien, value); }
        }

        private ObservableCollection<OrdnerViewModel> _dateiListe;
        public ObservableCollection<OrdnerViewModel> DateiListe
        {
            get { return _dateiListe; }
            set
            {
                if (SetProperty("DateiListe", ref _dateiListe, value))
                {
                    IsOk = Validate();
                }
            }
        }

        public MyCommand AlleCommand { get; set; }
        public MyCommand KeineCommand { get; set; }
        public MyCommand ExportCommand { get; set; }

        public Action CloseAction { get; set; }

        internal bool DoExport { get; set; }
        public bool CsvVisible { get; set; }

        internal ExportManagerViewModel(IEnumerable<EinzelteilExportDto> listeKomponenten, bool mitCsv)
        {
            DoExport = false;

            AlleKategorien = true;
            AlleDateiendungen = true;

            ListKategorien = new ObservableCollection<FilterViewModel>();
            ListDateiendungen = new ObservableCollection<FilterViewModel>();

            AlleCommand = new MyCommand(OnAlle);
            KeineCommand = new MyCommand(OnKeine);
            ExportCommand = new MyCommand(OnExport);

            DateiListe = new ObservableCollection<OrdnerViewModel>();

            MitCsv = mitCsv;
            CsvVisible = mitCsv;

            foreach (var komponente in listeKomponenten)
            {
                var dateiListe = new List<DateiViewModel>();

                foreach (var item in komponente.DokumentenListe)
                {
                    if (ListKategorien.All(teil => teil.Anzeige != item.Kategorie))
                    {
                        ListKategorien.Add(new FilterViewModel
                        {
                            Anzeige = item.Kategorie,
                            FilterTyp = 0,
                            AlleAction = OnFilterAlle,
                            KeineAction = OnFilterKeine
                        });
                    }
                    if (ListDateiendungen.All(teil => teil.Anzeige != item.Dateiendung))
                    {
                        ListDateiendungen.Add(new FilterViewModel
                        {
                            Anzeige = item.Dateiendung,
                            FilterTyp = 1,
                            AlleAction = OnFilterAlle,
                            KeineAction = OnFilterKeine
                        });
                    }
                    dateiListe.Add(new DateiViewModel(item));
                }

                if (dateiListe.Count > 0)
                {
                    var folderViewModel = new OrdnerViewModel(dateiListe)
                    {
                        Komponente = komponente.Komponente,
                        Guid = komponente.Guid,
                        AnzeigeText = komponente.Hersteller + " " + komponente.Beschreibung
                    };

                    folderViewModel.PropertyChanged += ContentPropertyChanged;

                    DateiListe.Add(folderViewModel);
                }
            }

            IsOk = Validate();
        }

        private void OnExport()
        {
            if (IsOk)
            {
                DoExport = true;
            }
            CloseAction();
        }

        private void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsOk = Validate();
        }

        internal void OnAlle()
        {
            OnFilter(2, "", true);
        }

        internal void OnKeine()
        {
            OnFilter(2, "", false);
        }

        internal void OnFilterKeine(int filterTyp, string filter)
        {
            OnFilter(filterTyp, filter, false);
        }

        internal void OnFilterAlle(int filterTyp, string filter)
        {
            OnFilter(filterTyp, filter, true);
        }

        internal void OnFilter(int filterTyp, string filter, bool isChecked)
        {
            switch (filterTyp)
            {
                case 0:
                    {
                        foreach (var komponente in DateiListe)
                        {
                            foreach (var item in komponente.DateiViewModelListe.Where(item => item.Kategorie == filter))
                            {
                                item.IsChecked = isChecked;
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        foreach (var komponente in DateiListe)
                        {
                            foreach (var item in komponente.DateiViewModelListe.Where(item => item.Dateiendung == filter))
                            {
                                item.IsChecked = isChecked;
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        foreach (var komponente in DateiListe)
                        {
                            foreach (var item in komponente.DateiViewModelListe)
                            {
                                item.IsChecked = isChecked;
                            }
                        }
                        break;
                    }
            }
        }

        private bool Validate()
        {
            return MitCsv || DateiListe.Any(teil => teil.DateiViewModelListe.Any(item => item.IsChecked));
        }
    }
}

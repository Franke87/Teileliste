using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Enums;
using TeileListe.Common.ViewModel;
using System;
using System.Windows;
using TeileListe.Szenariorechner.View;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class SzenariorechnerViewModel : MyCommonViewModel
    {
        public string Teststring { get { return SelectedKomponente.Komponente; } }

        public int GesamtGewicht { get { return VergleichsListe.Sum(x => x.Gewicht); } }

        public int GesamtDifferenz { get { return VergleichsListe.Sum(x => x.AlternativeDifferenz); } }

        public bool TeilSelected { get { return SelectedKomponente != null; } }

        public WebAuswahlViewModel DatenbankViewModel { get; set; }

        private bool _restekisteFilterAktiv;
        public bool RestekisteFilterAktiv
        {
            get { return _restekisteFilterAktiv; }
            set
            {
                if(SetProperty("RestekisteFilterAktiv", ref _restekisteFilterAktiv, value))
                {
                    AlleRestteile.Refresh();
                }
            }
        }

        private bool _wunschlisteFilterAktiv;
        public bool WunschlisteFilterAktiv
        {
            get { return _wunschlisteFilterAktiv; }
            set
            {
                if (SetProperty("WunschlisteFilterAktiv", ref _wunschlisteFilterAktiv, value))
                {
                    AlleWunschteile.Refresh();
                }
            }
        }

        private SzenarioKomponenteViewModel _selectedKomponente;
        public SzenarioKomponenteViewModel SelectedKomponente
        {
            get { return _selectedKomponente; }
            set
            {
                if(SetProperty("SelectedKomponente", ref _selectedKomponente, value))
                {
                    UpdateProperty("TeilSelected");

                    foreach (var item in Restekiste)
                    {
                        item.Differenz = _selectedKomponente != null ? item.Gewicht - SelectedKomponente.Gewicht : 0;
                    }

                    foreach (var item in Wunschliste)
                    {
                        item.Differenz = _selectedKomponente != null ? item.Gewicht - SelectedKomponente.Gewicht : 0;
                    }

                    DatenbankViewModel.SelectedTeilGewicht = _selectedKomponente != null ? SelectedKomponente.Gewicht : -1;

                    AlleRestteile.Refresh();
                    AlleWunschteile.Refresh();
                }
            }
        }

        private bool FilterRestekiste(object item)
        {
            return TeileFilter(true, item);
        }

        private bool FilterWunschliste(object item)
        {
            return TeileFilter(false, item);
        }

        private bool TeileFilter(bool isRestekiste, object item)
        {
            if(SelectedKomponente == null)
            {
                return true;
            }

            if((isRestekiste && !RestekisteFilterAktiv)
                || (!isRestekiste && !WunschlisteFilterAktiv))
            {
                return true;
            }

            var komponente = item as SzenarioAlternativeViewModel;

            if (komponente == null)
            {
                return true;
            }

            return komponente.Komponente == SelectedKomponente.Komponente;
        }

        private ObservableCollection<SzenarioKomponenteViewModel> _vergleichsListe;
        public ObservableCollection<SzenarioKomponenteViewModel> VergleichsListe
        {
            get { return _vergleichsListe; }
            set { SetProperty("VergleichsListe", ref _vergleichsListe, value); }
        }

        private ObservableCollection<SzenarioKomponenteViewModel> _ohneZuordnung;
        public ObservableCollection<SzenarioKomponenteViewModel> OhneZuordnung
        {
            get { return _ohneZuordnung; }
            set { SetProperty("OhneZuordnung", ref _ohneZuordnung, value); }
        }

        private ObservableCollection<SzenarioAlternativeViewModel> _restekiste;
        public ObservableCollection<SzenarioAlternativeViewModel> Restekiste
        {
            get { return _restekiste; }
            set { SetProperty("Restekiste", ref _restekiste, value); }
        }

        private ICollectionView _alleRestteile;
        public ICollectionView AlleRestteile
        {
            get { return _alleRestteile; }
            set { SetProperty("AlleRestteile", ref _alleRestteile, value); }
        }

        private ObservableCollection<SzenarioAlternativeViewModel> _wunschliste;
        public ObservableCollection<SzenarioAlternativeViewModel> Wunschliste
        {
            get { return _wunschliste; }
            set { SetProperty("Wunschliste", ref _wunschliste, value); }
        }

        private ICollectionView _alleWunschteile;
        public ICollectionView AlleWunschteile
        {
            get { return _alleWunschteile; }
            set { SetProperty("AlleWunschteile", ref _alleWunschteile, value); }
        }

        public MyParameterCommand<Window> HinzufuegenCommand { get; set; }

        public SzenariorechnerViewModel(FahrradDto selectedFahrrad, FahrradDto vergleichsFahrrad)
        {
            var alternativenListe = new List<KomponenteDto>();
            PluginManager.DbManager.GetKomponente(vergleichsFahrrad.Guid, ref alternativenListe);

            var komponentenListe = new List<KomponenteDto>();
            PluginManager.DbManager.GetKomponente(selectedFahrrad.Guid, ref komponentenListe);

            VergleichsListe = new ObservableCollection<SzenarioKomponenteViewModel>();
            OhneZuordnung = new ObservableCollection<SzenarioKomponenteViewModel>();
            Restekiste = new ObservableCollection<SzenarioAlternativeViewModel>();
            Wunschliste = new ObservableCollection<SzenarioAlternativeViewModel>();

            foreach (var item in komponentenListe)
            {
                var vm = new SzenarioKomponenteViewModel()
                {
                    Komponente = item.Komponente,
                    Gewicht = item.Gewicht,
                    Guid = item.Guid,
                    Beschreibung = HilfsFunktionen.GetAnzeigeName(item),
                    AlternativeVorhanden = false,
                    LoeschenAction = ZeileLoeschen
                };
                vm.PropertyChanged += ContentPropertyChanged;

                var alternative = alternativenListe.Find(x => x.Komponente == item.Komponente);
                if (alternative != null)
                {
                    vm.AlternativeHersteller = alternative.Hersteller;
                    vm.AlternativeBeschreibung = alternative.Beschreibung;
                    vm.AlternativeGroesse = alternative.Groesse;
                    vm.AlternativeJahr = alternative.Jahr;
                    vm.AlternativeDifferenz = alternative.Gewicht - vm.Gewicht;
                    vm.AlternativeVorhanden = true;
                    alternativenListe.Remove(alternative);
                }
                else
                {
                    vm.AlternativeVorhanden = false;
                    vm.AlternativeHersteller = "";
                    vm.AlternativeBeschreibung = "";
                    vm.AlternativeGroesse = "";
                    vm.AlternativeJahr = "";
                    vm.AlternativeDifferenz = -vm.Gewicht;
                    OhneZuordnung.Add(vm);
                }
                VergleichsListe.Add(vm);
            }

            foreach(var item in alternativenListe)
            {
                var vm = new SzenarioKomponenteViewModel
                {
                    Komponente = item.Komponente,
                    Gewicht = 0,
                    Beschreibung = null,
                    Guid = item.Guid,
                    AlternativeHersteller = item.Hersteller,
                    AlternativeBeschreibung = item.Beschreibung,
                    AlternativeGroesse = item.Groesse,
                    AlternativeJahr = item.Jahr,
                    AlternativeDifferenz = item.Gewicht,
                    AlternativeVorhanden = true,
                    LoeschenAction = ZeileLoeschen
                };
                vm.PropertyChanged += ContentPropertyChanged;
                VergleichsListe.Add(vm);
            }

            SelectedKomponente = null;

            var restekiste = new List<RestteilDto>();
            PluginManager.DbManager.GetEinzelteile(ref restekiste);

            foreach(var restteil in restekiste)
            {
                var vm = new SzenarioAlternativeViewModel()
                {
                    Komponente = restteil.Komponente,
                    Gewicht = restteil.Gewicht,
                    Beschreibung = restteil.Beschreibung, 
                    Groesse = restteil.Groesse,
                    Jahr = restteil.Jahr,
                    Guid = restteil.Guid,
                    Differenz = 0,
                    EinbauenAction = EinbauenRestekiste,
                    TauschenAction = TauschenRestekiste
                };
                Restekiste.Add(vm);
            }

            AlleRestteile = CollectionViewSource.GetDefaultView(Restekiste);
            RestekisteFilterAktiv = true;
            AlleRestteile.Filter = FilterRestekiste;

            var wunschliste = new List<WunschteilDto>();
            PluginManager.DbManager.GetWunschteile(ref wunschliste);

            foreach (var wunschteil in wunschliste)
            {
                var vm = new SzenarioAlternativeViewModel()
                {
                    Komponente = wunschteil.Komponente,
                    Gewicht = wunschteil.Gewicht,
                    Hersteller = wunschteil.Hersteller,
                    Beschreibung = wunschteil.Beschreibung, 
                    Groesse = wunschteil.Groesse, 
                    Jahr = wunschteil.Jahr,
                    Guid = wunschteil.Guid,
                    Differenz = 0,
                    EinbauenAction = EinbauenWunschliste,
                    TauschenAction = TauschenWunschliste
                };
                Wunschliste.Add(vm);
            }

            AlleWunschteile = CollectionViewSource.GetDefaultView(Wunschliste);
            WunschlisteFilterAktiv = true;
            AlleWunschteile.Filter = FilterWunschliste;

            var datenbanken = new List<DatenbankDto>
            {
                new DatenbankDto { Datenbank = "mtb-news.de"},
                new DatenbankDto { Datenbank = "rennrad-news.de"}
            };

            PluginManager.DbManager.GetDatenbankDaten(ref datenbanken);

            DatenbankViewModel = new WebAuswahlViewModel(datenbanken, DatenbankModus.NoneSelection);
            DatenbankViewModel.EinbauenAction = EinbauenGewichtsdatenbank;
            DatenbankViewModel.TauschenAction = TauschenGewichtsdatenbank;

            HinzufuegenCommand = new MyParameterCommand<Window>(OnHinzufuegen);
        }

        private void OnHinzufuegen(Window window)
        {
            var dialog = new NeueAlternativeDialog()
            {
                Owner = window
            };

            var viewModel = new NeueAlternativeViewModel();
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (viewModel.IsOk)
            {
                UpdateProperty("GesamtDifferenz");
            }
        }

        private void EinbauenGewichtsdatenbank(string komponente, 
                                                string hersteller, 
                                                string beschreibung, 
                                                string groesse, 
                                                string jahr, 
                                                int gewicht)
        {
            var vm = new SzenarioKomponenteViewModel()
            {
                Komponente = komponente,
                Beschreibung = null,
                Gewicht = 0,
                Guid = Guid.NewGuid().ToString(),
                AlternativeHersteller = hersteller,
                AlternativeBeschreibung = beschreibung, 
                AlternativeGroesse = groesse, 
                AlternativeJahr = jahr,
                AlternativeDifferenz = gewicht,
                AlternativeVorhanden = true,
                LoeschenAction = ZeileLoeschen
            };
            vm.PropertyChanged += ContentPropertyChanged;
            VergleichsListe.Add(vm);
            UpdateProperty("GesamtDifferenz");
        }

        private void EinbauenWunschliste(string guid)
        {
            var wunschteil = Wunschliste.First(teil => teil.Guid == guid);
            if (wunschteil != null)
            {
                var vm = new SzenarioKomponenteViewModel()
                {
                    Komponente = wunschteil.Komponente,
                    Beschreibung = null,
                    Gewicht = 0,
                    Guid = wunschteil.Guid,
                    AlternativeHersteller = wunschteil.Hersteller,
                    AlternativeBeschreibung = wunschteil.Beschreibung,
                    AlternativeGroesse = wunschteil.Groesse,
                    AlternativeJahr = wunschteil.Jahr,
                    AlternativeDifferenz = wunschteil.Gewicht,
                    AlternativeVorhanden = true,
                    LoeschenAction = ZeileLoeschen
                };
                vm.PropertyChanged += ContentPropertyChanged;
                VergleichsListe.Add(vm);
                UpdateProperty("GesamtDifferenz");
                Wunschliste.Remove(wunschteil);
            }
        }

        private void EinbauenRestekiste(string guid)
        {
            var restteil = Restekiste.First(teil => teil.Guid == guid);
            if (restteil != null)
            {
                var vm = new SzenarioKomponenteViewModel()
                {
                    Komponente = restteil.Komponente,
                    Beschreibung = null,
                    Gewicht = 0,
                    Guid = restteil.Guid,
                    AlternativeHersteller = restteil.Hersteller,
                    AlternativeBeschreibung = restteil.Beschreibung, 
                    AlternativeGroesse = restteil.Groesse,
                    AlternativeJahr = restteil.Jahr,
                    AlternativeDifferenz = restteil.Gewicht,
                    AlternativeVorhanden = true,
                    LoeschenAction = ZeileLoeschen
                };
                vm.PropertyChanged += ContentPropertyChanged;
                VergleichsListe.Add(vm);
                UpdateProperty("GesamtDifferenz");
                Restekiste.Remove(restteil);
            }
        }

        private void TauschenGewichtsdatenbank(string hersteller, 
                                                string beschreibung, 
                                                string groesse, 
                                                string jahr, 
                                                int differenz)
        {
            SelectedKomponente.AlternativeVorhanden = true;
            SelectedKomponente.AlternativeHersteller = hersteller;
            SelectedKomponente.AlternativeBeschreibung = beschreibung;
            SelectedKomponente.AlternativeGroesse = groesse;
            SelectedKomponente.AlternativeJahr = jahr;
            SelectedKomponente.AlternativeDifferenz = differenz;
            UpdateProperty("GesamtDifferenz");
        }

        private void TauschenWunschliste(string guid)
        {
            var wunschteil = Wunschliste.First(teil => teil.Guid == guid);
            if (wunschteil != null)
            {
                SelectedKomponente.AlternativeVorhanden = true;
                SelectedKomponente.AlternativeHersteller = wunschteil.Hersteller;
                SelectedKomponente.AlternativeBeschreibung = wunschteil.Beschreibung;
                SelectedKomponente.AlternativeGroesse = wunschteil.Groesse;
                SelectedKomponente.AlternativeJahr = wunschteil.Jahr;
                SelectedKomponente.AlternativeDifferenz = wunschteil.Differenz;
                UpdateProperty("GesamtDifferenz");
                Wunschliste.Remove(wunschteil);
            }
        }

        private void TauschenRestekiste(string guid)
        {
            var restteil = Restekiste.First(teil => teil.Guid == guid);
            if (restteil != null)
            {
                SelectedKomponente.AlternativeVorhanden = true;
                SelectedKomponente.AlternativeHersteller = restteil.Hersteller;
                SelectedKomponente.AlternativeBeschreibung = restteil.Beschreibung;
                SelectedKomponente.AlternativeGroesse = restteil.Groesse;
                SelectedKomponente.AlternativeJahr = restteil.Jahr;
                SelectedKomponente.AlternativeDifferenz = restteil.Differenz;
                UpdateProperty("GesamtDifferenz");
                Restekiste.Remove(restteil);
            }
        }

        private void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.CompareOrdinal(e.PropertyName, "AlternativeDifferenz") == 0)
            {
                UpdateProperty("GesamtDifferenz");
            }
        }

        private void ZeileLoeschen(string guid)
        {
            var teil = VergleichsListe.First(item => item.Guid == guid);
            if(teil != null)
            {
                VergleichsListe.Remove(teil);
                UpdateProperty("GesamtDifferenz");
            }
        }
    }
}

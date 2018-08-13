﻿using System.Collections.Generic;
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

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class SzenariorechnerViewModel : MyCommonViewModel
    {
        #region Allgemeine Properties

        public int GesamtGewicht { get { return VergleichsListe.Sum(x => x.Gewicht); } }

        public int GesamtDifferenz { get { return VergleichsListe.Sum(x => x.AlternativeDifferenz); } }

        public WebAuswahlViewModel DatenbankViewModel { get; set; }

        public bool TeilSelected { get { return SelectedKomponente != null; } }

        private SzenarioKomponenteViewModel _selectedKomponente;
        public SzenarioKomponenteViewModel SelectedKomponente
        {
            get { return _selectedKomponente; }
            set
            {
                if (SetProperty("SelectedKomponente", ref _selectedKomponente, value))
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

                    if (AlternativeBearbeiten)
                    {
                        NeueKomponente = _selectedKomponente != null ? _selectedKomponente.Komponente : "";
                        NeuerHersteller = _selectedKomponente != null ? _selectedKomponente.AlternativeHersteller : "";
                        NeueBeschreibung = _selectedKomponente != null ? _selectedKomponente.AlternativeBeschreibung : "";
                        NeueGroesse = _selectedKomponente != null ? _selectedKomponente.AlternativeGroesse : "";
                        NeuesJahr = _selectedKomponente != null ? _selectedKomponente.AlternativeJahr : "";
                        NeuesGewicht = _selectedKomponente != null ? _selectedKomponente.AlternativeDifferenz + _selectedKomponente.Gewicht : 0;
                        KomponenteEnabled = _selectedKomponente != null ? string.IsNullOrWhiteSpace(_selectedKomponente.Beschreibung) : true;
                    }

                    if (_selectedKomponente != null)
                    {
                        if (_selectedKomponente.Beschreibung == null)
                        {
                            foreach (var item in OhneAlternative)
                            {
                                item.Differenz = _selectedKomponente.AlternativeDifferenz - item.Gewicht;
                            }
                        }
                        else if (!_selectedKomponente.AlternativeVorhanden)
                        {
                            foreach (var item in OhneKomponente)
                            {
                                item.Differenz = item.Gewicht - _selectedKomponente.Gewicht;
                            }
                        }
                    }

                    AlleRestteile.Refresh();
                    AlleWunschteile.Refresh();
                }
            }
        }

        private bool TeileFilter(bool isRestekiste, object item)
        {
            if (SelectedKomponente == null)
            {
                return true;
            }

            if ((isRestekiste && !RestekisteFilterAktiv)
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

        #endregion

        #region Restekiste

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

        private bool FilterRestekiste(object item)
        {
            return TeileFilter(true, item);
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

        #endregion

        #region Wunschliste

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

        private bool FilterWunschliste(object item)
        {
            return TeileFilter(false, item);
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

        #endregion

        #region Manuelle Eingabefelder

        private bool _kompnenteEnabled;
        public bool KomponenteEnabled
        {
            get { return _kompnenteEnabled; }
            set { SetProperty("KomponenteEnabled", ref _kompnenteEnabled, value); }
        }

        private string _neueKomponente;
        public string NeueKomponente
        {
            get { return _neueKomponente; }
            set
            {
                if(SetProperty("NeueKomponente", ref _neueKomponente, value))
                {
                    UpdateProperty("HinzufuegenEnabled");
                }
            }
        }

        private string _neuerHersteller;
        public string NeuerHersteller
        {
            get { return _neuerHersteller; }
            set
            {
                if(SetProperty("NeuerHersteller", ref _neuerHersteller, value))
                {
                    UpdateProperty("HinzufuegenEnabled");
                }
            }
        }

        private string _neueBeschreibung;
        public string NeueBeschreibung
        {
            get { return _neueBeschreibung; }
            set
            {
                if(SetProperty("NeueBeschreibung", ref _neueBeschreibung, value))
                {
                    UpdateProperty("HinzufuegenEnabled");
                }
            }
        }

        private string _neueGroesse;
        public string NeueGroesse
        {
            get { return _neueGroesse; }
            set
            {
                if(SetProperty("NeueGroesse", ref _neueGroesse, value))
                {
                    UpdateProperty("HinzufuegenEnabled");
                }
            }
        }

        private int _neuesGewicht;
        public int NeuesGewicht
        {
            get { return _neuesGewicht; }
            set
            {
                SetProperty("NeuesGewicht", ref _neuesGewicht, value);
                UpdateProperty("NeueDifferenz");
            }
        }

        public int NeueDifferenz
        {
            get
            {
                if(AlternativeBearbeiten)
                {
                    return SelectedKomponente != null ? NeuesGewicht - SelectedKomponente.Gewicht : NeuesGewicht;
                }
                else
                {
                    return NeuesGewicht;
                }
            }
        }

        private string _neuesJahr;
        public string NeuesJahr
        {
            get { return _neuesJahr; }
            set
            {
                if(SetProperty("NeuesJahr", ref _neuesJahr, value))
                {
                    UpdateProperty("HinzufuegenEnabled");
                }
            }
        }

        private bool _alternativeBearbeiten;
        public bool AlternativeBearbeiten
        {
            get { return _alternativeBearbeiten; }
            set
            {
                if(SetProperty("AlternativeBearbeiten", ref _alternativeBearbeiten, value))
                {
                    if(AlternativeBearbeiten)
                    {
                        NeueKomponente = _selectedKomponente != null ? _selectedKomponente.Komponente : "";
                        NeuerHersteller = _selectedKomponente != null ? _selectedKomponente.AlternativeHersteller : "";
                        NeueBeschreibung = _selectedKomponente != null ? _selectedKomponente.AlternativeBeschreibung : "";
                        NeueGroesse = _selectedKomponente != null ? _selectedKomponente.AlternativeGroesse : "";
                        NeuesJahr = _selectedKomponente != null ? _selectedKomponente.AlternativeJahr : "";
                        NeuesGewicht = _selectedKomponente != null ? _selectedKomponente.AlternativeDifferenz + _selectedKomponente.Gewicht : 0;
                        KomponenteEnabled = _selectedKomponente != null ? string.IsNullOrWhiteSpace(_selectedKomponente.Beschreibung) : true;
                    }
                    else
                    {
                        NeueKomponente = "";
                        NeuerHersteller = "";
                        NeueBeschreibung = "";
                        NeueGroesse = "";
                        NeuesJahr = "";
                        NeuesGewicht = 0;
                        KomponenteEnabled = true;
                    }
                }
            }
        }

        public bool HinzufuegenEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(NeueKomponente)
                    && (!string.IsNullOrWhiteSpace(NeuerHersteller)
                        || !string.IsNullOrWhiteSpace(NeueBeschreibung)
                        || !string.IsNullOrWhiteSpace(NeueGroesse)
                        || !string.IsNullOrWhiteSpace(NeuesJahr));
            }
        }

        #endregion

        #region Hauptliste

        private ObservableCollection<SzenarioKomponenteViewModel> _vergleichsListe;
        public ObservableCollection<SzenarioKomponenteViewModel> VergleichsListe
        {
            get { return _vergleichsListe; }
            set { SetProperty("VergleichsListe", ref _vergleichsListe, value); }
        }

        #endregion

        #region Zuordnungslisten

        private ObservableCollection<OhneZuordnungViewModel> _ohneAlternative;
        public ObservableCollection<OhneZuordnungViewModel> OhneAlternative
        {
            get { return _ohneAlternative; }
            set { SetProperty("OhneAlternative", ref _ohneAlternative, value); }
        }

        private ObservableCollection<OhneZuordnungViewModel> _ohneKomponente;
        public ObservableCollection<OhneZuordnungViewModel> OhneKomponente
        {
            get { return _ohneKomponente; }
            set { SetProperty("OhneKomponente", ref _ohneKomponente, value); }
        }

        #endregion

        #region Commands und Actions

        public MyCommand HinzufuegenCommand { get; set; }
        public MyCommand TauschenCommand { get; set; }

        #endregion

        #region Konstruktor

        public SzenariorechnerViewModel(FahrradDto selectedFahrrad, FahrradDto vergleichsFahrrad)
        {
            var alternativenListe = new List<KomponenteDto>();
            PluginManager.DbManager.GetKomponente(vergleichsFahrrad.Guid, ref alternativenListe);

            var komponentenListe = new List<KomponenteDto>();
            PluginManager.DbManager.GetKomponente(selectedFahrrad.Guid, ref komponentenListe);

            VergleichsListe = new ObservableCollection<SzenarioKomponenteViewModel>();
            OhneAlternative = new ObservableCollection<OhneZuordnungViewModel>();
            OhneKomponente = new ObservableCollection<OhneZuordnungViewModel>();
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

                    var zuord = new OhneZuordnungViewModel
                    {
                        Guid = vm.Guid,
                        Komponente = vm.Komponente,
                        Beschreibung = vm.Beschreibung,
                        Gewicht = vm.Gewicht,
                        Differenz = vm.AlternativeDifferenz,
                        Alternative = vm.AlternativeName
                    };
                    zuord.ZuordnenAction = OnZuordnenOhneAlternative;
                    OhneAlternative.Add(zuord);
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

                var zuord = new OhneZuordnungViewModel
                {
                    Guid = vm.Guid,
                    Komponente = vm.Komponente, 
                    Beschreibung = vm.Beschreibung, 
                    Alternative = vm.AlternativeName,
                    Gewicht = vm.AlternativeDifferenz,
                    Differenz = vm.AlternativeDifferenz
                };
                zuord.ZuordnenAction = OnZuordnenOhneKomponente;
                OhneKomponente.Add(zuord);
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

            NeueKomponente = "";
            NeuerHersteller = "";
            NeueBeschreibung = "";
            NeueGroesse = "";
            NeuesJahr = "";
            KomponenteEnabled = true;

            AlternativeBearbeiten = true;

            HinzufuegenCommand = new MyCommand(OnHinzufuegen);
            TauschenCommand = new MyCommand(OnTauschen);
        }

        #endregion

        #region Action und Commandfunktionen

        private void OnTauschen()
        {
            SelectedKomponente.Komponente = NeueKomponente;
            SelectedKomponente.AlternativeHersteller = NeuerHersteller;
            SelectedKomponente.AlternativeBeschreibung = NeueBeschreibung;
            SelectedKomponente.AlternativeGroesse = NeueGroesse;
            SelectedKomponente.AlternativeJahr = NeuesJahr;
            SelectedKomponente.AlternativeDifferenz = NeueDifferenz;
            SelectedKomponente.AlternativeVorhanden = true;

            var ohneZuordnung = OhneKomponente.First(teil => teil.Guid == SelectedKomponente.Guid);
            if(ohneZuordnung != null)
            {
                ohneZuordnung.Komponente = SelectedKomponente.Komponente;
                ohneZuordnung.Alternative = SelectedKomponente.AlternativeName;
                ohneZuordnung.Gewicht = NeuesGewicht;
            }
        }

        private void OnHinzufuegen()
        {
            var vm = new SzenarioKomponenteViewModel
            {
                Komponente = NeueKomponente,
                Gewicht = 0,
                Beschreibung = null,
                Guid = Guid.NewGuid().ToString(),
                AlternativeHersteller = NeuerHersteller,
                AlternativeBeschreibung = NeueBeschreibung,
                AlternativeGroesse = NeueGroesse,
                AlternativeJahr = NeuesJahr,
                AlternativeDifferenz = NeuesGewicht,
                AlternativeVorhanden = true,
                LoeschenAction = ZeileLoeschen
            };
            vm.PropertyChanged += ContentPropertyChanged;
            VergleichsListe.Add(vm);

            var ohneZuordnung = new OhneZuordnungViewModel
            {
                Guid = vm.Guid,
                Komponente = vm.Komponente,
                Beschreibung = vm.Beschreibung,
                Alternative = vm.AlternativeName,
                Gewicht = vm.AlternativeDifferenz,
                Differenz = vm.AlternativeDifferenz
            };
            ohneZuordnung.ZuordnenAction = OnZuordnenOhneKomponente;
            OhneKomponente.Add(ohneZuordnung);

            UpdateProperty("GesamtDifferenz");

            NeueKomponente = "";
            NeuerHersteller = "";
            NeueBeschreibung = "";
            NeueGroesse = "";
            NeuesJahr = "";
            NeuesGewicht = 0;
            KomponenteEnabled = true;
        }

        void OnZuordnenOhneKomponente(string guid)
        {
            var item = OhneKomponente.First(teil => teil.Guid == guid);
            var komponente = VergleichsListe.First(teil => teil.Guid == guid);
            if (item != null && komponente != null && SelectedKomponente != null)
            {
                SelectedKomponente.AlternativeHersteller = komponente.AlternativeHersteller;
                SelectedKomponente.AlternativeBeschreibung = komponente.AlternativeBeschreibung;
                SelectedKomponente.AlternativeGroesse = komponente.AlternativeGroesse;
                SelectedKomponente.AlternativeJahr = komponente.AlternativeJahr;
                SelectedKomponente.AlternativeDifferenz = komponente.AlternativeDifferenz - SelectedKomponente.Gewicht;
                SelectedKomponente.AlternativeVorhanden = true;
                OhneKomponente.Remove(item);

                var ohneZuordnung = OhneAlternative.First(teil => teil.Guid == SelectedKomponente.Guid);
                if(ohneZuordnung != null)
                {
                    OhneAlternative.Remove(ohneZuordnung);
                }

                VergleichsListe.Remove(komponente);
            }
        }

        void OnZuordnenOhneAlternative(string guid)
        {
            var item = OhneAlternative.First(teil => teil.Guid == guid);
            var komponente = VergleichsListe.First(teil => teil.Guid == guid);
            if(item != null && komponente != null && SelectedKomponente != null)
            {
                komponente.AlternativeHersteller = SelectedKomponente.AlternativeHersteller;
                komponente.AlternativeBeschreibung = SelectedKomponente.AlternativeBeschreibung;
                komponente.AlternativeGroesse = SelectedKomponente.AlternativeGroesse;
                komponente.AlternativeJahr = SelectedKomponente.AlternativeJahr;
                komponente.AlternativeDifferenz = SelectedKomponente.AlternativeDifferenz - komponente.Gewicht;
                komponente.AlternativeVorhanden = true;
                OhneAlternative.Remove(item);
                
                var ohneZuordnung = OhneKomponente.First(teil => teil.Guid == SelectedKomponente.Guid);
                if (ohneZuordnung != null)
                {
                    OhneKomponente.Remove(ohneZuordnung);
                }

                VergleichsListe.Remove(SelectedKomponente);
                SelectedKomponente = VergleichsListe.First(teil => teil.Guid == guid);
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

        private void ZeileLoeschen(string guid, bool nurAlternative)
        {
            var teil = VergleichsListe.First(item => item.Guid == guid);
            if (teil != null)
            {
                if (nurAlternative)
                {
                    var ohneZuordnung = new OhneZuordnungViewModel
                    {
                        Guid = teil.Guid,
                        Komponente = teil.Komponente,
                        Beschreibung = teil.Beschreibung,
                        Differenz = teil.AlternativeDifferenz,
                        Gewicht = teil.Gewicht,
                        Alternative = teil.AlternativeName
                    };
                    ohneZuordnung.ZuordnenAction = OnZuordnenOhneAlternative;
                    OhneAlternative.Add(ohneZuordnung);
                }
                else
                {
                    VergleichsListe.Remove(teil);
                }

                NeuerHersteller = "";
                NeueBeschreibung = "";
                NeueGroesse = "";
                NeuesJahr = "";
                NeuesGewicht = 0;
                UpdateProperty("GesamtDifferenz");
            }
        }

        #endregion
    }
}

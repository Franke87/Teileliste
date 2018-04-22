using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Enums;
using TeileListe.NeuesEinzelteil.View;
using TeileListe.NeuesEinzelteil.ViewModel;
using TeileListe.NeuesFahrrad.View;
using TeileListe.NeuesFahrrad.ViewModel;
using TeileListe.Restekiste.View;
using TeileListe.Restekiste.ViewModel;
using TeileListe.Wunschliste.View;
using TeileListe.Wunschliste.ViewModel;

namespace TeileListe.Teileliste.ViewModel
{
    internal class TeilelisteViewModel : INotifyPropertyChanged
    {
        #region Commands

        public MyParameterCommand<Window> ExportCommand { get; set; }
        public MyCommand ZuruecksetzenCommand { get; set; }
        public MyCommand SichernCommand { get; set; }
        public MyCommand HinzufuegenCommand { get; set; }
        public MyParameterCommand<Window> RestekisteCommand { get; set; }
        public MyParameterCommand<Window> WunschlisteCommand { get; set; }
        public MyParameterCommand<Window> NeuesFahrradCommand { get; set; }

        #endregion

        #region Readonly Properties

        public int InhaltRestekiste
        {
            get { return ResteListe.Count; }
        }

        public int GewichtRestekiste
        {
            get { return ResteListe.Sum(teile => teile.Gewicht); }
        }

        public int WertRestekiste
        {
            get { return ResteListe.Sum(teile => teile.Preis); }
        }

        public int InhaltWunschliste
        {
            get { return WunschListe.Count; }
        }

        public int GewichtWunschliste
        {
            get { return WunschListe.Sum(teile => teile.Gewicht); }
        }

        public int WertWunschliste
        {
            get { return WunschListe.Sum(teile => teile.Preis); }
        }

        public int GesamtPreis
        {
            get { return KomponentenListe.Sum(teile => teile.Preis); }
        }

        public int BereitsGezahlt
        {
            get { return KomponentenListe.Sum(teile => teile.Gekauft ? teile.Preis : 0); }
        }

        public int GesamtGewicht
        {
            get { return KomponentenListe.Sum(teile => teile.Gewicht); }
        }

        public int SchonGewogen
        {
            get { return KomponentenListe.Sum(teile => teile.Gewogen ? teile.Gewicht : 0); }
        }

        public int SummeEinsparpotenzial
        {
            get { return KomponentenListe.Sum(teil => teil.Alternativen.Any(alt => alt.Einsparung < 0) ? 
                                                        teil.Alternativen.Min(min => min.Einsparung) : 
                                                        0); }
        }

        public string CustomExportKuerzel
        {
            get { return PluginManager.ExportManager.GetKuerzel(); }
        }

        private readonly List<LoeschenDto> _deletedKomponenten;
        private readonly List<LoeschenDto> _deletedTeile;
        private readonly List<LoeschenDto> _deletedWunschteile;

        #endregion

        #region Public Properties

        private List<RestteilDto> _resteListe;
        public List<RestteilDto> ResteListe
        {
            get { return _resteListe; }
            set
            {
                SetTeilelisteListProperty("ResteListe", ref _resteListe, value);
                UpdateResteKisteProperties();
            }
        }

        private List<WunschteilDto> _wunschListe;
        public List<WunschteilDto> WunschListe
        {
            get { return _wunschListe; }
            set
            {
                SetWunschlisteListProperty("WunschListe", ref _wunschListe, value);
                UpdateWunschlisteProperties();
            }
        }

        private bool _exportformatCsv;
        public bool ExportformatCsv
        {
            get { return _exportformatCsv; }
            set { SetTeilelisteBoolProperty("ExportformatCsv", ref _exportformatCsv, value); }
        }

        private string _selectedFahrrad;
        public string SelectedFahrrad
        {
            get { return _selectedFahrrad; }
            set
            {
                if (SetTeilelisteStringProperty("SelectedFahrrad", ref _selectedFahrrad, value))
                {
                    if (IsDirty)
                    {
                        if (HilfsFunktionen.ShowQuestionBox(Application.Current.MainWindow, "Teileliste"))
                        {
                            Sichern();
                        }
                    }
                    
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(Zuruecksetzen),
                                                        DispatcherPriority.ApplicationIdle);
                } 
            }
        }

        private ObservableCollection<string> _fahrradListe;
        public ObservableCollection<string> FahrradListe
        {
            get { return _fahrradListe; }
            set { SetTeilelisteCollectionStringProperty("FahrradListe", ref _fahrradListe, value); }
        }
 
        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                SetTeilelisteBoolProperty("IsDirty", ref _isDirty, value);
                OnPropertyChanged("GesamtPreis");
                OnPropertyChanged("GesamtGewicht");
                OnPropertyChanged("SchonGewogen");
                OnPropertyChanged("BereitsGezahlt");
                OnPropertyChanged("SummeEinsparpotenzial");
            }
        }

        private ObservableCollection<KomponenteViewModel> _komponentenListe;
        public ObservableCollection<KomponenteViewModel> KomponentenListe
        {
            get { return _komponentenListe; }
            set 
            { 
                IsDirty = SetTeilelisteCollectionKomponenteProperty("KomponentenListe", 
                                                                    ref _komponentenListe, 
                                                                    value); 
            }
        }

        #endregion

        #region Constructor

        internal TeilelisteViewModel()
        {
            KomponentenListe = new ObservableCollection<KomponenteViewModel>();
            _deletedKomponenten = new List<LoeschenDto>();
            ResteListe = new List<RestteilDto>();
            _deletedTeile = new List<LoeschenDto>();
            WunschListe = new List<WunschteilDto>();
            _deletedWunschteile = new List<LoeschenDto>();
            FahrradListe = new ObservableCollection<string>();
            ExportformatCsv = true;
            IsDirty = false;

            ExportCommand = new MyParameterCommand<Window>(OnExport);
            ZuruecksetzenCommand = new MyCommand(Zuruecksetzen);
            SichernCommand = new MyCommand(Sichern);
            HinzufuegenCommand = new MyCommand(Hinzufuegen);
            RestekisteCommand = new MyParameterCommand<Window>(Restekiste);
            WunschlisteCommand = new MyParameterCommand<Window>(Wunschliste);
            NeuesFahrradCommand = new MyParameterCommand<Window>(OnNeuesFahrrad);
            
            var liste = new List<string>();
            PluginManager.DbManager.GetFahrraeder(ref liste);
            foreach (var item in liste)
            {
                FahrradListe.Add(item);
            }

            SelectedFahrrad = FahrradListe.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(SelectedFahrrad))
            {
                Zuruecksetzen();
            }
        }

        #endregion

        #region Commandfunktionen

        private void OnNeuesFahrrad(Window window)
        {
            var dialog = new NeuesFahrradDialog();
            var viewModel =
                new NeuesFahrradViewModel(FahrradListe.Aggregate(string.Empty,
                    (current, item) => current + (";" + item + ";")));
            dialog.DataContext = viewModel;
            dialog.Owner = Application.Current.MainWindow;
            viewModel.CloseAction = dialog.Close;
            dialog.ShowDialog();

            if (viewModel.IsOk)
            {
                FahrradListe.Add(viewModel.Name);

                PluginManager.DbManager.SaveFahrraeder(FahrradListe.ToList());
                if (!viewModel.NeuesFahrradAusgewaehlt)
                {
                    try
                    {
                        var importer = new TeileImporter();
                        PluginManager.DbManager.SaveKomponente(viewModel.Name,
                                                                importer.ImportFahrrad(viewModel.Datei));
                        foreach (var datei in importer.DateiCache)
                        {
                            PluginManager.DbManager.SaveDateiInfos(datei.Item1, datei.Item2);
                        }
                    }
                    catch(Exception ex)
                    {
                        var message = "Die Datei konnte nicht importiert werden.";

                        message += Environment.NewLine + Environment.NewLine + ex.Message;

                        HilfsFunktionen.ShowMessageBox(window, "Teileliste", message, true);
                    }
                    
                }

                SelectedFahrrad = viewModel.Name;
            }
        }

        private void Restekiste(Window window)
        {
            var dialog = new RestekisteDialog(window);
            var viewModel = new RestekisteViewModel();
            dialog.DataContext = viewModel;
            dialog.Closing += viewModel.OnClosing;
            dialog.ShowDialog();
            dialog.Closing -= viewModel.OnClosing;
            Zuruecksetzen();
        }

        private void Wunschliste(Window window)
        {
            var dialog = new WunschlisteDialog(window);
            var viewModel = new WunschlisteViewModel();
            dialog.DataContext = viewModel;
            dialog.Closing += viewModel.OnClosing;
            dialog.ShowDialog();
            dialog.Closing -= viewModel.OnClosing;
            Zuruecksetzen();
        }

        private void Hinzufuegen()
        {
            var window = Application.Current.MainWindow;
            var dia = new NeuesEinzelteilDialog
            {
                Top = window.Top + 40,
                Left = window.Left + (window.ActualWidth - 600) / 2,
                Owner = window
            };
            var listRestekiste = ResteListe.Select(teil => new EinzelteilAuswahlViewModel
            {
                Guid = teil.Guid,
                Komponente = teil.Komponente,
                Hersteller = teil.Hersteller,
                Beschreibung = teil.Beschreibung,
                Groesse = teil.Groesse,
                Jahr = teil.Jahr,
                DatenbankId = teil.DatenbankId,
                DatenbankLink = teil.DatenbankLink,
                Gewicht = teil.Gewicht,
                Preis = teil.Preis
            }).ToList();
            var listWunschliste = WunschListe.Select(teil => new WunschteilAuswahlViewModel
            {
                Guid = teil.Guid,
                Komponente = teil.Komponente,
                Hersteller = teil.Hersteller,
                Beschreibung = teil.Beschreibung,
                Groesse = teil.Groesse,
                Jahr = teil.Jahr,
                Shop = teil.Shop,
                Link = teil.Link,
                DatenbankId = teil.DatenbankId,
                DatenbankLink = teil.DatenbankLink,
                Gewicht = teil.Gewicht,
                Preis = teil.Preis
            }).ToList();
            var vm = new NeuesEinzelteilViewModel(EinzelteilBearbeitenEnum.Komponente,
                                                    listRestekiste,
                                                    listWunschliste)
            {
                CloseAction = dia.Close
            };
            dia.DataContext = vm;
            dia.ShowDialog();

            if (vm.IsOk)
            {
                switch (vm.Auswahl)
                {
                    case SourceEnum.NeuesEinzelteil:
                    {
                        var neuesTeil = new KomponenteViewModel(new KomponenteDto
                        {
                            Guid = Guid.NewGuid().ToString()
                        })
                        {
                            Komponente = vm.NeuViewModel.Komponente,
                            Hersteller = vm.NeuViewModel.Hersteller,
                            Gewicht = vm.NeuViewModel.Gewicht,
                            Preis = vm.NeuViewModel.Preis,
                            Beschreibung = vm.NeuViewModel.Beschreibung,
                            Groesse = vm.NeuViewModel.Groesse,
                            Jahr = vm.NeuViewModel.Jahr,
                            DatenbankId = "",
                            DatenbankLink = "",
                            Gekauft = vm.NeuViewModel.Gekauft,
                            Gewogen = vm.NeuViewModel.Gewogen,
                            Link = vm.NeuViewModel.Link,
                            Shop = vm.NeuViewModel.Shop
                        };
                        neuesTeil.PropertyChanged += ContentPropertyChanged;
                        neuesTeil.AusbauenAction = Ausbauen;
                        neuesTeil.NachObenAction = NachObenSortieren;
                        neuesTeil.NachUntenAction = NachUntenSortieren;
                        neuesTeil.LoeschenAction = Loeschen;
                        KomponentenListe.Add(neuesTeil);
                        break;
                    }
                    case SourceEnum.AusRestekiste:
                    {
                        foreach (var teil in vm.RestekisteViewModel.EinzelTeile.Where(teil => teil.IsChecked).ToList())
                        {
                            if (teil.IsChecked)
                            {
                                var neuesTeil = new KomponenteViewModel(new KomponenteDto
                                {
                                    Guid = teil.Guid
                                })
                                {
                                    Komponente = teil.Komponente,
                                    Hersteller = teil.Hersteller,
                                    Beschreibung = teil.Beschreibung,
                                    Groesse = teil.Groesse,
                                    Jahr = teil.Jahr,
                                    Shop = "Restekiste",
                                    Link = "",
                                    DatenbankId = teil.DatenbankId,
                                    DatenbankLink = teil.DatenbankLink,
                                    Preis = teil.Preis,
                                    Gewicht = teil.Gewicht,
                                    Gekauft = true,
                                    Gewogen = true


                                };
                                neuesTeil.PropertyChanged += ContentPropertyChanged;
                                neuesTeil.AusbauenAction = Ausbauen;
                                neuesTeil.NachObenAction = NachObenSortieren;
                                neuesTeil.NachUntenAction = NachUntenSortieren;
                                neuesTeil.LoeschenAction = Loeschen;
                                KomponentenListe.Add(neuesTeil);
                                _deletedTeile.Add(new LoeschenDto { Guid = teil.Guid, DokumenteLoeschen = false });
                                ResteListe.Remove(ResteListe.First(resteTeil => resteTeil.Guid == teil.Guid));
                            }
                        }
                        UpdateResteKisteProperties();
                        break;
                    }
                    case SourceEnum.AusWunschliste:
                    {
                        foreach (var teil in vm.WunschlisteViewModel.WunschTeile.Where(teil => teil.IsChecked).ToList())
                        {
                            if (teil.IsChecked)
                            {
                                var neuesTeil = new KomponenteViewModel(new KomponenteDto
                                {
                                    Guid = teil.Guid
                                })
                                {
                                    Komponente = teil.Komponente,
                                    Hersteller = teil.Hersteller,
                                    Beschreibung = teil.Beschreibung,
                                    Groesse = teil.Groesse,
                                    Jahr = teil.Jahr,
                                    Shop = teil.Shop,
                                    Link = teil.Link,
                                    DatenbankId = teil.DatenbankId,
                                    DatenbankLink = teil.DatenbankLink,
                                    Gekauft = false,
                                    Gewogen = false,
                                    Preis = teil.Preis,
                                    Gewicht = teil.Gewicht,
                                };
                                neuesTeil.PropertyChanged += ContentPropertyChanged;
                                neuesTeil.AusbauenAction = Ausbauen;
                                neuesTeil.NachObenAction = NachObenSortieren;
                                neuesTeil.NachUntenAction = NachUntenSortieren;
                                neuesTeil.LoeschenAction = Loeschen;
                                KomponentenListe.Add(neuesTeil);
                                _deletedWunschteile.Add(new LoeschenDto { Guid = teil.Guid, DokumenteLoeschen = false });
                                WunschListe.Remove(
                                    WunschListe.First(wunschTeil => wunschTeil.Guid == teil.Guid));
                            }
                        }
                        UpdateWunschlisteProperties();
                        break;
                    }
                    case SourceEnum.AusGewichtsdatenbank:
                    {
                        foreach (var teil in vm.DatenbankViewModel.Datenbankteile.Where(teil => teil.IsChecked).ToList())
                        {
                            if (teil.IsChecked)
                            {
                                var neuesTeil = new KomponenteViewModel(new KomponenteDto
                                {
                                    Guid = Guid.NewGuid().ToString()
                                })
                                {
                                    Komponente = teil.Komponente,
                                    Hersteller = teil.Hersteller,
                                    Beschreibung = teil.Beschreibung,
                                    Groesse = teil.Groesse,
                                    Jahr = teil.Jahr,
                                    Link = "",
                                    Shop = "",
                                    DatenbankId = teil.DatenbankId,
                                    DatenbankLink = teil.DatenbankLink,
                                    Preis = 0,
                                    Gewicht = teil.Gewicht,
                                    Gekauft = false,
                                    Gewogen = false,
                                };
                                neuesTeil.PropertyChanged += ContentPropertyChanged;
                                neuesTeil.AusbauenAction = Ausbauen;
                                neuesTeil.NachObenAction = NachObenSortieren;
                                neuesTeil.NachUntenAction = NachUntenSortieren;
                                neuesTeil.LoeschenAction = Loeschen;
                                KomponentenListe.Add(neuesTeil);
                            }
                        }
                        break;
                    }
                }
                IsDirty = true;
            }
        }

        private void Sichern()
        {
            PluginManager.DbManager.DeleteKomponenten(SelectedFahrrad, _deletedKomponenten);
            _deletedKomponenten.Clear();
            PluginManager.DbManager.SaveKomponente(SelectedFahrrad, KomponentenListe.Select(item => new KomponenteDto
                                                            {
                                                                Guid = item.Guid,
                                                                Komponente = item.Komponente, 
                                                                Hersteller = item.Hersteller,
                                                                Beschreibung = item.Beschreibung, 
                                                                Groesse = item.Groesse,
                                                                Jahr = item.Jahr,
                                                                Shop = item.Shop,
                                                                Link = item.Link, 
                                                                DatenbankId = item.DatenbankId,
                                                                DatenbankLink = item.DatenbankLink,
                                                                Preis = item.Preis, 
                                                                Gewicht = item.Gewicht, 
                                                                Gekauft = item.Gekauft, 
                                                                Gewogen = item.Gewogen
                                                            }).ToList());
            PluginManager.DbManager.DeleteEinzelteile(_deletedTeile);
            _deletedTeile.Clear();
            PluginManager.DbManager.SaveEinzelteile(ResteListe.Select(item => new RestteilDto
                                                            {
                                                                Guid = item.Guid,
                                                                Komponente = item.Komponente,
                                                                Beschreibung = item.Beschreibung,
                                                                Hersteller = item.Hersteller,
                                                                Groesse = item.Groesse, 
                                                                Jahr = item.Jahr,
                                                                DatenbankId = item.DatenbankId,
                                                                DatenbankLink = item.DatenbankLink,
                                                                Preis = item.Preis,
                                                                Gewicht = item.Gewicht,
                                                            }).ToList());
            PluginManager.DbManager.DeleteWunschteile(_deletedWunschteile);
            _deletedWunschteile.Clear();
            PluginManager.DbManager.SaveWunschteile(WunschListe);
            IsDirty = false;

            if (KomponentenListe.Count == 0)
            {
                PluginManager.DbManager.DeleteFahrrad(SelectedFahrrad);
                PluginManager.DbManager.SaveFahrraeder(FahrradListe.Where(item => item != SelectedFahrrad).ToList());
                FahrradListe.Remove(SelectedFahrrad);
                SelectedFahrrad = FahrradListe.FirstOrDefault();
            }

            Zuruecksetzen();
        }

        private void Zuruecksetzen()
        {
            KomponentenListe.Clear();
            _deletedKomponenten.Clear();
            ResteListe.Clear();
            _deletedTeile.Clear();
            WunschListe.Clear();
            _deletedWunschteile.Clear();
            
            var wunschListe = new List<WunschteilDto>();
            PluginManager.DbManager.GetWunschteile(ref wunschListe);
            foreach (var item in wunschListe)
            {
                WunschListe.Add(item);
            }

            var teileListe = new List<RestteilDto>();
            PluginManager.DbManager.GetEinzelteile(ref teileListe);
            foreach (var item in teileListe)
            {
                ResteListe.Add(item);
            }

            if (!string.IsNullOrWhiteSpace(SelectedFahrrad))
            {
                var komponentenListe = new List<KomponenteDto>();
                PluginManager.DbManager.GetKomponente(SelectedFahrrad, ref komponentenListe);
                foreach (var item in komponentenListe)
                {
                    var viewmodel = new KomponenteViewModel(item);
                    viewmodel.PropertyChanged += ContentPropertyChanged;
                    viewmodel.AusbauenAction = Ausbauen;
                    viewmodel.NachObenAction = NachObenSortieren;
                    viewmodel.NachUntenAction = NachUntenSortieren;
                    viewmodel.LoeschenAction = Loeschen;
                    foreach (var alternative in WunschListe.FindAll(x => x.Komponente == item.Komponente))
                    {
                        var dto = new AlternativeViewModel
                        {
                            Einsparung = alternative.Gewicht - viewmodel.Gewicht,
                            Guid = alternative.Guid, 
                            ParentGuid = viewmodel.Guid,
                            Preis = alternative.Preis, 
                            Hersteller = alternative.Hersteller,
                            Beschreibung = alternative.Beschreibung, 
                            Groesse = alternative.Groesse,
                            Jahr = alternative.Jahr,
                            Shop = alternative.Shop, 
                            Link = alternative.Link, 
                            TauschenAction = Tauschen
                        };
                        viewmodel.Alternativen.Add(dto);
                    }
                    foreach(var alternative in ResteListe.FindAll(x => x.Komponente == item.Komponente))
                    {
                        var dto = new AlternativeViewModel
                        {
                            Einsparung = alternative.Gewicht - viewmodel.Gewicht,
                            Guid = alternative.Guid,
                            ParentGuid = viewmodel.Guid,
                            Hersteller = alternative.Hersteller,
                            Beschreibung = alternative.Beschreibung,
                            Groesse = alternative.Groesse,
                            Jahr = alternative.Jahr,
                            Preis = alternative.Preis,
                            Link = "", 
                            Shop = "Restekiste",
                            TauschenAction = Tauschen
                        };
                        viewmodel.Alternativen.Add(dto);
                    }
                    KomponentenListe.Add(viewmodel);
                }
            }

            UpdateResteKisteProperties();
            UpdateWunschlisteProperties();
            OnPropertyChanged("GesamtPreis");
            OnPropertyChanged("GesamtGewicht");
            OnPropertyChanged("SchonGewogen");
            OnPropertyChanged("BereitsGezahlt");
            OnPropertyChanged("SummeEinsparpotenzial");
            IsDirty = false;
        }

        private void OnExport(Window window)
        {
            if (ExportformatCsv)
            {
                OnExportCsv(window);
            }
            else
            {
                OnExportCustom(window);
            }
        }

        private void OnExportCsv(Window window)
        {
            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var file = Path.Combine(path, "Teileliste.csv");
                var i = 1;
                while (File.Exists(file))
                {
                    file = Path.Combine(path, string.Format("Teileliste ({0}).csv", i++));
                }
                using (var formatter = new CsvFormatter())
                {
                    using (var sw = new StreamWriter(file,
                                                        false,
                                                        Encoding.Default))
                    {
                        sw.Write(formatter.GetFormattetKomponenten(KomponentenListe));
                    }
                    Process.Start(new ProcessStartInfo("explorer.exe")
                    {
                        Arguments = "/select, \"" + file + "\""
                    });
                }
            }
            catch (IOException ex)
            {
                HilfsFunktionen.ShowMessageBox(window, ex.Message, "Teileliste", true);
            }
        }

        private void OnExportCustom(Window window)
        {
            try
            {
                var liste = KomponentenListe.Select(item => new EinzelteilExportDto
                                                    {
                                                        Guid = item.Guid,
                                                        Komponente = item.Komponente,
                                                        Hersteller = item.Hersteller,
                                                        Beschreibung = item.Beschreibung,
                                                        Groesse = item.Groesse,
                                                        Jahr = item.Jahr,
                                                        Shop = item.Shop,
                                                        Link = item.Link,
                                                        DatenbankId = item.DatenbankId,
                                                        DatenbankLink = item.DatenbankLink,
                                                        Preis = item.Preis,
                                                        Gewicht = item.Gewicht,
                                                        Gekauft = item.Gekauft,
                                                        Gewogen = item.Gewogen,
                                                        DokumentenListe = new List<DateiDto>()
                                                    }).ToList();

                foreach(var item in liste)
                {
                    var dateiListe = new List<DateiDto>();
                    PluginManager.DbManager.GetDateiInfos(item.Guid, ref dateiListe);
                    item.DokumentenListe.AddRange(dateiListe);
                }

                var csvExport = "";

                using (var formatter = new CsvFormatter())
                {
                    csvExport = formatter.GetFormattetKomponenten(KomponentenListe);

                }

                PluginManager.ExportManager.ExportKomponenten(new WindowInteropHelper(window).Handle,
                                                                SelectedFahrrad,
                                                                csvExport,
                                                                liste);
            }
            catch (Exception ex)
            {
                var message = ex.Message;

                if (ex.InnerException != null)
                {
                    message += Environment.NewLine + Environment.NewLine + ex.InnerException.Message;
                }

                HilfsFunktionen.ShowMessageBox(window, "Teileliste", message, true);
            }
                
        }

        #endregion

        #region Actionfunktionen

        private void Loeschen(string guid)
        {
            var item = KomponentenListe.First(teil => teil.Guid == guid);
            _deletedKomponenten.Add(new LoeschenDto { Guid = guid, DokumenteLoeschen = true });
            KomponentenListe.Remove(item);
            IsDirty = true;
        }

        public void NachObenSortieren(string guid)
        {
            var teil1 = KomponentenListe.First(teil => teil.Guid == guid);
            if (teil1 != null && KomponentenListe.IndexOf(teil1) + 1 > 1)
            {
                var teil2 = KomponentenListe[KomponentenListe.IndexOf(teil1) - 1];
                if (teil2 != null)
                {
                    KomponentenListe.Move(KomponentenListe.IndexOf(teil1), 
                                            KomponentenListe.IndexOf(teil2));
                    IsDirty = true;
                }
            }
        }

        public void NachUntenSortieren(string guid)
        {
            var teil1 = KomponentenListe.First(teil => teil.Guid == guid);
            if (teil1 != null && KomponentenListe.IndexOf(teil1) + 1 < KomponentenListe.Count)
            {
                var teil2 = KomponentenListe[KomponentenListe.IndexOf(teil1) + 1];
                if (teil2 != null)
                {
                    KomponentenListe.Move(KomponentenListe.IndexOf(teil1), 
                                            KomponentenListe.IndexOf(teil2));
                    IsDirty = true;
                }
            }
        }

        public void Tauschen(string guid, string alternativeGuid)
        {
            var alteKomponente = KomponentenListe.First(teil => teil.Guid == guid);
            if (alteKomponente != null)
            {
                var index = KomponentenListe.IndexOf(alteKomponente);

                var wunschteil = WunschListe.Find(altteil => altteil.Guid == alternativeGuid);
                if (wunschteil != null)
                {
                    Ausbauen(guid);
                    var neueKomponente = new KomponenteViewModel(new KomponenteDto
                    {
                        Guid = wunschteil.Guid,
                        Komponente = wunschteil.Komponente,
                        Hersteller = wunschteil.Hersteller,
                        Beschreibung = wunschteil.Beschreibung,
                        Groesse = wunschteil.Groesse,
                        Jahr = wunschteil.Jahr,
                        Shop = wunschteil.Shop,
                        Link = wunschteil.Link,
                        Preis = wunschteil.Preis,
                        Gekauft = false,
                        Gewicht = wunschteil.Gewicht,
                        Gewogen = false
                    });
                    neueKomponente.PropertyChanged += ContentPropertyChanged;
                    neueKomponente.AusbauenAction = Ausbauen;
                    neueKomponente.NachObenAction = NachObenSortieren;
                    neueKomponente.NachUntenAction = NachUntenSortieren;
                    neueKomponente.LoeschenAction = Loeschen;
                    KomponentenListe.Insert(index, neueKomponente);
                    _deletedWunschteile.Add(new LoeschenDto { Guid = wunschteil.Guid, DokumenteLoeschen = false });
                    WunschListe.Remove(wunschteil);
                }
                else
                {
                    var restteil = ResteListe.Find(teil => teil.Guid == alternativeGuid);
                    if (restteil != null)
                    {
                        Ausbauen(guid);
                        var neueKomponente = new KomponenteViewModel(new KomponenteDto
                        {
                            Guid = restteil.Guid,
                            Komponente = restteil.Komponente,
                            Hersteller = restteil.Hersteller,
                            Beschreibung = restteil.Beschreibung,
                            Groesse = restteil.Groesse,
                            Jahr = restteil.Jahr,
                            Shop = "Restekiste",
                            Link = "",
                            Preis = restteil.Preis,
                            Gekauft = true,
                            Gewicht = restteil.Gewicht,
                            Gewogen = true
                        });
                        neueKomponente.PropertyChanged += ContentPropertyChanged;
                        neueKomponente.AusbauenAction = Ausbauen;
                        neueKomponente.NachObenAction = NachObenSortieren;
                        neueKomponente.NachUntenAction = NachUntenSortieren;
                        neueKomponente.LoeschenAction = Loeschen;
                        KomponentenListe.Insert(index, neueKomponente);
                        _deletedTeile.Add(new LoeschenDto { Guid = restteil.Guid, DokumenteLoeschen = false });
                        ResteListe.Remove(restteil);
                    }
                }
            }
            
            UpdateResteKisteProperties();
            UpdateWunschlisteProperties();
        }

        public void Ausbauen(string guid)
        {
            var einzelteil = KomponentenListe.First(teil => teil.Guid == guid);
            if(einzelteil != null)
            {
                _deletedKomponenten.Add(new LoeschenDto { Guid = guid, DokumenteLoeschen = false });
                KomponentenListe.Remove(einzelteil);
                ResteListe.Add(new RestteilDto
                {
                    Guid = guid,
                    Beschreibung = einzelteil.Beschreibung, 
                    Groesse = einzelteil.Groesse,
                    Jahr = einzelteil.Jahr,
                    Hersteller = einzelteil.Hersteller,
                    Komponente = einzelteil.Komponente, 
                    Gewicht = einzelteil.Gewicht, 
                    Preis = einzelteil.Preis
                });
                IsDirty = true;
                UpdateResteKisteProperties();
            }
        }

        #endregion

        #region Hilfsfunktionen

        private void UpdateResteKisteProperties()
        {
            OnPropertyChanged("InhaltRestekiste");
            OnPropertyChanged("GewichtRestekiste");
            OnPropertyChanged("WertRestekiste");
        }

        private void UpdateWunschlisteProperties()
        {
            OnPropertyChanged("InhaltWunschliste");
            OnPropertyChanged("GewichtWunschliste");
            OnPropertyChanged("WertWunschliste");
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (IsDirty)
            {
                var window = sender as Window;
                var owner = window ?? Application.Current.MainWindow;
                if (HilfsFunktionen.ShowQuestionBox(owner, "Teileliste"))
                {
                    Sichern();
                }
            }

            PluginManager.CleanUp();
        }

        void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.CompareOrdinal(e.PropertyName, "AlternativenAnzeigen") != 0)
            {
                IsDirty = true;
                OnPropertyChanged("GesamtPreis");
                OnPropertyChanged("GesamtGewicht");
                OnPropertyChanged("SchonGewogen");
                OnPropertyChanged("BereitsGezahlt");
                OnPropertyChanged("SummeEinsparpotenzial");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal bool SetTeilelisteCollectionKomponenteProperty(
            string propertyName, 
            ref ObservableCollection<KomponenteViewModel> backingField, 
            ObservableCollection<KomponenteViewModel> newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        internal bool SetTeilelisteStringProperty(string propertyName, 
                                                    ref string backingField, 
                                                    string newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        internal void SetTeilelisteBoolProperty(string propertyName, 
                                                ref bool backingField, 
                                                bool newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                OnPropertyChanged(propertyName);
            }
        }

        internal void SetTeilelisteCollectionStringProperty(string propertyName,
                                                ref ObservableCollection<string> backingField, 
                                                ObservableCollection<string> newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                OnPropertyChanged(propertyName);
            }
        }

        internal void OnPropertyChanged(string property)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        internal void SetTeilelisteListProperty(string propertyName,
            ref List<RestteilDto> backingField,
            List<RestteilDto> newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                OnPropertyChanged(propertyName);
            }
        }

        internal void SetWunschlisteListProperty(string propertyName,
            ref List<WunschteilDto> backingField,
            List<WunschteilDto> newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                OnPropertyChanged(propertyName);
            }
        }

        #endregion
    }
}

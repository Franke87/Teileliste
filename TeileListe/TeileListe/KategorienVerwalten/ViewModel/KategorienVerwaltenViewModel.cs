using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.KategorienVerwalten.View;

namespace TeileListe.KategorienVerwalten.ViewModel
{
    internal class KategorienVerwaltenViewModel : MyCommonViewModel
    {
        #region Properties

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { SetProperty("IsDirty", ref _isDirty, value); }
        }

        private ObservableCollection<KategorieViewModel> _kategorienListe;
        public ObservableCollection<KategorieViewModel> KategorienListe
        {
            get { return _kategorienListe; }
            set { SetProperty("KategorienListe", ref _kategorienListe, value); }
        }

        private readonly List<string> _deletedItems;

        #endregion

        #region Commands

        public MyCommand ZuruecksetzenCommand { get; set; }
        public MyCommand SichernCommand { get; set; }
        public MyParameterCommand<Window> HinzufuegenCommand { get; set; }

        #endregion

        internal KategorienVerwaltenViewModel(string currentKategorie)
        {
            ZuruecksetzenCommand = new MyCommand(Zureucksetzen);
            SichernCommand = new MyCommand(Sichern);
            HinzufuegenCommand = new MyParameterCommand<Window>(Hinzufuegen);

            _deletedItems = new List<string>();

            var list = new List<string>();
            PluginManager.DbManager.GetDateiKategorien(ref list);
            KategorienListe = new ObservableCollection<KategorieViewModel>();

            foreach(var kategorie in list)
            {
                var viewModel = new KategorieViewModel(kategorie)
                {
                    LoeschenAction = Loeschen,
                    NachObenAction = NachObenSortieren,
                    NachUntenAction = NachUntenSortieren, 
                    GetBlackList = GetBlackList
                };
                viewModel.PropertyChanged += ContentPropertyChanged;
                KategorienListe.Add(viewModel);
            }

            IsDirty = false;

            if (!string.IsNullOrWhiteSpace(currentKategorie))
            {
                if (!KategorienListe.Any(item => item.Kategorie == currentKategorie))
                {
                    var viewModel = new KategorieViewModel(currentKategorie)
                    {
                        LoeschenAction = Loeschen,
                        NachObenAction = NachObenSortieren,
                        NachUntenAction = NachUntenSortieren,
                        GetBlackList = GetBlackList
                    };
                    viewModel.PropertyChanged += ContentPropertyChanged;
                    KategorienListe.Add(viewModel);
                    IsDirty = true;
                }
            }
        }

        #region Internal Functions

        internal void Zureucksetzen()
        {
            KategorienListe.Clear();
            _deletedItems.Clear();

            var list = new List<string>();
            PluginManager.DbManager.GetDateiKategorien(ref list);

            foreach (var kategorie in list)
            {
                var viewModel = new KategorieViewModel(kategorie)
                {
                    LoeschenAction = Loeschen,
                    NachObenAction = NachObenSortieren,
                    NachUntenAction = NachUntenSortieren,
                    GetBlackList = GetBlackList
                };
                viewModel.PropertyChanged += ContentPropertyChanged;
                KategorienListe.Add(viewModel);
            }

            IsDirty = false;
        }

        internal void Sichern()
        {
            PluginManager.DbManager.SaveDateiKategorien(KategorienListe.Select(item => item.Kategorie).ToList());

            IsDirty = false;
        }

        public void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }

        internal void Hinzufuegen(Window window)
        {
            var dialog = new KategorieBearbeitenView()
            {
                Owner = window
            };
            var viewModel = new KategorieBearbeitenViewModel(GetBlackList(), "")
            {
                CloseAction = dialog.Close
            };
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (viewModel.IsOk)
            {
                var newViewModel = new KategorieViewModel(viewModel.Kategorie)
                {
                    LoeschenAction = Loeschen,
                    NachObenAction = NachObenSortieren,
                    NachUntenAction = NachUntenSortieren,
                    GetBlackList = GetBlackList
                };
                newViewModel.PropertyChanged += ContentPropertyChanged;
                KategorienListe.Add(newViewModel);
                IsDirty = true;
            }
        }

        private void Loeschen(string guid)
        {
            var item = KategorienListe.First(teil => teil.Guid == guid);
            _deletedItems.Add(item.Kategorie);
            KategorienListe.Remove(item);
            IsDirty = true;
        }

        public void NachObenSortieren(string guid)
        {
            var teil1 = KategorienListe.First(teil => teil.Guid == guid);
            if (teil1 != null && KategorienListe.IndexOf(teil1) + 1 > 1)
            {
                var teil2 = KategorienListe[KategorienListe.IndexOf(teil1) - 1];

                if (teil2 != null && teil1.Kategorie != "Gewichtsmessung" && teil2.Kategorie != "Gewichtsmessung")
                {
                    KategorienListe.Move(KategorienListe.IndexOf(teil1), KategorienListe.IndexOf(teil2));
                    IsDirty = true;
                }
            }
        }

        public void NachUntenSortieren(string guid)
        {
            var teil1 = KategorienListe.First(teil => teil.Guid == guid);

            if (teil1 != null && KategorienListe.IndexOf(teil1) + 1 < KategorienListe.Count)
            {
                var teil2 = KategorienListe[KategorienListe.IndexOf(teil1) + 1];

                if (teil2 != null && teil1.Kategorie != "Gewichtsmessung" && teil2.Kategorie != "Gewichtsmessung")
                {
                    KategorienListe.Move(KategorienListe.IndexOf(teil1), KategorienListe.IndexOf(teil2));
                    IsDirty = true;
                }
            }
        }

        public string GetBlackList()
        {
            return KategorienListe.Aggregate(string.Empty, (current, item) => current + (";" + item.Kategorie + ";"));
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            if (IsDirty)
            {
                var window = sender as Window;
                var owner = window ?? Application.Current.MainWindow;
                if (HilfsFunktionen.ShowQuestionBox(owner, "Kategorien"))
                {
                    Sichern();
                }
            }
        }

        #endregion
    }
}

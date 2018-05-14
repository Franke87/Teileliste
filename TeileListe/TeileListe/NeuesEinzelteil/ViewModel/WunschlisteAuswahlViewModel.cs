using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TeileListe.Common.Classes;

namespace TeileListe.NeuesEinzelteil.ViewModel
{
    internal class WunschlisteAuswahlViewModel : MyCommonViewModel
    {
        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private ObservableCollection<WunschteilAuswahlViewModel> _wunschteile;

        public ObservableCollection<WunschteilAuswahlViewModel> WunschTeile
        {
            get { return _wunschteile; }
            set
            {
                SetProperty("WunschTeile", ref _wunschteile, value);
                HasError = !_wunschteile.Any(teil => teil.IsChecked);
            }
        }

        public WunschlisteAuswahlViewModel(List<WunschteilAuswahlViewModel> listWunschliste)
        {
            foreach (var item in listWunschliste)
            {
                item.PropertyChanged += ContentPropertyChanged;
            }
            
            WunschTeile = new ObservableCollection<WunschteilAuswahlViewModel>(listWunschliste);

            HasError = true;
        }

        private void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasError = !WunschTeile.Any(item => item.IsChecked);
        }
    }
}

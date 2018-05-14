using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TeileListe.Common.Classes;

namespace TeileListe.NeuesEinzelteil.ViewModel
{
    internal class RestekisteAuswahlViewModel : MyCommonViewModel
    {
        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private ObservableCollection<EinzelteilAuswahlViewModel> _einzelteile;

        public ObservableCollection<EinzelteilAuswahlViewModel> EinzelTeile
        {
            get { return _einzelteile; }
            set
            {
                SetProperty("EinzelTeile", ref _einzelteile, value);
                HasError = !_einzelteile.Any(teil => teil.IsChecked);
            }
        }

        public RestekisteAuswahlViewModel(List<EinzelteilAuswahlViewModel> listRestekiste)
        {
            foreach (var item in listRestekiste)
            {
                item.PropertyChanged += ContentPropertyChanged;
            }
            
            EinzelTeile = new ObservableCollection<EinzelteilAuswahlViewModel>(listRestekiste);

            HasError = true;
        }

        private void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasError = !EinzelTeile.Any(item => item.IsChecked);
        }
    }
}

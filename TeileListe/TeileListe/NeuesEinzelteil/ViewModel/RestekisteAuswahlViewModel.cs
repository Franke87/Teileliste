using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace TeileListe.NeuesEinzelteil.ViewModel
{
    class RestekisteAuswahlViewModel : INotifyPropertyChanged
    {
        #region Properties

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetRestekisteAuswahlBoolProperty("HasError", ref _hasError, value); }
        }

        private ObservableCollection<EinzelteilAuswahlViewModel> _einzelteile;

        public ObservableCollection<EinzelteilAuswahlViewModel> EinzelTeile
        {
            get { return _einzelteile; }
            set
            {
                SetRestekisteAuswahlCollectionProperty("EinzelTeile", ref _einzelteile, value);
                HasError = !_einzelteile.Any(teil => teil.IsChecked);
            }
        }

        #endregion

        public RestekisteAuswahlViewModel(List<EinzelteilAuswahlViewModel> listRestekiste)
        {
            foreach (var item in listRestekiste)
            {
                item.PropertyChanged += ContentPropertyChanged;
            }
            
            EinzelTeile = new ObservableCollection<EinzelteilAuswahlViewModel>(listRestekiste);

            HasError = true;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetRestekisteAuswahlCollectionProperty(string propertyName,
            ref ObservableCollection<EinzelteilAuswahlViewModel> backingField,
            ObservableCollection<EinzelteilAuswahlViewModel> newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        internal void SetRestekisteAuswahlBoolProperty(string propertyName, ref bool backingField, bool newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasError = !EinzelTeile.Any(item => item.IsChecked);
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace TeileListe.NeuesEinzelteil.ViewModel
{
    class WunschlisteAuswahlViewModel : INotifyPropertyChanged
    {
        #region Properties

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetWunschlisteAuswahlBoolProperty("HasError", ref _hasError, value); }
        }

        private ObservableCollection<WunschteilAuswahlViewModel> _wunschteile;

        public ObservableCollection<WunschteilAuswahlViewModel> WunschTeile
        {
            get { return _wunschteile; }
            set
            {
                SetWunschlisteAuswahlCollectionProperty("WunschTeile", ref _wunschteile, value);
                HasError = !_wunschteile.Any(teil => teil.IsChecked);
            }
        }

        #endregion

        public WunschlisteAuswahlViewModel(List<WunschteilAuswahlViewModel> listWunschliste)
        {
            foreach (var item in listWunschliste)
            {
                item.PropertyChanged += ContentPropertyChanged;
            }
            
            WunschTeile = new ObservableCollection<WunschteilAuswahlViewModel>(listWunschliste);

            HasError = true;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetWunschlisteAuswahlCollectionProperty(string propertyName,
            ref ObservableCollection<WunschteilAuswahlViewModel> backingField,
            ObservableCollection<WunschteilAuswahlViewModel> newValue)
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

        internal void SetWunschlisteAuswahlBoolProperty(string propertyName, ref bool backingField, bool newValue)
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
            HasError = !WunschTeile.Any(item => item.IsChecked);
        }

        #endregion
    }
}

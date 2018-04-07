using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TeileListe.Common.ViewModel
{
    public class KategorienViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<KategorienViewModel> _unterKategorien;

        public ObservableCollection<KategorienViewModel> UnterKategorien
        {
            get { return _unterKategorien; }
            set { SetKategorienViewModelProperty("UnterKategorien", ref _unterKategorien, value); }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetKategorienViewModelProperty("IsSelected", ref _isSelected, value);
                if (SelectionChanged != null)
                {
                    SelectionChanged.Invoke();
                }
            }
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public bool EnthaeltProdukte { get; set; }
        public int AnzahlProdukte { get; set; }

        public Action SelectionChanged { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetKategorienViewModelProperty<T>(string propertyName, ref T backingField, T newValue)
        {
            bool changed;

            // ReSharper disable CompareNonConstrainedGenericWithNull
            if (newValue == null && backingField != null || newValue != null && backingField == null)
            {
                changed = true;
            }
            else if ((newValue == null && backingField == null) || backingField.Equals(newValue))
            {
                changed = false;
            }
            else
            {
                changed = true;
            }
            if (changed)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            // ReSharper restore CompareNonConstrainedGenericWithNull
        }

        #endregion
    }
}

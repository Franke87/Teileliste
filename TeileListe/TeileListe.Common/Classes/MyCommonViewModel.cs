using System.ComponentModel;

namespace TeileListe.Common.Classes
{
    public class MyCommonViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public bool SetProperty<T>(string propertyName, ref T backingField, T newValue)
        {
            bool changed;

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            return changed;
        }

        public void UpdateProperty(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

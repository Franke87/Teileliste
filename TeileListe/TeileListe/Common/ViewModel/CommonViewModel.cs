using System.ComponentModel;

namespace TeileListe.Common.ViewModel
{
    public class CommonViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetDecimalProperty(string propertyName,
                                                    ref decimal backingField,
                                                    decimal newValue)
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

        internal bool SetCommonBoolProperty(string propertyName,
                                            ref bool backingField,
                                            bool newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                return true;
            }
            return false;
        }

        internal bool SetCommonIntProperty(string propertyName,
                                            ref int backingField,
                                            int newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                return true;
            }
            return false;
        }

        internal bool SetCommonStringProperty(string propertyName,
                                                ref string backingField,
                                                string newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                return true;
            }
            return false;
        }

        internal void SetProperty<T>(string propertyName, ref T backingField, T newValue)
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
        }

        #endregion
    }
}

using System.ComponentModel;
using TeileListe.Common.Classes;

namespace TeileListe.NeuesEinzelteil.ViewModel
{
    public class DatenbankteilAuswahlViewModel : INotifyPropertyChanged
    {
        public string Komponente { get; set; }
        public string Hersteller { get; set; }
        public string Beschreibung { get; set; }
        public string Groesse { get; set; }
        public string Jahr { get; set; }
        public string DatenbankId { get; set; }
        public string DatenbankLink { get; set; }
        public int Gewicht { get; set; }

        public string AnzeigeName
        {
            get { return HilfsFunktionen.GetAnzeigeName(Hersteller, Beschreibung, Groesse, Jahr); }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetWunschteilAuswahlBoolProperty("IsChecked", ref _isChecked, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetWunschteilAuswahlBoolProperty(string propertyName, ref bool backingField, bool newValue)
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
    }
}

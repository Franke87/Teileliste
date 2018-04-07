using System.ComponentModel;
using TeileListe.Enums;

namespace TeileListe.NeuesEinzelteil.ViewModel
{
    public class NeuesEinzelteilNeuViewModel : INotifyPropertyChanged
    {
        #region Properties

        public EinzelteilBearbeitenEnum Typ { get; set; }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetNeuesEinzelteilBoolProperty("HasError", ref _hasError, value); }
        }

        private string _komponente;
        public string Komponente
        {
            get { return _komponente; }
            set
            {
                SetNeuesEinzelteilStringProperty("Komponente", ref _komponente, value);
                HasError = string.IsNullOrWhiteSpace(Komponente);
            }
        }

        private string _hersteller;
        public string Hersteller
        {
            get { return _hersteller; }
            set { SetNeuesEinzelteilStringProperty("Hersteller", ref _hersteller, value); }
        }

        private string _beschreibung;
        public string Beschreibung
        {
            get { return _beschreibung; }
            set { SetNeuesEinzelteilStringProperty("Beschreibung", ref _beschreibung, value); }
        }

        private string _groesse;
        public string Groesse
        {
            get { return _groesse; }
            set { SetNeuesEinzelteilStringProperty("Groesse", ref _groesse, value); }
        }

        private string _jahr;
        public string Jahr
        {
            get { return _jahr; }
            set { SetNeuesEinzelteilStringProperty("Jahr", ref _jahr, value); }
        }

        private string _shop;
        public string Shop
        {
            get { return _shop; }
            set { SetNeuesEinzelteilStringProperty("Shop", ref _shop, value); }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { SetNeuesEinzelteilStringProperty("Link", ref _link, value); }
        }

        public string DatenbankId { get { return string.Empty; }}

        private int _preis;
        public int Preis
        {
            get { return _preis; }
            set { SetNeuesEinzelteilIntProperty("Preis", ref _preis, value); }
        }

        private int _gewicht;
        public int Gewicht
        {
            get { return _gewicht; }
            set { SetNeuesEinzelteilIntProperty("Gewicht", ref _gewicht, value); }
        }

        private bool _gekauft;
        public bool Gekauft
        {
            get { return _gekauft; }
            set { SetNeuesEinzelteilBoolProperty("Gekauft", ref _gekauft, value); }
        }

        private bool _gewogen;
        public bool Gewogen
        {
            get { return _gewogen; }
            set { SetNeuesEinzelteilBoolProperty("Gewogen", ref _gewogen, value); }
        }

        #endregion

        #region Konstruktor

        public NeuesEinzelteilNeuViewModel(EinzelteilBearbeitenEnum typ)
        {
            HasError = true;
            Typ = typ;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetNeuesEinzelteilIntProperty(string propertyName,
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
            }
        }

        internal void SetNeuesEinzelteilStringProperty(string propertyName,
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
            }
        }

        internal void SetNeuesEinzelteilBoolProperty(string propertyName,
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
            }
        }

        #endregion
    }
}

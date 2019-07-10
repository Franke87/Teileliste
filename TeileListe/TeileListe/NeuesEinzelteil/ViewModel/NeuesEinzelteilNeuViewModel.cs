using System.ComponentModel;
using TeileListe.Common.Classes;
using TeileListe.Enums;

namespace TeileListe.NeuesEinzelteil.ViewModel
{
    public class NeuesEinzelteilNeuViewModel : MyCommonViewModel
    {
        #region Properties

        public EinzelteilBearbeitenEnum Typ { get; set; }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private string _komponente;
        public string Komponente
        {
            get { return _komponente; }
            set
            {
                SetProperty("Komponente", ref _komponente, value);
                HasError = string.IsNullOrWhiteSpace(Komponente);
            }
        }

        private string _hersteller;
        public string Hersteller
        {
            get { return _hersteller; }
            set { SetProperty("Hersteller", ref _hersteller, value); }
        }

        private string _beschreibung;
        public string Beschreibung
        {
            get { return _beschreibung; }
            set { SetProperty("Beschreibung", ref _beschreibung, value); }
        }

        private string _groesse;
        public string Groesse
        {
            get { return _groesse; }
            set { SetProperty("Groesse", ref _groesse, value); }
        }

        private string _jahr;
        public string Jahr
        {
            get { return _jahr; }
            set { SetProperty("Jahr", ref _jahr, value); }
        }

        private string _shop;
        public string Shop
        {
            get { return _shop; }
            set { SetProperty("Shop", ref _shop, value); }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { SetProperty("Link", ref _link, value); }
        }

        public string DatenbankId { get { return string.Empty; }}

        private int _preis;
        public int Preis
        {
            get { return _preis; }
            set { SetProperty("Preis", ref _preis, value); }
        }

        private int _gewicht;
        public int Gewicht
        {
            get { return _gewicht; }
            set { SetProperty("Gewicht", ref _gewicht, value); }
        }

        private bool _gekauft;
        public bool Gekauft
        {
            get { return _gekauft; }
            set { SetProperty("Gekauft", ref _gekauft, value); }
        }

        private bool _gewogen;
        public bool Gewogen
        {
            get { return _gewogen; }
            set { SetProperty("Gewogen", ref _gewogen, value); }
        }

        #endregion

        #region Konstruktor

        public NeuesEinzelteilNeuViewModel(EinzelteilBearbeitenEnum typ)
        {
            HasError = true;
            Typ = typ;
        }

        #endregion
    }
}

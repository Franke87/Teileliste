using System;
using TeileListe.Common.Classes;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class SzenarioKomponenteViewModel : MyCommonViewModel
    {
        #region Komponente Properties

        private string _komponente;
        public string Komponente
        {
            get { return _komponente; }
            set { SetProperty("Komponente", ref _komponente, value); }
        }

        public string Beschreibung { get; set; }

        public int Gewicht { get; set; }
        
        public string Guid { get; set; }

        #endregion

        #region Alternative Properties

        private string _alternativeHersteller;
        public string AlternativeHersteller
        {
            get { return _alternativeHersteller; }
            set
            {
                if (SetProperty("AlternativeHersteller", ref _alternativeHersteller, value))
                {
                    UpdateProperty("AlternativeName");
                }
            }
        }

        private string _alternativeBeschreibung;
        public string AlternativeBeschreibung
        {
            get { return _alternativeBeschreibung; }
            set
            {
                if (SetProperty("AlternativeBeschreibung", ref _alternativeBeschreibung, value))
                {
                    UpdateProperty("AlternativeName");
                }
            }
        }

        private string _alternativeGroesse;
        public string AlternativeGroesse
        {
            get { return _alternativeGroesse; }
            set
            {
                if (SetProperty("AlternativeGroesse", ref _alternativeGroesse, value))
                {
                    UpdateProperty("AlternativeName");
                }
            }
        }

        private string _alternativeJahr;
        public string AlternativeJahr
        {
            get { return _alternativeJahr; }
            set
            {
                if (SetProperty("AlternativeJahr", ref _alternativeJahr, value))
                {
                    UpdateProperty("AlternativeName");
                }
            }
        }

        private int _alternativeDifferenz;
        public int AlternativeDifferenz
        {
            get { return _alternativeDifferenz; }
            set { SetProperty("AlternativeDifferenz", ref _alternativeDifferenz, value); }
        }

        private bool _alternativeVorhanden;
        public bool AlternativeVorhanden
        {
            get { return _alternativeVorhanden; }
            set
            {
                if (SetProperty("AlternativeVorhanden", ref _alternativeVorhanden, value))
                {
                    UpdateProperty("ZuordnenVisible");
                    UpdateProperty("KannEntfernen");
                    UpdateProperty("AlternativeName");
                }
            }
        }

        public string AlternativeName
        {
            get
            {
                if(AlternativeVorhanden)
                {
                    return HilfsFunktionen.GetAnzeigeName(AlternativeHersteller, 
                                                            AlternativeBeschreibung, 
                                                            AlternativeGroesse, 
                                                            AlternativeJahr);
                }
                return null;
            }
        }

        #endregion

        #region Links und Actions

        public bool ZuordnenVisible { get { return Beschreibung == null || AlternativeName == null; } }

        public bool KannEntfernen { get { return Beschreibung == null || AlternativeName != null; } }

        public MyCommand EntfernenCommand { get; set; }

        public Action<string> LoeschenAction { get; set; }

        #endregion

        #region Konstruktor

        public SzenarioKomponenteViewModel()
        {
            EntfernenCommand = new MyCommand(OnEntfernen);
        }

        #endregion

        #region Funktionen

        private void OnEntfernen()
        {
            if(AlternativeName != null)
            {
                AlternativeVorhanden = false;
                AlternativeHersteller = "";
                AlternativeBeschreibung = "";
                AlternativeGroesse = "";
                AlternativeJahr = "";
                AlternativeDifferenz = 0 - Gewicht;
            }

            if (Beschreibung == null)
            {
                LoeschenAction(Guid);
            }
        }

        #endregion
    }
}

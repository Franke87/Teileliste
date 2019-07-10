using System;
using TeileListe.Common.Classes;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class SzenarioAlternativeViewModel : MyCommonViewModel
    {
        public string Guid { get; set; }
        public string Komponente { get; set; }
        public string Hersteller { get; set; }
        public string Beschreibung { get; set; }
        public string Groesse { get; set; }
        public string Jahr { get; set; }
        public int Gewicht { get; set; }

        public string AnzeigeName
        {
            get
            {
                return HilfsFunktionen.GetAnzeigeName(Hersteller, Beschreibung, Groesse, Jahr);
            }
        }

        private int _differenz;
        public int Differenz
        {
            get { return _differenz; }
            set { SetProperty("Differenz", ref _differenz, value); }
        }

        public MyCommand EinbauenCommand { get; set; }
        public MyCommand TauschenCommand { get; set; }

        public Action<string> EinbauenAction { get; set; }
        public Action<string> TauschenAction { get; set; }

        internal SzenarioAlternativeViewModel()
        {
            EinbauenCommand = new MyCommand(OnEinbauen);
            TauschenCommand = new MyCommand(OnTauschen);
        }

        private void OnEinbauen()
        {
            EinbauenAction(Guid);
        }

        private void OnTauschen()
        {
            TauschenAction(Guid);
        }
    }
}

using System;
using TeileListe.Common.Classes;

namespace TeileListe.NeuesEinzelteil.ViewModel
{
    public class DatenbankteilAuswahlViewModel : MyCommonViewModel
    {
        public string Komponente { get; set; }
        public string Hersteller { get; set; }
        public string Beschreibung { get; set; }
        public string Groesse { get; set; }
        public string Jahr { get; set; }
        public string DatenbankId { get; set; }
        public string DatenbankLink { get; set; }
        public int Gewicht { get; set; }

        private int _differenz;
        public int Differenz
        {
            get { return _differenz; }
            set { SetProperty("Differenz", ref _differenz, value); }
        }

        public string AnzeigeName
        {
            get { return HilfsFunktionen.GetAnzeigeName(Hersteller, Beschreibung, Groesse, Jahr); }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty("IsChecked", ref _isChecked, value); }
        }

        public MyCommand EinbauenCommand { get; set; }
        public MyCommand TauschenCommand { get; set; }

        public Action<string, string, int> EinbauenAction { get; set; }
        public Action<string, int> TauschenAction { get; set; }

        internal DatenbankteilAuswahlViewModel()
        {
            EinbauenCommand = new MyCommand(OnEinbauen);
            TauschenCommand = new MyCommand(OnTauschen);
        }

        private void OnEinbauen()
        {
            EinbauenAction(Komponente, AnzeigeName, Gewicht);
        }

        private void OnTauschen()
        {
            TauschenAction(AnzeigeName, Differenz);
        }
    }
}

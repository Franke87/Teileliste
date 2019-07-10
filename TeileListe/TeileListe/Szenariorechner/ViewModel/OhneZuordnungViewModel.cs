using System;
using TeileListe.Common.Classes;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class OhneZuordnungViewModel : MyCommonViewModel
    {
        private string _komponente;
        public string Komponente
        {
            get { return _komponente; }
            set { SetProperty("Komponente", ref _komponente, value); }
        }

        public string Beschreibung { get; set; }

        private int _gewicht;
        public int Gewicht
        {
            get { return _gewicht; }
            set { SetProperty("Gewicht", ref _gewicht, value); }
        }

        public string Guid { get; set; }

        private int _differenz;
        public int Differenz
        {
            get { return _differenz; }
            set { SetProperty("Differenz", ref _differenz, value); }
        }

        private string _alternative;
        public string Alternative
        {
            get { return _alternative; }
            set { SetProperty("Alternative", ref _alternative, value); }
        }

        public Action<string> ZuordnenAction { get; set; }
        public MyCommand ZuordnenCommand { get; set; }

        internal OhneZuordnungViewModel()
        {
            ZuordnenCommand = new MyCommand(OnZuordnen);
        }

        private void OnZuordnen()
        {
            ZuordnenAction(Guid);
        }
    }
}

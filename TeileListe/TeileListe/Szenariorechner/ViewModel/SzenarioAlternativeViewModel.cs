using System;
using TeileListe.Common.Classes;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class SzenarioAlternativeViewModel : MyCommonViewModel
    {
        public string Komponente { get; set; }
        public int Gewicht { get; set; }
        public string AnzeigeName { get; set; }
        public string Guid { get; set; }

        private int _differenz;
        public int Differenz
        {
            get { return _differenz; }
            set { SetProperty("Differenz", ref _differenz, value); }
        }

        public MyCommand EinbauenCommand { get; set; }

        public Action<string> EinbauenAction { get; set; }

        internal SzenarioAlternativeViewModel()
        {
            EinbauenCommand = new MyCommand(OnEinbauen);
        }

        private void OnEinbauen()
        {
            EinbauenAction(Guid);
        }
    }
}

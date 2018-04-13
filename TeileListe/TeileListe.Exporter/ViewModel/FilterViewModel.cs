using System;
using TeileListe.Common.Classes;

namespace TeileListe.Exporter.ViewModel
{
    internal class FilterViewModel : MyCommonViewModel
    {
        public string Anzeige { get; set; }
        public int FilterTyp { get; set; }

        public Action<int, string> AlleAction { get; set; }
        public Action<int, string> KeineAction { get; set; }

        public MyCommand AlleCommand { get; set; }
        public MyCommand KeineCommand { get; set; }

        internal FilterViewModel()
        {
            AlleCommand = new MyCommand(OnAlle);
            KeineCommand = new MyCommand(OnKeine);
        }

        private void OnAlle()
        {
            AlleAction(FilterTyp, Anzeige);
        }

        private void OnKeine()
        {
            KeineAction(FilterTyp, Anzeige);
        }
    }
}

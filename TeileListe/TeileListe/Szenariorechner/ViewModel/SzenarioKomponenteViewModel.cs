using System.Windows;
using TeileListe.Common.Classes;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class SzenarioKomponenteViewModel : MyCommonViewModel
    {
        public string Komponente { get; set; }
        public int Gewicht { get; set; }
        public string Beschreibung { get; set; }
        public string Alternative { get; set; }
        public int Differenz { get; set; }

        public MyParameterCommand<Window> TauschenCommand { get; set; }

        public SzenarioKomponenteViewModel()
        {
            TauschenCommand = new MyParameterCommand<Window>(OnTauschen);
        }

        private void OnTauschen(Window window)
        {
            HilfsFunktionen.ShowMessageBox(window, "Test", Komponente, false);
        }

    }
}

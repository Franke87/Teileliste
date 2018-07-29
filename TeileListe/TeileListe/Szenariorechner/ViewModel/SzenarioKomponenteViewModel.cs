﻿using System.Windows;
using TeileListe.Common.Classes;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class SzenarioKomponenteViewModel : MyCommonViewModel
    {
        public string Komponente { get; set; }
        public int Gewicht { get; set; }
        public string Beschreibung { get; set; }

        private string _alternative;
        public string Alternative
        {
            get { return _alternative; }
            set
            {
                if(SetProperty("Alternative", ref _alternative, value))
                {
                    UpdateProperty("ZuordnenVisible");
                }
            }
        }

        private int _differenz;
        public int Differenz
        {
            get { return _differenz; }
            set { SetProperty("Differenz", ref _differenz, value); }
        }

        public bool ZuordnenVisible { get { return Beschreibung == null || Alternative == null; } }

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

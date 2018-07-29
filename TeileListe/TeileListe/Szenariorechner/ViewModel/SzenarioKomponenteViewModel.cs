using System;
using TeileListe.Common.Classes;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class SzenarioKomponenteViewModel : MyCommonViewModel
    {
        public string Komponente { get; set; }
        public int Gewicht { get; set; }
        public string Beschreibung { get; set; }
        public string Guid { get; set; }

        private string _alternative;
        public string Alternative
        {
            get { return _alternative; }
            set
            {
                if(SetProperty("Alternative", ref _alternative, value))
                {
                    UpdateProperty("ZuordnenVisible");
                    UpdateProperty("KannEntfernen");
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

        public bool KannEntfernen { get { return Beschreibung == null || Alternative != null; } }

        public MyCommand EntfernenCommand { get; set; }

        public Action<string> LoeschenAction { get; set; }

        public SzenarioKomponenteViewModel()
        {
            EntfernenCommand = new MyCommand(OnEntfernen);
        }

        private void OnEntfernen()
        {
            if(Alternative != null)
            {
                Alternative = null;
                Differenz = 0;
            }

            if (Beschreibung == null)
            {
                LoeschenAction(Guid);
            }
        }

    }
}

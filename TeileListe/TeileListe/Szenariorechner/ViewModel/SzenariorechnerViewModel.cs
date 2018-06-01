using System.Collections.Generic;
using System.Collections.ObjectModel;
using TeileListe.Classes;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Teileliste.ViewModel;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class SzenariorechnerViewModel : MyCommonViewModel
    {
        public string Teststring { get { return "Test"; } }

        private ObservableCollection<SzenarioKomponenteViewModel> _vergleichsListe;
        public ObservableCollection<SzenarioKomponenteViewModel> VergleichsListe
        {
            get { return _vergleichsListe; }
            set { SetProperty("VergleichsListe", ref _vergleichsListe, value); }
        }

        public SzenariorechnerViewModel(List<KomponenteViewModel> list, string alternative)
        {
            var liste = new List<KomponenteDto>();
            PluginManager.DbManager.GetKomponente(alternative, ref liste);

            VergleichsListe = new ObservableCollection<SzenarioKomponenteViewModel>();

            foreach(var item in list)
            {

                var vm = new SzenarioKomponenteViewModel();
                vm.Komponente = item.Komponente;
                vm.Gewicht = item.Gewicht;
                vm.Beschreibung = item.AnzeigeName;

                var komponente = liste.Find(x => x.Komponente == item.Komponente);
                if (komponente != null)
                {
                    vm.Alternative = komponente.Beschreibung;
                    vm.Differenz = komponente.Gewicht - vm.Gewicht;
                }
                else
                {
                    vm.Alternative = "";
                    vm.Differenz = 0;
                }
                VergleichsListe.Add(vm);
            }
        }
    }
}

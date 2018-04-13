using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TeileListe.Common.Classes;

namespace TeileListe.Exporter.ViewModel
{
    internal class OrdnerViewModel : MyCommonViewModel
    {
        public string Komponente { get; set; }
        public string AnzeigeText { get; set; }
        public string Guid { get; set; }

        private ObservableCollection<DateiViewModel> _dateiViewModelListe;
        public ObservableCollection<DateiViewModel> DateiViewModelListe
        {
            get { return _dateiViewModelListe; }
            set { SetProperty("DateiViewModelListe", ref _dateiViewModelListe, value); }
        }

        internal OrdnerViewModel(IEnumerable<DateiViewModel> dateiViewmodels)
        {
            DateiViewModelListe = new ObservableCollection<DateiViewModel>();

            foreach (var item in dateiViewmodels)
            {
                item.PropertyChanged += ContentPropertyChanged;
                DateiViewModelListe.Add(item);
            }
        }

        private void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateProperty("DateiViewModelListe");
        }
    }
}

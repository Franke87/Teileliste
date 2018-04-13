using TeileListe.Common.Classes;
using TeileListe.Common.Dto;

namespace TeileListe.Exporter.ViewModel
{
    internal class DateiViewModel : MyCommonViewModel
    {
        public string Kategorie { get; set; }
        public string Beschreibung { get; set; }
        public string Dateiendung { get; set; }
        public string Guid { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty("IsChecked", ref _isChecked, value); }
        }

        internal DateiViewModel(DateiDto datei)
        {
            Kategorie = datei.Kategorie;
            Beschreibung = datei.Beschreibung;
            Dateiendung = datei.Dateiendung;
            Guid = datei.Guid;
            IsChecked = true;
        }
    }
}

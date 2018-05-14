using System;
using System.Collections.ObjectModel;
using TeileListe.Common.Classes;

namespace TeileListe.Common.ViewModel
{
    public class KategorienViewModel : MyCommonViewModel
    {
        private ObservableCollection<KategorienViewModel> _unterKategorien;

        public ObservableCollection<KategorienViewModel> UnterKategorien
        {
            get { return _unterKategorien; }
            set { SetProperty("UnterKategorien", ref _unterKategorien, value); }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty("IsSelected", ref _isSelected, value);
                if (SelectionChanged != null)
                {
                    SelectionChanged.Invoke();
                }
            }
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public bool EnthaeltProdukte { get; set; }
        public int AnzahlProdukte { get; set; }

        public Action SelectionChanged { get; set; }
    }
}

using TeileListe.Common.Classes;

namespace TeileListe.Szenariorechner.ViewModel
{
    internal class NeueAlternativeViewModel : MyCommonViewModel
    {
        public bool IsOk { get; set; }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        internal NeueAlternativeViewModel()
        {
            HasError = true;
            IsOk = false;
        }
    }
}

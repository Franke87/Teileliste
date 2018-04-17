using System;
using TeileListe.Common.Classes;

namespace TeileListe.KategorienVerwalten.ViewModel
{
    internal class KategorieBearbeitenViewModel : MyCommonViewModel
    {
        private string _blackList;

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private string _kategorie;
        public string Kategorie
        {
            get { return _kategorie; }
            set
            {
                SetProperty("Kategorie", ref _kategorie, value);
                HasError = HasValidationError();
            }
        }

        public MyCommand OnOkCommand { get; set; }

        public Action CloseAction { get; set; }

        public bool IsOk { get; set; }

        public KategorieBearbeitenViewModel(string blackList, string kategorie)
        {
            _blackList = blackList;
            OnOkCommand = new MyCommand(OnOkFunc);
            Kategorie = kategorie;
            HasError = HasValidationError();
        }

        internal bool HasValidationError()
        {
            return string.IsNullOrWhiteSpace(Kategorie) || _blackList.Contains(";" + Kategorie + ";");
        }

        public void OnOkFunc()
        {
            IsOk = true;
            CloseAction();
        }
    }
}
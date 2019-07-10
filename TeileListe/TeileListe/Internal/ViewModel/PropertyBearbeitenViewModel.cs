using System;
using TeileListe.Common.Classes;

namespace TeileListe.Internal.ViewModel
{
    internal class PropertyBearbeitenViewModel : MyCommonViewModel
    {
        private string _blackList;

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private string _property;
        public string Property
        {
            get { return _property; }
            set
            {
                SetProperty("Property", ref _property, value);
                HasError = HasValidationError();
            }
        }

        public string LabelText { get; set; }

        public MyCommand OnOkCommand { get; set; }

        public Action CloseAction { get; set; }

        public bool IsOk { get; set; }

        public PropertyBearbeitenViewModel(string blackList, string property, string labelText)
        {
            _blackList = blackList;
            OnOkCommand = new MyCommand(OnOkFunc);
            Property = property;
            LabelText = labelText;
            HasError = HasValidationError();
        }

        internal bool HasValidationError()
        {
            return string.IsNullOrWhiteSpace(Property) || _blackList.Contains(";" + Property + ";");
        }

        public void OnOkFunc()
        {
            IsOk = true;
            CloseAction();
        }
    }
}
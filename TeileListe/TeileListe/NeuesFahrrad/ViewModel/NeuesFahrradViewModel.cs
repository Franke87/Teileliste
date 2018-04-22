using System;
using System.ComponentModel;
using Microsoft.Win32;
using TeileListe.Classes;
using TeileListe.Common.Classes;

namespace TeileListe.NeuesFahrrad.ViewModel
{
    internal class NeuesFahrradViewModel : MyCommonViewModel
    {
        #region Properties

        public bool IsOk { get; set; }
        public string Blacklist { get; set; }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private bool _neuesFahrradAusgewaehlt;
        public bool NeuesFahrradAusgewaehlt
        {
            get { return _neuesFahrradAusgewaehlt; }
            set
            {
                SetProperty("NeuesFahrradAusgewaehlt", 
                                            ref _neuesFahrradAusgewaehlt, 
                                            value);
                HasError = HasValidationError();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty("Name", ref _name, value);
                HasError = HasValidationError();
            }
        }

        private string _datei;
        public string Datei
        {
            get { return _datei; }
            set
            {
                SetProperty("Datei", ref _datei, value);
                HasError = HasValidationError();
            }
        }

        public MyCommand OnOkCommand { get; set; }
        public MyCommand OnFileSelect { get; set; }

        public Action CloseAction { get; set; }

        #endregion

        internal NeuesFahrradViewModel(string blacklist)
        {
            Blacklist = blacklist;
            OnOkCommand = new MyCommand(OnOkFunc);
            OnFileSelect = new MyCommand(OpenFile);
            HasError = true;
            NeuesFahrradAusgewaehlt = true;
        }

        internal bool HasValidationError()
        {
            bool hasError = string.IsNullOrWhiteSpace(_name) || Blacklist.Contains(";" + _name + ";");
            if (!hasError && !NeuesFahrradAusgewaehlt)
            {
                hasError = string.IsNullOrWhiteSpace(Datei) || !(Datei.EndsWith(".csv") || Datei.EndsWith(".zip"));
            }
            return hasError;
        }

        private void OpenFile()
        {
            var dialog = new OpenFileDialog 
            {
                Filter = "Importdateien (.csv oder .zip)| *.csv; *.zip",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DereferenceLinks = false
            };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Datei = dialog.FileName;
            }
        }

        public void OnOkFunc()
        {
            IsOk = true;
            CloseAction();
        }

    }
}

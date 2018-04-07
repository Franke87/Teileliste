using System;
using System.ComponentModel;
using Microsoft.Win32;
using TeileListe.Classes;

namespace TeileListe.NeuesFahrrad.ViewModel
{
    internal class NeuesFahrradViewModel : INotifyPropertyChanged
    {
        #region Properties

        public bool IsOk { get; set; }
        public string Blacklist { get; set; }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetNeuesFahrradBoolProperty("HasError", ref _hasError, value); }
        }

        private bool _neuesFahrradAusgewaehlt;
        public bool NeuesFahrradAusgewaehlt
        {
            get { return _neuesFahrradAusgewaehlt; }
            set
            {
                SetNeuesFahrradBoolProperty("NeuesFahrradAusgewaehlt", 
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
                SetNeuesFahrradStringProperty("Name", ref _name, value);
                HasError = HasValidationError();
            }
        }

        private string _datei;
        public string Datei
        {
            get { return _datei; }
            set
            {
                SetNeuesFahrradStringProperty("Datei", ref _datei, value);
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
                hasError = string.IsNullOrWhiteSpace(Datei) || !Datei.EndsWith(".csv");
            }
            return hasError;
        }

        private void OpenFile()
        {
            var dialog = new OpenFileDialog 
            {
                DefaultExt = ".csv", 
                Filter = "csv-Dateien (.csv)|*.csv", 
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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetNeuesFahrradBoolProperty(string propertyName, 
                                                    ref bool backingField, 
                                                    bool newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        internal void SetNeuesFahrradStringProperty(string propertyName, 
                                                    ref string backingField, 
                                                    string newValue)
        {
            if (backingField != newValue)
            {
                backingField = newValue;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        #endregion
    }
}

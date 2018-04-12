using System;
using System.ComponentModel;
using Microsoft.Win32;
using TeileListe.Classes;
using TeileListe.Enums;

namespace TeileListe.Common.ViewModel
{
    public class CommonDateiViewModel : INotifyPropertyChanged
    {
        #region Actions

        public MyCommand OnFileSelect { get; set; }

        #endregion

        #region Properties

        private readonly DateiOeffnenEnum _typ;

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetNeuesEinzelteilBoolProperty("HasError", ref _hasError, value); }
        }

        private string _datei;
        public string Datei
        {
            get { return _datei; }
            set
            {
                SetNeuesEinzelteilStringProperty("Datei", ref _datei, value);
                HasError = string.IsNullOrWhiteSpace(Datei);
            }
        }

        #endregion

        #region Private Funktionen

        private void OpenFile()
        {
            var ext = string.Empty;
            var filter = string.Empty;

            switch (_typ)
            {
                case DateiOeffnenEnum.Csv:
                {
                    ext = ".csv";
                    filter = "csv-Dateien (.csv)|*.csv";
                    break;
                }
                case DateiOeffnenEnum.Image:
                {
                    ext = ".png, ";
                    filter = "Fotos |*.jpg;*.jpeg;*.png";
                    break;
                }
                case DateiOeffnenEnum.All:
                    {
                        ext = ".*, ";
                        filter = "Alle Dateien |*.*";
                        break;
                    }
            }
            var dialog = new OpenFileDialog
            {
                DefaultExt = ext,
                Filter = filter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DereferenceLinks = false
            };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Datei = dialog.FileName;
            }
        }

        #endregion

        #region Konstruktor

        public CommonDateiViewModel(DateiOeffnenEnum typ)
        {
            _typ = typ;
            OnFileSelect = new MyCommand(OpenFile);
            HasError = true;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetNeuesEinzelteilIntProperty(string propertyName,
                                                    ref int backingField,
                                                    int newValue)
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

        internal void SetNeuesEinzelteilStringProperty(string propertyName,
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

        internal void SetNeuesEinzelteilBoolProperty(string propertyName,
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

        #endregion
    }
}

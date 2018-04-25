using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;

namespace TeileListe.MessungHochladen.ViewModel
{
    public class DateiAuswahlViewModel : MyCommonViewModel
    {
        public string Guid { get; set; }
        public string Beschreibung { get; set; }
        public string Dateiendung { get; set; }

        private string _komponenteGuid;
        private string _titelText;

        public MyParameterCommand<Window> DateiOeffnenCommand { get; set; }

        internal DateiAuswahlViewModel(string komponenteGuid, string titelText, DateiDto datei)
        {
            _komponenteGuid = komponenteGuid;
            _titelText = titelText;

            Guid = datei.Guid;
            Beschreibung = datei.Beschreibung;
            Dateiendung = datei.Dateiendung;

            DateiOeffnenCommand = new MyParameterCommand<Window>(OnOeffnen);
        }

        internal void OnOeffnen(Window window)
        {
            var dateiName = Path.Combine("Daten", _komponenteGuid, Guid + "." + Dateiendung);

            if (!string.IsNullOrWhiteSpace(dateiName))
            {
                try
                {
                    if (File.Exists(dateiName))
                    {
                        Process.Start(dateiName);
                    }
                    else
                    {
                        dateiName = Path.Combine("Daten", "Temp", Guid + "." + Dateiendung);
                        if (File.Exists(dateiName))
                        {
                            Process.Start(dateiName);
                        }
                    }
                }
                catch (Exception e)
                {
                    var message = "Das Dokument konnte nicht geöffnet werden"
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + e.Message;
                    HilfsFunktionen.ShowMessageBox(window, _titelText, message, true);
                }
            }
        }
    }
}

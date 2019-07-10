using System;
using System.Diagnostics;
using System.Windows;
using TeileListe.Classes;
using TeileListe.Common.Classes;

namespace TeileListe.Teileliste.ViewModel
{
    public class AlternativeViewModel
    {
        public int Einsparung { get; set; }
        public string Guid { get; set; }
        public int Preis { get; set; }
        public string Hersteller { get; set; }
        public string Beschreibung { get; set; }
        public string Groesse { get; set; }
        public string Jahr { get; set; }
        public string ParentGuid { get; set; }
        public string Shop { get; set; }
        public string Link { get; set; }

        public string AnzeigeName
        {
            get { return HilfsFunktionen.GetAnzeigeName(Hersteller, Beschreibung, Groesse, Jahr); }
        }

        public MyCommand TauschenCommand { get; set; }
        public MyParameterCommand<Window> ShopCommand { get; set; }

        public Action<string, string> TauschenAction { get; set; }

        public AlternativeViewModel()
        {
            Preis = 0;
            Einsparung = 0;
            TauschenCommand = new MyCommand(OnTauschen);
            ShopCommand = new MyParameterCommand<Window>(OnShopClick);
        }

        public void OnTauschen()
        {
            TauschenAction(ParentGuid, Guid);
        }

        internal void OnShopClick(Window window)
        {
            if (!string.IsNullOrWhiteSpace(Link))
            {
                try
                {
                    Process.Start(Link);
                }
                catch (Exception e)
                {
                    var message = "Der Link konnte nicht geöffnet werden." 
                                    + Environment.NewLine 
                                    + Environment.NewLine 
                                    + e.Message;
                    HilfsFunktionen.ShowMessageBox(window, 
                                                    "Teileliste", 
                                                    message,
                                                    true);
                }
            }
        }
    }
}

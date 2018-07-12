using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TeileListe.Common.Dto;

namespace TeileListe.Table
{
    public class IniReader
    {
        #region Dll-Imports

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateProfileString(string lpAppName,
                                                            string lpKeyName, 
                                                            string lpDefault, 
                                                            StringBuilder lpReturnedString,
                                                            int nSize, 
                                                            string lpFileName);

        #endregion

        #region Properties

        private const string MainFile = @"Daten\Komponenten.ini";
        private const string RestekisteFile = @"Daten\Restekiste.ini";
        private const string WunschlisteFile = @"Daten\Wunschliste.ini";
        private const string DatenbankFile = @"Daten\Datenbanken.ini";
        private const string DateilisteFile = @"Daten\{0}\Dateiliste.ini";
        private const string KategorieFile = @"Daten\Kategorien.ini";

        #endregion

        #region Publicfunktionen

        public void GetFahrraeder(ref List<string> liste)
        {
            var buffer = new StringBuilder(254);
            var counter = 1;

            do
            {
                if (GetPrivateProfileString("Fahrraeder",
                    string.Format("{0}", counter++),
                    string.Empty,
                    buffer,
                    254,
                    MainFile) > 0)
                {
                    liste.Add(buffer.ToString().Trim());
                }
            } while (!string.IsNullOrWhiteSpace(buffer.ToString().Trim()));
        }

        public void GetKomponenteIds(string nameFahrrad, ref List<KomponenteDto> collection)
        {
            var buffer = new StringBuilder(254);
            var counter = 1;

            do
            {
                if (GetPrivateProfileString(nameFahrrad.PadRight(32) + "Komponenten", 
                                            string.Format("{0}", counter++), 
                                            string.Empty, 
                                            buffer, 
                                            254, 
                                            MainFile) > 0)
                {
                    collection.Add(new KomponenteDto { Guid = buffer.ToString().Trim() });
                }
            } while (!string.IsNullOrWhiteSpace(buffer.ToString().Trim()));

            if (collection.Count > 0)
            {
                CompleteList(nameFahrrad, ref collection);
            }
        }

        public void GetEinzelteileIds(ref List<RestteilDto> collection)
        {
            var buffer = new StringBuilder(254);
            var counter = 1;

            do
            {
                if (GetPrivateProfileString("Einzelteile", 
                                            string.Format("{0}", counter++), 
                                            string.Empty, 
                                            buffer, 
                                            254, 
                                            RestekisteFile) > 0)
                {
                    collection.Add(new RestteilDto { Guid = buffer.ToString().Trim()});
                }
            } while (!string.IsNullOrWhiteSpace(buffer.ToString().Trim()));

            if (collection.Count > 0)
            {
                CompleteEinzelteileListe(ref collection);
            }
        }

        public void GetWunschteileIds(ref List<WunschteilDto> collection)
        {
            var buffer = new StringBuilder(254);
            var counter = 1;

            do
            {
                if (GetPrivateProfileString("Wunschteile",
                                            string.Format("{0}", counter++),
                                            string.Empty,
                                            buffer,
                                            254,
                                            WunschlisteFile) > 0)
                {
                    collection.Add(new WunschteilDto { Guid = buffer.ToString().Trim() });
                }
            } while (!string.IsNullOrWhiteSpace(buffer.ToString().Trim()));

            if (collection.Count > 0)
            {
                CompleteWunschteileListe(ref collection);
            }
        }

        public void ReadDatenbank(ref DatenbankDto datenbank)
        {
            var buffer = new StringBuilder(254);

            if (GetPrivateProfileString("DatenbankApi",
                datenbank.Datenbank,
                string.Empty,
                buffer,
                254,
                DatenbankFile) > 0)
            {
                datenbank.ApiToken = buffer.ToString().Trim();
                
            }
        }

        public void ReadDefaultDatenbank(ref List<DatenbankDto> datenbanken)
        {
            var buffer = new StringBuilder(254);

            if (GetPrivateProfileString("DatenbankApi",
                    "Default",
                    "mtb-news.de",
                    buffer,
                    254,
                    DatenbankFile) > 0)
            {
                var defaultDb = buffer.ToString().Trim();
                int index = datenbanken.FindIndex(item => item.Datenbank == defaultDb);
                if (index >= 0)
                {
                    datenbanken[index].IsDefault = true;
                }
                else
                {
                    datenbanken.Find(item => item.Datenbank == "mtb-news.de").IsDefault = true;
                }
            }
            else
            {
                datenbanken.Find(item => item.Datenbank == "mtb-news.de").IsDefault = true;
            }
        }

        public void GetDateiInfos(string komponenteGuid, ref List<DateiDto> dateiListe)
        {
            var buffer = new StringBuilder(254);
            var counter = 1;

            do
            {
                if (GetPrivateProfileString("Dateien",
                    string.Format("{0}", counter++),
                    string.Empty,
                    buffer,
                    254,
                    string.Format(DateilisteFile, komponenteGuid)) > 0)
                {
                    dateiListe.Add(new DateiDto() { Guid = buffer.ToString().Trim() });
                }
            } while (!string.IsNullOrWhiteSpace(buffer.ToString().Trim()));

            if (dateiListe.Count > 0)
            {
                CompleteDateiListe(komponenteGuid, ref dateiListe);
            }
        }

        public void GetDateiKategorien(ref List<string> liste)
        {
            var buffer = new StringBuilder(254);
            var counter = 1;

            liste.Add("Gewichtsmessung");

            do
            {
                if (GetPrivateProfileString("Kategorien",
                    string.Format("{0}", counter++),
                    string.Empty,
                    buffer,
                    254,
                    KategorieFile) > 0)
                {
                    liste.Add(buffer.ToString().Trim());
                }
            } while (!string.IsNullOrWhiteSpace(buffer.ToString().Trim()));
        }

        #endregion

        #region Privatefuntionen

        void CompleteList(string nameFahrrad, ref List<KomponenteDto> collection)
        {
            foreach (var item in collection)
            {
                item.Komponente = GetProperty("Komponente", 
                                                nameFahrrad.PadRight(32) + item.Guid, 
                                                MainFile);
                item.Hersteller = GetProperty("Hersteller",
                                                nameFahrrad.PadRight(32) + item.Guid,
                                                MainFile);
                item.Beschreibung = GetProperty("Beschreibung", 
                                                nameFahrrad.PadRight(32) + item.Guid, 
                                                MainFile);
                item.Groesse = GetProperty("Groesse",
                                            nameFahrrad.PadRight(32) + item.Guid,
                                            MainFile);
                item.Jahr = GetProperty("Jahr",
                                        nameFahrrad.PadRight(32) + item.Guid,
                                        MainFile);
                item.Shop = GetProperty("Shop", nameFahrrad.PadRight(32) + item.Guid, MainFile);
                item.Link = GetProperty("Link", nameFahrrad.PadRight(32) + item.Guid, MainFile);
                item.DatenbankId = GetProperty("DatenbankId", 
                                                nameFahrrad.PadRight(32) + item.Guid, 
                                                MainFile);
                item.DatenbankLink = GetProperty("DatenbankLink", 
                                                    nameFahrrad.PadRight(32) + item.Guid, 
                                                    MainFile);
                int.TryParse(GetProperty("Preis", nameFahrrad.PadRight(32) + item.Guid, MainFile),
                                out int buffer);
                item.Preis = buffer;
                int.TryParse(GetProperty("Gewicht", nameFahrrad.PadRight(32) + item.Guid, MainFile), 
                                out buffer);
                item.Gewicht = buffer;
                item.Gekauft = GetProperty("Gekauft", 
                                            nameFahrrad.PadRight(32) + item.Guid, 
                                            MainFile) == "1";
                item.Gewogen = GetProperty("Gewogen", 
                                            nameFahrrad.PadRight(32) + item.Guid, 
                                            MainFile) == "1";
            }
        }

        void CompleteEinzelteileListe(ref List<RestteilDto> collection)
        {
            foreach (var item in collection)
            {
                item.Komponente = GetProperty("Einzelteil", item.Guid, RestekisteFile);
                item.Hersteller = GetProperty("Hersteller", item.Guid, RestekisteFile);
                item.Beschreibung = GetProperty("Beschreibung", item.Guid, RestekisteFile);
                item.Groesse = GetProperty("Groesse", item.Guid, RestekisteFile);
                item.Jahr = GetProperty("Jahr", item.Guid, RestekisteFile);
                item.DatenbankId = GetProperty("DatenbankId", item.Guid, RestekisteFile);
                item.DatenbankLink = GetProperty("DatenbankLink", item.Guid, RestekisteFile);
                int.TryParse(GetProperty("Preis", item.Guid, RestekisteFile), out int buffer);
                item.Preis = buffer;
                int.TryParse(GetProperty("Gewicht", item.Guid, RestekisteFile), out buffer);
                item.Gewicht = buffer;
            }
        }

        void CompleteWunschteileListe(ref List<WunschteilDto> collection)
        {
            foreach (var item in collection)
            {
                item.Komponente = GetProperty("Komponente", item.Guid, WunschlisteFile);
                item.Hersteller = GetProperty("Hersteller", item.Guid, WunschlisteFile);
                item.Beschreibung = GetProperty("Beschreibung", item.Guid, WunschlisteFile);
                item.Groesse = GetProperty("Groesse", item.Guid, WunschlisteFile);
                item.Jahr = GetProperty("Jahr", item.Guid, WunschlisteFile);
                item.Shop = GetProperty("Shop", item.Guid, WunschlisteFile); 
                item.Link = GetProperty("Link", item.Guid, WunschlisteFile);
                item.DatenbankId = GetProperty("DatenbankId", item.Guid, WunschlisteFile);
                item.DatenbankLink = GetProperty("DatenbankLink", item.Guid, WunschlisteFile);
                int.TryParse(GetProperty("Preis", item.Guid, WunschlisteFile), out int buffer);
                item.Preis = buffer;
                int.TryParse(GetProperty("Gewicht", item.Guid, WunschlisteFile), out buffer);
                item.Gewicht = buffer;
            }
        }

        void CompleteDateiListe(string komponenteGuid, ref List<DateiDto> collection)
        {
            foreach (var item in collection)
            {
                item.Kategorie = GetProperty("Kategorie", item.Guid, string.Format(DateilisteFile, komponenteGuid));
                item.Beschreibung = GetProperty("Beschreibung", item.Guid, string.Format(DateilisteFile, komponenteGuid));
                item.Dateiendung = GetProperty("Dateiendung", item.Guid, string.Format(DateilisteFile, komponenteGuid));
            }
        }

        private string GetProperty(string property, string komponente, string file)
        {
            var buffer = new StringBuilder(254);

            if (GetPrivateProfileString(komponente,
                                            property,
                                            string.Empty,
                                            buffer,
                                            254,
                                            file) > 0)
            {
                return buffer.ToString().Trim();
            }
            return string.Empty;
        }

        #endregion

        internal List<string> GetDateiListe()
        {
            return new List<string> { MainFile,
                                        RestekisteFile,
                                        WunschlisteFile,
                                        DatenbankFile,
                                        KategorieFile  };
        }
    }
}

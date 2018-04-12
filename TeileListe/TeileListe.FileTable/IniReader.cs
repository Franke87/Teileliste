using System.Collections.Generic;
using System.IO;
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

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)] 
        private static extern bool WritePrivateProfileString(string section, 
                                                                string key,
                                                                string value, 
                                                                string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileSection(string section,
                                                                string val, 
                                                                string filePath);


        #endregion

        #region Properties

        private const string MainFile = @"Daten\Komponenten.ini";
        private const string RestekisteFile = @"Daten\Restekiste.ini";
        private const string WunschlisteFile = @"Daten\Wunschliste.ini";
        private const string DatenbankFile = @"Daten\Datenbanken.ini";

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

        public void SaveFahrraeder(List<string> liste)
        {
            WritePrivateProfileSection("Fahrraeder", null, MainFile);
            var count = 1;

            foreach (var item in liste)
            {
                WritePrivateProfileString("Fahrraeder", string.Format("{0}", count++), item, MainFile);
            }
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

        public void DeleteKomponenten(string nameFahrrad, List<string> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                WritePrivateProfileSection(nameFahrrad.PadRight(32) + item, null, MainFile);
            }
        }

        public void SaveKomponenten(string nameFahrrad, List<KomponenteDto> collection)
        {
            WritePrivateProfileSection(nameFahrrad.PadRight(32) + "Komponenten", null, MainFile);
            var count = 1;

            foreach (var item in collection)
            {
                WritePrivateProfileString(nameFahrrad.PadRight(32) + "Komponenten", 
                                            string.Format("{0}", count++), 
                                            item.Guid, 
                                            MainFile);
                WritePrivateProfileSection(nameFahrrad.PadRight(32) + item.Guid, null, MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid, 
                                            "Komponente", 
                                            item.Komponente, 
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid,
                                            "Hersteller",
                                            item.Hersteller,
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid, 
                                            "Beschreibung", 
                                            item.Beschreibung, 
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid,
                                            "Groesse",
                                            item.Groesse,
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid,
                                            "Jahr",
                                            item.Jahr,
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid, 
                                            "Shop", 
                                            item.Shop, 
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid, 
                                            "Link", 
                                            item.Link, 
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid,
                                            "DatenbankId",
                                            item.DatenbankId,
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid,
                                            "DatenbankLink",
                                            item.DatenbankLink,
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid, 
                                            "Gewicht", 
                                            string.Format("{0}", item.Gewicht), 
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid, 
                                            "Preis", 
                                            string.Format("{0}", item.Preis), 
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid, 
                                            "Gekauft", 
                                            item.Gekauft ? "1" : "0", 
                                            MainFile);
                WritePrivateProfileString(nameFahrrad.PadRight(32) + item.Guid, 
                                            "Gewogen", 
                                            item.Gewogen ? "1" : "0", 
                                            MainFile);
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

        public void DeleteEinzelteile(List<string> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                WritePrivateProfileSection(item, null, RestekisteFile);
            }
        }

        public void SaveEinzelteile(List<RestteilDto> collection)
        {
            WritePrivateProfileSection("Einzelteile", null, RestekisteFile);
            var count = 1;

            foreach (var item in collection)
            {
                WritePrivateProfileString("Einzelteile", 
                                            string.Format("{0}", count++), 
                                            item.Guid, 
                                            RestekisteFile);
                WritePrivateProfileSection(item.Guid, null, RestekisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Einzelteil",
                                            item.Komponente,
                                            RestekisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Hersteller",
                                            item.Hersteller,
                                            RestekisteFile);
                WritePrivateProfileString(item.Guid, 
                                            "Beschreibung", 
                                            item.Beschreibung, 
                                            RestekisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Groesse",
                                            item.Groesse,
                                            RestekisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Jahr",
                                            item.Jahr,
                                            RestekisteFile);
                WritePrivateProfileString(item.Guid,
                                            "DatenbankId",
                                            item.DatenbankId,
                                            RestekisteFile);
                WritePrivateProfileString(item.Guid,
                                            "DatenbankLink",
                                            item.DatenbankLink,
                                            RestekisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Preis",
                                            string.Format("{0}", item.Preis),
                                            RestekisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Gewicht",
                                            string.Format("{0}", item.Gewicht),
                                            RestekisteFile);
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

        public void DeleteWunschteile(List<string> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                WritePrivateProfileSection(item, null, WunschlisteFile);
            }
        }

        public void SaveWunschteile(List<WunschteilDto> collection)
        {
            WritePrivateProfileSection("Wunschteile", null, WunschlisteFile);
            var count = 1;

            foreach (var item in collection)
            {
                WritePrivateProfileString("Wunschteile",
                                            string.Format("{0}", count++),
                                            item.Guid,
                                            WunschlisteFile);
                WritePrivateProfileSection(item.Guid, null, WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Komponente",
                                            item.Komponente,
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Hersteller",
                                            item.Hersteller,
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Beschreibung",
                                            item.Beschreibung,
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Groesse",
                                            item.Groesse,
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Jahr",
                                            item.Jahr,
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Shop",
                                            item.Shop,
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Link",
                                            item.Link,
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "DatenbankId",
                                            item.DatenbankId,
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "DatenbankLink",
                                            item.DatenbankLink,
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Preis",
                                            string.Format("{0}", item.Preis),
                                            WunschlisteFile);
                WritePrivateProfileString(item.Guid,
                                            "Gewicht",
                                            string.Format("{0}", item.Gewicht),
                                            WunschlisteFile);
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

        public void SaveDatenbankDaten(List<DatenbankDto> datenbanken)
        {
            WritePrivateProfileSection("DatenbankApi", null, DatenbankFile);

            foreach (var datenbank in datenbanken)
            {
                WritePrivateProfileString("DatenbankApi",
                                            datenbank.Datenbank,
                                            datenbank.ApiToken,
                                            DatenbankFile);
            }
        }

        public void SaveDefaultDatenbank(string defaultDatenbank)
        {
            WritePrivateProfileString("DatenbankApi",
                                        "Default",
                                        defaultDatenbank,
                                        DatenbankFile);
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
                int buffer;
                int.TryParse(GetProperty("Preis", nameFahrrad.PadRight(32) + item.Guid, MainFile), 
                                out buffer);
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
                int buffer;
                int.TryParse(GetProperty("Preis", item.Guid, RestekisteFile), out buffer);
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
                int buffer;
                int.TryParse(GetProperty("Preis", item.Guid, WunschlisteFile), out buffer);
                item.Preis = buffer;
                int.TryParse(GetProperty("Gewicht", item.Guid, WunschlisteFile), out buffer);
                item.Gewicht = buffer;
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

        internal void Initialize()
        {
            if (!Directory.Exists("Daten"))
            {
                Directory.CreateDirectory("Daten");
            }
        }
    }
}

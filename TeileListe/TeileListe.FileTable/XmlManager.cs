using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TeileListe.Common.Dto;

namespace TeileListe.Table
{
    internal class XmlManager
    {
        #region Properties

        private string _version;

        private const string FahrraederXmlFile = @"Daten\Fahrraeder.xml";
        private const string KomponentenXmlFile = @"Daten\{0}.xml";
        private const string RestekisteXmlFile = @"Daten\Restekiste.xml";
        private const string WunschlisteXmlFile = @"Daten\Wunschliste.xml";
        private const string DatenbankXmlFile = @"Daten\Datenbanken.xml";
        private const string DateilisteXmlFile = @"Daten\{0}\Dateiliste.xml";
        private const string KategorieXmlFile = @"Daten\Kategorien.xml";

        #endregion

        #region Functions

        internal void Initialize(string version)
        {
            _version = version;

            if (!Directory.Exists("Daten"))
            {
                Directory.CreateDirectory("Daten");
            }

            if (Directory.Exists("Daten\\Temp"))
            {
                try
                {
                    Directory.Delete("Daten\\Temp", true);
                }
                catch (Exception)
                {
                }
            }

            if (!Directory.Exists("Daten\\Temp"))
            {
                Directory.CreateDirectory("Daten\\Temp");
            }

            if (!File.Exists(KategorieXmlFile))
            {
                SaveDateiKategorien(new List<string> { "Rechnung", "Artikelfoto" });
            }

            if(!File.Exists(FahrraederXmlFile))
            {
                SaveFahrraeder(new List<FahrradDto>());
            }
        }

        public void DeleteTeile(List<LoeschenDto> deletedTeile)
        {
            foreach(var teil in deletedTeile)
            {
                if(teil.DokumenteLoeschen)
                {
                    try
                    {
                        if(Directory.Exists(Path.Combine("Daten", teil.Guid)))
                        {
                            Directory.Delete(Path.Combine("Daten", teil.Guid), true);
                        }
                    }
                    catch(Exception)
                    {
                    }
                }
            }
        }

        private void SaveTeile<T>(string fileName, List<T> teile)
        {
            using (var streamWriter = new StreamWriter(fileName, false, Encoding.Default))
            {
                var serialzer = new XmlSerializer(typeof(Dto.XmlBaseClass<T>));

                serialzer.Serialize(streamWriter, new Dto.XmlBaseClass<T>
                {
                    Daten = teile,
                    Version = _version
                });

                streamWriter.Close();
            }
        }

        private void GetTeile<T>(string fileName, ref List<T> teile)
        {
            if(File.Exists(fileName))
            {
                using (var streamReader = new StreamReader(fileName, Encoding.Default))
                {
                    var serializer = new XmlSerializer(typeof(Dto.XmlBaseClass<T>));

                    teile.AddRange(((Dto.XmlBaseClass<T>)serializer.Deserialize(streamReader)).Daten);

                    streamReader.Close();
                }
            }
        }

        #endregion

        #region Fahrraeder

        public void GetFahrraeder(ref List<FahrradDto> fahrraeder)
        {
            GetTeile(FahrraederXmlFile, ref fahrraeder);
        }

        public void SaveFahrraeder(List<FahrradDto> fahrraeder)
        {
            SaveTeile(FahrraederXmlFile, fahrraeder);
        }

        public void DeleteFahrrad(FahrradDto fahrrad)
        {
            if(File.Exists(string.Format(KomponentenXmlFile, fahrrad.Guid)))
            {
                File.Delete(string.Format(KomponentenXmlFile, fahrrad.Guid));
            }
        }

        #endregion

        #region Komponenten

        public void GetKomponenten(string fahrradGuid, ref List<KomponenteDto> komponenten)
        {
            GetTeile(string.Format(KomponentenXmlFile, fahrradGuid), ref komponenten);
        }

        public void SaveKomponenten(string fahrradGuid, List<KomponenteDto> komponenten)
        {
            SaveTeile(string.Format(KomponentenXmlFile, fahrradGuid), komponenten);
        }

        #endregion

        #region Wunschliste

        public void GetWunschliste(ref List<WunschteilDto> wunschteile)
        {
            GetTeile(WunschlisteXmlFile, ref wunschteile);
        }

        public void SaveWunschliste(List<WunschteilDto> wunschteile)
        {
            SaveTeile(WunschlisteXmlFile, wunschteile);
        }

        #endregion

        #region Restekiste

        public void GetEinzelteile(ref List<RestteilDto> einzelteile)
        {
            GetTeile(RestekisteXmlFile, ref einzelteile);
        }

        public void SaveEinzelteile(List<RestteilDto> einzelteile)
        {
            SaveTeile(RestekisteXmlFile, einzelteile);
        }

        #endregion

        #region Datenbanken

        public void ReadDatenbanken(ref List<DatenbankDto> datenbanken)
        {
            var datenbankDaten = new List<DatenbankDto>();

            GetTeile(DatenbankXmlFile, ref datenbankDaten);

            foreach(var datenbank in datenbanken)
            {
                foreach(var db in datenbankDaten)
                {
                    if(db.Datenbank == datenbank.Datenbank)
                    {
                        datenbank.ApiToken = db.ApiToken;
                        datenbank.IsDefault = db.IsDefault;
                        break;
                    }
                }
            }
        }

        public void SaveDatenbanken(List<DatenbankDto> datenbanken)
        {
            SaveTeile(DatenbankXmlFile, datenbanken);
        }

        #endregion

        #region Dateiinfos

        public void GetDateiInfos(string komponenteGuid, ref List<DateiDto> dateiliste)
        {
            GetTeile(string.Format(DateilisteXmlFile, komponenteGuid), ref dateiliste);
        }

        public void SaveDateiInfos(string komponenteGuid, List<DateiDto> dateiliste)
        {
            if(!Directory.Exists(Path.Combine("Daten", komponenteGuid)))
            {
                Directory.CreateDirectory(Path.Combine("Daten", komponenteGuid));
            }

            if(dateiliste.Count == 0)
            {
                try
                {
                    if(Directory.Exists(string.Format(DateilisteXmlFile, komponenteGuid)))
                    {
                        Directory.Delete(string.Format(DateilisteXmlFile, komponenteGuid), true);
                    }
                }
                catch(Exception)
                {
                }
            }
            else
            {
                foreach(var datei in dateiliste)
                {
                    if (!File.Exists(Path.Combine("Daten", komponenteGuid, datei.Guid + "." + datei.Dateiendung)))
                    {
                        File.Copy(Path.Combine("Daten", "Temp", datei.Guid + "." + datei.Dateiendung),
                                    Path.Combine("Daten", komponenteGuid, datei.Guid + "." + datei.Dateiendung));
                    }

                    SaveTeile(string.Format(DateilisteXmlFile, komponenteGuid), dateiliste);

                    try
                    {
                        foreach (var item in dateiliste)
                        {
                            if (File.Exists(Path.Combine("Daten", "Temp", item.Guid + "." + item.Dateiendung)))
                            {
                                File.Delete(Path.Combine("Daten", "Temp", item.Guid + "." + item.Dateiendung));
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void DeleteDateiInfos(string komponenteGuid, List<string> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                var dateiName = Path.Combine("Daten", komponenteGuid, item);
                if (File.Exists(dateiName))
                {
                    using (var file = File.Open(dateiName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        file.Close();
                    }
                }
                else
                {
                    dateiName = Path.Combine("Daten", "Temp", item);
                    using (var file = File.Open(dateiName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        file.Close();
                    }
                }
            }

            foreach (var item in deletedItems)
            {
                var dateiName = Path.Combine("Daten", komponenteGuid, item);
                if (File.Exists(dateiName))
                {
                    File.Delete(dateiName);
                }
                else
                {
                    dateiName = Path.Combine("Daten", "Temp", item);
                    if (File.Exists(dateiName))
                    {
                        File.Delete(dateiName);
                    }
                }
            }
        }

        #endregion

        #region Kategorien

        public void GetDateiKategorien(ref List<string> kategorien)
        {
            kategorien.Add("Gewichtsmessung");

            GetTeile(KategorieXmlFile, ref kategorien);
        }

        public void SaveDateiKategorien(List<string> kategorien)
        {
            SaveTeile(KategorieXmlFile, kategorien.Where(x => x != "Gewichtsmessung").ToList());
        }

        #endregion
    }
}

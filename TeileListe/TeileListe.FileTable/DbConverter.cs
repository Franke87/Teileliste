using System;
using System.Collections.Generic;
using System.IO;
using TeileListe.Common.Dto;

namespace TeileListe.Table
{
    internal class DbConverter : IDisposable
    {
        private IniReader _iniReader;
        private XmlManager _xmlManager;
        private List<string> _iniFiles;
        private List<string> _xmlFiles;

        public void Dispose()
        {
            if(_iniReader != null)
            {
                _iniReader = null;
            }

            if(_xmlManager != null)
            {
                _xmlManager = null;
            }
        }

        internal bool KonvertiereungErforderlich()
        {
            return File.Exists("Daten\\Kategorien.ini");
        }

        internal bool Konvertiere(string version)
        {
            bool bReturn = false;

            int step = 0;

            try
            {
                _iniReader = new IniReader();

                if(PruefeDateirechte())
                {
                    step++;

                    if(ErstelleSicherung())
                    {
                        step++;

                        _xmlManager = new XmlManager();
                        _xmlManager.Initialize(version);

                        if(KonvertiereDateien())
                        {
                            step++;

                            if(LoescheAlteDateien())
                            {
                                bReturn = true;
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
                if(step > 0)
                {
                    if(step > 1)
                    {
                        if(step > 2)
                        {
                            StelleSicherungHer();
                        }

                        LoescheKonvertierteDateien();
                    }

                    LoescheSicherung();
                }
            }

            return bReturn;
        }

        private bool PruefeDateirechte()
        {
            _iniFiles = new List<string>();
            _iniFiles.AddRange(_iniReader.GetDateiListe());

            foreach(var folder in Directory.EnumerateDirectories("Daten"))
            {
                foreach(var file in Directory.EnumerateFiles(folder))
                {
                    if(Path.GetFileName(file) == "Dateiliste.ini")
                    {
                        _iniFiles.Add(file);
                    }
                }
            }

            foreach(var datei in _iniFiles)
            {
                if(File.Exists(datei))
                {
                    using (var file = File.Open(datei, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        file.Close();
                    }
                }
            }

            return true;
        }

        private bool ErstelleSicherung()
        {
            if(Directory.Exists("Daten\\DbSicherung"))
            {
                Directory.Delete("Daten\\DbSicherung", true);
            }

            if(!Directory.Exists("Daten\\DbSicherung"))
            {
                Directory.CreateDirectory("Daten\\DbSicherung");
            }

            foreach(var datei in _iniFiles)
            {
                if(File.Exists(datei))
                {
                    var newFile = Path.Combine("Daten\\DbSicherung", datei.Substring(6));
                    if(!Directory.Exists(Path.GetDirectoryName(newFile)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newFile));

                    }
                    File.Copy(datei, newFile);
                }
            }

            return true;
        }

        private void LoescheSicherung()
        {
            if(Directory.Exists("Daten\\DbSicherung"))
            {
                Directory.Delete("Daten\\DbSicherung", true);
            }
        }

        private bool LoescheAlteDateien()
        {
            foreach(var datei in _iniFiles)
            {
                if(File.Exists(datei))
                {
                    File.Delete(datei);
                }
            }

            return true;
        }

        private void StelleSicherungHer()
        {
            foreach(var datei in _iniFiles)
            {
                if(File.Exists(Path.Combine("Daten\\DbSicherung", datei.Substring(6)))
                    && !File.Exists(datei))
                {
                    File.Copy(Path.Combine("Daten\\DbSicherung", datei.Substring(6)), datei);
                }
            }
        }

        private void LoescheKonvertierteDateien()
        {
            foreach (var datei in _xmlFiles)
            {
                if (File.Exists(datei))
                {
                    File.Delete(datei);
                }
            }
        }

        private bool KonvertiereDateien()
        {
            _xmlFiles = new List<string>();

            // Wir fangen an mit den Dateiinfos
            var list = new List<DateiDto>();
            foreach(var datei in _iniFiles)
            {
                list.Clear();

                if(Path.GetFileName(datei) == "Dateiliste.ini")
                {
                    var guid = datei.Substring(6, 36);
                    _iniReader.GetDateiInfos(guid, ref list);
                    _xmlManager.SaveDateiInfos(guid, list);
                    _xmlFiles.Add(Path.Combine("Daten", guid, "Dateiliste.xml"));
                }
            }

            // Wunschliste
            var wunschliste = new List<WunschteilDto>();
            _iniReader.GetWunschteileIds(ref wunschliste);
            _xmlManager.SaveWunschliste(wunschliste);
            _xmlFiles.Add("Daten\\Wunschliste.xml");

            // Restekiste
            var restekiste = new List<RestteilDto>();
            _iniReader.GetEinzelteileIds(ref restekiste);
            _xmlManager.SaveEinzelteile(restekiste);
            _xmlFiles.Add("Daten\\Restekiste.xml");

            // Komponenten
            var fahrraeder = new List<string>();
            _iniReader.GetFahrraeder(ref fahrraeder);
            var xmlFahrraeder = new List<FahrradDto>();
            foreach(var rad in fahrraeder)
            {
                var fahrrad = new FahrradDto { Name = rad, Guid = Guid.NewGuid().ToString() };

                var komponenten = new List<KomponenteDto>();
                _iniReader.GetKomponenteIds(fahrrad.Name, ref komponenten);
                _xmlManager.SaveKomponenten(fahrrad.Guid, komponenten);
                _xmlFiles.Add(Path.Combine("Daten", fahrrad.Guid + ".xml"));

                xmlFahrraeder.Add(fahrrad);
            }
            _xmlManager.SaveFahrraeder(xmlFahrraeder);
            _xmlFiles.Add("Daten\\Fahrraeder.xml");

            // Datenbanken
            var datenbanken = new List<DatenbankDto>();
            var mtb = new DatenbankDto { Datenbank = "mtb-news.de" };
            _iniReader.ReadDatenbank(ref mtb);
            datenbanken.Add(mtb);
            var rennrad = new DatenbankDto { Datenbank = "rennrad-news.de" };
            _iniReader.ReadDatenbank(ref rennrad);
            datenbanken.Add(rennrad);
            _iniReader.ReadDefaultDatenbank(ref datenbanken);
            _xmlManager.SaveDatenbanken(datenbanken);
            _xmlFiles.Add("Daten\\Datenbanken.xml");

            // Kategorien am Schluss
            var kategorien = new List<string>();
            _iniReader.GetDateiKategorien(ref kategorien);
            _xmlManager.SaveDateiKategorien(kategorien);
            _xmlFiles.Add("Daten\\Kategorien.xml");

            return true;
        }
    }
}

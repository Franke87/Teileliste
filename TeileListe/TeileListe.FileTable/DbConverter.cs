using System;
using System.Collections.Generic;
using System.IO;
using TeileListe.Common.Dto;

namespace TeileListe.Table
{
    internal class DbConverter : IDisposable
    {
        private XmlManager _xmlManager;
        private List<string> _oldFiles;
        private List<string> _convertedFiles;

        public void Dispose()
        {
            if(_xmlManager != null)
            {
                _xmlManager = null;
            }
        }

        internal bool KonvertierungErforderlich(string version)
        {
            _xmlManager = new XmlManager();
            _xmlManager.Initialize(version, true);

            return _xmlManager.KonvertierungErforderlich();
        }

        internal bool Konvertiere(string version)
        {
            bool bReturn = false;

            int step = 0;

            try
            {
                _xmlManager = new XmlManager();
                _xmlManager.Initialize(version, true);

                if (PruefeDateirechte())
                {
                    step++;

                    if (ErstelleSicherung())
                    {
                        step++;

                        if (KonvertiereDateien())
                        {
                            bReturn = true;
                        }
                    }
                }
            }
            catch(Exception)
            {
                if(step > 1)
                {
                    LoescheKonvertierteDateien();
                    StelleSicherungHer();
                    LoescheSicherung();
                }
            }

            return bReturn;
        }

        private bool PruefeDateirechte()
        {
            _oldFiles = new List<string>();
            _oldFiles.AddRange(_xmlManager.GetDateiListe());

            foreach(var folder in Directory.EnumerateDirectories("Daten"))
            {
                foreach(var file in Directory.EnumerateFiles(folder))
                {
                    if(Path.GetFileName(file) == "Dateiliste.xml")
                    {
                        _oldFiles.Add(file);
                    }
                }
            }

            foreach(var datei in _oldFiles)
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
            if(Directory.Exists("Daten\\DbSicherungv1.04"))
            {
                Directory.Delete("Daten\\DbSicherungv1.04", true);
            }

            if(!Directory.Exists("Daten\\DbSicherungv1.04"))
            {
                Directory.CreateDirectory("Daten\\DbSicherungv1.04");
            }

            foreach(var datei in _oldFiles)
            {
                if(File.Exists(datei))
                {
                    var newFile = Path.Combine("Daten\\DbSicherungv1.04", datei.Substring(6));
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
            if(Directory.Exists("Daten\\DbSicherungv1.04"))
            {
                Directory.Delete("Daten\\DbSicherungv1.04", true);
            }
        }

        private void StelleSicherungHer()
        {
            foreach(var datei in _oldFiles)
            {
                if(File.Exists(Path.Combine("Daten\\DbSicherungv1.04", datei.Substring(6)))
                    && !File.Exists(datei))
                {
                    File.Copy(Path.Combine("Daten\\DbSicherungv1.04", datei.Substring(6)), datei);
                }
            }
        }

        private void LoescheKonvertierteDateien()
        {
            foreach (var datei in _convertedFiles)
            {
                if (File.Exists(datei))
                {
                    File.Delete(datei);
                }
            }
        }

        private bool KonvertiereDateien()
        {
            _convertedFiles = new List<string>();

            // Wir fangen an mit den Dateiinfos
            var list = new List<DateiDto>();
            foreach (var datei in _oldFiles)
            {
                list.Clear();

                if (Path.GetFileName(datei) == "Dateiliste.xml")
                {
                    var guid = datei.Substring(6, 36);
                    _xmlManager.GetDateiInfos(guid, ref list);
                    _convertedFiles.Add(Path.Combine("Daten", guid, "Dateiliste.xml"));
                    _xmlManager.SaveDateiInfos(guid, list);
                }
            }

            // Wunschliste
            var wunschliste = new List<WunschteilDto>();
            _xmlManager.GetWunschliste(ref wunschliste);
            _convertedFiles.Add("Daten\\Wunschliste.xml");
            _xmlManager.SaveWunschliste(wunschliste);

            // Restekiste
            var restekiste = new List<RestteilDto>();
            _xmlManager.GetEinzelteile(ref restekiste);
            _convertedFiles.Add("Daten\\Restekiste.xml");
            _xmlManager.SaveEinzelteile(restekiste);

            // Datenbanken
            var datenbanken = new List<DatenbankDto>
            {
                new DatenbankDto { Datenbank = "mtb-news.de"},
                new DatenbankDto { Datenbank = "rennrad-news.de"}
            };
            _xmlManager.ReadDatenbanken(ref datenbanken);
            _convertedFiles.Add("Daten\\Datenbanken.xml");
            _xmlManager.SaveDatenbanken(datenbanken);

            // Komponenten
            var fahrraeder = new List<FahrradDto>();
            _xmlManager.GetFahrraeder(ref fahrraeder);
            foreach (var rad in fahrraeder)
            {
                var komponenten = new List<KomponenteDto>();
                _xmlManager.GetKomponenten(rad.Guid, ref komponenten);
                _convertedFiles.Add(Path.Combine("Daten", rad.Guid + ".xml"));
                _xmlManager.SaveKomponenten(rad.Guid, komponenten);
            }
            _convertedFiles.Add("Daten\\Fahrraeder.xml");
            _xmlManager.SaveFahrraeder(fahrraeder);

            // Kategorien am Schluss
            var kategorien = new List<string>();
            _xmlManager.GetDateiKategorien(ref kategorien);
            _convertedFiles.Add("Daten\\Kategorien.xml");
            _xmlManager.SaveDateiKategorien(kategorien);

            return true;
        }
    }
}

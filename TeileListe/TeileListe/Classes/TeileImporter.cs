using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using TeileListe.Common.Dto;
using TeileListe.Exporter.Dto;

namespace TeileListe.Classes
{
    internal class TeileImporter
    {
        private string _komponenteCsv;
        private string _guid;
        private List<DateiVerzeichnisEintragDto> _dateiVerzeichnis;
        private bool _isZip;

        internal List<Tuple<string, List<DateiDto>>> DateiCache;

        private void BereiteDateiAuf(string dateiName)
        {
            _guid = Guid.NewGuid().ToString();

            DateiCache = new List<Tuple<string, List<DateiDto>>>();

            if (Path.GetExtension(dateiName) == ".zip")
            {
                using (FileStream fs = new FileStream(dateiName, FileMode.Open))
                {
                    using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        arch.ExtractToDirectory(Path.Combine("Daten", "Temp", _guid));
                    }
                }
                _isZip = true;
                _komponenteCsv = Path.Combine("Daten", "Temp", _guid, "Komponenten.csv");
                _dateiVerzeichnis = new List<DateiVerzeichnisEintragDto>();

                if(!File.Exists(_komponenteCsv) || !File.Exists(Path.Combine("Daten", "Temp", _guid, "Dateiverzeichnis.csv")))
                {
                    CleanUp();
                    throw new Exception("Keine Daten zum importieren vorhanden.");
                }

                using (var reader = new StreamReader(Path.Combine("Daten", "Temp", _guid, "Dateiverzeichnis.csv"), Encoding.Default))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line != null
                            && line.Count(x => x == ';') == 4
                            && line.Length > 32)
                        {
                            var values = line.Split(';');
                            var eintrag = new DateiVerzeichnisEintragDto();
                            if (!string.IsNullOrWhiteSpace(values[0]))
                            {
                                eintrag.ParentGuid = values[0];
                                eintrag.FileName = values[1];
                                eintrag.Dateiendung = values[2];
                                eintrag.Kategorie = values[3];
                                if(!string.IsNullOrWhiteSpace(values[4]))
                                {
                                    eintrag.Beschreibung = values[4];
                                }
                                _dateiVerzeichnis.Add(eintrag);
                            }
                        }
                    }
                    reader.Close();
                }
            }
            else
            {
                _isZip = false;
                _komponenteCsv = dateiName;
                _dateiVerzeichnis = new List<DateiVerzeichnisEintragDto>();
            }
        }

        private void CleanUp()
        {
            if(_isZip)
            {
                try
                {
                    Directory.Delete(Path.Combine("Daten", "Temp", _guid), true);
                }
                catch(Exception)
                {
                }
            }
        }

        internal List<KomponenteDto> ImportFahrrad(string dateiName)
        {
            BereiteDateiAuf(dateiName);

            var list = new List<KomponenteDto>();

            using(var reader = new StreamReader(_komponenteCsv, Encoding.Default))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null
                        && !line.Equals(CsvFormatter.CsvHeaderV1)
                        && !line.Equals(CsvFormatter.CsvHeaderV2)
                        && !line.Equals(CsvFormatter.CsvHeaderV3)
                        && (line.Count(x => x == ';') == 7 || line.Count(x => x == ';') == 12 || line.Count(x => x == ';') == 13)
                        && line.Length > 7
                        && !line.StartsWith("Summe gesamt")
                        && !line.StartsWith("Summe bez./gew."))
                    {
                        var values = line.Split(';');
                        if (!string.IsNullOrWhiteSpace(values[0]))
                        {
                            var guid = Guid.NewGuid().ToString();
                            var dto = new KomponenteDto
                            {
                                Guid = guid,
                                Komponente = values[0],
                                Beschreibung = values[1],
                                Shop = values[2],
                                Link = values[3],
                            };
                            int intValue;
                            if (int.TryParse(values[4], out intValue))
                            {
                                dto.Preis = intValue >= 0 ? intValue : 0;
                            }
                            dto.Gekauft = values[5] == "True";
                            if (int.TryParse(values[6], out intValue))
                            {
                                dto.Gewicht = intValue >= 0 ? intValue : 0;
                            }
                            dto.Gewogen = values[7] == "True";
                            if (values.Length >= 13)
                            {
                                dto.Hersteller = values[8];
                                dto.Groesse = values[9];
                                dto.Jahr = values[10];
                                dto.DatenbankId = values[11];
                                dto.DatenbankLink = values[12];

                                if(values.Length == 14)
                                {
                                    if (_isZip)
                                    {
                                        var dateiList = new List<DateiDto>();
                                        var fileGuid = values[13];
                                        foreach(var file in _dateiVerzeichnis.Where(item => item.ParentGuid == fileGuid).ToList())
                                        {
                                            var dateiDto = new DateiDto
                                            {
                                                Guid = Guid.NewGuid().ToString(),
                                                Dateiendung = file.Dateiendung,
                                                Kategorie = file.Kategorie,
                                                Beschreibung = file.Beschreibung ?? ""
                                            };

                                            try
                                            {
                                                File.Copy(Path.Combine("Daten", "Temp", _guid, file.FileName), Path.Combine("Daten", "Temp", dateiDto.Guid + "." + dateiDto.Dateiendung));

                                                dateiList.Add(dateiDto);
                                            }
                                            catch(Exception)
                                            {
                                            }
                                        }

                                        if(dateiList.Count > 0)
                                        {
                                            DateiCache.Add(new Tuple<string, List<DateiDto>>(guid, dateiList));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                dto.Hersteller = "";
                                dto.Groesse = "";
                                dto.Jahr = "";
                                dto.DatenbankId = "";
                                dto.DatenbankLink = "";
                            }
                            list.Add(dto);
                        }
                    }
                }
                reader.Close();
            }

            CleanUp();

            if(list.Count == 0)
            {
                throw new Exception("Keine Daten zum importieren vorhanden.");
            }

            return list;
        }

        internal List<RestteilDto> ImportEinzelteile(string dateiName)
        {
            return ImportFahrrad(dateiName).Select(item => new RestteilDto
            {
                Guid = item.Guid,
                Komponente = item.Komponente, 
                Beschreibung = item.Beschreibung, 
                Gewicht = item.Gewicht, 
                Preis = item.Preis, 
                Hersteller = item.Hersteller,
                Groesse = item.Groesse,
                Jahr = item.Jahr,
                DatenbankId = item.DatenbankId,
                DatenbankLink = item.DatenbankLink
            }).ToList();
        }

        internal List<WunschteilDto> ImportWunschteile(string dateiName)
        {
            return ImportFahrrad(dateiName).Select(item => new WunschteilDto
            {
                Guid = item.Guid,
                Komponente = item.Komponente,
                Beschreibung = item.Beschreibung,
                Shop = item.Shop, 
                Link = item.Link, 
                Gewicht = item.Gewicht,
                Preis = item.Preis,
                Hersteller = item.Hersteller,
                Groesse = item.Groesse,
                Jahr = item.Jahr,
                DatenbankId = item.DatenbankId,
                DatenbankLink = item.DatenbankLink
            }).ToList();
        }
    }
}

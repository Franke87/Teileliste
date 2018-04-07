using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TeileListe.Common.Dto;

namespace TeileListe.Classes
{
    internal class TeileImporter
    {
        public List<KomponenteDto> ImportFahrrad(string dateiName)
        {
            var list = new List<KomponenteDto>();
            using (var reader = new StreamReader(dateiName, Encoding.Default))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null
                        && !line.Equals("Komponente;Beschreibung;Shop;Link;Preis;Gekauft;Gewicht;Gewogen")
                        && !line.Equals("Komponente;Beschreibung;Shop;Link;Preis;Gekauft;Gewicht;Gewogen;Hersteller;Groesse;Jahr;DatenbankId;DatenbankLink")
                        && (line.Count(x => x == ';') == 7 || line.Count(x => x == ';') == 12)
                        && line.Length > 7
                        && !line.StartsWith("Summe gesamt")
                        && !line.StartsWith("Summe bez./gew."))
                    {
                        var values = line.Split(';');
                        if (!string.IsNullOrWhiteSpace(values[0]))
                        {
                            var dto = new KomponenteDto
                            {
                                Guid = Guid.NewGuid().ToString(),
                                Komponente = values[0],
                                Beschreibung = values[1],
                                Shop = values[2],
                                Link = values[3],
                            };
                            int intValue;
                            if (int.TryParse(values[4], out intValue))
                            {
                                dto.Preis = intValue;
                            }
                            dto.Gekauft = values[5] == "True";
                            if (int.TryParse(values[6], out intValue))
                            {
                                dto.Gewicht = intValue;
                            }
                            dto.Gewogen = values[7] == "True";
                            if (values.Length == 13)
                            {
                                dto.Hersteller = values[8];
                                dto.Groesse = values[9];
                                dto.Jahr = values[10];
                                dto.DatenbankId = values[11];
                                dto.DatenbankLink = values[12];
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

            return list;
        }

        public List<RestteilDto> ImportEinzelteile(string dateiName)
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

        public List<WunschteilDto> ImportWunschteile(string dateiName)
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;

namespace TeileListe.Exporter
{
    public class ExportManager : IExportManager
    {
        public void Dispose()
        {
        }

        public string GetKuerzel()
        {
            return "txt";
        }

        public void ExportKomponenten(List<KomponenteDto> listeKomponenten, 
                                        int gesamtPreis, 
                                        int gesamtGewicht, 
                                        int bereitsGezahlt, 
                                        int schonGewogen)
        {
            try
            {
                var message = new StringBuilder();

                message.AppendLine(
                    "Komponente           Beschreibung                   Datenbank       Preis gek. Gewicht gew.");
                message.AppendLine("".PadLeft(91, '-'));

                foreach (var teil in listeKomponenten)
                {
                    var datenbank = string.Empty;
                    if (!string.IsNullOrWhiteSpace(teil.DatenbankId))
                    {
                        datenbank = teil.DatenbankId.Substring(0, 3);
                        if (datenbank != "mtb")
                        {
                            datenbank = "rr";
                        }

                        datenbank = teil.DatenbankId.Substring(teil.DatenbankId.IndexOf(':') + 1) + datenbank;
                    }

                    var line = string.Format("{0}|{1}|{2}|{3} |{4}|{5} |{6}|",
                        FormatStringRight(teil.Komponente, 20),
                        FormatStringRight(GetAnzeigeName(teil.Hersteller,
                            teil.Beschreibung,
                            teil.Groesse,
                            teil.Jahr),
                            30),
                        FormatStringRight(datenbank, 10),
                        ConvertPreis(teil.Preis).PadLeft(11),
                        teil.Gekauft ? "X" : " ",
                        ConvertGewicht(teil.Gewicht).PadLeft(9),
                        teil.Gewogen ? "X" : " ");
                    message.AppendLine(line);
                }

                message.AppendLine("".PadLeft(91, '-'));
                message.AppendFormat("Summe gesamt                                                  |{0}{1}",
                    ConvertPreis(gesamtPreis).PadLeft(11),
                    ConvertGewicht(gesamtGewicht).PadLeft(13));
                message.AppendLine("");
                message.AppendFormat("Summe bez./gew.                                               |{0}{1}",
                    ConvertPreis(bereitsGezahlt).PadLeft(11),
                    ConvertGewicht(schonGewogen).PadLeft(13));

                message.AppendLine();

                WriteAndOpenFile(message.ToString(), "Teileliste");
            }
            catch (IOException ex)
            {
                throw new Exception("Die Daten konnten nicht exportiert werden", ex);
            }
        }

        private void WriteAndOpenFile(string text, string baseFileName)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var file = Path.Combine(path, baseFileName + ".txt");
            var i = 1;

            while (File.Exists(file))
            {
                file = Path.Combine(path, string.Format(baseFileName + " ({0}).txt", i++));
            }

            using (var sw = new StreamWriter(file, false, Encoding.Default))
            {
                sw.Write(text);
            }

            Process.Start(new ProcessStartInfo("explorer.exe")
            {
                Arguments = "/select, \"" + file + "\""
            });
        }
            

        public void ExportRestekiste(List<RestteilDto> listeEinzelteile)
        {
            var message = new StringBuilder();

            message.AppendLine("Komponente           Beschreibung                   Datenbank        Preis    Gewicht");
            message.AppendLine("".PadLeft(85, '-'));

            foreach (var teil in listeEinzelteile)
            {
                var datenbank = string.Empty;
                if (!string.IsNullOrWhiteSpace(teil.DatenbankId))
                {
                    datenbank = teil.DatenbankId.Substring(0, 3);
                    if (datenbank != "mtb")
                    {
                        datenbank = "rr";
                    }

                    datenbank = teil.DatenbankId.Substring(teil.DatenbankId.IndexOf(':') + 1) + datenbank;
                }

                var line = string.Format("{0}|{1}|{2}|{3} |{4}",
                    FormatStringRight(teil.Komponente, 20),
                    FormatStringRight(GetAnzeigeName(teil.Hersteller,
                                                        teil.Beschreibung,
                                                        teil.Groesse,
                                                        teil.Jahr), 30),
                    FormatStringRight(datenbank, 10),
                    ConvertPreis(teil.Preis).PadLeft(11),
                    ConvertGewicht(teil.Gewicht).PadLeft(9));
                message.AppendLine(line);
            }

            message.AppendLine();

            WriteAndOpenFile(message.ToString(), "Restekiste");
        }

        public void ExportWunschliste(List<WunschteilDto> listeWunschteile)
        {
            var message = new StringBuilder();

            message.AppendLine("Komponente           Beschreibung                   Datenbank        Preis    Gewicht");
            message.AppendLine("".PadLeft(85, '-'));

            foreach (var teil in listeWunschteile)
            {
                var datenbank = string.Empty;
                if (!string.IsNullOrWhiteSpace(teil.DatenbankId))
                {
                    datenbank = teil.DatenbankId.Substring(0, 3);
                    if (datenbank != "mtb")
                    {
                        datenbank = "rr";
                    }

                    datenbank = teil.DatenbankId.Substring(teil.DatenbankId.IndexOf(':') + 1) + datenbank;
                }

                var line = string.Format("{0}|{1}|{2}|{3} |{4}",
                    FormatStringRight(teil.Komponente, 20),
                    FormatStringRight(GetAnzeigeName(teil.Hersteller,
                                                        teil.Beschreibung,
                                                        teil.Groesse,
                                                        teil.Jahr), 30),
                    FormatStringRight(datenbank, 10),
                    ConvertPreis(teil.Preis).PadLeft(11),
                    ConvertGewicht(teil.Gewicht).PadLeft(9));
                message.AppendLine(line);
            }

            message.AppendLine();

            WriteAndOpenFile(message.ToString(), "Wunschliste");
        }

        private string FormatStringRight(string formatString, int maxLength)
        {
            if (formatString.Length > maxLength)
            {
                return formatString.Substring(0, maxLength - 3) + "...";
            }

            return formatString.PadRight(maxLength);
        }

        private string ConvertPreis(int preis)
        {
            var intValue = preis;

            intValue = intValue < 0 ? 0 : intValue;

            var retVal = String.Format(new CultureInfo("de-DE"), "{0:C2}", (float) intValue/100);

            return retVal;
        }

        private string ConvertGewicht(int gewicht)
        {
            if (gewicht >= 1000)
            {
                var retValue = (decimal)gewicht / 1000;
                return retValue.ToString("N3") + " kg";
            }

            return gewicht.ToString("N0") + " g";
        }

        private string GetAnzeigeName(string hersteller,
                                        string beschreibung,
                                        string groesse,
                                        string jahr)
        {
            var strBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(hersteller))
            {
                strBuilder.Append(hersteller.Trim() + " ");
            }

            if (!string.IsNullOrWhiteSpace(beschreibung))
            {
                strBuilder.Append(beschreibung.Trim() + " ");
            }

            if (!string.IsNullOrWhiteSpace(groesse))
            {
                strBuilder.Append(groesse.Trim() + " ");
            }

            if (!string.IsNullOrWhiteSpace(jahr))
            {
                strBuilder.Append(jahr.Trim() + " ");
            }

            return strBuilder.ToString().Trim();
        }
    }
}

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
            return "zip";
        }

        public void ExportKomponenten(List<KomponenteDto> listeKomponenten, 
                                        int gesamtPreis, 
                                        int gesamtGewicht, 
                                        int bereitsGezahlt, 
                                        int schonGewogen)
        {
            try
            {
            }
            catch (IOException ex)
            {
                throw new Exception("Die Daten konnten nicht exportiert werden", ex);
            }
        }

        private void WriteAndOpenFile(string text, string baseFileName)
        {
        }
            

        public void ExportRestekiste(List<RestteilDto> listeEinzelteile)
        {
        }

        public void ExportWunschliste(List<WunschteilDto> listeWunschteile)
        {
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

using System;
using System.Collections.ObjectModel;
using System.Text;
using TeileListe.Restekiste.ViewModel;
using TeileListe.Szenariorechner.ViewModel;
using TeileListe.Teileliste.ViewModel;
using TeileListe.Wunschliste.ViewModel;

namespace TeileListe.Classes
{
    internal static class CsvFormatter
    {
        internal static string GetFormattetKomponenten(ObservableCollection<KomponenteViewModel> listeTeile)
        {
            var message = new StringBuilder();
            message.AppendLine("Komponente;Beschreibung;Shop;Link;Preis;Gekauft;Gewicht;Gewogen;Hersteller;Groesse;Jahr;DatenbankId;DatenbankLink;Guid");

            foreach (var teil in listeTeile)
            {
                message.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13}", 
                                                    teil.Komponente.Replace(";", ""),
                                                    teil.Beschreibung == null ? "" : teil.Beschreibung.Replace(";", ""),
                                                    teil.Shop == null ? "" : teil.Shop.Replace(";", ""),
                                                    teil.Link == null ? "" : teil.Link.Replace(";", ""), 
                                                    teil.Preis, 
                                                    teil.Gekauft, 
                                                    teil.Gewicht, 
                                                    teil.Gekauft,
                                                    teil.Hersteller == null ? "" : teil.Hersteller.Replace(";", ""),
                                                    teil.Groesse == null ? "" : teil.Groesse.Replace(";", ""),
                                                    teil.Jahr == null ? "" : teil.Jahr.Replace(";", ""),
                                                    teil.DatenbankId == null ? "" : teil.DatenbankId.Replace(";", ""),
                                                    teil.DatenbankLink == null ? "" : teil.DatenbankLink.Replace(";", ""), 
                                                    teil.Guid == null ? "" : teil.Guid));
            }

            return message.ToString();
        }

        internal static string GetFormattetRestekiste(ObservableCollection<RestteilViewModel> listeTeile)
        {
            var message = new StringBuilder();
            message.AppendLine("Komponente;Beschreibung;Shop;Link;Preis;Gekauft;Gewicht;Gewogen;Hersteller;Groesse;Jahr;DatenbankId;DatenbankLink");

            foreach (var teil in listeTeile)
            {
                message.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13}",
                                                    teil.Komponente.Replace(";", ""),
                                                    teil.Beschreibung != null ? teil.Beschreibung.Replace(";", "") : "",
                                                    "Restekiste",
                                                    "",
                                                    teil.Preis,
                                                    "True",
                                                    teil.Gewicht,
                                                    "True",
                                                    teil.Hersteller == null ? "" : teil.Hersteller.Replace(";", ""),
                                                    teil.Groesse == null ? "" : teil.Groesse.Replace(";", ""),
                                                    teil.Jahr == null ? "" : teil.Jahr.Replace(";", ""),
                                                    teil.DatenbankId == null ? "" : teil.DatenbankId.Replace(";", ""),
                                                    teil.DatenbankLink == null ? "" : teil.DatenbankLink.Replace(";", ""),
                                                    teil.Guid == null ? "" : teil.Guid));
            }
            return message.ToString();
        }

        internal static string GetFormattetWunschliste(ObservableCollection<WunschteilViewModel> listeTeile)
        {
            var message = new StringBuilder();
            message.AppendLine("Komponente;Beschreibung;Shop;Link;Preis;Gekauft;Gewicht;Gewogen;Hersteller;Groesse;Jahr;DatenbankId;DatenbankLink;Guid");

            foreach (var teil in listeTeile)
            {
                message.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13}",
                                                    teil.Komponente.Replace(";", ""),
                                                    teil.Beschreibung == null ? "" : teil.Beschreibung.Replace(";", ""),
                                                    teil.Shop == null ? "" : teil.Shop.Replace(";", ""),
                                                    teil.Link == null ? "" : teil.Link.Replace(";", ""),
                                                    teil.Preis,
                                                    "False",
                                                    teil.Gewicht,
                                                    "False",
                                                    teil.Hersteller == null ? "" : teil.Hersteller.Replace(";", ""),
                                                    teil.Groesse == null ? "" : teil.Groesse.Replace(";", ""),
                                                    teil.Jahr == null ? "" : teil.Jahr.Replace(";", ""),
                                                    teil.DatenbankId == null ? "" : teil.DatenbankId.Replace(";", ""),
                                                    teil.DatenbankLink == null ? "" : teil.DatenbankLink.Replace(";", ""),
                                                    teil.Guid == null ? "" : teil.Guid));
            }
            return message.ToString();
        }

        internal static string GetFormattetAlternativen(ObservableCollection<SzenarioKomponenteViewModel> vergleichsListe)
        {
            var message = new StringBuilder();
            message.AppendLine("Komponente;Beschreibung;Shop;Link;Preis;Gekauft;Gewicht;Gewogen;Hersteller;Groesse;Jahr;DatenbankId;DatenbankLink;Guid");

            foreach (var teil in vergleichsListe)
            {
                message.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13}",
                                                    teil.Komponente.Replace(";", ""),
                                                    teil.AlternativeBeschreibung == null ? "" : teil.AlternativeBeschreibung.Replace(";", ""),
                                                    "Szenariorechner",
                                                    "",
                                                    0,
                                                    "False",
                                                    teil.AlternativeGewicht,
                                                    "False",
                                                    teil.AlternativeHersteller == null ? "" : teil.AlternativeHersteller.Replace(";", ""),
                                                    teil.AlternativeGroesse == null ? "" : teil.AlternativeGroesse.Replace(";", ""),
                                                    teil.AlternativeJahr == null ? "" : teil.AlternativeJahr.Replace(";", ""),
                                                    "",
                                                    "",
                                                    Guid.NewGuid().ToString()));
            }
            return message.ToString();
        }
    }
}

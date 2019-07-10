using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using TeileListe.Common.Dto;
using TeileListe.Common.View;

namespace TeileListe.Common.Classes
{
    public static class HilfsFunktionen
    {
        public static string GetValidFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '_'));
        }

        public static string GetAnzeigeName(RestteilDto teil)
        {
            return GetAnzeigeName(teil.Hersteller,
                                    teil.Beschreibung,
                                    teil.Groesse,
                                    teil.Jahr);
        }

        public static string GetAnzeigeName(string hersteller,
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

        public static void ShowMessageBox(Window window, 
                                            string titelText, 
                                            string meldungText, 
                                            bool isError)
        {
            var dialog = new MyMessageBox(titelText, meldungText, isError)
            {
                Owner = window
            };

            dialog.ShowDialog();
        }

        public static bool ShowCloseQuestionBox(Window window, string titelText)
        {
            return ShowQuestionBox(window, titelText, "Wollen Sie die Änderungen speichern?");
        }

        public static bool ShowQuestionBox(Window window, string titelText, string questionBox)
        {
            var dialog = new MyQuestionBox(titelText, questionBox)
            {
                Owner = window
            };

            dialog.ShowDialog();

            return dialog.JaGeklickt;
        }
    }
}

using System.Text;
using System.Windows;

namespace TeileListe.Classes
{
    internal static class HilfsFunktionen
    {
        internal static string GetAnzeigeName(string hersteller,
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

        internal static void ShowMessageBox(Window window, 
                                            string titelText, 
                                            string meldungText, 
                                            bool isError)
        {
            var dialog = new MyMessageBox.MyMessageBox(titelText, meldungText, isError)
            {
                Owner = window
            };

            dialog.ShowDialog();
        }

        internal static bool ShowQuestionBox(Window window, string titelText)
        {
            var dialog = new MyMessageBox.MyQuestionBox(titelText)
            {
                Owner = window
            };

            dialog.ShowDialog();

            return dialog.JaGeklickt;
        }
    }
}

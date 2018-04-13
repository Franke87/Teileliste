using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Interop;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;
using TeileListe.Exporter.View;
using TeileListe.Exporter.ViewModel;

namespace TeileListe.Exporter
{
    public class ExportManager : IExportManager
    {
        public void Dispose()
        {
        }

        public void ExportKomponenten(IntPtr parent,
                                        string dateiName,
                                        string csvContent,
                                        List<EinzelteilExportDto> listeKomponenten)
        {
            var dialog = new ExportManagerDialog();
            new WindowInteropHelper(dialog).Owner = parent;

            var viewModel = new ExportManagerViewModel(listeKomponenten)
            {
                CloseAction = dialog.Close
            };

            dialog.DataContext = viewModel;

            dialog.ShowDialog();

            if (viewModel.DoExport)
            {
                try
                {
                }
                catch (IOException ex)
                {
                    throw new Exception("Die Daten konnten nicht exportiert werden", ex);
                }
            }
        }

        public string GetKuerzel()
        {
            return "zip";
        }
    }
}

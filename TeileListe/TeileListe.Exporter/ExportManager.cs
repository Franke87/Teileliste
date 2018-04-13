using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Interop;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;
using TeileListe.Exporter.Dto;
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

            var viewModel = new ExportManagerViewModel(listeKomponenten, !string.IsNullOrWhiteSpace(csvContent))
            {
                CloseAction = dialog.Close
            };

            dialog.DataContext = viewModel;

            dialog.ShowDialog();

            if (viewModel.DoExport)
            {
                try
                {
                    var komponentenList = new List<ZipOrdnerDto>();

                    foreach (var komponente in viewModel.DateiListe)
                    {
                        if (komponente.DateiViewModelListe.Any(teil => teil.IsChecked))
                        {
                            var newItem = new ZipOrdnerDto
                            {
                                ParentGuid = komponente.Guid,
                                FolderName = HilfsFunktionen.GetValidFileName(komponente.Komponente)
                            };
                            if (newItem.FolderName.Length >= 64)
                            {
                                newItem.FolderName = newItem.FolderName.Substring(0, 64);
                            }

                            newItem.FileList = new List<ZipDateiDto>();
                            foreach (var item in komponente.DateiViewModelListe)
                            {
                                newItem.FileList.Add(new ZipDateiDto()
                                {
                                    Dateiendung = item.Dateiendung,
                                    Guid = item.Guid,
                                    Kategorie = item.Kategorie,
                                    Beschreibung = item.Beschreibung
                                });
                            }

                            komponentenList.Add(newItem);
                        }
                    }

                    PackAndOpenZipFile(komponentenList, dateiName, viewModel.MitCsv ? csvContent : "");
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

        private void PackAndOpenZipFile(IEnumerable<ZipOrdnerDto> fileList, string baseFileName, string csvDatei)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var file = Path.Combine(path, HilfsFunktionen.GetValidFileName(baseFileName) + ".zip");
            var i = 1;

            while (File.Exists(file))
            {
                file = Path.Combine(path, string.Format(baseFileName + " ({0}).zip", i++));
            }

            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    if(!string.IsNullOrWhiteSpace(csvDatei))
                    {
                        var entry = arch.CreateEntry(HilfsFunktionen.GetValidFileName(baseFileName) + ".csv");
                        entry.LastWriteTime = DateTimeOffset.Now;
                        using (var entryStream = entry.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream))
                            {
                                streamWriter.Write(csvDatei);
                            }
                        }
                    }

                    foreach (var folder in fileList)
                    {
                        var folderName = HilfsFunktionen.GetValidFileName(folder.FolderName);

                        foreach (var item in folder.FileList)
                        {
                            var subFolder = HilfsFunktionen.GetValidFileName(item.Kategorie);
                            var fileName = HilfsFunktionen.GetValidFileName(item.Beschreibung);

                            arch.CreateEntryFromFile(Path.Combine("Daten", folder.ParentGuid, item.Guid + "." + item.Dateiendung),
                                                        Path.Combine(folderName, subFolder, fileName + "." + item.Dateiendung));
                        }
                    }
                }
            }

            Process.Start(new ProcessStartInfo("explorer.exe")
            {
                Arguments = "/select, \"" + file + "\""
            });
        }
    }
}

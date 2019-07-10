using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
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
        public string InterfaceVersion { get { return "v1.05"; } }

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
            var secureBaseFileName = HilfsFunktionen.GetValidFileName(baseFileName);
            var file = Path.Combine(path, secureBaseFileName + ".zip");
            var i = 1;

            while (File.Exists(file))
            {
                file = Path.Combine(path, string.Format(secureBaseFileName + " ({0}).zip", i++));
            }

            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    if (!string.IsNullOrWhiteSpace(csvDatei))
                    {
                        var entry = arch.CreateEntry("Komponenten.csv");
                        entry.LastWriteTime = DateTimeOffset.Now;
                        using (var entryStream = entry.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream, System.Text.Encoding.Default))
                            {
                                streamWriter.Write(csvDatei);
                            }
                        }
                    }

                    var zipFileList = new List<DateiVerzeichnisEintragDto>();

                    foreach (var folder in fileList)
                    {
                        var folderName = HilfsFunktionen.GetValidFileName(folder.FolderName);

                        foreach (var item in folder.FileList)
                        {
                            var subFolder = HilfsFunktionen.GetValidFileName(item.Kategorie);
                            var fileName = HilfsFunktionen.GetValidFileName(item.Beschreibung);

                            var baseFile = Path.Combine(folderName, subFolder, fileName);
                            var secureFileName = baseFile;

                            var j = 1;

                            while (!IsUniqueFileName(secureFileName, item.Dateiendung, zipFileList))
                            {
                                secureFileName = string.Format("{0} ({1})", secureFileName, j++);
                            }

                            zipFileList.Add(new DateiVerzeichnisEintragDto
                            {
                                ParentGuid = folder.ParentGuid,
                                FileName = secureFileName + "." + item.Dateiendung,
                                Kategorie = item.Kategorie,
                                Dateiendung = item.Dateiendung,
                                Beschreibung = item.Beschreibung
                            });

                            var fileToExport = Path.Combine("Daten", folder.ParentGuid, item.Guid + "." + item.Dateiendung);
                            if(!File.Exists(fileToExport))
                            {
                                fileToExport = Path.Combine("Daten", "Temp", item.Guid + "." + item.Dateiendung);
                            }

                            arch.CreateEntryFromFile(fileToExport, secureFileName + "." + item.Dateiendung);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(csvDatei))
                    {
                        var entry = arch.CreateEntry("Dateiverzeichnis.csv");
                        entry.LastWriteTime = DateTimeOffset.Now;
                        using (var entryStream = entry.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream, Encoding.Default))
                            {
                                var dateiMap = new StringBuilder();

                                foreach (var item in zipFileList)
                                {
                                    dateiMap.AppendLine(string.Format("{0};{1};{2};{3};{4}",
                                                                        item.ParentGuid,
                                                                        item.FileName,
                                                                        item.Dateiendung,
                                                                        item.Kategorie == null ? "" : item.Kategorie.Replace(";", ""),
                                                                        item.Beschreibung == null ? "" : item.Beschreibung.Replace(";", "")));
                                }

                                streamWriter.Write(dateiMap.ToString());
                            }
                        }
                    }
                }
            }

            Process.Start(new ProcessStartInfo("explorer.exe")
            {
                Arguments = "/select, \"" + file + "\""
            });
        }

        private bool IsUniqueFileName(string fileName, string dateiendung, IEnumerable<DateiVerzeichnisEintragDto> fileList)
        {
            var file = fileName + "." + dateiendung;
            return !fileList.Any(item => item.FileName == file);
        }
    }
}

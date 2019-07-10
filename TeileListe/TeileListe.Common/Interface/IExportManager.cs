using System;
using System.Collections.Generic;
using TeileListe.Common.Dto;

namespace TeileListe.Common.Interface
{
    public interface IExportManager : ITeileListeInterface
    {
        string GetKuerzel();

        void ExportKomponenten(IntPtr parent,
                                string dateiName,
                                string csvContent,
                                List<EinzelteilExportDto> listeKomponenten);
    }
}

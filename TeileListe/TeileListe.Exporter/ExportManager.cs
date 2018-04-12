using System.Collections.Generic;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;

namespace TeileListe.Exporter
{
    public class ExportManager : IExportManager
    {
        public void Dispose()
        {
        }

        public void ExportKomponenten(List<KomponenteDto> listeKomponenten, int gesamtPreis, int gesamtGewicht, int bereitsGezahlt, int schonGewogen)
        {
        }

        public void ExportRestekiste(List<RestteilDto> listeEinzelteile)
        {
        }

        public void ExportWunschliste(List<WunschteilDto> listeWunschteile)
        {
        }

        public string GetKuerzel()
        {
            return "zip";
        }
    }
}

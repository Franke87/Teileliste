using System;
using System.Collections.Generic;
using TeileListe.Common.Dto;

namespace TeileListe.Common.Interface
{
    public interface IExportManager : IDisposable
    {
        string GetKuerzel();

        void ExportKomponenten(List<KomponenteDto> listeKomponenten,
                                int gesamtPreis,
                                int gesamtGewicht,
                                int bereitsGezahlt,
                                int schonGewogen);

        void ExportRestekiste(List<RestteilDto> listeEinzelteile);

        void ExportWunschliste(List<WunschteilDto> listeWunschteile);
    }
}

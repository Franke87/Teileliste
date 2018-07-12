using System.Collections.Generic;
using TeileListe.Common.Dto;

namespace TeileListe.Common.Interface
{
    public interface IDbManager : ITeileListeInterface
    {
        void GetFahrraeder(ref List<FahrradDto> liste);
        void SaveFahrraeder(List<FahrradDto> liste);
        void DeleteFahrrad(FahrradDto nameFahrrad);

        void DeleteTeile(List<LoeschenDto> deletedItems);

        void GetKomponente(string fahrradGuid, ref List<KomponenteDto> collection);
        void SaveKomponente(string fahrradGuid, List<KomponenteDto> collection);

        void GetEinzelteile(ref List<RestteilDto> liste);
        void SaveEinzelteile(List<RestteilDto> liste);

        void GetWunschteile(ref List<WunschteilDto> liste);
        void SaveWunschteile(List<WunschteilDto> liste);

        void GetDatenbankDaten(ref List<DatenbankDto> datenbanken);
        void SaveDatenbankDaten(List<DatenbankDto> datenbanken);

        void GetDateiInfos(string komponenteGuid, ref List<DateiDto> dateiListe);
        void SaveDateiInfos(string komponenteGuid, List<DateiDto> dateiListe);
        void DeleteDateiInfos(string komponenteGuid, List<string> deletedItems);

        void GetDateiKategorien(ref List<string> liste);
        void SaveDateiKategorien(List<string> liste);
        void DeleteDateiKategorien(List<string> deletedItems);

        bool KonvertierungErforderlich();
        bool Konvertiere();
    }
}

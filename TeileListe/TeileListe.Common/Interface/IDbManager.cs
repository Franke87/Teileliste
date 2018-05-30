using System;
using System.Collections.Generic;
using TeileListe.Common.Dto;

namespace TeileListe.Common.Interface
{
    public interface IDbManager : ITeileListeInterface
    {
        void GetFahrraeder(ref List<string> liste);
        void SaveFahrraeder(List<string> liste);
        void DeleteFahrrad(string nameFahrrad);

        void GetKomponente(string nameFahrrad, ref List<KomponenteDto> collection);
        void SaveKomponente(string nameFahrrad, List<KomponenteDto> collection);
        void DeleteKomponenten(string nameFahrrad, List<LoeschenDto> deletedItems);

        void GetEinzelteile(ref List<RestteilDto> liste);
        void SaveEinzelteile(List<RestteilDto> liste);
        void DeleteEinzelteile(List<LoeschenDto> deletedItems);

        void GetWunschteile(ref List<WunschteilDto> liste);
        void SaveWunschteile(List<WunschteilDto> liste);
        void DeleteWunschteile(List<LoeschenDto> deletedItems);

        void GetDatenbankDaten(ref List<DatenbankDto> datenbanken);
        void SaveDatenbankDaten(List<DatenbankDto> datenbanken);
        void SaveDefaultDatenbank(string datenbank);

        void GetDateiInfos(string komponenteGuid, ref List<DateiDto> dateiListe);
        void SaveDateiInfos(string komponenteGuid, List<DateiDto> dateiListe);
        void DeleteDateiInfos(string komponenteGuid, List<string> deletedItems);

        void GetDateiKategorien(ref List<string> liste);
        void SaveDateiKategorien(List<string> liste);
        void DeleteDateiKategorien(List<string> deletedItems);
    }
}

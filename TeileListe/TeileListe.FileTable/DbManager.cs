using System.Collections.Generic;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;

namespace TeileListe.Table
{
    public class DbManager : IDbManager
    {
        private readonly IniReader _iniReader;

        public string InterfaceVersion { get { return "v1.04"; } }

        public DbManager()
        {
            _iniReader = new IniReader();
            _iniReader.Initialize();
        }

        public void GetFahrraeder(ref List<string> liste)
        {
            _iniReader.GetFahrraeder(ref liste);
        }

        public void SaveFahrraeder(List<string> liste)
        {
            _iniReader.SaveFahrraeder(liste);
        }

        public void DeleteFahrrad(string nameFahrrad)
        {
            
        }

        public void SaveKomponente(string nameFahrrad, List<KomponenteDto> collection)
        {
            _iniReader.SaveKomponenten(nameFahrrad, collection);
        }

        public void GetKomponente(string nameFahrrad, ref List<KomponenteDto> collection)
        {
            _iniReader.GetKomponenteIds(nameFahrrad, ref collection);
        }

        public void DeleteKomponenten(string nameFahrrad, List<LoeschenDto> deletedItems)
        {
            _iniReader.DeleteKomponenten(nameFahrrad, deletedItems);
        }

        public void GetEinzelteile(ref List<RestteilDto> liste)
        {
            _iniReader.GetEinzelteileIds(ref liste);
        }

        public void SaveEinzelteile(List<RestteilDto> liste)
        {
            _iniReader.SaveEinzelteile(liste);
        }

        public void DeleteEinzelteile(List<LoeschenDto> deletedItems)
        {
            _iniReader.DeleteEinzelteile(deletedItems);
        }

        public void GetWunschteile(ref List<WunschteilDto> liste)
        {
            _iniReader.GetWunschteileIds(ref liste);
        }

        public void SaveWunschteile(List<WunschteilDto> liste)
        {
            _iniReader.SaveWunschteile(liste);
        }

        public void DeleteWunschteile(List<LoeschenDto> deletedItems)
        {
            _iniReader.DeleteWunschteile(deletedItems);
        }

        public void GetDatenbankDaten(ref List<DatenbankDto> datenbanken)
        {
            datenbanken.ForEach(item => _iniReader.ReadDatenbank(ref item));
            _iniReader.ReadDefaultDatenbank(ref datenbanken);
        }

        public void SaveDatenbankDaten(List<DatenbankDto> datenbanken)
        {
            _iniReader.SaveDatenbankDaten(datenbanken);
        }

        public void SaveDefaultDatenbank(string datenbank)
        {
            _iniReader.SaveDefaultDatenbank(datenbank);
        }

        public void GetDateiInfos(string komponenteGuid, ref List<DateiDto> dateiListe)
        {
            _iniReader.GetDateiInfos(komponenteGuid, ref dateiListe);
        }

        public void SaveDateiInfos(string komponenteGuid, List<DateiDto> dateiListe)
        {
            _iniReader.SaveDateiInfos(komponenteGuid, dateiListe);
        }

        public void DeleteDateiInfos(string komponenteGuid, List<string> deletedItems)
        {
            _iniReader.DeleteDateiInfos(komponenteGuid, deletedItems);
        }

        public void GetDateiKategorien(ref List<string> liste)
        {
            _iniReader.GetDateiKategorien(ref liste);
        }

        public void SaveDateiKategorien(List<string> liste)
        {
            _iniReader.SaveDateiKategorien(liste);
        }

        public void DeleteDateiKategorien(List<string> deletedItems)
        {

        }

        public void Dispose()
        {
        }
    }
}

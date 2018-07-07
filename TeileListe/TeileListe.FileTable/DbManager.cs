using System.Collections.Generic;
using TeileListe.Common.Dto;
using TeileListe.Common.Interface;

namespace TeileListe.Table
{
    public class DbManager : IDbManager
    {
        private readonly XmlManager _xmlManager;

        public string InterfaceVersion { get { return "v1.04"; } }

        public DbManager()
        {
            _xmlManager = new XmlManager();
            _xmlManager.Initialize(InterfaceVersion);
        }

        public void GetFahrraeder(ref List<FahrradDto> fahrraeder)
        {
            _xmlManager.GetFahrraeder(ref fahrraeder);
        }

        public void SaveFahrraeder(List<FahrradDto> fahrraeder)
        {
            _xmlManager.SaveFahrraeder(fahrraeder);
        }

        public void DeleteFahrrad(FahrradDto fahrrad)
        {
            _xmlManager.DeleteFahrrad(fahrrad);
        }

        public void SaveKomponente(string fahrradGuid, List<KomponenteDto> collection)
        {
            _xmlManager.SaveKomponenten(fahrradGuid, collection);
        }

        public void GetKomponente(string fahrradGuid, ref List<KomponenteDto> collection)
        {
            _xmlManager.GetKomponenten(fahrradGuid, ref collection);
        }

        public void DeleteTeile(List<LoeschenDto> deletedItems)
        {
            _xmlManager.DeleteTeile(deletedItems);
        }

        public void GetEinzelteile(ref List<RestteilDto> liste)
        {
            _xmlManager.GetEinzelteile(ref liste);
        }

        public void SaveEinzelteile(List<RestteilDto> liste)
        {
            _xmlManager.SaveEinzelteile(liste);
        }

        public void GetWunschteile(ref List<WunschteilDto> liste)
        {
            _xmlManager.GetWunschliste(ref liste);
        }

        public void SaveWunschteile(List<WunschteilDto> liste)
        {
            _xmlManager.SaveWunschliste(liste);
        }

        public void GetDatenbankDaten(ref List<DatenbankDto> datenbanken)
        {
            _xmlManager.ReadDatenbanken(ref datenbanken);
        }

        public void SaveDatenbankDaten(List<DatenbankDto> datenbanken)
        {
            _xmlManager.SaveDatenbanken(datenbanken);
        }

        public void GetDateiInfos(string komponenteGuid, ref List<DateiDto> dateiListe)
        {
            _xmlManager.GetDateiInfos(komponenteGuid, ref dateiListe);
        }

        public void SaveDateiInfos(string komponenteGuid, List<DateiDto> dateiListe)
        {
            _xmlManager.SaveDateiInfos(komponenteGuid, dateiListe);
        }

        public void DeleteDateiInfos(string komponenteGuid, List<string> deletedItems)
        {
            _xmlManager.DeleteDateiInfos(komponenteGuid, deletedItems);
        }

        public void GetDateiKategorien(ref List<string> liste)
        {
            _xmlManager.GetDateiKategorien(ref liste);
        }

        public void SaveDateiKategorien(List<string> liste)
        {
            _xmlManager.SaveDateiKategorien(liste);
        }

        public void DeleteDateiKategorien(List<string> deletedItems)
        {

        }

        public bool KonvertierungErforderlich()
        {
            using (var converter = new DbConverter())
            {
                return converter.KonvertiereungErforderlich();
            }
        }

        public bool Konvertiere()
        {
            using (var converter = new DbConverter())
            {
                return converter.Konvertiere(InterfaceVersion);
            }
        }

        public void Dispose()
        {
        }
    }
}

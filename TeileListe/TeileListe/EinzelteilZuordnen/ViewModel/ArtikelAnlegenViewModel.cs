using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TeileListe.Common.Classes;
using TeileListe.Common.Dto;
using TeileListe.Gewichtsdatenbanken.ViewModel;
using TeileListe.Enums;
using TeileListe.Internal.ViewModel;
using TeileListe.MessungHochladen.ViewModel;

namespace TeileListe.EinzelteilZuordnen.ViewModel
{
    public class ArtikelAnlegenViewModel : MyCommonViewModel
    {
        #region Properties

        private string _beschreibung;
        public string Beschreibung
        {
            get { return _beschreibung; }
            set
            {
                SetProperty("Beschreibung", ref _beschreibung, value);
                HasError = HasValidationError();
            }
        }

        private string _groesse;
        public string Groesse
        {
            get { return _groesse; }
            set { SetProperty("Groesse", ref _groesse, value); }
        }

        private string _jahr;
        public string Jahr
        {
            get { return _jahr; }
            set
            {
                SetProperty("Jahr", ref _jahr, value);
                HasError = HasValidationError();
            }
        }

        private decimal _gewicht;
        public decimal Gewicht
        {
            get { return _gewicht; }
            set 
            { 
                SetProperty("Gewicht", ref _gewicht, value);
                HasError = HasValidationError();
            }
        }

        private decimal _gewichtHersteller;
        public decimal GewichtHersteller
        {
            get { return _gewichtHersteller; }
            set { SetProperty("GewichtHersteller", ref _gewichtHersteller, value); }
        }

        private string _kommentar;
        public string Kommentar
        {
            get { return _kommentar; }
            set { SetProperty("Kommentar", ref _kommentar, value); }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { SetProperty("Link", ref _link, value); }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty("HasError", ref _hasError, value); }
        }

        private ObservableCollection<DateiAuswahlViewModel> _dateiListe;
        public ObservableCollection<DateiAuswahlViewModel> DateiListe
        {
            get { return _dateiListe; }
            set
            {
                SetProperty("DateiListe", ref _dateiListe, value);
                HasError = HasValidationError();
            }
        }

        private bool _neuesAusgewaehlt;
        public bool NeuesAusgewaehlt
        {
            get { return _neuesAusgewaehlt; }
            set
            {
                SetProperty("NeuesAusgewaehlt", ref _neuesAusgewaehlt, value);
                HasError = HasValidationError();
            }
        }

        private DateiAuswahlViewModel _selectedDatei;
        public DateiAuswahlViewModel SelectedDatei
        {
            get { return _selectedDatei; }
            set
            {
                if (SetProperty("SelectedDatei", ref _selectedDatei, value))
                {
                    HasError = HasValidationError();
                }
            }
        }

        public CommonDateiViewModel DateiViewModel { get; set; }
        public WebAuswahlViewModel DatenbankViewModel { get; set; }

        public string Guid { get; set; }
        public bool AuswahlEnabled { get; set; }

        #endregion

        #region Konstruktor

        public ArtikelAnlegenViewModel(List<DatenbankDto> datenbanken, List<DateiDto> listeDateien, KomponenteDto einzelteil)
        {
            DatenbankViewModel = new WebAuswahlViewModel(datenbanken, DatenbankModus.HerstellerKategorieSelection);
            DatenbankViewModel.PropertyChanged += ContentPropertyChanged;

            DateiViewModel = new CommonDateiViewModel(DateiOeffnenEnum.Image);
            DateiViewModel.PropertyChanged += ContentPropertyChanged;

            Beschreibung = einzelteil.Beschreibung;
            Groesse = einzelteil.Groesse;
            Jahr = einzelteil.Jahr;
            Gewicht = einzelteil.Gewicht;
            GewichtHersteller = 0;
            Kommentar = "";
            Link = "";

            Guid = einzelteil.Guid;

            var liste = new List<DateiDto>(listeDateien);
            liste.RemoveAll(item => item.Kategorie != "Gewichtsmessung");
            liste.RemoveAll(item => !(item.Dateiendung.ToLower() == "png"
                                    || item.Dateiendung.ToLower() == "jpg"
                                    || item.Dateiendung.ToLower() == "jpeg"));

            NeuesAusgewaehlt = liste.Count == 0;
            AuswahlEnabled = liste.Count > 0;

            DateiListe = new ObservableCollection<DateiAuswahlViewModel>();

            foreach (var item in liste)
            {
                DateiListe.Add(new DateiAuswahlViewModel(Guid, "Teileliste", item));
            }

            SelectedDatei = DateiListe.FirstOrDefault();

            HasError = HasValidationError();
        }

        #endregion

        #region Funktionen

        void ContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasError = HasValidationError();
        }

        public bool HasValidationError()
        {
            bool hasError = DatenbankViewModel.HasError 
                            || string.IsNullOrWhiteSpace(Beschreibung)
                            || Gewicht == 0
                            || string.IsNullOrWhiteSpace(Jahr);

            if (!hasError)
            {
                hasError = NeuesAusgewaehlt ? DateiViewModel.HasError : SelectedDatei == null; 
            }

            return hasError;
        }

        #endregion

    }
}

using System;
using System.Windows;
using TeileListe.Common.Classes;

namespace TeileListe.Teileliste.ViewModel
{
    internal class FahrradViewModel : MyCommonViewModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty("Name", ref _name, value); }
        }

        private string _guid;
        public string Guid
        {
            get { return _guid; }
            set { SetProperty("Guid", ref _guid, value); }
        }

        public Action<string> FahrradLoeschenAction { get; set; }

        public MyParameterCommand<Window> LoeschenCommand { get; set; }

        internal FahrradViewModel()
        {
            LoeschenCommand = new MyParameterCommand<Window>(OnFahrradLoeschen);
        }

        private void OnFahrradLoeschen(Window owner)
        {
            var text = "Möchten Sie das Fahrrad wirklich löschen?";

            if(HilfsFunktionen.ShowQuestionBox(owner, "Teileliste", text))
            {
                FahrradLoeschenAction(Guid);
            }
        }
    }
}

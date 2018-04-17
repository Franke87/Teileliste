﻿using System;
using System.Windows;
using TeileListe.Common.Classes;
using TeileListe.KategorienVerwalten.View;

namespace TeileListe.KategorienVerwalten.ViewModel
{
    internal class KategorieViewModel : MyCommonViewModel
    {
        #region Commands

        public MyParameterCommand<Window> ChangeCommand { get; set; }
        public MyCommand NachObenCommand { get; set; }
        public MyCommand NachUntenCommand { get; set; }
        public MyCommand LoeschenCommand { get; set; }

        #endregion

        #region Actions

        public Action<string> NachObenAction { get; set; }
        public Action<string> NachUntenAction { get; set; }
        public Action<string> LoeschenAction { get; set; }
        public Func<string> GetBlackList { get; set; }

        #endregion

        #region Properties

        public string Guid { get; set; }
        public bool ContextBarEnbaled { get; set; }

        private string _kategorie;
        public string Kategorie
        {
            get { return _kategorie; }
            set { SetProperty("Kategorie", ref _kategorie, value); }
        }

        #endregion

        internal KategorieViewModel(string kategorie)
        {
            Guid = System.Guid.NewGuid().ToString();
            Kategorie = kategorie;

            ContextBarEnbaled = Kategorie != "Gewichtsmessung";

            ChangeCommand = new MyParameterCommand<Window>(OnChange);
            LoeschenCommand = new MyCommand(OnLoeschen);
            NachObenCommand = new MyCommand(NachOben);
            NachUntenCommand = new MyCommand(NachUnten);
        }

        #region Functions

        private void NachOben()
        {
            NachObenAction(Guid);
        }

        private void NachUnten()
        {
            NachUntenAction(Guid);
        }

        private void OnChange(Window window)
        {
            var dialog = new KategorieBearbeitenView()
            {
                Owner = window
            };
            var viewModel = new KategorieBearbeitenViewModel(GetBlackList(), Kategorie)
            {
                CloseAction = dialog.Close
            };
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (viewModel.IsOk)
            {
                Kategorie = viewModel.Kategorie;
            }
        }

        private void OnLoeschen()
        {
            LoeschenAction(Guid);
        }

        #endregion
    }
}

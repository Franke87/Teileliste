using System;
using System.Windows;
using System.Windows.Controls;
using TeileListe.Teileliste.ViewModel;

namespace TeileListe.Teileliste.View
{
    public partial class TeilelisteView
    {
        public TeilelisteView()
        {
            InitializeComponent();
            var viewModel = new TeilelisteViewModel();
            Closing += viewModel.OnWindowClosing;
            DataContext = viewModel;
        }

        private void CheckForToolTipNeeded(object sender, ToolTipEventArgs e)
        {
            var test = sender as TextBlock;
            if (test != null && test.TextTrimming != TextTrimming.None)
            {
                test.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                e.Handled = test.ActualWidth >= test.DesiredSize.Width;
            }
        }
    }
}

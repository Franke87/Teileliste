using System;
using System.Windows;
using System.Windows.Controls;

namespace TeileListe.Teileliste.UserControl
{
    /// <summary>
    /// Interaction logic for FahrradAuswahlControl.xaml
    /// </summary>
    public partial class FahrradAuswahlControl
    {
        public FahrradAuswahlControl()
        {
            InitializeComponent();
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

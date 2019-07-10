using System;
using System.Windows;
using System.Windows.Controls;

namespace TeileListe.Wunschliste.UserControl
{
    /// <summary>
    /// Interaction logic for WunschlisteListeControl.xaml
    /// </summary>
    public partial class WunschlisteListeControl
    {
        public WunschlisteListeControl()
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

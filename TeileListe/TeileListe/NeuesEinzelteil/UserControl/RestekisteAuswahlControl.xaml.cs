using System;
using System.Windows;
using System.Windows.Controls;

namespace TeileListe.NeuesEinzelteil.UserControl
{
    /// <summary>
    /// Interaction logic for RestekisteAuswahlControl.xaml
    /// </summary>
    public partial class RestekisteAuswahlControl
    {
        public RestekisteAuswahlControl()
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

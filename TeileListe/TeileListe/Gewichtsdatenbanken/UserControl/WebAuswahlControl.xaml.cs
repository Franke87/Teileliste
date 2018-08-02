using System;
using System.Windows;
using System.Windows.Controls;

namespace TeileListe.Gewichtsdatenbanken.UserControl
{
    /// <summary>
    /// Interaction logic for WebAuswahlControl.xaml
    /// </summary>
    public partial class WebAuswahlControl
    {
        public WebAuswahlControl()
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

using System;
using System.Windows;
using System.Windows.Controls;

namespace TeileListe.Restekiste.UserControl
{
    /// <summary>
    /// Interaction logic for RestekisteListeControl.xaml
    /// </summary>
    public partial class RestekisteListeControl
    {
        public RestekisteListeControl()
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

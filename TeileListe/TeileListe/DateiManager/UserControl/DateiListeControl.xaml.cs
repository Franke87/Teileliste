using System;
using System.Windows;
using System.Windows.Controls;

namespace TeileListe.DateiManager.UserControl
{
    /// <summary>
    /// Interaktionslogik für DateiListeControl.xaml
    /// </summary>
    public partial class DateiListeControl
    {
        public DateiListeControl()
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

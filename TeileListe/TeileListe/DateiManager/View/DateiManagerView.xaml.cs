using System;
using System.Windows;
using System.Windows.Controls;

namespace TeileListe.DateiManager.View
{
    /// <summary>
    /// Interaktionslogik für DateiManagerView.xaml
    /// </summary>
    public partial class DateiManagerView : Window
    {
        public DateiManagerView(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            Top = owner.Top + 40;
            Left = owner.Left + (owner.ActualWidth - 600) / 2;
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

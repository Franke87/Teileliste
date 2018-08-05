using System;
using System.Windows;
using System.Windows.Controls;

namespace TeileListe.Szenariorechner.View
{
    public partial class SzenariorechnerDialog : Window
    {
        public SzenariorechnerDialog(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            Height = Owner.ActualHeight;
            Width = Owner.ActualWidth;
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

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BottomRow.MaxHeight = MainGrid.ActualHeight - 200;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;

namespace TeileListe.Restekiste.View
{
    /// <summary>
    /// Interaction logic for RestekisteDialog.xaml
    /// </summary>
    public partial class RestekisteDialog
    {
        public RestekisteDialog(Window owner)
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
    }
}

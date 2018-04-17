using System.Windows;

namespace TeileListe.KategorienVerwalten.View
{
    /// <summary>
    /// Interaktionslogik für KategorienVerwaltenView.xaml
    /// </summary>
    public partial class KategorienVerwaltenView : Window
    {
        public KategorienVerwaltenView(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            Height = Owner.ActualHeight > 300 ? Owner.ActualHeight : 300;
            Width = Owner.ActualWidth;
        }
    }
}

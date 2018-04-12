using System.Windows;

namespace TeileListe.DateiManager.UserControl
{
    /// <summary>
    /// Interaktionslogik für DateiBearbeitenControl.xaml
    /// </summary>
    public partial class DateiBearbeitenControl
    {
        public Visibility DateiauswahlVisible
        {
            get { return (Visibility)GetValue(DateiauswahlVisibleProperty); }
            set { SetValue(DateiauswahlVisibleProperty, value); }
        }

        public static readonly DependencyProperty DateiauswahlVisibleProperty
            = DependencyProperty.Register("DateiauswahlVisible",
                                          typeof(Visibility),
                                          typeof(DateiBearbeitenControl),
                                          new UIPropertyMetadata(Visibility.Visible));

        public DateiBearbeitenControl()
        {
            InitializeComponent();
        }
    }
}

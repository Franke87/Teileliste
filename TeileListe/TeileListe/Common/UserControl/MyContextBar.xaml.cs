using System.Windows;
using System.Windows.Media;

namespace TeileListe.Common.UserControl
{
    /// <summary>
    /// Interaction logic for MyContextBar.xaml
    /// </summary>
    public partial class MyContextBar
    {
        public Visibility AusbauenVisible
        {
            get { return (Visibility)GetValue(AusbauenVisibleProperty); }
            set { SetValue(AusbauenVisibleProperty, value); }
        }

        public static readonly DependencyProperty AusbauenVisibleProperty
            = DependencyProperty.Register("AusbauenVisible",
                                          typeof(Visibility),
                                          typeof(MyContextBar),
                                          new UIPropertyMetadata(Visibility.Visible));

        public Visibility GewichtsdatenbankVisible
        {
            get { return (Visibility)GetValue(GewichtsdatenbankVisibleProperty); }
            set { SetValue(GewichtsdatenbankVisibleProperty, value); }
        }

        public static readonly DependencyProperty GewichtsdatenbankVisibleProperty
            = DependencyProperty.Register("GewichtsdatenbankVisible",
                                          typeof(Visibility),
                                          typeof(MyContextBar),
                                          new UIPropertyMetadata(Visibility.Visible));

        public Visibility LeerenVisible
        {
            get { return (Visibility)GetValue(LeerenVisibleProperty); }
            set { SetValue(LeerenVisibleProperty, value); }
        }

        public static readonly DependencyProperty LeerenVisibleProperty
            = DependencyProperty.Register("LeerenVisible",
                                          typeof(Visibility),
                                          typeof(MyContextBar),
                                          new UIPropertyMetadata(Visibility.Visible));

        public string DateiManagerToolTip
        {
            get { return (string)GetValue(DateiManagerToolTipProperty); }
            set { SetValue(DateiManagerToolTipProperty, value); }
        }

        public static readonly DependencyProperty DateiManagerToolTipProperty
            = DependencyProperty.Register("DateiManagerToolTip",
                                          typeof(string),
                                          typeof(MyContextBar),
                                          new UIPropertyMetadata("Dokumente verwalten"));

        public Brush ContextBarBackground
        {
            get { return (Brush)GetValue(ContextBarBackgroundProperty); }
            set { SetValue(ContextBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ContextBarBackgroundProperty
            = DependencyProperty.Register("ContextBarBackground",
                                          typeof(Brush),
                                          typeof(MyContextBar),
                                          new UIPropertyMetadata(Brushes.GhostWhite));

        public MyContextBar()
        {
            InitializeComponent();
        }
    }
}

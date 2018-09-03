using System.Windows;
using System.Windows.Media;

namespace TeileListe.Common.UserControl
{
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

        public Thickness EntfernenMargin
        {
            get { return (Thickness)GetValue(EntfernenMarginProperty); }
            set { SetValue(EntfernenMarginProperty, value); }
        }

        public static readonly DependencyProperty EntfernenMarginProperty
            = DependencyProperty.Register("EntfernenMargin",
                                          typeof(Thickness),
                                          typeof(MyContextBar),
                                          new UIPropertyMetadata(new Thickness(3,0,3,0)));

        public bool StandardLinksEnabled
        {
            get { return (bool)GetValue(StandardLinksEnabledProperty); }
            set { SetValue(StandardLinksEnabledProperty, value); }
        }

        public static readonly DependencyProperty StandardLinksEnabledProperty
            = DependencyProperty.Register("StandardLinksEnabled",
                                          typeof(bool),
                                          typeof(MyContextBar),
                                          new UIPropertyMetadata(true));

        public MyContextBar()
        {
            InitializeComponent();
        }
    }
}

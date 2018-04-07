using System.Windows;

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

        // Using a DependencyProperty as the backing store for Property1.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AusbauenVisibleProperty
            = DependencyProperty.Register("AusbauenVisible",
                                          typeof(Visibility),
                                          typeof(MyContextBar),
                                          new UIPropertyMetadata(Visibility.Visible));

        public MyContextBar()
        {
            InitializeComponent();
        }
    }
}

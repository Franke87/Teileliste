using System.Windows;
using TeileListe.Classes;

namespace TeileListe.Common.UserControl
{
    /// <summary>
    /// Interaction logic for CommonPreviewControl.xaml
    /// </summary>
    public partial class CommonPreviewControl
    {
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextProperty
            = DependencyProperty.Register("HeaderText",
                                          typeof(string),
                                          typeof(CommonPreviewControl),
                                          new UIPropertyMetadata(""));

        public string Wert
        {
            get { return (string)GetValue(WertProperty); }
            set { SetValue(WertProperty, value); }
        }

        public static readonly DependencyProperty WertProperty
            = DependencyProperty.Register("Wert",
                                          typeof(string),
                                          typeof(CommonPreviewControl),
                                          new UIPropertyMetadata(""));

        public int Inhalt
        {
            get { return (int)GetValue(InhaltProperty); }
            set { SetValue(InhaltProperty, value); }
        }

        public static readonly DependencyProperty InhaltProperty
            = DependencyProperty.Register("Inhalt",
                                          typeof(int),
                                          typeof(CommonPreviewControl),
                                          new UIPropertyMetadata(0));

        public string Gewicht
        {
            get { return (string)GetValue(GewichtProperty); }
            set { SetValue(GewichtProperty, value); }
        }

        public static readonly DependencyProperty GewichtProperty
            = DependencyProperty.Register("Gewicht",
                                          typeof(string),
                                          typeof(CommonPreviewControl),
                                          new UIPropertyMetadata(""));

        public MyParameterCommand<Window> OeffnenCommand
        {
            get { return (MyParameterCommand<Window>)GetValue(OeffnenCommandProperty); }
            set { SetValue(OeffnenCommandProperty, value); }
        }

        public static readonly DependencyProperty OeffnenCommandProperty
            = DependencyProperty.Register("OeffnenCommand",
                                          typeof(MyParameterCommand<Window>),
                                          typeof(CommonPreviewControl));

        public CommonPreviewControl()
        {
            InitializeComponent();
        }
    }
}

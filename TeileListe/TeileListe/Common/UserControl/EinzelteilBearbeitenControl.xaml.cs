using System.Windows.Input;

namespace TeileListe.Common.UserControl
{
    /// <summary>
    /// Interaction logic for EinzelteilBearbeitenControl.xaml
    /// </summary>
    public partial class EinzelteilBearbeitenControl
    {
        public EinzelteilBearbeitenControl()
        {
            InitializeComponent();
            Dispatcher.BeginInvoke(new System.Action(() => Keyboard.Focus(KomponenteText)),
                                    System.Windows.Threading.DispatcherPriority.Loaded);
        }
    }
}

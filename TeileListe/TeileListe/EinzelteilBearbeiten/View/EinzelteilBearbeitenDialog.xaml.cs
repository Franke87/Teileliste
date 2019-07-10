namespace TeileListe.EinzelteilBearbeiten.View
{
    /// <summary>
    /// Interaction logic for EinzelteilBearbeitenDialog.xaml
    /// </summary>
    public partial class EinzelteilBearbeitenDialog
    {
        public EinzelteilBearbeitenDialog(bool isEinzelteil)
        {
            InitializeComponent();
            if(isEinzelteil)
            {
                DateiControl.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                EinzelteilControl.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}

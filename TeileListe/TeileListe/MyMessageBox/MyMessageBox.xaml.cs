namespace TeileListe.MyMessageBox
{
    public partial class MyMessageBox
    {
        public string MeldungsText { get; set; }
        public string TitelText { get; set; }
        public bool IsError { get; set; }

        public MyMessageBox(string titelText, string meldungsText, bool isError)
        {
            InitializeComponent();

            TitelText = titelText;
            MeldungsText = meldungsText;
            IsError = isError;

            DataContext = this;
        }
    }
}

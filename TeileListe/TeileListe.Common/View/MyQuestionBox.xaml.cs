using TeileListe.Common.Classes;

namespace TeileListe.Common.View
{
    public partial class MyQuestionBox
    {
        public string TitelText { get; set; }

        public string QuestionText { get; set; }

        public bool JaGeklickt { get; set; }

        public MyCommand JaCommand { get; set; }

        public MyQuestionBox(string titelText, string questionText)
        {
            InitializeComponent();

            TitelText = titelText;
            QuestionText = questionText;
            JaGeklickt = false;

            JaCommand = new MyCommand(OnJa);

            DataContext = this;
        }

        private void OnJa()
        {
            JaGeklickt = true;
            Close();
        }
    }
}

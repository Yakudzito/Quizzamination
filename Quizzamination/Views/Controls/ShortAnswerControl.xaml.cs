using System.Windows.Controls;
using Quizzamination.Models;

namespace Quizzamination.Views.Controls
{
    /// <summary>
    /// Interaction logic for ShortAnswerControl.xaml
    /// </summary>
    public partial class ShortAnswerControl : UserControl
    {
        public Question Question { get; }

        public ShortAnswerControl(Question question, string? savedAnswer = null)
        {
            InitializeComponent();
            Question = question;
            this.DataContext = question;

            if (!string.IsNullOrWhiteSpace(savedAnswer))
            {
                AnswerBox.Text = savedAnswer;
            }
        }

        public string GetAnswerText() => AnswerBox.Text.Trim();
    }
}

using System.Windows.Controls;
using System.Windows.Media;
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

        public async void ShowAnswer()
        {
            if (Question.CorrectShortAnswer == null)
                return;
            
            string correctAnswer = Question.CorrectShortAnswer;
            AnswerBox.Text = correctAnswer;
            AnswerBox.Foreground = Brushes.Green;

            await Task.Delay(1000);

            AnswerBox.Foreground = Brushes.Black;
        }
    }
}

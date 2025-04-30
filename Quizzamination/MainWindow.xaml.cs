using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Quizzamination.Models;
using Quizzamination.Services;
using Quizzamination.Views;

namespace Quizzamination
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Question> _questions;
        private List<AnswerResult> _results = new();
        private int _currentIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            _questions = TestLoader.LoadFromFile("test1.json");
            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            var question = _questions[_currentIndex];
            UserControl control = question.Type switch
            {
                QuestionType.SingleChoice => new SingleChoiceControl(question),
                QuestionType.TrueFalse => new TrueFalseControl(question),
                QuestionType.MultipleChoice => new MultipleChoiceControl(question),
                QuestionType.Matching => new MatchingControl(question),
                QuestionType.ShortAnswer => new ShortAnswerControl(question),
                _ => new UserControl { Content = new TextBlock { Text = "Цей тип питання ще не реалізований" } }
            };

            QuestionHost.Content = control;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var question = _questions[_currentIndex];
            object? userAnswer = GetUserAnswer(question);
            bool isCorrect = EvaluateAnswer(question, userAnswer);
            _results.Add(new AnswerResult(question, userAnswer, isCorrect));

            if (_currentIndex < _questions.Count - 1)
            {
                _currentIndex++;
                ShowCurrentQuestion();
            }
            else
            {
                int correctCount = _results.Count(r => r.IsCorrect);
                MessageBox.Show($"Тест завершено!\nПравильних відповідей: {correctCount} з {_results.Count}.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private object? GetUserAnswer(Question question)
        {
            return QuestionHost.Content switch
            {
                SingleChoiceControl single => single.GetSelectedIndex(),
                MultipleChoiceControl multiple => multiple.GetSelectedIndexes(),
                TrueFalseControl tf => tf.GetSelectedIndex(),
                ShortAnswerControl sa => sa.GetAnswerText(),
                _ => null
            };
            // Matching та інші типи пізніше
        }

        private bool EvaluateAnswer(Question question, object? userAnswer)
        {

            if (userAnswer == null) return false;

            return question.Type switch
            {
                QuestionType.SingleChoice or QuestionType.TrueFalse =>
                    question.CorrectAnswers != null && question.CorrectAnswers.FirstOrDefault() == (int?)userAnswer,

                QuestionType.MultipleChoice =>
                    userAnswer is List<int> selected && question.CorrectAnswers != null &&
                    selected.OrderBy(x => x).SequenceEqual(question.CorrectAnswers.OrderBy(x => x)),

                QuestionType.ShortAnswer =>
                    question.CorrectShortAnswer != null &&
                    string.Equals(question.CorrectShortAnswer.Trim(), (userAnswer as string)?.Trim(), System.StringComparison.OrdinalIgnoreCase),

                _ => false
            };
        }
    }
}
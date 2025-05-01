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
using Quizzamination.Views.Controls;

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
            if (_currentIndex >= _questions.Count) return;
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
                var resultsWindow = new ResultsWindow(_results);
                resultsWindow.ShowDialog();
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
                MatchingControl mc => mc.GetSelectedPairs(),
                _ => null
            };
        }

        private bool EvaluateAnswer(Question question, object? userAnswer)
        {
            if (question.Type != QuestionType.ShortAnswer && question.Type != QuestionType.Matching &&
                (userAnswer == null || question.CorrectAnswers == null))
                return false;

            System.Diagnostics.Debug.WriteLine($"Очікується: {string.Join(", ", question.CorrectAnswers ?? new List<int>())}");
            System.Diagnostics.Debug.WriteLine($"Отримано: {userAnswer}");

            return question.Type switch
            {
                QuestionType.SingleChoice or QuestionType.TrueFalse =>
                    userAnswer is int selected && question.CorrectAnswers!.Contains(selected),

                QuestionType.MultipleChoice =>
                    userAnswer is List<int> selectedList &&
                    question.CorrectAnswers != null &&
                    selectedList.OrderBy(x => x).SequenceEqual(question.CorrectAnswers.OrderBy(x => x)),

                QuestionType.ShortAnswer =>
                    string.Equals(question.CorrectShortAnswer, userAnswer as string, System.StringComparison.OrdinalIgnoreCase),

                QuestionType.Matching =>
                    userAnswer is Dictionary<string, string> dict &&
                    question.MatchPairs != null &&
                    dict.Count == question.MatchPairs.Count &&
                    !question.MatchPairs.Except(dict).Any(),

                _ => false
            };
        }
    }
}
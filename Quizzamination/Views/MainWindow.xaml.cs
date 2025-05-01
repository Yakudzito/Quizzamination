using System.Windows;
using System.Windows.Controls;
using Quizzamination.Models;
using Quizzamination.Services;
using Quizzamination.Views.Controls;

namespace Quizzamination.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _currentIndex = 0;
        private List<Question> _questions;
        private List<AnswerResult> _results = new();
        private Dictionary<int, object?> _userAnswers = new(); // зберігає відповідь по індексу

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
            _userAnswers.TryGetValue(_currentIndex, out var savedAnswer);

            UserControl control = question.Type switch
            {
                QuestionType.SingleChoice => new SingleChoiceControl(question, savedAnswer as int?),
                QuestionType.TrueFalse => new TrueFalseControl(question, savedAnswer as int?),
                QuestionType.MultipleChoice => new MultipleChoiceControl(question, savedAnswer as List<int>),
                QuestionType.Matching => new MatchingControl(question, savedAnswer as Dictionary<string, string>),
                QuestionType.ShortAnswer => new ShortAnswerControl(question, savedAnswer as string),
                _ => new UserControl { Content = new TextBlock { Text = "Цей тип ще не підтримується" } }
            };

            QuestionHost.Content = control;
        }
        private void SaveCurrentAnswer()
        {
            var question = _questions[_currentIndex];
            var answer = GetUserAnswer(question);
            _userAnswers[_currentIndex] = answer;
        }
        private void GenerateResults()
        {
            _results.Clear();
            for (int i = 0; i < _questions.Count; i++)
            {
                var q = _questions[i];
                var a = _userAnswers.ContainsKey(i) ? _userAnswers[i] : null;
                bool isCorrect = EvaluateAnswer(q, a);
                _results.Add(new AnswerResult(q, a, isCorrect));
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentAnswer();
            if (_currentIndex < _questions.Count - 1)
            {
                _currentIndex++;
                ShowCurrentQuestion();
            }
            else
            {
                GenerateResults();
                var resultsWindow = new ResultsWindow(_results);
                resultsWindow.ShowDialog();
            }
        }
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentAnswer();
            if (_currentIndex > 0)
            {
                _currentIndex--;
                ShowCurrentQuestion();
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
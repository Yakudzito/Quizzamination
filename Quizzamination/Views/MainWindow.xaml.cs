using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quizzamination.Models;
using Quizzamination.Services;
using Quizzamination.Views.Controls;
using System.Windows.Threading;

namespace Quizzamination.Views
{
    public static class ListExtensions
    {
        private static readonly Random _random = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isLearningMode = true;

        private bool _isLimitedTime = false;
        private TimeSpan _timeLimit = TimeSpan.FromSeconds(1000);
        
        // Question limitation mode
        private bool _isLimitedQuestions = false;
        private int _questionCount = 10;

        private bool _isShuffled = false;

        private DispatcherTimer _timer = null!;
        private TimeSpan _elapsed;
        
        private int _currentIndex;
        private List<Question> _questions = [];
        private readonly List<AnswerResult> _results = [];
        private readonly Dictionary<int, object?> _userAnswers = new(); // зберігає відповідь по індексу

        public MainWindow()
        {
            InitializeComponent();
            if (_isLearningMode) CheckAnswerButton.Visibility = Visibility.Visible;
            LoadQuestions("test.json", _isShuffled);
            ShowCurrentQuestion();
            StartTimer();
        }


        private void LoadQuestions(string filePath, bool isShuffled)
        {
            _questions = TestLoader.LoadFromFile(filePath);
            if (isShuffled) _questions.Shuffle();
            if (_isLimitedQuestions && _questionCount > 0 && _questionCount < _questions.Count)
                _questions = _questions.Take(_questionCount).ToList();
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

            QuestionNumberTextBlock.Text = $"Питання {_currentIndex + 1} з {_questions.Count}";
            NextButton.Content = (_currentIndex == _questions.Count - 1) ? "Завершити" : "Далі";
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
            var resultsWindow = new ResultsWindow(_results);
            resultsWindow.ShowDialog();
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
                TimerTextBlock.Foreground = Brushes.Blue;
                _timer.Stop();
                GenerateResults();
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

        private void StartTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _elapsed = _isLimitedTime ? _timeLimit : TimeSpan.Zero;
            _timer.Tick += TickEvent;
            _timer.Start();
        }

        private void TickEvent(object? sender, EventArgs e)
        {
            if (_isLimitedTime)
            {
                _elapsed -= TimeSpan.FromSeconds(1);
            }
            else
            {
                _elapsed += TimeSpan.FromSeconds(1);
            }
            TimerTextBlock.Text = $"Час: {_elapsed.Hours:D2}:{_elapsed.Minutes:D2}:{_elapsed.Seconds:D2}";

            if (_elapsed == TimeSpan.Zero && _isLimitedTime)
            {
                _timer.Stop();
                GenerateResults();
            }
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
                    string.Equals(question.CorrectShortAnswer, userAnswer as string, StringComparison.OrdinalIgnoreCase),

                QuestionType.Matching =>
                    userAnswer is Dictionary<string, string> dict &&
                    question.MatchPairs != null &&
                    dict.Count == question.MatchPairs.Count &&
                    !question.MatchPairs.Except(dict).Any(),

                _ => false
            };
        }

        private void CheckAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            switch (QuestionHost.Content)
            {
                case SingleChoiceControl single:
                    single.HighlightAnswer();
                    break;
                case MultipleChoiceControl multiple:
                    multiple.HighlightAnswer();
                    break;
                case TrueFalseControl tf:
                    tf.HighlightAnswer();
                    break;
                case ShortAnswerControl @short:
                    @short.ShowAnswer();
                    break;
            }
        }
    }
}
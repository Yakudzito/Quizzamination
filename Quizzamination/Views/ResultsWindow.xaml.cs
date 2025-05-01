using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quizzamination.Models;

namespace Quizzamination.Views
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow(List<AnswerResult> results)
        {
            InitializeComponent();
            LoadResults(results);
        }

        private void LoadResults(List<AnswerResult> results)
        {
            var panel = new StackPanel { Margin = new Thickness(10) };

            foreach (var result in results)
            {
                var block = new StackPanel { Margin = new Thickness(0, 5, 0, 5) };

                block.Children.Add(new TextBlock
                {
                    Text = $"Питання: {result.Question.Text}",
                    FontWeight = FontWeights.Bold
                });

                block.Children.Add(new TextBlock
                {
                    Text = $"Ваша відповідь: {FormatAnswer(result.UserAnswer, result.Question.Type, result.Question)}"
                });

                block.Children.Add(new TextBlock
                {
                    Text = result.IsCorrect ? "✅ Правильно" : "❌ Неправильно",
                    Foreground = result.IsCorrect ? Brushes.Green : Brushes.Red
                });

                if (!result.IsCorrect)
                {
                    block.Children.Add(new TextBlock
                    {
                        Text = $"Правильна відповідь: {FormatCorrectAnswer(result.Question)}",
                        Foreground = Brushes.Blue
                    });
                }

                panel.Children.Add(block);
            }

            Content = new ScrollViewer { Content = panel };
        }

        private string FormatAnswer(object? answer, QuestionType type, Question question)
        {
            if (type == QuestionType.TrueFalse && answer is int tfVal and (0 or 1))
                return tfVal == 0 ? "Правда" : "Неправда";

            return answer switch
            {
                null => "(немає відповіді)",
                int index => question.Options != null && index < question.Options.Count
                    ? question.Options[index]
                    : index.ToString(),

                List<int> list => question.Options != null
                    ? string.Join(", ", list.Where(i => i < question.Options.Count).Select(i => question.Options[i]))
                    : string.Join(", ", list),

                Dictionary<string, string> dict => string.Join(", ", dict.Select(kvp => $"{kvp.Key} → {kvp.Value}")),
                _ => answer.ToString() ?? ""
            };
        }
        private string FormatCorrectAnswer(Question question)
        {
            return question.Type switch
            {
                QuestionType.SingleChoice or QuestionType.TrueFalse =>
                    question is { Options: not null, CorrectAnswers.Count: > 0 }
                        ? question.Options[question.CorrectAnswers[0]]
                        : "(немає)",

                QuestionType.MultipleChoice =>
                    question is { Options: not null, CorrectAnswers: not null }
                        ? string.Join(", ", question.CorrectAnswers.Select(i => question.Options[i]))
                        : "(немає)",

                QuestionType.ShortAnswer => question.CorrectShortAnswer ?? "(немає)",

                QuestionType.Matching => question.MatchPairs != null
                    ? string.Join(", ", question.MatchPairs.Select(kvp => $"{kvp.Key} → {kvp.Value}"))
                    : "(немає)",

                _ => "(невідомо)"
            };
        }

    }
}

using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quizzamination.Models;
using Microsoft.Win32;
namespace Quizzamination.Views
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        private readonly List<AnswerResult> _results;
        public ResultsWindow(List<AnswerResult> results)
        {
            InitializeComponent();
            _results = results;
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

            var saveButton = new Button
            {
                Content = "Зберегти результати",
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            saveButton.Click += SaveButton_Click;

            panel.Children.Add(saveButton);

            Content = new ScrollViewer { Content = panel };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveResultsToFile(_results);
        }

        private void SaveResultsToFile(List<AnswerResult> results)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Зберегти результати у файл",
                FileName = $"results_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            string txtPath;
            string jsonPath;

            if (dialog.ShowDialog() == true)
            {
                txtPath = dialog.FileName;
                jsonPath = Path.ChangeExtension(txtPath, ".json");
            }
            else
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string defaultDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quizzamination");
                Directory.CreateDirectory(defaultDir);
                txtPath = Path.Combine(defaultDir, $"results_{timestamp}.txt");
                jsonPath = Path.Combine(defaultDir, $"results_{timestamp}.json");
            }

            SaveTxt(results, txtPath);
            SaveJson(results, jsonPath);

            MessageBox.Show($"Результати збережено:\n{txtPath}\n{jsonPath}", "Успішно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveTxt(List<AnswerResult> results, string path)
        {
            var lines = new List<string>();

            foreach (var result in results)
            {
                lines.Add($"Питання: {result.Question.Text}");
                lines.Add($"Ваша відповідь: {FormatAnswer(result.UserAnswer, result.Question.Type, result.Question)}");
                lines.Add($"Статус: {(result.IsCorrect ? "✅ Правильно" : "❌ Неправильно")}");
                if (!result.IsCorrect)
                {
                    lines.Add($"Правильна відповідь: {FormatCorrectAnswer(result.Question)}");
                }
                lines.Add(""); // порожній рядок
            }

            File.WriteAllLines(path, lines);
        }

        private void SaveJson(List<AnswerResult> results, string path)
        {
            var simplified = results.Select(r => new
            {
                Question = r.Question.Text,
                Answer = FormatAnswer(r.UserAnswer, r.Question.Type, r.Question),
                Correct = r.IsCorrect
            });

            var json = JsonSerializer.Serialize(simplified, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        private string FormatAnswer(object? answer, QuestionType type, Question question)
        {
            if (type == QuestionType.TrueFalse && answer is int tfVal && (tfVal == 0 || tfVal == 1))
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

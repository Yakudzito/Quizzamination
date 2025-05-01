using System.Windows;
using System.Windows.Controls;
using Quizzamination.Models;

namespace Quizzamination.Views.Controls
{
    /// <summary>
    /// Interaction logic for MatchingControl.xaml
    /// </summary>
    public partial class MatchingControl : UserControl
    {
        public Question Question { get; }
        private readonly Dictionary<string, ComboBox> _comboBoxes = new();

        public MatchingControl(Question question, Dictionary<string, string>? savedPairs = null)
        {
            InitializeComponent();
            Question = question;
            this.DataContext = question;
            LoadMatchingItems(savedPairs);
        }

        private void LoadMatchingItems(Dictionary<string, string>? savedPairs)
        {
            var panel = new StackPanel();
            if (Question.MatchPairs != null)
            {
                var values = Question.MatchPairs.Values.Distinct().ToList();

                foreach (var pair in Question.MatchPairs)
                {
                    var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 4) };
                    row.Children.Add(new TextBlock { Text = pair.Key, Width = 150 });

                    var combo = new ComboBox
                    {
                        Width = 150,
                        ItemsSource = values,
                        Tag = pair.Key
                    };

                    if (savedPairs != null && savedPairs.TryGetValue(pair.Key, out string? selected))
                    {
                        combo.SelectedItem = selected;
                    }

                    _comboBoxes[pair.Key] = combo;
                    row.Children.Add(combo);
                    panel.Children.Add(row);
                }
            }
            MatchList.Items.Clear();
            MatchList.Items.Add(panel);
        }

        public Dictionary<string, string> GetSelectedPairs()
        {
            var result = new Dictionary<string, string>();
            foreach (var pair in _comboBoxes)
            {
                if (pair.Value.SelectedItem is string selected)
                {
                    result[pair.Key] = selected;
                }
            }
            return result;
        }
    }
}


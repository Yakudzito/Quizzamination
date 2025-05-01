using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Quizzamination.Models;

namespace Quizzamination.Views
{
    /// <summary>
    /// Interaction logic for MatchingControl.xaml
    /// </summary>
    public partial class MatchingControl : UserControl
    {
        public Question Question { get; }
        private readonly Dictionary<string, ComboBox> _comboBoxes = new();

        public MatchingControl(Question question)
        {
            InitializeComponent();
            Question = question;
            this.DataContext = question;
            LoadMatchingItems();
        }

        private void LoadMatchingItems()
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


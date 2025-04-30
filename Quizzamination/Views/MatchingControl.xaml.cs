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
                foreach (var pair in Question.MatchPairs)
                {
                    var row = new StackPanel { Orientation = Orientation.Horizontal };
                    row.Children.Add(new TextBlock { Text = pair.Key, Width = 150 });
                    row.Children.Add(new ComboBox { Width = 150, ItemsSource = Question.MatchPairs.Values });
                    panel.Children.Add(row);
                }
            }
            MatchList.Items.Clear();
            MatchList.Items.Add(panel);
        }
    }
}


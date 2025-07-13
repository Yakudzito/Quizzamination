using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quizzamination.Models;

namespace Quizzamination.Views.Controls
{
    /// <summary>
    /// Interaction logic for MultipleChoiceControl.xaml
    /// </summary>
    public partial class MultipleChoiceControl : UserControl
    {
        public Question Question { get; }
        public MultipleChoiceControl(Question question, List<int>? savedAnswers = null)
        {
            InitializeComponent();
            Question = question;
            this.DataContext = question;
            LoadOptions(savedAnswers);
        }

        private void LoadOptions(List<int>? savedAnswers)
        {
            OptionsList.Children.Clear();
            for (int i = 0; i < Question.Options?.Count; i++)
            {
                var option = Question.Options[i];
                var checkbox = new CheckBox
                {
                    Content = option,
                    Tag = i,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                if (savedAnswers != null && savedAnswers.Contains(i))
                {
                    checkbox.IsChecked = true;
                }
                OptionsList.Children.Add(checkbox);
            }
        }

        public List<int> GetSelectedIndexes()
        {
            var selected = new List<int>();
            foreach (var child in OptionsList.Children)
                if (child is CheckBox {IsChecked: true} cb)
                    selected.Add((int)cb.Tag);
            return selected;
        }

        public void HighlightAnswer()
        {
            if (Question.CorrectAnswers == null || Question.CorrectAnswers.Count == 0)
                return;

            var correctIndexes = Question.CorrectAnswers;
            var chosenIndexes = GetSelectedIndexes();

            for (int i = 0; i < OptionsList.Children.Count; i++)
            {
                if (OptionsList.Children[i] is CheckBox { Tag: int index } cb)
                {
                    bool isCorrect = correctIndexes.Contains(index);
                    bool isChosen = chosenIndexes.Contains(index);

                    cb.Foreground = isCorrect switch
                    {
                        true => Brushes.Green,
                        false when isChosen => Brushes.Red,
                        _ => Brushes.Black
                    };
                }
            }
        }
    }
}

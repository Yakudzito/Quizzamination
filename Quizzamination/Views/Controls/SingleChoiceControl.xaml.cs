using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quizzamination.Models;

namespace Quizzamination.Views.Controls
{
    /// <summary>
    /// Interaction logic for SingleChoiceControl.xaml
    /// </summary>
    public partial class SingleChoiceControl : UserControl
    {
        private Question Question { get; }

        RadioButton CreateWrappedRadioButton(string text, object tag)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
            };

            return new RadioButton
            {
                Content = textBlock,
                Tag = tag,
                GroupName = "single",
                VerticalContentAlignment = VerticalAlignment.Center
            };
        }
        public SingleChoiceControl(Question question, int? savedIndex = null)
        {
            InitializeComponent();
            Question = question;
            this.DataContext = question;
            LoadOptions(savedIndex);
        }

        private void LoadOptions(int? savedIndex)
        {
            OptionsList.Children.Clear();
            for (int i = 0; i < Question.Options?.Count; i++)
            {
                var option = Question.Options[i];
                var radio = CreateWrappedRadioButton(option, i);
                if (savedIndex.HasValue && savedIndex.Value == i)
                {
                    radio.IsChecked = true;
                }
                OptionsList.Children.Add(radio);
            }
        }

        public int? GetSelectedIndex()
        {
            foreach (var child in OptionsList.Children)
                if (child is RadioButton rb && rb.IsChecked == true)
                    return (int?)rb.Tag;
            return null;
        }

        public void HighlightAnswer()
        {
            if (Question.CorrectAnswers == null || Question.CorrectAnswers.Count == 0)
                return;

            int correctIndex = Question.CorrectAnswers[0];
            int? selectedIndex = GetSelectedIndex();

            for (int i = 0; i < OptionsList.Children.Count; i++)
            {
                if (OptionsList.Children[i] is RadioButton rb)
                {
                    if ((int)rb.Tag == correctIndex) rb.Foreground = Brushes.Green;
                    else if (selectedIndex.HasValue && (int)rb.Tag == selectedIndex.Value &&
                        selectedIndex.Value != correctIndex)
                        rb.Foreground = Brushes.Red;
                    else rb.Foreground = Brushes.Black;
                }
            }
        }
    }
}

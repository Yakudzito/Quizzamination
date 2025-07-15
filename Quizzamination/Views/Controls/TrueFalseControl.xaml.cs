using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quizzamination.Models;

namespace Quizzamination.Views.Controls
{
    /// <summary>
    /// Interaction logic for TrueFalseControl.xaml
    /// </summary>
    public partial class TrueFalseControl : UserControl
    {
        public Question Question { get; }

        public TrueFalseControl(Question question, int? savedIndex = null)
        {
            InitializeComponent();
            Question = question;
            this.DataContext = question;
            LoadOptions(savedIndex);
        }

        private void LoadOptions(int? savedIndex)
        {
            OptionsPanel.Children.Clear();
            var trueRadio = new RadioButton
            {
                Content = "Правда", 
                Tag = 0, 
                GroupName = "tf",
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            var falseRadio = new RadioButton
            {
                Content = "Неправда", 
                Tag = 1, 
                GroupName = "tf",
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            if (savedIndex == 0) trueRadio.IsChecked = true;
            if (savedIndex == 1) falseRadio.IsChecked = true;

            OptionsPanel.Children.Add(trueRadio);
            OptionsPanel.Children.Add(falseRadio);
        }

        public int? GetSelectedIndex()
        {
            foreach (var child in OptionsPanel.Children)
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

            for (int i = 0; i < OptionsPanel.Children.Count; i++)
            {
                if (OptionsPanel.Children[i] is RadioButton rb)
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

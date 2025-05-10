using System.Windows;
using System.Windows.Controls;
using Quizzamination.Models;

namespace Quizzamination.Views.Controls
{
    /// <summary>
    /// Interaction logic for SingleChoiceControl.xaml
    /// </summary>
    public partial class SingleChoiceControl : UserControl
    {
        public Question Question { get; }

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
                var radio = new RadioButton
                {
                    Content = option, 
                    Tag = i, 
                    GroupName = "single",
                    VerticalContentAlignment = VerticalAlignment.Center
                };
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
    }
}

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

        public SingleChoiceControl(Question question)
        {
            InitializeComponent();
            Question = question;
            this.DataContext = question;
            LoadOptions();
        }

        private void LoadOptions()
        {
            OptionsList.Children.Clear();
            for (int i = 0; i < Question.Options?.Count; i++)
            {
                var option = Question.Options[i];
                var radio = new RadioButton { Content = option, Tag = i };
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

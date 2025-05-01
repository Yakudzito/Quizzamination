using System.Windows.Controls;
using Quizzamination.Models;

namespace Quizzamination.Views.Controls
{
    /// <summary>
    /// Interaction logic for MultipleChoiceControl.xaml
    /// </summary>
    public partial class MultipleChoiceControl : UserControl
    {
        public Question Question { get; }
        public MultipleChoiceControl(Question question)
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
                var checkbox = new CheckBox { Content = option, Tag = i };
                OptionsList.Children.Add(checkbox);
            }
        }
        public List<int> GetSelectedIndexes()
        {
            var selected = new List<int>();
            foreach (var child in OptionsList.Children)
                if (child is CheckBox cb && cb.IsChecked == true)
                    selected.Add((int)cb.Tag);
            return selected;
        }
    }
}

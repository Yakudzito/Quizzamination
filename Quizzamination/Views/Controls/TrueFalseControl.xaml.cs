using System.Windows.Controls;
using Quizzamination.Models;

namespace Quizzamination.Views.Controls
{
    /// <summary>
    /// Interaction logic for TrueFalseControl.xaml
    /// </summary>
    public partial class TrueFalseControl : UserControl
    {
        public Question Question { get; }

        public TrueFalseControl(Question question)
        {
            InitializeComponent();
            Question = question;
            this.DataContext = question;
            LoadOptions();
        }

        private void LoadOptions()
        {
            OptionsPanel.Children.Clear();
            var trueRadio = new RadioButton { Content = "Правда", Tag = 0 };
            var falseRadio = new RadioButton { Content = "Неправда", Tag = 1 };

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
    }
}

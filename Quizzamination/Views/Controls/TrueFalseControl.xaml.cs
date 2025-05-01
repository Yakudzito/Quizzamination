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
            var trueRadio = new RadioButton { Content = "Правда", Tag = 0, GroupName = "tf" };
            var falseRadio = new RadioButton { Content = "Неправда", Tag = 1, GroupName = "tf" };

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
    }

}

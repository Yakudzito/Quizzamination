using System;
using System.Collections.Generic;
using System.Data;
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

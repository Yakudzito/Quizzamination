using Quizzamination.Models;
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

namespace Quizzamination.Views
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
    }
}

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
    }
}

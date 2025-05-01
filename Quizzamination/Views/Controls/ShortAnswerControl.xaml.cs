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
    /// Interaction logic for ShortAnswerControl.xaml
    /// </summary>
    public partial class ShortAnswerControl : UserControl
    {
        public Question Question { get; }

        public ShortAnswerControl(Question question)
        {
            InitializeComponent();
            Question = question;
            this.DataContext = question;
        }

        public string GetAnswerText() => AnswerBox.Text.Trim();
    }
}

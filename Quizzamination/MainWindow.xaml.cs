using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Quizzamination.Models;
using Quizzamination.Services;
using Quizzamination.Views;

namespace Quizzamination
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Question> _questions;
        private int _currentIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            _questions = TestLoader.LoadFromFile("test1.json");
            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            var question = _questions[_currentIndex];
            UserControl control = question.Type switch
            {
                QuestionType.SingleChoice => new SingleChoiceControl(question),
                QuestionType.TrueFalse => new TrueFalseControl(question),
                QuestionType.MultipleChoice => new MultipleChoiceControl(question),
                QuestionType.Matching => new MatchingControl(question),
                QuestionType.ShortAnswer => new ShortAnswerControl(question),
                _ => new UserControl { Content = new TextBlock { Text = "Цей тип питання ще не реалізований" } }
            };

            QuestionHost.Content = control;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex < _questions.Count - 1)
            {
                _currentIndex++;
                ShowCurrentQuestion();
            }
        }
    }
}
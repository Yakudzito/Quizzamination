using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels.Questions;

public class UnsupportedQuestionViewModel : IQuestionSessionViewModel
{
    public Question Question { get; }
    public string Text => Question.Text;
    public object? UserAnswer => null;

    public UnsupportedQuestionViewModel(Question q) => Question = q;
}
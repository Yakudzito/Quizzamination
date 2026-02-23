using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels.Questions;

public interface IQuestionSessionViewModel
{
    Question Question { get; }
    string Text { get; }
    object? UserAnswer { get; }
}
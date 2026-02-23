using CommunityToolkit.Mvvm.ComponentModel;
using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels.Questions;

public partial class ShortAnswerQuestionViewModel : ObservableObject, IQuestionSessionViewModel
{
    public Question Question { get; }
    public string Text => Question.Text;

    [ObservableProperty] private string? answer;

    public object? UserAnswer => Answer;

    public ShortAnswerQuestionViewModel(Question q) => Question = q;
}
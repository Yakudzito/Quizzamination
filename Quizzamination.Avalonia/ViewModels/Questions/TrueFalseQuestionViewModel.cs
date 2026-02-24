using CommunityToolkit.Mvvm.ComponentModel;
using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels.Questions;

public partial class TrueFalseQuestionViewModel : ObservableObject, IQuestionSessionViewModel
{
    public Question Question { get; }
    public string Text => Question.Text;

    // -1 = not selected, 0 = False, 1 = True
    [ObservableProperty] private int selected = -1;

    public object? UserAnswer => Selected;

    public TrueFalseQuestionViewModel(Question q) => Question = q;
}
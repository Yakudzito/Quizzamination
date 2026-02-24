using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels.Questions;

public partial class SingleChoiceQuestionViewModel : ObservableObject, IQuestionSessionViewModel
{
    public Question Question { get; }
    public string Text => Question.Text;

    public IReadOnlyList<string> Options { get; }

    [ObservableProperty] private int selectedIndex = -1;

    public object? UserAnswer => SelectedIndex;

    public SingleChoiceQuestionViewModel(Question q)
    {
        Question = q;
        Options = q.Options ?? new List<string>();
    }
}
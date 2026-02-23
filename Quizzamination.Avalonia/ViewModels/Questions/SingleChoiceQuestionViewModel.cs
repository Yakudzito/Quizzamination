using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels.Questions;

public partial class SingleChoiceQuestionViewModel : ObservableObject, IQuestionSessionViewModel
{
    public Question Question { get; }
    public string Text => Question.Text;
    public IReadOnlyList<string> Options =>
        (IReadOnlyList<string>?)Question.Options ?? Array.Empty<string>();

    [ObservableProperty] private int selectedIndex = -1;

    public object? UserAnswer => SelectedIndex;

    public SingleChoiceQuestionViewModel(Question q) => Question = q;
}
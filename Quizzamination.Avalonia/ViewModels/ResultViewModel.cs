using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels;

public partial class ResultsViewModel : ObservableObject
{
    public IReadOnlyList<AnswerResult> Results { get; }
    public int Total => Results.Count;
    public int Correct => Results.Count(r => r.IsCorrect);

    public event Action? OnBack;

    public ResultsViewModel(IReadOnlyList<AnswerResult> results) => Results = results;

    [RelayCommand]
    private void Back() => OnBack?.Invoke();
}
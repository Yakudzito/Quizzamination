using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels.Questions;

public class MatchingPair : ObservableObject
{
    public string Left { get; }
    public IReadOnlyList<string> RightOptions { get; }

    private string? _selectedRight;
    public string? SelectedRight
    {
        get => _selectedRight;
        set => SetProperty(ref _selectedRight, value);
    }

    public MatchingPair(string left, IReadOnlyList<string> rightOptions)
    {
        Left = left;
        RightOptions = rightOptions;
    }
}

public partial class MatchingQuestionViewModel : ObservableObject, IQuestionSessionViewModel
{
    public Question Question { get; }
    public string Text => Question.Text;

    public ObservableCollection<MatchingPair> Pairs { get; }

    public object? UserAnswer => Pairs.ToDictionary(p => p.Left, p => p.SelectedRight ?? "");

    public MatchingQuestionViewModel(Question q)
    {
        Question = q;

        var pairs = q.MatchPairs ?? new Dictionary<string, string>();
        var rights = pairs.Values.Distinct().ToList();

        Pairs = new ObservableCollection<MatchingPair>(
            pairs.Keys.Select(k => new MatchingPair(k, rights))
        );
    }
}
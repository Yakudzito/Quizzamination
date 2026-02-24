using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels.Questions;

public partial class MatchingItemViewModel : ObservableObject
{
    public string Left { get; }
    public IReadOnlyList<string> RightOptions { get; }

    [ObservableProperty] private string? selectedRight;

    public MatchingItemViewModel(string left, IReadOnlyList<string> rightOptions)
    {
        Left = left;
        RightOptions = rightOptions;
    }
}

public partial class MatchingQuestionViewModel : ObservableObject, IQuestionSessionViewModel
{
    public Question Question { get; }
    public string Text => Question.Text;

    public ObservableCollection<MatchingItemViewModel> Items { get; }

    public IReadOnlyList<string> RightOptions { get; }

    public object? UserAnswer =>
        Items.ToDictionary(i => i.Left, i => i.SelectedRight);

    public MatchingQuestionViewModel(Question q)
    {
        Question = q;

        var pairs = q.MatchPairs ?? new Dictionary<string, string>();

        var shuffled = pairs.Values.ToList();
        Shuffle(shuffled);
        var rights = shuffled.AsReadOnly();

        Items = new ObservableCollection<MatchingItemViewModel>(
            pairs.Keys.Select(k => new MatchingItemViewModel(k, rights))
        );
    }

    private static void Shuffle<T>(IList<T> list)
    {
        var rng = new Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
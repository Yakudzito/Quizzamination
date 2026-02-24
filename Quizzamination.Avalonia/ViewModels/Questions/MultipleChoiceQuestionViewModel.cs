using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels.Questions;

public partial class ChoiceItemViewModel : ObservableObject
{
    public int Index { get; }
    public string Text { get; }

    [ObservableProperty] private bool isChecked;

    public ChoiceItemViewModel(int index, string text)
    {
        Index = index;
        Text = text;
    }
}

public partial class MultipleChoiceQuestionViewModel : ObservableObject, IQuestionSessionViewModel
{
    public Question Question { get; }
    public string Text => Question.Text;

    public ObservableCollection<ChoiceItemViewModel> Items { get; }

    public object? UserAnswer =>
        Items.Where(i => i.IsChecked).Select(i => i.Index).ToList();

    public MultipleChoiceQuestionViewModel(Question q)
    {
        Question = q;

        var opts = q.Options ?? new List<string>();
        Items = new ObservableCollection<ChoiceItemViewModel>(
            opts.Select((t, i) => new ChoiceItemViewModel(i, t))
        );
    }
}
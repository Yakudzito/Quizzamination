using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzamination.Avalonia.ViewModels.Questions;
using Quizzamination.Models;
using Quizzamination.Services;

namespace Quizzamination.Avalonia.ViewModels;

public partial class RunnerViewModel : ObservableObject
{
    private readonly List<IQuestionSessionViewModel> _sessions;

    [ObservableProperty] private int index;
    [ObservableProperty] private string status = "";

    public int Total => _sessions.Count;
    public IQuestionSessionViewModel Current => _sessions[Index];

    public bool CanPrev => Index > 0;
    public bool CanNext => Index < Total - 1;

    public event Action<IReadOnlyList<AnswerResult>>? OnFinished;

    public RunnerViewModel(IReadOnlyList<Question> questions)
    {
        _sessions = questions.Select(Wrap).ToList();
        Index = 0;
        UpdateStatus();
    }

    partial void OnIndexChanged(int value)
    {
        OnPropertyChanged(nameof(Current));
        OnPropertyChanged(nameof(CanPrev));
        OnPropertyChanged(nameof(CanNext));
        PrevCommand.NotifyCanExecuteChanged();
        NextCommand.NotifyCanExecuteChanged();
        UpdateStatus();
    }

    private void UpdateStatus() => Status = $"Питання {Index + 1} з {Total}";

    private static IQuestionSessionViewModel Wrap(Question q) => q.Type switch
    {
        QuestionType.SingleChoice => new SingleChoiceQuestionViewModel(q),
        QuestionType.ShortAnswer => new ShortAnswerQuestionViewModel(q),
        _ => new UnsupportedQuestionViewModel(q)
    };

    [RelayCommand(CanExecute = nameof(CanPrev))]
    private void Prev() => Index--;

    [RelayCommand(CanExecute = nameof(CanNext))]
    private void Next() => Index++;

    [RelayCommand]
    private void Finish()
    {
        var results = _sessions
            .Select(s => new AnswerResult(s.Question, s.UserAnswer, AnswerChecker.IsCorrect(s.Question, s.UserAnswer)))
            .ToList();

        OnFinished?.Invoke(results);
    }
}
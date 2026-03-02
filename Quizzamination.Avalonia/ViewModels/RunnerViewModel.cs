using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    [ObservableProperty] private bool isAnswerRevealed;
    [ObservableProperty] private string revealedAnswerText = "";

    public int Total => _sessions.Count;
    public IQuestionSessionViewModel Current => _sessions[Index];
    public bool LearningModeEnabled { get; }

    public bool CanPrev => Index > 0;
    public bool CanNext => Index < Total - 1;
    public bool CanShowAnswer => LearningModeEnabled;

    public event Action<IReadOnlyList<AnswerResult>>? OnFinished;

    public RunnerViewModel(IReadOnlyList<Question> questions, TestStartOptions? options = null)
    {
        _sessions = questions.Select(Wrap).ToList();
        LearningModeEnabled = options?.LearningModeEnabled ?? false;
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
        ResetRevealedAnswer();
        UpdateStatus();
    }

    partial void OnIsAnswerRevealedChanged(bool value) => ShowAnswerCommand.NotifyCanExecuteChanged();

    private void UpdateStatus() => Status = $"Питання {Index + 1} з {Total}";

    private void ResetRevealedAnswer()
    {
        IsAnswerRevealed = false;
        RevealedAnswerText = string.Empty;
    }

    private static IQuestionSessionViewModel Wrap(Question q) => q.Type switch
    {
        QuestionType.SingleChoice => new SingleChoiceQuestionViewModel(q),
        QuestionType.ShortAnswer => new ShortAnswerQuestionViewModel(q),
        QuestionType.TrueFalse => new TrueFalseQuestionViewModel(q),
        QuestionType.MultipleChoice => new MultipleChoiceQuestionViewModel(q),
        QuestionType.Matching => new MatchingQuestionViewModel(q),
        _ => new UnsupportedQuestionViewModel(q)
    };

    [RelayCommand(CanExecute = nameof(CanPrev))]
    private void Prev() => Index--;

    [RelayCommand(CanExecute = nameof(CanNext))]
    private void Next() => Index++;

    [RelayCommand(CanExecute = nameof(CanShowAnswer))]
    private void ShowAnswer()
    {
        RevealedAnswerText = FormatCorrectAnswer(Current.Question);
        IsAnswerRevealed = true;
    }

    [RelayCommand]
    private void Finish()
    {
        var results = _sessions
            .Select(s => new AnswerResult(s.Question, s.UserAnswer, AnswerChecker.IsCorrect(s.Question, s.UserAnswer)))
            .ToList();

        OnFinished?.Invoke(results);
    }

    private static string FormatCorrectAnswer(Question q) => q.Type switch
    {
        QuestionType.SingleChoice => FormatSingleChoice(q),
        QuestionType.MultipleChoice => FormatMultipleChoice(q),
        QuestionType.TrueFalse => FormatTrueFalse(q),
        QuestionType.ShortAnswer => string.IsNullOrWhiteSpace(q.CorrectShortAnswer)
            ? "Немає даних правильної відповіді."
            : q.CorrectShortAnswer,
        QuestionType.Matching => FormatMatching(q),
        _ => "Підказка для цього типу питання недоступна."
    };

    private static string FormatSingleChoice(Question q)
    {
        var index = q.CorrectAnswers?.FirstOrDefault() ?? -1;
        if (index < 0 || q.Options is null || index >= q.Options.Count)
            return "Немає даних правильної відповіді.";

        return $"{index + 1}. {q.Options[index]}";
    }

    private static string FormatMultipleChoice(Question q)
    {
        if (q.CorrectAnswers is null || q.Options is null)
            return "Немає даних правильної відповіді.";

        var valid = q.CorrectAnswers
            .Distinct()
            .Where(i => i >= 0 && i < q.Options.Count)
            .OrderBy(i => i)
            .ToList();

        if (valid.Count == 0)
            return "Немає даних правильної відповіді.";

        return string.Join(Environment.NewLine, valid.Select(i => $"{i + 1}. {q.Options[i]}"));
    }

    private static string FormatTrueFalse(Question q)
    {
        var answer = q.CorrectAnswers?.FirstOrDefault() ?? 0;
        return answer == 1 ? "True" : "False";
    }

    private static string FormatMatching(Question q)
    {
        if (q.MatchPairs is null || q.MatchPairs.Count == 0)
            return "Немає даних правильної відповіді.";

        var sb = new StringBuilder();
        foreach (var (left, right) in q.MatchPairs)
        {
            if (sb.Length > 0)
                sb.AppendLine();
            sb.Append(left).Append(" → ").Append(right);
        }

        return sb.ToString();
    }
}

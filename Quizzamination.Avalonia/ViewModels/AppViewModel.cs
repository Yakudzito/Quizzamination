using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Quizzamination.Models;

namespace Quizzamination.Avalonia.ViewModels;

public partial class AppViewModel : ObservableObject
{
    [ObservableProperty] private object? current;

    public LoadViewModel Load { get; }
    public RunnerViewModel? Runner { get; private set; }
    public ResultsViewModel? Results { get; private set; }

    public AppViewModel()
    {
        Load = new LoadViewModel();
        Load.OnStart += StartTest;
        Current = Load;
    }

    private void StartTest(IReadOnlyList<Question> questions, TestStartOptions options)
    {
        Runner = new RunnerViewModel(questions, options);
        Runner.OnFinished += ShowResults;
        Current = Runner;
    }

    private void ShowResults(IReadOnlyList<AnswerResult> results)
    {
        Results = new ResultsViewModel(results);
        Results.OnBack += () => Current = Load;
        Current = Results;
    }
}

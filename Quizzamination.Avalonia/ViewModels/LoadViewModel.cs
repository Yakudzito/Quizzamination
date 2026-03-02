using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzamination.Models;
using Quizzamination.Services;
using System.Collections.Generic;
using System.Linq;

namespace Quizzamination.Avalonia.ViewModels;

public partial class LoadViewModel : ObservableObject
{
    [ObservableProperty] private string status = "Обери файл тесту (.json/.txt), завантаж і натисни Почати.";
    [ObservableProperty] private string? selectedFilePath;
    [ObservableProperty] private bool shuffleQuestions = true;
    [ObservableProperty] private bool learningModeEnabled;
    
    public ObservableCollection<Question> Questions { get; } = new();

    public event Action<IReadOnlyList<Question>, TestStartOptions>? OnStart;
    
    [RelayCommand]
    private async Task LoadTestAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedFilePath))
        {
            Status = "Файл не обрано.";
            return;
        }
        if (!File.Exists(SelectedFilePath))
        {
            Status = "Файл не знайдено.";
            return;
        }

        try
        {
            Status = "Завантаження...";
            var loaded = await Task.Run(() => TestLoader.LoadFromFile(SelectedFilePath));

            Questions.Clear();
            foreach (var q in loaded)
                Questions.Add(q);

            Status = $"Завантажено: {Questions.Count} питань.";
        }
        catch (Exception ex)
        {
            Status = $"Помилка: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Start()
    {
        if (Questions.Count == 0)
        {
            Status = "Немає питань. Спочатку завантаж тест.";
            return;
        }
        
        var list = Questions.ToList();

        if (ShuffleQuestions)
            Shuffle(list);

        var options = new TestStartOptions
        {
            LearningModeEnabled = LearningModeEnabled
        };

        OnStart?.Invoke(list, options);
    }

    private static void Shuffle<T>(IList<T> list)
    {
        var rng = Random.Shared;
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

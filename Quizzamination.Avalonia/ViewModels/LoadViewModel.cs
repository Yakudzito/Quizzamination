using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzamination.Models;
using Quizzamination.Services;
using System.Collections.Generic;

namespace Quizzamination.Avalonia.ViewModels;

public partial class LoadViewModel : ObservableObject
{
    [ObservableProperty] private string status = "Обери файл тесту (.json/.txt), завантаж і натисни Почати.";
    [ObservableProperty] private string? selectedFilePath;

    public ObservableCollection<Question> Questions { get; } = new();

    public event Action<IReadOnlyList<Question>>? OnStart;

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

        OnStart?.Invoke(Questions);
    }
}
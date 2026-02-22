using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzamination.Models;
using Quizzamination.Services;

namespace Quizzamination.Avalonia.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private string status = "Вкажи шлях до файлу тесту (.json або .txt) і натисни Завантажити.";
    [ObservableProperty] private string? selectedFilePath;

    public ObservableCollection<Question> Questions { get; } = new();

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
}

using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Quizzamination.Avalonia.ViewModels;

namespace Quizzamination.Avalonia.Views;

public partial class LoadView : UserControl
{
    public LoadView()
    {
        InitializeComponent();
    }

    private async void Browse_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is null)
            return;

        var options = new FilePickerOpenOptions
        {
            Title = "Обери файл тесту",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Quizzamination tests") { Patterns = ["*.json", "*.txt"] },
                FilePickerFileTypes.All
            ]
        };

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);
        var file = files.FirstOrDefault();
        if (file is null)
            return;

        var path = file.Path.LocalPath;
        if (DataContext is LoadViewModel vm)
            vm.SelectedFilePath = path;
    }
}
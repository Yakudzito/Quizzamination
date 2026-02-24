using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Quizzamination.Avalonia.Converters;

public sealed class IntToBoolConverter : IValueConverter
{
    public static IntToBoolConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int i || parameter is null)
            return false;

        if (!int.TryParse(parameter.ToString(), out var p))
            return false;

        return i == p;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b && parameter is not null &&
            int.TryParse(parameter.ToString(), out var p))
        {
            return p;
        }

        return -1; // нічого не вибрано
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Controls.Templates;
using Avalonia.Data.Converters;

namespace RemoteMouse.Converters;

public class DataLoaderConverter : IValueConverter
{
    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public required IDataTemplate OnLoading { get; set; }

    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public required IDataTemplate OnData { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null or true ? OnLoading : OnData;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
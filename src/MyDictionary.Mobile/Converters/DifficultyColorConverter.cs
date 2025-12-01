using System.Globalization;
using MyDictionary.Core.Entities;

namespace MyDictionary.Mobile.Converters;

public class DifficultyColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => Color.FromArgb("#10B981"),    // Green
                Difficulty.Medium => Color.FromArgb("#F59E0B"),  // Orange
                Difficulty.Hard => Color.FromArgb("#EF4444"),    // Red
                _ => Color.FromArgb("#6B7280")                   // Gray
            };
        }
        
        return Color.FromArgb("#6B7280");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

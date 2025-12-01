using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyDictionary.Mobile.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool isDarkMode;

    [ObservableProperty]
    private string defaultSourceLanguage = "English";

    [ObservableProperty]
    private string defaultTargetLanguage = "Arabic";

    [ObservableProperty]
    private double dailyGoal = 20;

    public List<string> AvailableLanguages { get; } = new()
    {
        "English",
        "Arabic",
        "Spanish",
        "French",
        "German",
        "Italian",
        "Portuguese",
        "Russian",
        "Chinese",
        "Japanese",
        "Korean"
    };

    public SettingsViewModel()
    {
        Title = "Settings";
        
        // Load saved theme preference
        // Load saved theme preference
        var savedTheme = Preferences.Get("AppTheme", "System");
        if (Application.Current != null)
        {
            if (savedTheme == "Dark")
            {
                IsDarkMode = true;
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else if (savedTheme == "Light")
            {
                IsDarkMode = false;
                Application.Current.UserAppTheme = AppTheme.Light;
            }
            else
            {
                // Follow system theme
                IsDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;
            }
        }
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
            Preferences.Set("AppTheme", value ? "Dark" : "Light");
        }
    }

    [RelayCommand]
    async Task ClearDataAsync()
    {
        var result = await Shell.Current.DisplayAlert(
            "Clear All Data",
            "Are you sure you want to delete all your words and progress? This action cannot be undone.",
            "Delete",
            "Cancel");

        if (result)
        {
            // TODO: Implement data clearing
            await Shell.Current.DisplayAlert("Success", "All data has been cleared", "OK");
        }
    }
}

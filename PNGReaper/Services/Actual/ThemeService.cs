using System;
using System.Windows;
using ControlzEx.Theming;
using MahApps.Metro.Theming;
using PNGReaper.Services.Abstract;

namespace PNGReaper.Services.Actual;

internal class ThemeService : IThemeService
{
    private const    string          HcDarkTheme  = "pack://application:,,,/Styles/Themes/HC.Dark.Blue.xaml";
    private const    string          HcLightTheme = "pack://application:,,,/Styles/Themes/HC.Light.Blue.xaml";
    private readonly IPersistService _persistService;

    public ThemeService(IPersistService persistService)
    {
        _persistService = persistService;
    }

    public void InitializeTheme()
    {
        ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcDarkTheme),
            MahAppsLibraryThemeProvider.DefaultInstance));
        ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcLightTheme),
            MahAppsLibraryThemeProvider.DefaultInstance));

        var theme = GetCurrentTheme();
        SetTheme(theme);
    }

    public void SetTheme(AppTheme theme)
    {
        if (theme == AppTheme.Default)
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();
        }
        else
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithHighContrast;
            ThemeManager.Current.SyncTheme();
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.Blue", SystemParameters.HighContrast);
        }

        _persistService.Theme = theme;
    }

    public AppTheme GetCurrentTheme()
    {
        return _persistService.Theme;
    }
}
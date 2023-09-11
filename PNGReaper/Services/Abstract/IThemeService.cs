namespace PNGReaper.Services.Abstract;

public enum AppTheme
{
    Default,
    Light,
    Dark
}

public interface IThemeService
{
    void InitializeTheme();
    void SetTheme(AppTheme theme);
}
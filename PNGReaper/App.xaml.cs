using System.Windows;
using PNGReaper.Helpers;
using PNGReaper.Services.Abstract;
using PNGReaper.Services.Actual;
using PNGReaper.ViewModels;
using PNGReaper.Views;
using Prism.Events;
using Prism.Ioc;

namespace PNGReaper;

public partial class App
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ShellView, ShellViewModel>();

        containerRegistry.RegisterSingleton<IFileService, FileService>();
        containerRegistry.RegisterSingleton<IPersistService, PersistService>();
        containerRegistry.RegisterSingleton<IThemeService, ThemeService>();
        containerRegistry.RegisterSingleton<IPngParseService, PngParseService>();
        containerRegistry.RegisterSingleton<IWindowPositionService, WindowPositionService>();
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<ShellView>();
    }

    protected override void OnInitialized()
    {
        var themeHandler = Container.Resolve<IThemeService>();
        if (themeHandler is not null)
        {
            themeHandler.InitializeTheme();
            themeHandler.SetTheme(AppTheme.Default);
        }

        base.OnInitialized();
    }

    protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
        var agg = Container.Resolve<IEventAggregator>();
        agg.GetEvent<ExitMessageEvent>().Publish(new ExitMessage());
        
        var persist = Container.Resolve<IPersistService>();
        persist.Save();
        base.OnSessionEnding(e);
    }
}
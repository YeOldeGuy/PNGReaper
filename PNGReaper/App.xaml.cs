using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using PNGReaper.Helpers;
using PNGReaper.Services.Abstract;
using PNGReaper.Services.Actual;
using PNGReaper.ViewModels;
using PNGReaper.Views;
using Prism.Ioc;

namespace PNGReaper;

public partial class App
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IFileService, FileService>();
        containerRegistry.RegisterSingleton<IPersistService, PersistService>();
        containerRegistry.RegisterSingleton<IThemeService, ThemeService>();
        containerRegistry.RegisterSingleton<IPngParseService, PngParseService>();
        
        containerRegistry.RegisterForNavigation<ShellView, ShellViewModel>();

        var configuration = BuildConfiguration();
        var appConfig = configuration
                        .GetSection(nameof(AppConfig))
                        .Get<AppConfig>();

        containerRegistry.RegisterInstance(configuration);
        containerRegistry.RegisterInstance(appConfig);
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<ShellView>();
    }

    private static IConfiguration BuildConfiguration()
    {
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
        return new ConfigurationBuilder()
               .SetBasePath(appLocation!)
               .AddJsonFile("appsettings.json", true)
               .AddCommandLine(Environment.GetCommandLineArgs())
               .Build();
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
}
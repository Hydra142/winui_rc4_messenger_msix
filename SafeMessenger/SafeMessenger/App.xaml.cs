using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SafeMessenge.Services;
using SafeMessenge.ViewModels;
using SafeMessenge.Views;
using System;
using WinUIEx;

namespace SafeMessenge;

public sealed partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Ioc.Default.ConfigureServices(new ServiceCollection()
            .AddSingleton<NavigationService>()
            .AddSingleton<ApiService>()
            .AddSingleton<AppDataService>()

            .AddTransient<ShellPageViewModel>()
            .AddTransient<LoginPageViewModel>()
            .AddTransient<MessengerViewModel>()
            .AddTransient<ShellPage>()
            .AddTransient<LoginPage>()
            .AddTransient<MessengerPage>()
            .BuildServiceProvider());
    }

    public static T GetService<T>() where T : class
    {
        if (Ioc.Default.GetService<T>() is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }
        return service;
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        MainWindow.Content = Ioc.Default.GetService<ShellPage>();
        MainWindow.Activate();
    }

    public static WindowEx MainWindow { get; } = new MainWindow();
}

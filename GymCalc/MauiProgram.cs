using CommunityToolkit.Maui;
using GymCalc.Data;
using GymCalc.Pages;
using GymCalc.Services;
using GymCalc.ViewModels;
using Microsoft.Extensions.Logging;
using InputKit.Handlers;

namespace GymCalc;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureMauiHandlers(handlers => { handlers.AddInputKitHandlers(); });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        RegisterDependencyInjection(builder);

        return builder.Build();
    }

    private static void RegisterDependencyInjection(MauiAppBuilder builder)
    {
        builder.Services
            // Singleton pages
            .AddSingleton<CalculatorPage>()
            .AddSingleton<SettingsPage>()
            .AddSingleton<HtmlPage>()
            // Transient pages
            .AddTransient<ListPage>()
            .AddTransient<EditPage>()
            .AddTransient<DeletePage>()
            .AddTransient<ResetPage>()
            // Database and repositories
            .AddSingleton<Database>()
            .AddSingleton<BarRepository>()
            .AddSingleton<PlateRepository>()
            .AddSingleton<DumbbellRepository>()
            .AddSingleton<KettlebellRepository>()
            // ViewModels
            .AddSingleton<CalculatorViewModel>()
            .AddSingleton<ListViewModel>()
            .AddSingleton<DeleteViewModel>()
            // Services
            .AddSingleton<HtmlUpdaterService>()
            ;
    }
}

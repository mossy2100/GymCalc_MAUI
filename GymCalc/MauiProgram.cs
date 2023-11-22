using CommunityToolkit.Maui;
using GymCalc.Data;
using GymCalc.Pages;
using GymCalc.Services;
using GymCalc.ViewModels;
using InputKit.Handlers;
using Microsoft.Extensions.Logging;

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
            // Database and repositories
            .AddSingleton<Database>()
            .AddSingleton<BarRepository>()
            .AddSingleton<PlateRepository>()
            .AddSingleton<BarbellRepository>()
            .AddSingleton<DumbbellRepository>()
            .AddSingleton<KettlebellRepository>()
            // Pages
            .AddSingleton<CalculatorPage>()
            .AddTransient<DeletePage>()
            .AddTransient<EditPage>()
            .AddSingleton<HtmlPage>()
            .AddSingleton<ListPage>()
            .AddSingleton<ResetPage>()
            .AddSingleton<SettingsPage>()
            .AddSingleton<WeightsPage>()
            // ViewModels
            .AddSingleton<CalculatorViewModel>()
            .AddTransient<DeleteViewModel>()
            .AddTransient<EditViewModel>()
            .AddSingleton<HtmlViewModel>()
            .AddSingleton<ListViewModel>()
            .AddSingleton<ResetViewModel>()
            .AddSingleton<SettingsViewModel>()
            .AddSingleton<WeightsViewModel>()
            // Services
            .AddSingleton<HtmlUpdaterService>()
            ;
    }
}

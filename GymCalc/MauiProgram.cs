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
            // Database.
            .AddSingleton<Database>()
            // Repositories.
            .AddSingleton<BarRepository>()
            .AddSingleton<PlateRepository>()
            .AddSingleton<BarbellRepository>()
            .AddSingleton<DumbbellRepository>()
            .AddSingleton<KettlebellRepository>()
            // Singleton pages.
            .AddSingleton<CalculatorPage>()
            .AddSingleton<HtmlPage>()
            .AddSingleton<ListPage>()
            .AddSingleton<ResetPage>()
            .AddSingleton<SettingsPage>()
            .AddSingleton<WeightsPage>()
            // Transient pages. Pages must be transient if they have more than one parameter.
            .AddTransient<DeletePage>()
            .AddTransient<EditPage>()
            // ViewModels.
            .AddSingleton<CalculatorViewModel>()
            .AddSingleton<DeleteViewModel>()
            .AddSingleton<EditViewModel>()
            .AddSingleton<HtmlViewModel>()
            .AddSingleton<ListViewModel>()
            .AddSingleton<ResetViewModel>()
            .AddSingleton<SettingsViewModel>()
            .AddSingleton<WeightsViewModel>()
            // Services.
            .AddSingleton<HtmlUpdaterService>()
            ;
    }
}

using CommunityToolkit.Maui;
using GymCalc.Data;
using GymCalc.Pages;
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

        // BlazorWebView support.
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Logging.AddDebug();
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        RegisterDependencyInjection(builder);
        RegisterRoutes();

        return builder.Build();
    }

    private static void RegisterDependencyInjection(MauiAppBuilder builder)
    {
        builder.Services
            // Singleton pages
            .AddSingleton<CalculatorPage>()
            .AddSingleton<SettingsPage>()
            // Transient pages
            .AddTransient<ListPage>()
            .AddTransient<EditPage>()
            .AddTransient<DeletePage>()
            .AddTransient<ResetPage>()
            .AddTransient<HtmlPage>()
            // Database and repositories
            .AddSingleton<Database>()
            .AddSingleton<BarRepository>()
            .AddSingleton<PlateRepository>()
            .AddSingleton<DumbbellRepository>()
            .AddSingleton<KettlebellRepository>()
            // ViewModels
            .AddSingleton<CalculatorViewModel>()
            ;
    }

    private static void RegisterRoutes()
    {
        // Routing.RegisterRoute("login", typeof(LoginPage));
    }
}

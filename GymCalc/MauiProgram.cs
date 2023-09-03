using CommunityToolkit.Maui;
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
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddInputKitHandlers();
            })
            .Services
            // Singleton pages
            .AddSingleton<CalculatorPage>()
            .AddSingleton<AboutPage>()
            .AddSingleton<InstructionsPage>()
            .AddSingleton<SettingsPage>()
            // Transient pages
            .AddTransient<ListPage>()
            .AddTransient<EditPage>()
            .AddTransient<DeletePage>()
            .AddTransient<ResetPage>()
            // ViewModels
            .AddSingleton<CalculatorViewModel>()
            ;

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

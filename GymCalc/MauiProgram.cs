using CommunityToolkit.Maui;
using GymCalc.Data;
using GymCalc.Pages;
using GymCalc.Services;
using GymCalc.ViewModels;
using InputKit.Handlers;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

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
            });

        builder.Services.AddMauiBlazorWebView();

        ModifyEntry();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        RegisterDependencyInjection(builder);

        return builder.Build();
    }

    private static void ModifyEntry()
    {
        EntryHandler.Mapper.AppendToMapping("BorderlessEntry", (handler, view) =>
        {
#if ANDROID
            var nativeEntry = handler.PlatformView as Android.Widget.EditText;
            if (nativeEntry != null)
            {
                // Remove the border on Android.
                nativeEntry.Background = null; // This removes the underline
                nativeEntry.SetPadding(0, nativeEntry.PaddingTop, 0, nativeEntry.PaddingBottom);
            }
#endif
        });
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
            .AddSingleton<SettingsPage>()
            .AddSingleton<WeightsPage>()
            // Transient pages. Pages must be transient if they have more than one parameter.
            .AddTransient<EditPage>()
            // ViewModels.
            .AddSingleton<CalculatorViewModel>()
            .AddSingleton<EditViewModel>()
            .AddSingleton<HtmlViewModel>()
            .AddSingleton<ListViewModel>()
            .AddSingleton<SettingsViewModel>()
            .AddSingleton<WeightsViewModel>()
            // Services.
            .AddSingleton<HtmlUpdaterService>()
            ;
    }
}

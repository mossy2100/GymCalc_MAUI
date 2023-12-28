using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Maui;
using InputKit.Handlers;
using GymCalc.Pages;
using GymCalc.Repositories;
using GymCalc.Services;
using GymCalc.ViewModels;

namespace GymCalc;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
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

        RemoveUnderlineFromEntryControls();
        RemoveUnderlineFromPickerControls();

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
            .AddSingleton<ResultsPage>()
            .AddSingleton<ListPage>()
            .AddSingleton<SettingsPage>()
            .AddSingleton<WeightsPage>()
            // Transient pages.
            // It seems pages must be transient if they have more than one parameter.
            .AddTransient<EditPage>()
            .AddTransient<HtmlPage>()
            // ViewModels.
            .AddSingleton<CalculatorViewModel>()
            .AddSingleton<ResultsViewModel>()
            .AddSingleton<ListViewModel>()
            .AddSingleton<SettingsViewModel>()
            .AddSingleton<WeightsViewModel>()
            .AddSingleton<EditViewModel>()
            .AddSingleton<HtmlViewModel>()
            // Services.
            .AddSingleton<CalculatorService>()
            .AddSingleton<HtmlUpdaterService>()
            ;
    }

    private static void RemoveUnderlineFromEntryControls()
    {
        EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
#if ANDROID
            if (handler.PlatformView is Android.Widget.EditText nativeEntry)
            {
                nativeEntry.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            }
#endif
        });
    }

    private static void RemoveUnderlineFromPickerControls()
    {
        PickerHandler.Mapper.AppendToMapping(nameof(Picker), (handler, view) =>
        {
#if ANDROID
            if (handler.PlatformView is Android.Widget.EditText nativePicker)
            {
                nativePicker.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            }
#endif
        });
    }
}

using CommunityToolkit.Maui.Markup;
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
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddInputKitHandlers();
            })
            .UseMauiCommunityToolkitMarkup();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

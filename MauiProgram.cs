using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using myYSTU.Parsers;
using myYSTU.Utils;
using The49.Maui.BottomSheet;

namespace myYSTU
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseBottomSheet()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            DependencyService.Register<INetUtils, NetUtils>();
            DependencyService.RegisterSingleton(new ParseManager());

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

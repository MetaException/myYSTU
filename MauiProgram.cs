using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using myYSTU.Model;
using myYSTU.Utils;
using NLog;
using NLog.Extensions.Logging;
using The49.Maui.BottomSheet;

namespace myYSTU
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var logger = NLog.LogManager.Setup().RegisterMauiLog()
                             .LoadConfiguration(c => c.ForLogger().FilterMinLevel(NLog.LogLevel.Debug).WriteToMauiLog())
                             .GetCurrentClassLogger();

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

            HttpClientHandler handler = new HttpClientHandler() {AllowAutoRedirect = false};

            var client = new HttpClient(handler) { BaseAddress = new Uri(Links.BaseUri) };

            DependencyService.RegisterSingleton<NetUtils>(new NetUtils(handler, client));
            DependencyService.RegisterSingleton(logger);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Add NLog for Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog();

            return builder.Build();
        }
    }
}

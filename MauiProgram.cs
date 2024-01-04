using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using myYSTU.Model;
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

            HttpClientHandler handler = new HttpClientHandler() {AllowAutoRedirect = false};

            var client = new HttpClient(handler) { BaseAddress = new Uri(Links.BaseUri) };

            DependencyService.RegisterSingleton<NetUtils>(new NetUtils(handler, client));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

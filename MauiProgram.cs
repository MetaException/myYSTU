using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using myYSTU.Models;
using myYSTU.Parsers;
using myYSTU.Utils;
using myYSTU.ViewModels;
using myYSTU.Views;
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

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Add NLog for Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog();

            builder.Services.AddSingleton<NetUtils>();
            builder.Services.AddSingleton<NLog.ILogger>(logger);

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPageViewModel>();

            builder.Services.AddTransient<AuthPage>();
            builder.Services.AddTransient<AuthPageViewModel>();

            builder.Services.AddTransient<GradesPage>();
            builder.Services.AddTransient<GradesPageViewModel>();

            builder.Services.AddTransient<TimeTablePage>();
            builder.Services.AddTransient<TimeTablePageViewModel>();

            builder.Services.AddTransient<StaffPage>();
            builder.Services.AddTransient<StaffPageViewModel>();

            return builder.Build();
        }
    }
}

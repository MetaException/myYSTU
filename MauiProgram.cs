using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using myYSTU.Models;
using myYSTU.Parsers;
using myYSTU.Services.Auth;
using myYSTU.Services.Http;
using myYSTU.ViewModels;
using myYSTU.Views;
using Serilog;
using Serilog.Events;
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

#if DEBUG
            builder.Logging.AddDebug();
#endif
            var flushInterval = new TimeSpan(0, 0, 1);
            var file = Path.Combine(FileSystem.AppDataDirectory, "myYSTU.log");
 
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(file, flushToDiskInterval: flushInterval, encoding: System.Text.Encoding.UTF8, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 22)
                .WriteTo.Debug()
                .CreateLogger();

            builder.Logging.AddSerilog(dispose: true);

            var handler = new HttpClientHandler { AllowAutoRedirect = true };
            builder.Services.AddSingleton(handler);
            builder.Services.AddSingleton(sp => new HttpClient(handler) { BaseAddress = new Uri(Links.BaseUri) });

            builder.Services.AddScoped<IHttpService, HttpService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();

            builder.Services.AddTransient<LoadingPage>();

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

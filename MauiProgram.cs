using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using The49.Maui.BottomSheet;
using myYSTU.Utils;

namespace myYSTU;

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
#if ANDROID
		DependencyService.Register<INetUtils, NetUtilAndroid>();
#else
		DependencyService.Register<INetUtils, NetUtils>();
#endif

        builder.Services.AddLogging(configure =>
        {

            // You don't need the debug logger on Android if you use AndroidLoggerProvider.
            // configure.AddDebug();

#if ANDROID
#if DEBUG
    LogLevel androidLogLevel = LogLevel.Debug;
#else
    LogLevel androidLogLevel = LogLevel.Information;
#endif

    configure.AddProvider(new MyMauiApp.AndroidLoggerProvider())
                .AddFilter("MyMauiApp", androidLogLevel);
#else
#if DEBUG
    builder.Logging.AddDebug();
#endif
#endif

        });

        return builder.Build();
	}
}

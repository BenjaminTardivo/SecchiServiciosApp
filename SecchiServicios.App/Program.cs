using Avalonia;
using System;
using System.Globalization;
using QuestPDF.Infrastructure;
using SQLitePCL;

namespace SecchiServicios.App;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Batteries.Init();

        CultureInfo culture = new CultureInfo("es-AR");
        
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        QuestPDF.Settings.License = LicenseType.Community;
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    } 
        

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

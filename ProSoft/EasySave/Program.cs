using Avalonia;
using Avalonia.ReactiveUI;

using System;
using System.Threading;

using EasySave.Utils;
using EasySave.Models.Data;


namespace EasySave;

sealed class Program
{
    private static Mutex __mutex;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        __mutex = new Mutex(true, "EasySave", out bool oldInstance);

        if (!oldInstance) {
            Console.WriteLine("An instance of EasySave is already running.");
            Environment.Exit(-9);
        }
        LogUtils.Init();
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace AvaloniaApp
{
	class Program
	{
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static void Main(string[] args)
		{
			BuildAvaloniaApp()
			// workaround for https://github.com/AvaloniaUI/Avalonia/issues/3533
			.With(new AvaloniaNativePlatformOptions { UseGpu = false })
			.StartWithClassicDesktopLifetime(args);
		}


		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToDebug();
	}
}

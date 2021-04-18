using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CefNet;
using CefNet.CApi;
using CefNet.Unsafe;
using WinFormsCoreApp;

namespace AvaloniaApp
{
	class Program
	{
		private static CefAppImpl app;
		private static Timer messagePump;
		private const int messagePumpDelay = 10;

		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static unsafe void Main(string[] args)
		{

			string cefPath;
			bool externalMessagePump = args.Contains("--external-message-pump");

			if (PlatformInfo.IsMacOS)
			{
				externalMessagePump = true;
				cefPath = Path.Combine(GetProjectPath(), "Contents", "Frameworks", "Chromium Embedded Framework.framework");
			}
			else
			{
				cefPath = @"D:\Projects\Libs\CefNet\cef";
			}

			var settings = new CefSettings();
			settings.MultiThreadedMessageLoop = !externalMessagePump;
			settings.ExternalMessagePump = externalMessagePump;
			settings.NoSandbox = true;
			settings.WindowlessRenderingEnabled = true;
			settings.LocalesDirPath = Path.Combine(cefPath, "Resources", "locales");
			settings.ResourcesDirPath = Path.Combine(cefPath, "Resources");
			settings.LogSeverity = CefLogSeverity.Warning;
			settings.IgnoreCertificateErrors = true;
			settings.UncaughtExceptionStackSize = 8;

			App.FrameworkInitialized += App_FrameworkInitialized;
			App.FrameworkShutdown += App_FrameworkShutdown;

			app = new CefAppImpl();
			app.ScheduleMessagePumpWorkCallback = OnScheduleMessagePumpWork;
			app.CefProcessMessageReceived += AppProcessMessageReceived;
			app.Initialize(PlatformInfo.IsMacOS ? cefPath : Path.Combine(cefPath, "Release"), settings);

			BuildAvaloniaApp()
			// workaround for https://github.com/AvaloniaUI/Avalonia/issues/3533
			.With(new AvaloniaNativePlatformOptions { UseGpu = !PlatformInfo.IsMacOS })
			.StartWithCefNetApplicationLifetime(args);
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToTrace();


		private static unsafe void AppProcessMessageReceived(object sender, CefProcessMessageReceivedEventArgs e)
		{
			if (e.Name != "test")
				return;

			CefV8Context context = e.Frame.V8Context;
			context.Enter();
			try
			{
				string filename = PlatformInfo.IsMacOS ? "/Users/osx/work/CefNet/bin/log.txt" : @"G:\log.txt";
				
				var sb = new StringBuilder($"[{DateTime.UtcNow.ToString()}] Start Test\r\n");
				sb.Append("1 = ").Append(context.Eval("1", null).Type).AppendLine();
				sb.Append("'a' = ").Append(context.Eval("'a'", null).Type).AppendLine();
				sb.Append("null = ").Append(context.Eval("null", null).Type).AppendLine();
				sb.Append("true = ").Append(context.Eval("true", null).Type).AppendLine();
				sb.Append("2.2 = ").Append(context.Eval("2.2", null).Type).AppendLine();
				sb.Append("Date = ").Append(context.Eval("new Date()", null).Type).AppendLine();
				sb.Append("Object = ").Append(context.Eval("new Object()", null).Type).AppendLine();


				File.AppendAllText(filename, sb.ToString());

			}
			finally
			{
				context.Exit();
			}
		}
		private static int _mkv;

		private static void App_FrameworkInitialized(object sender, EventArgs e)
		{
			if (CefNetApplication.Instance.UsesExternalMessageLoop)
			{
				messagePump = new Timer(_ => Dispatcher.UIThread.Post(CefApi.DoMessageLoopWork), null, messagePumpDelay, messagePumpDelay);
			}
		}

		private static void App_FrameworkShutdown(object sender, EventArgs e)
		{
			messagePump?.Dispose();
		}

		private static async void OnScheduleMessagePumpWork(long delayMs)
		{
			await Task.Delay((int)delayMs);
			Dispatcher.UIThread.Post(CefApi.DoMessageLoopWork);
		}

		private static string GetProjectPath()
		{
			string projectPath = Path.GetDirectoryName(typeof(App).Assembly.Location);
			string projectName = PlatformInfo.IsMacOS ? "AvaloniaApp.app" : "AvaloniaApp";
			string rootPath = Path.GetPathRoot(projectPath);
			while (Path.GetFileName(projectPath) != projectName)
			{
				if (projectPath == rootPath)
					throw new DirectoryNotFoundException("Could not find the project directory.");
				projectPath = Path.GetDirectoryName(projectPath);
			}
			return projectPath;
		}
	}
}

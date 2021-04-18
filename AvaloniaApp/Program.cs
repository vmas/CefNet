using System;
using System.IO;
using System.Linq;
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
		public static void Main(string[] args)
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
				File.AppendAllText(filename, $"[{DateTime.UtcNow.ToString()}] Start Test\r\n");


				string code = "1";
				string scriptUrl = null;

				fixed (char* s0 = code)
				fixed (char* s1 = scriptUrl)
				{
					var cstr0 = new cef_string_t { Str = s0, Length = code.Length };
					var cstr1 = new cef_string_t { Str = s1, Length = scriptUrl != null ? scriptUrl.Length : 0 };


					cef_v8value_t* rv = null;
					cef_v8value_t** pRv = &rv;
					cef_v8exception_t* jsex = null;
					cef_v8exception_t** pJsex = &jsex;
					string ok = context.NativeInstance->Eval(&cstr0, &cstr1, 1, pRv, pJsex).ToString();
					File.AppendAllText(filename, ok + "\r\n");

					RefCountedWrapperStruct* ws = RefCountedWrapperStruct.FromRefCounted(rv);
					V8ValueImplLayout* cppobj = ((V8ValueImplLayout*)(ws->cppObject));
					File.AppendAllText(filename, cppobj->Type.ToString() + "\r\n");

				}
			}
			finally
			{
				context.Exit();
			}
		}

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

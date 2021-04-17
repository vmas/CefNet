using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	partial class CefAppGlue
	{
		internal bool AvoidGetCookieableSchemes()
		{
			return false;
		}

		public unsafe void GetCookieableSchemes(CefStringList schemes, ref int includeDefaults)
		{
			_application.GetCookieableSchemes(schemes, ref includeDefaults);
		}

		public void OnContextInitialized()
		{
			_application.OnContextInitialized();
		}

		internal bool AvoidOnBeforeChildProcessLaunch()
		{
			return false;
		}

		public void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
		{
			_application.OnBeforeChildProcessLaunch(new BeforeChildProcessLaunchEventArgs(commandLine));
		}

		internal bool AvoidOnScheduleMessagePumpWork()
		{
			return false;
		}

		public void OnScheduleMessagePumpWork(long delayMs)
		{
			_application.OnScheduleMessagePumpWork(delayMs);
		}

		public CefClient GetDefaultClient()
		{
			return _application.GetDefaultClient();
		}

	}
}

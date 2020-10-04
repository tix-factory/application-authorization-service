using TixFactory.Logging;
using TixFactory.Logging.Windows;

namespace TixFactory.ApplicationAuthorization.Service
{
	public class Startup : TixFactory.Http.Service.Startup
	{
		public Startup()
			: base(CreateLogger())
		{
		}

		private static ILogger CreateLogger()
		{
			var eventLogSettings = new WindowsEventLoggerSettings();
			eventLogSettings.LogName = eventLogSettings.LogSource = "TFAAS1.TixFactory.ApplicationAuthorization.Service";
			return new WindowsEventLogger(eventLogSettings);
		}
	}
}

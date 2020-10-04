using Microsoft.Extensions.DependencyInjection;
using TixFactory.ApplicationAuthorizations;
using TixFactory.Logging;
using TixFactory.Logging.Windows;

namespace TixFactory.ApplicationAuthorization.Service
{
	public class Startup : TixFactory.Http.Service.Startup
	{
		private readonly IApplicationAuthorizationsOperations _ApplicationAuthorizationsOperations;

		public Startup()
			: base(CreateLogger())
		{
			_ApplicationAuthorizationsOperations = new ApplicationAuthorizationsOperations(Logger);
		}

		public override void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient(s => _ApplicationAuthorizationsOperations);

			base.ConfigureServices(services);
		}

		private static ILogger CreateLogger()
		{
			var eventLogSettings = new WindowsEventLoggerSettings();
			eventLogSettings.LogName = eventLogSettings.LogSource = "TFAAS1.TixFactory.ApplicationAuthorization.Service";
			return new WindowsEventLogger(eventLogSettings);
		}
	}
}

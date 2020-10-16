using Microsoft.Extensions.DependencyInjection;
using TixFactory.Http.Client;
using TixFactory.Logging;
using TixFactory.Logging.Client;

namespace TixFactory.ApplicationAuthorization.Service
{
	public class Startup : TixFactory.Http.Service.Startup
	{
		private readonly IApplicationAuthorizationOperations _ApplicationAuthorizationsOperations;

		public Startup()
			: base(CreateLogger())
		{
			_ApplicationAuthorizationsOperations = new ApplicationAuthorizationOperations(Logger);
		}

		public override void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient(s => _ApplicationAuthorizationsOperations);

			base.ConfigureServices(services);
		}

		private static ILogger CreateLogger()
		{
			var httpClient = new HttpClient();
			var consoleLogger = new ConsoleLogger();
			return new NetworkLogger(httpClient, consoleLogger, "TFAAS1.TixFactory.ApplicationAuthorization.Service", "tix-factory-monitoring");
		}
	}
}

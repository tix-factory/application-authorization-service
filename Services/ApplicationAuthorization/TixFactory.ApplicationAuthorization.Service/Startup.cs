using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TixFactory.Http.Client;
using TixFactory.Logging;
using TixFactory.Logging.Client;

namespace TixFactory.ApplicationAuthorization.Service
{
	public class Startup : TixFactory.Http.Service.Startup
	{
		private readonly IApplicationAuthorizationOperations _ApplicationAuthorizationOperations;

		public Startup()
			: base(CreateLogger())
		{
			_ApplicationAuthorizationOperations = new ApplicationAuthorizationOperations(Logger, ApplicationContext);
		}

		public override void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient(s => _ApplicationAuthorizationOperations);

			base.ConfigureServices(services);
		}

		protected override void ConfigureMvc(MvcOptions options)
		{
			options.Filters.Add(new ValidateApiKeyAttribute(ApplicationContext, _ApplicationAuthorizationOperations.ApplicationKeyValidator));

			base.ConfigureMvc(options);
		}

		private static ILogger CreateLogger()
		{
			var httpClient = new HttpClient();
			var consoleLogger = new ConsoleLogger();
			var loggingServiceHost = Environment.GetEnvironmentVariable("LoggingServiceHost");
			return new NetworkLogger(httpClient, consoleLogger, "TFAAS1.TixFactory.ApplicationAuthorization.Service", loggingServiceHost);
		}
	}
}

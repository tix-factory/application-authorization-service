using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.ApplicationContext;
using TixFactory.Configuration;
using TixFactory.Data.MySql;
using TixFactory.Logging;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	public class ApplicationAuthorizationOperations : IApplicationAuthorizationOperations
	{
		private const string _OperationNameSuffix = "Operation";
		private const string _SetupKeyName = "Application Setup";

		private readonly ILogger _Logger;
		private readonly IApplicationContext _ApplicationContext;
		private readonly IOperationNameProvider _OperationNameProvider;
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;
		private readonly IApplicationOperationAuthorizationEntityFactory _ApplicationOperationAuthorizationEntityFactory;

		public IApplicationKeyValidator ApplicationKeyValidator { get; }

		public IAsyncOperation<string, ApplicationResult> GetApplicationOperation { get; }

		public IAsyncOperation<RegisterApplicationRequest, EmptyResult> RegisterApplicationOperation { get; }

		public IAsyncOperation<RegisterOperationRequest, EmptyResult> RegisterOperationOperation { get; }

		public IAsyncOperation<ToggleOperationEnabledRequest, EmptyResult> ToggleOperationEnabledOperation { get; }

		public IAsyncOperation<CreateApplicationKeyRequest, Guid> CreateApplicationKeyOperation { get; }

		public IAsyncOperation<DeleteApplicationKeyRequest, EmptyResult> DeleteApplicationKeyOperation { get; }

		public IAsyncOperation<ToggleApplicationKeyEnabledRequest, EmptyResult> ToggleApplicationKeyEnabledOperation { get; }

		public IAsyncOperation<GetAuthorizedOperationsRequest, ICollection<string>> GetAuthorizedOperationsOperation { get; }

		public IAsyncOperation<ToggleOperationAuthorizationRequest, EmptyResult> ToggleOperationAuthorizationOperation { get; }

		public IAsyncOperation<Guid, WhoAmIResult> WhoAmIOperation { get; }

		public ApplicationAuthorizationOperations(ILogger logger, IApplicationContext applicationContext)
		{
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
			_OperationNameProvider = new OperationNameProvider();

			var keyHasher = new KeyHasher();
			var connectionString = new Setting<string>(Environment.GetEnvironmentVariable("APPLICATION_AUTHORIZATIONS_CONNECTION_STRING"));
			var databaseConnection = new DatabaseConnection(connectionString, logger);
			var applicationEntityFactory = _ApplicationEntityFactory = new ApplicationEntityFactory(databaseConnection);
			var operationEntityFactory = _OperationEntityFactory = new OperationEntityFactory(databaseConnection);
			var applicationKeyEntityFactory = _ApplicationKeyEntityFactory = new ApplicationKeyEntityFactory(databaseConnection, keyHasher);
			var applicationOperationAuthorizationEntityFactory = _ApplicationOperationAuthorizationEntityFactory = new ApplicationOperationAuthorizationEntityFactory(databaseConnection);
			var applicationKeyValidator = ApplicationKeyValidator = new ApplicationKeyValidator(applicationEntityFactory, operationEntityFactory, applicationKeyEntityFactory, applicationOperationAuthorizationEntityFactory);

			GetApplicationOperation = new GetApplicationOperation(applicationEntityFactory, operationEntityFactory);

			RegisterApplicationOperation = new RegisterApplicationOperation(applicationEntityFactory);

			RegisterOperationOperation = new RegisterOperationOperation(applicationEntityFactory, operationEntityFactory);
			ToggleOperationEnabledOperation = new ToggleOperationEnabledOperation(applicationEntityFactory, operationEntityFactory);

			CreateApplicationKeyOperation = new CreateApplicationKeyOperation(applicationEntityFactory, applicationKeyEntityFactory);
			DeleteApplicationKeyOperation = new DeleteApplicationKeyOperation(applicationEntityFactory, applicationKeyEntityFactory);
			ToggleApplicationKeyEnabledOperation = new ToggleApplicationKeyEnabledOperation(applicationEntityFactory, applicationKeyEntityFactory);
			GetAuthorizedOperationsOperation = new GetAuthorizedOperationsOperation(applicationEntityFactory, applicationKeyEntityFactory, applicationKeyValidator);
			ToggleOperationAuthorizationOperation = new ToggleOperationAuthorizationOperation(applicationEntityFactory, operationEntityFactory, applicationOperationAuthorizationEntityFactory);
			WhoAmIOperation = new WhoAmIOperation(applicationEntityFactory, applicationKeyEntityFactory);

			ThreadPool.QueueUserWorkItem(SelfRegistration);
		}

		private void SelfRegistration(object state)
		{
			try
			{
				var application = _ApplicationEntityFactory.GetApplicationByName(_ApplicationContext.Name);
				if (application == null)
				{
					application = _ApplicationEntityFactory.CreateApplication(_ApplicationContext.Name);
				}

				var applicationKeys = _ApplicationKeyEntityFactory.GetApplicationKeysByApplicationId(application.Id);
				var setupKey = applicationKeys.FirstOrDefault();
				if (setupKey == null)
				{
					var keyGuid = Guid.NewGuid();
					setupKey = _ApplicationKeyEntityFactory.CreateApplicationKey(application.Id, _SetupKeyName, keyGuid);

					Console.WriteLine($"Setup ApiKey ({setupKey.Name}): {keyGuid}");
				}

				// Make sure the ApplicationAuthorization service has access to itself.
				var selfAuthorizations = _ApplicationOperationAuthorizationEntityFactory.GetApplicationOperationAuthorizationsByApplicationId(application.Id);

				foreach (var operationProperty in GetType().GetProperties())
				{
					if (!operationProperty.Name.EndsWith(_OperationNameSuffix))
					{
						continue;
					}

					var operationClass = operationProperty.GetValue(this);
					if (operationClass != null)
					{
						var operationName = _OperationNameProvider.GetOperationName(operationClass.GetType());
						var operation = _OperationEntityFactory.GetOperationByName(application.Id, operationName);
						if (operation == null)
						{
							operation = _OperationEntityFactory.CreateOperation(application.Id, operationName);
						}

						if (!operation.Enabled)
						{
							operation.Enabled = true;
							_OperationEntityFactory.UpdateOperation(operation);
						}

						if (selfAuthorizations.All(a => a.OperationId != operation.Id))
						{
							_ApplicationOperationAuthorizationEntityFactory.CreateApplicationOperationAuthorization(application.Id, operation.Id);
						}
					}
				}
			}
			catch (Exception e)
			{
				_Logger.Error($"{nameof(ApplicationAuthorizationOperations)}.{nameof(SelfRegistration)}\n{e}");
			}
		}
	}
}

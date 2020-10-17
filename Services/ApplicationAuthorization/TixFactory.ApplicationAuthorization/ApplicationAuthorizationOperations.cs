using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.ApplicationContext;
using TixFactory.Configuration;
using TixFactory.Logging;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	public class ApplicationAuthorizationOperations : IApplicationAuthorizationOperations
	{
		private const string _OperationNameSuffix = "Operation";

		private readonly ILogger _Logger;
		private readonly IApplicationContext _ApplicationContext;
		private readonly IOperationNameProvider _OperationNameProvider;
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;
		private readonly IApplicationOperationAuthorizationEntityFactory _ApplicationOperationAuthorizationEntityFactory;
		private readonly ILazyWithRetry<MySqlConnection> _MySqlConnection;

		public IApplicationKeyValidator ApplicationKeyValidator { get; }

		public IOperation<string, ApplicationResult> GetApplicationOperation { get; }

		public IOperation<RegisterApplicationRequest, EmptyResult> RegisterApplicationOperation { get; }

		public IOperation<RegisterOperationRequest, EmptyResult> RegisterOperationOperation { get; }

		public IOperation<GetAuthorizedOperationsRequest, ICollection<string>> GetAuthorizedOperationsOperation { get; }

		public ApplicationAuthorizationOperations(ILogger logger, IApplicationContext applicationContext)
		{
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
			_OperationNameProvider = new OperationNameProvider();

			var keyHasher = new KeyHasher();
			var mySqlConnection = _MySqlConnection = new LazyWithRetry<MySqlConnection>(BuildConnection);
			var databaseConnection = new DatabaseConnection(mySqlConnection);
			var applicationEntityFactory = _ApplicationEntityFactory = new ApplicationEntityFactory(databaseConnection);
			var operationEntityFactory = _OperationEntityFactory = new OperationEntityFactory(databaseConnection);
			var applicationKeyEntityFactory = _ApplicationKeyEntityFactory = new ApplicationKeyEntityFactory(databaseConnection, keyHasher);
			var applicationOperationAuthorizationEntityFactory = _ApplicationOperationAuthorizationEntityFactory = new ApplicationOperationAuthorizationEntityFactory(databaseConnection);
			var applicationKeyValidator = ApplicationKeyValidator = new ApplicationKeyValidator(applicationEntityFactory, operationEntityFactory, applicationKeyEntityFactory, applicationOperationAuthorizationEntityFactory);

			GetApplicationOperation = new GetApplicationOperation(applicationEntityFactory, operationEntityFactory);
			RegisterApplicationOperation = new RegisterApplicationOperation(applicationEntityFactory);
			RegisterOperationOperation = new RegisterOperationOperation(applicationEntityFactory, operationEntityFactory);
			GetAuthorizedOperationsOperation = new GetAuthorizedOperationsOperation(applicationEntityFactory, applicationKeyEntityFactory, applicationKeyValidator);

			ThreadPool.QueueUserWorkItem(SelfRegistration);
		}

		private MySqlConnection BuildConnection()
		{
			var connection = new MySqlConnection(Environment.GetEnvironmentVariable("APPLICATION_AUTHORIZATIONS_CONNECTION_STRING"));
			connection.StateChange += ConnectionStateChange;
			connection.Open();

			return connection;
		}

		private void ConnectionStateChange(object sender, StateChangeEventArgs e)
		{
			switch (e.CurrentState)
			{
				case ConnectionState.Broken:
				case ConnectionState.Closed:
					_MySqlConnection.Refresh();
					return;
			}
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
					_ApplicationKeyEntityFactory.CreateApplicationKey(application.Id, keyGuid);

					Console.WriteLine($"Setup ApiKey: {keyGuid}");
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

using System;
using System.Data;
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
		private readonly ILogger _Logger;
		private readonly IApplicationContext _ApplicationContext;
		private readonly IOperationNameProvider _OperationNameProvider;
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;
		private readonly ILazyWithRetry<MySqlConnection> _MySqlConnection;

		public IOperation<string, ApplicationResult> GetApplicationOperation { get; }

		public ApplicationAuthorizationOperations(ILogger logger, IApplicationContext applicationContext)
		{
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
			_OperationNameProvider = new OperationNameProvider();

			var mySqlConnection = _MySqlConnection = new LazyWithRetry<MySqlConnection>(BuildConnection);
			var databaseConnection = new DatabaseConnection(mySqlConnection);
			var applicationEntityFactory = _ApplicationEntityFactory = new ApplicationEntityFactory(databaseConnection);
			var operationEntityFactory = _OperationEntityFactory = new OperationEntityFactory(databaseConnection);

			GetApplicationOperation = new GetApplicationOperation(applicationEntityFactory, operationEntityFactory);

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

				foreach (var operationProperty in GetType().GetProperties())
				{
					var operationClass = operationProperty.GetValue(this);
					if (operationClass != null)
					{
						var operationName = _OperationNameProvider.GetOperationName(operationClass.GetType());
						var operation = _OperationEntityFactory.GetOperationByName(application.Id, operationName);
						if (operation == null)
						{
							_OperationEntityFactory.CreateOperation(application.Id, operationName);
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

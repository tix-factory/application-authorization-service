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
		private readonly IServiceEntityFactory _ServiceEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;
		private readonly ILazyWithRetry<MySqlConnection> _MySqlConnection;

		public IOperation<string, ServiceResult> GetServiceOperation { get; }

		public ApplicationAuthorizationOperations(ILogger logger, IApplicationContext applicationContext)
		{
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
			_OperationNameProvider = new OperationNameProvider();

			var mySqlConnection = _MySqlConnection = new LazyWithRetry<MySqlConnection>(BuildConnection);
			var databaseConnection = new DatabaseConnection(mySqlConnection);
			var serviceEntityFactory = _ServiceEntityFactory = new ServiceEntityFactory(databaseConnection);
			var operationEntityFactory = _OperationEntityFactory = new OperationEntityFactory(databaseConnection);

			GetServiceOperation = new GetServiceOperation(serviceEntityFactory);

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
				var service = _ServiceEntityFactory.GetServiceByName(_ApplicationContext.Name);
				if (service == null)
				{
					service = _ServiceEntityFactory.CreateService(_ApplicationContext.Name);
				}

				foreach (var operationProperty in GetType().GetProperties())
				{
					var operationClass = operationProperty.GetValue(this);
					if (operationClass != null)
					{
						var operationName = _OperationNameProvider.GetOperationName(operationClass.GetType());
						var operation = _OperationEntityFactory.GetOperationByName(service.Id, operationName);
						if (operation == null)
						{
							_OperationEntityFactory.CreateOperation(service.Id, operationName);
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

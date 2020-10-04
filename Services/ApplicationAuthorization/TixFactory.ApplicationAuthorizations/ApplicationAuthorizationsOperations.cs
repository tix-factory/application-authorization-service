using System;
using System.Data;
using MySql.Data.MySqlClient;
using TixFactory.ApplicationAuthorizations.Entities;
using TixFactory.Configuration;
using TixFactory.Logging;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorizations
{
	public class ApplicationAuthorizationsOperations : IApplicationAuthorizationsOperations
	{
		private readonly ILazyWithRetry<MySqlConnection> _MySqlConnection;

		public IOperation<string, ServiceResult> GetServiceOperation { get; }

		public ApplicationAuthorizationsOperations(ILogger logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			var mySqlConnection = _MySqlConnection = new LazyWithRetry<MySqlConnection>(BuildConnection);
			var databaseConnection = new DatabaseConnection(mySqlConnection);
			var serviceEntityFactory = new ServiceEntityFactory(databaseConnection);

			GetServiceOperation = new GetServiceOperation(serviceEntityFactory);
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
	}
}

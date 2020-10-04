using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using TixFactory.Configuration;

namespace TixFactory.ApplicationAuthorizations.Entities
{
	internal class ServiceEntityFactory : IServiceEntityFactory
	{
		private const string _GetServicesStoredProcedureName = "GetServices";
		private const int _ServicesMaxCount = 1000;
		private static readonly TimeSpan _ServiceCacheTime = TimeSpan.FromMinutes(1);
		private readonly IDatabaseConnection _DatabaseConnection;
		private readonly LazyWithRetry<IReadOnlyCollection<Service>> _Services;

		public ServiceEntityFactory(IDatabaseConnection databaseConnection)
		{
			_DatabaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
			_Services = new LazyWithRetry<IReadOnlyCollection<Service>>(LoadServices);
		}

		public Service CreateService(string name)
		{
			throw new System.NotImplementedException();
		}

		public IReadOnlyCollection<Service> GetServices()
		{
			return _Services.Value;
		}

		public Service GetServiceByName(string name)
		{
			return _Services.Value.FirstOrDefault(s => string.Equals(name, s.Name, StringComparison.OrdinalIgnoreCase));
		}

		public void UpdateService(Service service)
		{
			throw new System.NotImplementedException();
		}

		public void DeleteService(long id)
		{
			throw new System.NotImplementedException();
		}

		private IReadOnlyCollection<Service> LoadServices()
		{
			var services = _DatabaseConnection.ExecuteReadStoredProcedure<Service>(_GetServicesStoredProcedureName, new[]
			{
				new MySqlParameter("@_Count", _ServicesMaxCount)
			});

			ThreadPool.QueueUserWorkItem(ExpireServices);

			return services;
		}

		private void ExpireServices(object state)
		{
			Thread.Sleep(_ServiceCacheTime);
			_Services.Refresh();
		}
	}
}

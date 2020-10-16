using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using TixFactory.Configuration;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal class ServiceEntityFactory : IServiceEntityFactory
	{
		private const string _InsertServiceStoredProcedure = "InsertService";
		private const string _GetServicesStoredProcedureName = "GetServices";
		private const string _UpdateServiceStoredProcedureName = "UpdateService";
		private const string _DeleteServiceStoredProcedureName = "DeleteService";
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
			var serviceId = _DatabaseConnection.ExecuteInsertStoredProcedure<long>(_InsertServiceStoredProcedure, new[]
			{
				new MySqlParameter("@_Name", name)
			});

			_Services.Refresh();

			return _Services.Value.First(s => s.Id == serviceId);
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
			_DatabaseConnection.ExecuteWriteStoredProcedure(_UpdateServiceStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", service.Id),
				new MySqlParameter("@_Name", service.Name)
			});
		}

		public void DeleteService(long id)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_DeleteServiceStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", id)
			});
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using TixFactory.Configuration;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal class ApplicationEntityFactory : IApplicationEntityFactory
	{
		private const string _InsertApplicationStoredProcedure = "InsertApplication";
		private const string _GetApplicationsStoredProcedureName = "GetApplications";
		private const string _UpdateApplicationStoredProcedureName = "UpdateApplication";
		private const string _DeleteApplicationStoredProcedureName = "DeleteApplication";
		private const int _ApplicationsMaxCount = 1000;
		private static readonly TimeSpan _ApplicationCacheTime = TimeSpan.FromMinutes(1);
		private readonly IDatabaseConnection _DatabaseConnection;
		private readonly LazyWithRetry<IReadOnlyCollection<Application>> _Applications;

		public ApplicationEntityFactory(IDatabaseConnection databaseConnection)
		{
			_DatabaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
			_Applications = new LazyWithRetry<IReadOnlyCollection<Application>>(LoadApplications);
		}

		public Application CreateApplication(string name)
		{
			var applications = GetApplications();
			if (applications.Count >= _ApplicationsMaxCount)
			{
				throw new ApplicationException($"Cannot create more than {_ApplicationsMaxCount} applications.");
			}

			var applicationId = _DatabaseConnection.ExecuteInsertStoredProcedure<long>(_InsertApplicationStoredProcedure, new[]
			{
				new MySqlParameter("@_Name", name)
			});

			_Applications.Refresh();

			return GetApplicationById(applicationId);
		}

		public IReadOnlyCollection<Application> GetApplications()
		{
			return _Applications.Value;
		}

		public Application GetApplicationById(long id)
		{
			return _Applications.Value.First(s => s.Id == id);
		}

		public Application GetApplicationByName(string name)
		{
			return _Applications.Value.FirstOrDefault(s => string.Equals(name, s.Name, StringComparison.OrdinalIgnoreCase));
		}

		public void UpdateApplication(Application application)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_UpdateApplicationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", application.Id),
				new MySqlParameter("@_Name", application.Name)
			});
		}

		public void DeleteApplication(long id)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_DeleteApplicationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", id)
			});
		}

		private IReadOnlyCollection<Application> LoadApplications()
		{
			var applications = _DatabaseConnection.ExecuteReadStoredProcedure<Application>(_GetApplicationsStoredProcedureName, new[]
			{
				new MySqlParameter("@_Count", _ApplicationsMaxCount)
			});

			ThreadPool.QueueUserWorkItem(ExpireApplications);

			return applications;
		}

		private void ExpireApplications(object state)
		{
			Thread.Sleep(_ApplicationCacheTime);
			_Applications.Refresh();
		}
	}
}

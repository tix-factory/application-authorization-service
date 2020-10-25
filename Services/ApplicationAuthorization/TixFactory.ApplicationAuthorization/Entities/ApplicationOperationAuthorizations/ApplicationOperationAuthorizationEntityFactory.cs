using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using TixFactory.Collections;
using TixFactory.Data.MySql;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal class ApplicationOperationAuthorizationEntityFactory : IApplicationOperationAuthorizationEntityFactory
	{
		private const string _InsertApplicationOperationAuthorizationStoredProcedure = "InsertApplicationOperationAuthorization";
		private const string _GetApplicationOperationAuthorizationsByApplicationIdStoredProcedureName = "GetApplicationOperationAuthorizationsByApplicationId";
		private const string _GetApplicationOperationAuthorizationsByOperationIdStoredProcedureName = "GetApplicationOperationAuthorizationsByOperationId";
		private const string _UpdateApplicationOperationAuthorizationStoredProcedureName = "UpdateApplicationOperationAuthorization";
		private const string _DeleteApplicationOperationAuthorizationStoredProcedureName = "DeleteApplicationOperationAuthorization";
		private const int _ApplicationOperationAuthorizationsMaxCount = 1000;
		private static readonly TimeSpan _ApplicationOperationAuthorizationsCacheTime = TimeSpan.FromMinutes(1);
		private readonly IDatabaseConnection _DatabaseConnection;
		private readonly ExpirableDictionary<long, IReadOnlyCollection<ApplicationOperationAuthorization>> _ApplicationOperationAuthorizationsByApplicationId;
		private readonly ExpirableDictionary<long, IReadOnlyCollection<ApplicationOperationAuthorization>> _ApplicationOperationAuthorizationsByOperationId;

		public ApplicationOperationAuthorizationEntityFactory(IDatabaseConnection databaseConnection)
		{
			_DatabaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
			_ApplicationOperationAuthorizationsByApplicationId = new ExpirableDictionary<long, IReadOnlyCollection<ApplicationOperationAuthorization>>(_ApplicationOperationAuthorizationsCacheTime, ExpirationPolicy.RenewOnWrite);
			_ApplicationOperationAuthorizationsByOperationId = new ExpirableDictionary<long, IReadOnlyCollection<ApplicationOperationAuthorization>>(_ApplicationOperationAuthorizationsCacheTime, ExpirationPolicy.RenewOnWrite);
		}

		public ApplicationOperationAuthorization CreateApplicationOperationAuthorization(long applicationId, long operationId)
		{
			var applicationOperationAuthorizations = GetApplicationOperationAuthorizationsByApplicationId(applicationId);
			if (applicationOperationAuthorizations.Count >= _ApplicationOperationAuthorizationsMaxCount)
			{
				throw new ApplicationException($"Cannot create more than {_ApplicationOperationAuthorizationsMaxCount} authorizations per application.");
			}

			var applicationOperationAuthorizationId = _DatabaseConnection.ExecuteInsertStoredProcedure<long>(_InsertApplicationOperationAuthorizationStoredProcedure, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_OperationID", operationId)
			});

			_ApplicationOperationAuthorizationsByApplicationId.Remove(applicationId);
			_ApplicationOperationAuthorizationsByOperationId.Remove(operationId);

			applicationOperationAuthorizations = GetApplicationOperationAuthorizationsByApplicationId(applicationId);
			return applicationOperationAuthorizations.First(a => a.Id == applicationOperationAuthorizationId);
		}

		public IReadOnlyCollection<ApplicationOperationAuthorization> GetApplicationOperationAuthorizationsByApplicationId(long applicationId)
		{
			if (_ApplicationOperationAuthorizationsByApplicationId.TryGetValue(applicationId, out var applicationOperationAuthorizations))
			{
				return applicationOperationAuthorizations;
			}

			applicationOperationAuthorizations = _ApplicationOperationAuthorizationsByApplicationId[applicationId] = _DatabaseConnection.ExecuteReadStoredProcedure<ApplicationOperationAuthorization>(_GetApplicationOperationAuthorizationsByApplicationIdStoredProcedureName, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_Count", _ApplicationOperationAuthorizationsMaxCount)
			});

			return applicationOperationAuthorizations;
		}

		public IReadOnlyCollection<ApplicationOperationAuthorization> GetApplicationOperationAuthorizationsByOperationId(long operationId)
		{
			if (_ApplicationOperationAuthorizationsByOperationId.TryGetValue(operationId, out var applicationOperationAuthorizations))
			{
				return applicationOperationAuthorizations;
			}

			applicationOperationAuthorizations = _ApplicationOperationAuthorizationsByOperationId[operationId] = _DatabaseConnection.ExecuteReadStoredProcedure<ApplicationOperationAuthorization>(_GetApplicationOperationAuthorizationsByOperationIdStoredProcedureName, new[]
			{
				new MySqlParameter("@_OperationID", operationId),
				new MySqlParameter("@_Count", _ApplicationOperationAuthorizationsMaxCount)
			});

			return applicationOperationAuthorizations;
		}

		public void UpdateApplicationOperationAuthorization(ApplicationOperationAuthorization applicationOperationAuthorization)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_UpdateApplicationOperationAuthorizationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", applicationOperationAuthorization.Id),
				new MySqlParameter("@_ApplicationID", applicationOperationAuthorization.ApplicationId),
				new MySqlParameter("@_OperationID", applicationOperationAuthorization.OperationId)
			});
		}

		public void DeleteApplicationOperationAuthorization(long id)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_DeleteApplicationOperationAuthorizationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", id)
			});
		}
	}
}

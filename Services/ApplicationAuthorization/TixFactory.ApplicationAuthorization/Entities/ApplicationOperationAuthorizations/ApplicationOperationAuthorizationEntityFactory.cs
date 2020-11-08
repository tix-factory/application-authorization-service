using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		public async Task<ApplicationOperationAuthorization> CreateApplicationOperationAuthorization(long applicationId, long operationId, CancellationToken cancellationToken)
		{
			var applicationOperationAuthorizations = await GetApplicationOperationAuthorizationsByApplicationId(applicationId, cancellationToken).ConfigureAwait(false);
			if (applicationOperationAuthorizations.Count >= _ApplicationOperationAuthorizationsMaxCount)
			{
				throw new ApplicationException($"Cannot create more than {_ApplicationOperationAuthorizationsMaxCount} authorizations per application.");
			}

			var applicationOperationAuthorizationId = await _DatabaseConnection.ExecuteInsertStoredProcedureAsync<long>(_InsertApplicationOperationAuthorizationStoredProcedure, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_OperationID", operationId)
			}, cancellationToken).ConfigureAwait(false);

			_ApplicationOperationAuthorizationsByApplicationId.Remove(applicationId);
			_ApplicationOperationAuthorizationsByOperationId.Remove(operationId);

			applicationOperationAuthorizations = await GetApplicationOperationAuthorizationsByApplicationId(applicationId, cancellationToken).ConfigureAwait(false);
			return applicationOperationAuthorizations.First(a => a.Id == applicationOperationAuthorizationId);
		}

		public async Task<IReadOnlyCollection<ApplicationOperationAuthorization>> GetApplicationOperationAuthorizationsByApplicationId(long applicationId, CancellationToken cancellationToken)
		{
			if (_ApplicationOperationAuthorizationsByApplicationId.TryGetValue(applicationId, out var applicationOperationAuthorizations))
			{
				return applicationOperationAuthorizations;
			}

			applicationOperationAuthorizations = _ApplicationOperationAuthorizationsByApplicationId[applicationId] = await _DatabaseConnection.ExecuteReadStoredProcedureAsync<ApplicationOperationAuthorization>(_GetApplicationOperationAuthorizationsByApplicationIdStoredProcedureName, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_Count", _ApplicationOperationAuthorizationsMaxCount)
			}, cancellationToken).ConfigureAwait(false);

			return applicationOperationAuthorizations;
		}

		public async Task<IReadOnlyCollection<ApplicationOperationAuthorization>> GetApplicationOperationAuthorizationsByOperationId(long operationId, CancellationToken cancellationToken)
		{
			if (_ApplicationOperationAuthorizationsByOperationId.TryGetValue(operationId, out var applicationOperationAuthorizations))
			{
				return applicationOperationAuthorizations;
			}

			applicationOperationAuthorizations = _ApplicationOperationAuthorizationsByOperationId[operationId] = await _DatabaseConnection.ExecuteReadStoredProcedureAsync<ApplicationOperationAuthorization>(_GetApplicationOperationAuthorizationsByOperationIdStoredProcedureName, new[]
			{
				new MySqlParameter("@_OperationID", operationId),
				new MySqlParameter("@_Count", _ApplicationOperationAuthorizationsMaxCount)
			}, cancellationToken).ConfigureAwait(false);

			return applicationOperationAuthorizations;
		}

		public async Task DeleteApplicationOperationAuthorization(ApplicationOperationAuthorization applicationOperationAuthorization, CancellationToken cancellationToken)
		{
			await _DatabaseConnection.ExecuteWriteStoredProcedureAsync(_DeleteApplicationOperationAuthorizationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", applicationOperationAuthorization.Id)
			}, cancellationToken).ConfigureAwait(false);

			_ApplicationOperationAuthorizationsByOperationId.Remove(applicationOperationAuthorization.OperationId);
			_ApplicationOperationAuthorizationsByApplicationId.Remove(applicationOperationAuthorization.ApplicationId);
		}
	}
}

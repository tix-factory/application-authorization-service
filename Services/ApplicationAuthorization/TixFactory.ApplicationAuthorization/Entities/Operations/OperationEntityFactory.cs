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
	internal class OperationEntityFactory : IOperationEntityFactory
	{
		private const string _InsertOperationStoredProcedureName = "InsertOperation";
		private const string _GetOperationsByApplicationIdStoredProcedureName = "GetOperationsByApplicationId";
		private const string _UpdateOperationStoredProcedureName = "UpdateOperation";
		private const string _DeleteOperationStoredProcedureName = "DeleteOperation";
		private const int _OperationsMaxCount = 1000;
		private static readonly TimeSpan _OperationsCacheTime = TimeSpan.FromMinutes(1);
		private readonly IDatabaseConnection _DatabaseConnection;
		private readonly ExpirableDictionary<long, IReadOnlyCollection<Operation>> _OperationsByApplicationId;

		public OperationEntityFactory(IDatabaseConnection databaseConnection)
		{
			_DatabaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
			_OperationsByApplicationId = new ExpirableDictionary<long, IReadOnlyCollection<Operation>>(_OperationsCacheTime, ExpirationPolicy.RenewOnWrite);
		}

		public async Task<Operation> CreateOperation(long applicationId, string name, CancellationToken cancellationToken)
		{
			var operations = await GetOperations(applicationId, cancellationToken).ConfigureAwait(false);
			if (operations.Count >= _OperationsMaxCount)
			{
				throw new ApplicationException($"Cannot create more than {_OperationsMaxCount} operations per application.");
			}

			var operationId = await _DatabaseConnection.ExecuteInsertStoredProcedureAsync<long>(_InsertOperationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_Name", name),
				new MySqlParameter("@_Enabled", true)
			}, cancellationToken).ConfigureAwait(false);

			_OperationsByApplicationId.Remove(applicationId);

			operations = await GetOperations(applicationId, cancellationToken).ConfigureAwait(false);
			return operations.First(o => o.Id == operationId);
		}

		public async Task<IReadOnlyCollection<Operation>> GetOperations(long applicationId, CancellationToken cancellationToken)
		{
			if (_OperationsByApplicationId.TryGetValue(applicationId, out var operations))
			{
				return operations;
			}

			_OperationsByApplicationId[applicationId] = operations = await _DatabaseConnection.ExecuteReadStoredProcedureAsync<Operation>(_GetOperationsByApplicationIdStoredProcedureName, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_Count", _OperationsMaxCount)
			}, cancellationToken).ConfigureAwait(false);

			return operations;
		}

		public async Task<Operation> GetOperationByName(long applicationId, string name, CancellationToken cancellationToken)
		{
			var operations = await GetOperations(applicationId, cancellationToken).ConfigureAwait(false);
			return operations.FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		public async Task UpdateOperation(Operation operation, CancellationToken cancellationToken)
		{
			await _DatabaseConnection.ExecuteWriteStoredProcedureAsync(_UpdateOperationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", operation.Id),
				new MySqlParameter("@_ApplicationID", operation.ApplicationId),
				new MySqlParameter("@_Name", operation.Name),
				new MySqlParameter("@_Enabled", operation.Enabled)
			}, cancellationToken).ConfigureAwait(false);
		}

		public async Task DeleteOperation(Operation operation, CancellationToken cancellationToken)
		{
			await _DatabaseConnection.ExecuteWriteStoredProcedureAsync(_DeleteOperationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", operation.Id)
			}, cancellationToken).ConfigureAwait(false);

			_OperationsByApplicationId.Remove(operation.ApplicationId);
		}
	}
}

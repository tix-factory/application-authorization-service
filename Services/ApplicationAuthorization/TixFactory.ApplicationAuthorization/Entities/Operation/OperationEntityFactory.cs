using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using TixFactory.Collections;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal class OperationEntityFactory : IOperationEntityFactory
	{
		private const string _InsertOperationStoredProcedureName = "InsertOperation";
		private const string _GetOperationsByServiceIdStoredProcedureName = "GetOperationsByServiceId";
		private const string _UpdateOperationStoredProcedureName = "UpdateOperation";
		private const string _DeleteOperationStoredProcedureName = "DeleteOperation";
		private const int _OperationsMaxCount = 1000;
		private static readonly TimeSpan _OperationsCacheTime = TimeSpan.FromMinutes(1);
		private readonly IDatabaseConnection _DatabaseConnection;
		private readonly ExpirableDictionary<long, IReadOnlyCollection<Operation>> _OperationsByServiceId;

		public OperationEntityFactory(IDatabaseConnection databaseConnection)
		{
			_DatabaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
			_OperationsByServiceId = new ExpirableDictionary<long, IReadOnlyCollection<Operation>>(_OperationsCacheTime, ExpirationPolicy.RenewOnWrite);
		}

		public Operation CreateOperation(long serviceId, string name)
		{
			var operationId = _DatabaseConnection.ExecuteInsertStoredProcedure<long>(_InsertOperationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ServiceID", serviceId),
				new MySqlParameter("@_Name", name),
				new MySqlParameter("@_Enabled", false),
			});

			_OperationsByServiceId.Remove(serviceId);

			var operations = GetOperations(serviceId);
			return operations.First(o => o.Id == operationId);
		}

		public IReadOnlyCollection<Operation> GetOperations(long serviceId)
		{
			if (_OperationsByServiceId.TryGetValue(serviceId, out var operations))
			{
				return operations;
			}

			_OperationsByServiceId[serviceId] = operations = _DatabaseConnection.ExecuteReadStoredProcedure<Operation>(_GetOperationsByServiceIdStoredProcedureName, new[]
			{
				new MySqlParameter("@_ServiceID", serviceId),
				new MySqlParameter("@_Count", _OperationsMaxCount)
			});

			return operations;
		}

		public Operation GetOperationByName(long serviceId, string name)
		{
			var operations = GetOperations(serviceId);
			return operations.FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		public void UpdateOperation(Operation operation)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_UpdateOperationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", operation.Id),
				new MySqlParameter("@_ServiceID", operation.ServiceId),
				new MySqlParameter("@_Name", operation.Name),
				new MySqlParameter("@_Enabled", operation.Enabled)
			});
		}

		public void DeleteOperation(long id)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_DeleteOperationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", id)
			});
		}
	}
}

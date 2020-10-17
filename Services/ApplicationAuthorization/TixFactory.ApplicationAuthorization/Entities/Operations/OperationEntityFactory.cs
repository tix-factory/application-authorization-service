﻿using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using TixFactory.Collections;

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

		public Operation CreateOperation(long applicationId, string name)
		{
			var operationId = _DatabaseConnection.ExecuteInsertStoredProcedure<long>(_InsertOperationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_Name", name),
				new MySqlParameter("@_Enabled", false),
			});

			_OperationsByApplicationId.Remove(applicationId);

			var operations = GetOperations(applicationId);
			return operations.First(o => o.Id == operationId);
		}

		public IReadOnlyCollection<Operation> GetOperations(long applicationId)
		{
			if (_OperationsByApplicationId.TryGetValue(applicationId, out var operations))
			{
				return operations;
			}

			_OperationsByApplicationId[applicationId] = operations = _DatabaseConnection.ExecuteReadStoredProcedure<Operation>(_GetOperationsByApplicationIdStoredProcedureName, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_Count", _OperationsMaxCount)
			});

			return operations;
		}

		public Operation GetOperationByName(long applicationId, string name)
		{
			var operations = GetOperations(applicationId);
			return operations.FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		public void UpdateOperation(Operation operation)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_UpdateOperationStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", operation.Id),
				new MySqlParameter("@_ApplicationID", operation.ApplicationId),
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
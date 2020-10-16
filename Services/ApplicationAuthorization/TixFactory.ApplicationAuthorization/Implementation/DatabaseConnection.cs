using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using MySql.Data.MySqlClient;
using TixFactory.Configuration;

namespace TixFactory.ApplicationAuthorization
{
	internal class DatabaseConnection : IDatabaseConnection
	{
		private readonly ILazyWithRetry<MySqlConnection> _ConnectionLazy;

		public DatabaseConnection(ILazyWithRetry<MySqlConnection> connectionLazy)
		{
			_ConnectionLazy = connectionLazy ?? throw new ArgumentNullException(nameof(connectionLazy));
		}

		public IReadOnlyCollection<T> ExecuteReadStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
			where T : class
		{
			var command = new MySqlCommand(storedProcedureName, _ConnectionLazy.Value);
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.AddRange(mySqlParameters.ToArray());
			return ExecuteCommand<T>(command);
		}

		private IReadOnlyCollection<T> ExecuteCommand<T>(MySqlCommand command)
			where T : class
		{
			var rows = new List<T>();

			using var reader = command.ExecuteReader();
			while (reader.Read())
			{
				var row = new Dictionary<string, object>();
				for (var i = 0; i < reader.FieldCount; i++)
				{
					row.Add(reader.GetName(i), reader.GetValue(i));
				}

				// TODO: Is there a better way to convert reader object -> T?
				var serializedRow = JsonSerializer.Serialize(row);
				var deserializedRow = JsonSerializer.Deserialize<T>(serializedRow);

				if (deserializedRow != default(T))
				{
					rows.Add(deserializedRow);
				}
			}

			return rows;
		}
	}
}

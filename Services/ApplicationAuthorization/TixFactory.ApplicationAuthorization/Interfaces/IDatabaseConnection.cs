using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TixFactory.ApplicationAuthorization
{
	internal interface IDatabaseConnection
	{
		IReadOnlyCollection<T> ExecuteReadStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
			where T : class;
	}
}

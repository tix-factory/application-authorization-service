using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal interface IOperationEntityFactory
	{
		Operation CreateOperation(long serviceId, string name);

		IReadOnlyCollection<Operation> GetOperations(long serviceId);

		Operation GetOperationByName(long serviceId, string name);

		void UpdateOperation(Operation operation);

		void DeleteOperation(long id);
	}
}

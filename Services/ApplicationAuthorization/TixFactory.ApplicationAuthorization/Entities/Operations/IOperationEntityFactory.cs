using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal interface IOperationEntityFactory
	{
		Operation CreateOperation(long applicationId, string name);

		IReadOnlyCollection<Operation> GetOperations(long applicationId);

		Operation GetOperationByName(long applicationId, string name);

		void UpdateOperation(Operation operation);

		void DeleteOperation(long id);
	}
}

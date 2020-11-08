using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal interface IOperationEntityFactory
	{
		Task<Operation> CreateOperation(long applicationId, string name, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<Operation>> GetOperations(long applicationId, CancellationToken cancellationToken);

		Task<Operation> GetOperationByName(long applicationId, string name, CancellationToken cancellationToken);

		Task UpdateOperation(Operation operation, CancellationToken cancellationToken);

		Task DeleteOperation(Operation operation, CancellationToken cancellationToken);
	}
}

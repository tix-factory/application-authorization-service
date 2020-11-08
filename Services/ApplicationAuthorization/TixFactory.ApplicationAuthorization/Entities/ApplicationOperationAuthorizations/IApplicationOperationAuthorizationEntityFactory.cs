using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal interface IApplicationOperationAuthorizationEntityFactory
	{
		Task<ApplicationOperationAuthorization> CreateApplicationOperationAuthorization(long applicationId, long operationId, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<ApplicationOperationAuthorization>> GetApplicationOperationAuthorizationsByApplicationId(long applicationId, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<ApplicationOperationAuthorization>> GetApplicationOperationAuthorizationsByOperationId(long operationId, CancellationToken cancellationToken);

		Task DeleteApplicationOperationAuthorization(ApplicationOperationAuthorization applicationOperationAuthorization, CancellationToken cancellationToken);
	}
}

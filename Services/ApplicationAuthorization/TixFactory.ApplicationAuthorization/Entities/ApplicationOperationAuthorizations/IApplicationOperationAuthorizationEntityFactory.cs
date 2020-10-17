using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal interface IApplicationOperationAuthorizationEntityFactory
	{
		ApplicationOperationAuthorization CreateApplicationOperationAuthorization(long applicationId, long operationId);

		IReadOnlyCollection<ApplicationOperationAuthorization> GetApplicationOperationAuthorizationsByApplicationId(long applicationId);

		IReadOnlyCollection<ApplicationOperationAuthorization> GetApplicationOperationAuthorizationsByOperationId(long operationId);

		void UpdateApplicationOperationAuthorization(ApplicationOperationAuthorization applicationOperationAuthorization);

		void DeleteApplicationOperationAuthorization(long id);
	}
}

using System.Collections.Generic;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationAuthorizationOperations
	{
		IApplicationKeyValidator ApplicationKeyValidator { get; }

		IOperation<string, ApplicationResult> GetApplicationOperation { get; }

		IOperation<RegisterApplicationRequest, EmptyResult> RegisterApplicationOperation { get; }

		IOperation<RegisterOperationRequest, EmptyResult> RegisterOperationOperation { get; }

		IOperation<GetAuthorizedOperationsRequest, ICollection<string>> GetAuthorizedOperationsOperation { get; }
	}
}

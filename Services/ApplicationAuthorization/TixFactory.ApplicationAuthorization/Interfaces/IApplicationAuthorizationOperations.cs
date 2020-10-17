using System.Collections.Generic;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationAuthorizationOperations
	{
		IApplicationKeyValidator ApplicationKeyValidator { get; }

		IOperation<string, ApplicationResult> GetApplicationOperation { get; }

		IOperation<GetAuthorizedOperationsRequest, ICollection<string>> GetAuthorizedOperationsOperation { get; }
	}
}

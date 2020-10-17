using System;
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

		IOperation<ToggleOperationEnabledRequest, EmptyResult> ToggleOperationEnabledOperation { get; }

		IOperation<CreateApplicationKeyRequest, Guid> CreateApplicationKeyOperation { get; }

		IOperation<DeleteApplicationKeyRequest, EmptyResult> DeleteApplicationKeyOperation { get; }

		IOperation<ToggleApplicationKeyEnabledRequest, EmptyResult> ToggleApplicationKeyEnabledOperation { get; }

		IOperation<GetAuthorizedOperationsRequest, ICollection<string>> GetAuthorizedOperationsOperation { get; }
	}
}

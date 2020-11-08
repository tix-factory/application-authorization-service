using System;
using System.Collections.Generic;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationAuthorizationOperations
	{
		IApplicationKeyValidator ApplicationKeyValidator { get; }

		IAsyncOperation<string, ApplicationResult> GetApplicationOperation { get; }

		IAsyncOperation<RegisterApplicationRequest, EmptyResult> RegisterApplicationOperation { get; }

		IAsyncOperation<RegisterOperationRequest, EmptyResult> RegisterOperationOperation { get; }

		IAsyncOperation<ToggleOperationEnabledRequest, EmptyResult> ToggleOperationEnabledOperation { get; }

		IAsyncOperation<CreateApplicationKeyRequest, Guid> CreateApplicationKeyOperation { get; }

		IAsyncOperation<DeleteApplicationKeyRequest, EmptyResult> DeleteApplicationKeyOperation { get; }

		IAsyncOperation<ToggleApplicationKeyEnabledRequest, EmptyResult> ToggleApplicationKeyEnabledOperation { get; }

		IAsyncOperation<GetAuthorizedOperationsRequest, ICollection<string>> GetAuthorizedOperationsOperation { get; }

		IAsyncOperation<ToggleOperationAuthorizationRequest, EmptyResult> ToggleOperationAuthorizationOperation { get; }

		IAsyncOperation<Guid, WhoAmIResult> WhoAmIOperation { get; }
	}
}

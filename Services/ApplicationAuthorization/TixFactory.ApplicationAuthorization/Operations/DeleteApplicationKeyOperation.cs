using System;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class DeleteApplicationKeyOperation : IAsyncOperation<DeleteApplicationKeyRequest, EmptyResult>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;

		public DeleteApplicationKeyOperation(IApplicationEntityFactory applicationEntityFactory, IApplicationKeyEntityFactory applicationKeyEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_ApplicationKeyEntityFactory = applicationKeyEntityFactory ?? throw new ArgumentNullException(nameof(applicationKeyEntityFactory));
		}

		public async Task<(EmptyResult output, OperationError error)> Execute(DeleteApplicationKeyRequest request, CancellationToken cancellationToken)
		{
			var application = _ApplicationEntityFactory.GetApplicationByName(request.ApplicationName);
			if (application == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidApplicationName));
			}

			var applicationKey = await _ApplicationKeyEntityFactory.GetApplicationKeyByApplicationIdAndName(application.Id, request.KeyName, cancellationToken).ConfigureAwait(false);
			if (applicationKey != null)
			{
				await _ApplicationKeyEntityFactory.DeleteApplicationKey(applicationKey, cancellationToken).ConfigureAwait(false);
			}

			return (new EmptyResult(), null);
		}
	}
}

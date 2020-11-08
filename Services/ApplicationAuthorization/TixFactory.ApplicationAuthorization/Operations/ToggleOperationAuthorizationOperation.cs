using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class ToggleOperationAuthorizationOperation : IAsyncOperation<ToggleOperationAuthorizationRequest, EmptyResult>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;
		private readonly IApplicationOperationAuthorizationEntityFactory _ApplicationOperationAuthorizationEntityFactory;

		public ToggleOperationAuthorizationOperation(
			IApplicationEntityFactory applicationEntityFactory,
			IOperationEntityFactory operationEntityFactory,
			IApplicationOperationAuthorizationEntityFactory applicationOperationAuthorizationEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_OperationEntityFactory = operationEntityFactory ?? throw new ArgumentNullException(nameof(operationEntityFactory));
			_ApplicationOperationAuthorizationEntityFactory = applicationOperationAuthorizationEntityFactory ?? throw new ArgumentNullException(nameof(applicationOperationAuthorizationEntityFactory));
		}

		public async Task<(EmptyResult output, OperationError error)> Execute(ToggleOperationAuthorizationRequest request, CancellationToken cancellationToken)
		{
			var application = _ApplicationEntityFactory.GetApplicationByName(request.ApplicationName);
			if (application == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidApplicationName));
			}

			var targetApplication = _ApplicationEntityFactory.GetApplicationByName(request.TargetApplicationName);
			if (targetApplication == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidTargetApplicationName));
			}

			var operation = _OperationEntityFactory.GetOperationByName(targetApplication.Id, request.TargetOperationName);
			if (operation == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidOperationName));
			}

			var applicationAuthorizations = await _ApplicationOperationAuthorizationEntityFactory.GetApplicationOperationAuthorizationsByApplicationId(application.Id, cancellationToken).ConfigureAwait(false);
			var authorizationEntity = applicationAuthorizations.FirstOrDefault(a => a.OperationId == operation.Id);

			if (request.Authorized && authorizationEntity == null)
			{
				await _ApplicationOperationAuthorizationEntityFactory.CreateApplicationOperationAuthorization(application.Id, operation.Id, cancellationToken).ConfigureAwait(false);
			}
			else if (!request.Authorized && authorizationEntity != null)
			{
				await _ApplicationOperationAuthorizationEntityFactory.DeleteApplicationOperationAuthorization(authorizationEntity, cancellationToken).ConfigureAwait(false);
			}

			return (new EmptyResult(), null);
		}
	}
}

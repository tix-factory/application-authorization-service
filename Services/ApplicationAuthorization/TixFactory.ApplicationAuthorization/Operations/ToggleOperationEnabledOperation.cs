using System;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class ToggleOperationEnabledOperation : IOperation<ToggleOperationEnabledRequest, EmptyResult>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;

		public ToggleOperationEnabledOperation(IApplicationEntityFactory applicationEntityFactory, IOperationEntityFactory operationEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_OperationEntityFactory = operationEntityFactory ?? throw new ArgumentNullException(nameof(operationEntityFactory));
		}

		public (EmptyResult output, OperationError error) Execute(ToggleOperationEnabledRequest request)
		{
			var application = _ApplicationEntityFactory.GetApplicationByName(request.ApplicationName);
			if (application == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidApplicationName));
			}

			var operation = _OperationEntityFactory.GetOperationByName(application.Id, request.OperationName);
			if (operation == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidOperationName));
			}

			if (operation.Enabled != request.Enabled)
			{
				operation.Enabled = request.Enabled;
				_OperationEntityFactory.UpdateOperation(operation);
			}

			return (new EmptyResult(), null);
		}
	}
}

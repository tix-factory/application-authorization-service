using System;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class RegisterOperationOperation : IAsyncOperation<RegisterOperationRequest, EmptyResult>
	{
		private const int _MaxNameLength = 50;
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;

		public RegisterOperationOperation(IApplicationEntityFactory applicationEntityFactory, IOperationEntityFactory operationEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_OperationEntityFactory = operationEntityFactory ?? throw new ArgumentNullException(nameof(operationEntityFactory));
		}

		public async Task<(EmptyResult output, OperationError error)> Execute(RegisterOperationRequest request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request?.OperationName) || request.OperationName.Length > _MaxNameLength)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidOperationName));
			}

			var application = _ApplicationEntityFactory.GetApplicationByName(request.ApplicationName);
			if (application == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidApplicationName));
			}

			var operation = await _OperationEntityFactory.GetOperationByName(application.Id, request.OperationName, cancellationToken).ConfigureAwait(false);
			if (operation == null)
			{
				operation = await _OperationEntityFactory.CreateOperation(application.Id, request.OperationName, cancellationToken).ConfigureAwait(false);
			}

			return (new EmptyResult(), null);
		}
	}
}

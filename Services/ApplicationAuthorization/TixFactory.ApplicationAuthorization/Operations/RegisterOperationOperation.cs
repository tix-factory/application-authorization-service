using System;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class RegisterOperationOperation : IOperation<RegisterOperationRequest, EmptyResult>
	{
		private const int _MaxNameLength = 50;
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;

		public RegisterOperationOperation(IApplicationEntityFactory applicationEntityFactory, IOperationEntityFactory operationEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_OperationEntityFactory = operationEntityFactory ?? throw new ArgumentNullException(nameof(operationEntityFactory));
		}

		public (EmptyResult output, OperationError error) Execute(RegisterOperationRequest request)
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

			var operation = _OperationEntityFactory.GetOperationByName(application.Id, request.OperationName);
			if (operation == null)
			{
				operation = _OperationEntityFactory.CreateOperation(application.Id, request.OperationName);
			}

			return (new EmptyResult(), null);
		}
	}
}

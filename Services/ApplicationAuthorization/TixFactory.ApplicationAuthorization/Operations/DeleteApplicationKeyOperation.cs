using System;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class DeleteApplicationKeyOperation : IOperation<DeleteApplicationKeyRequest, EmptyResult>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;

		public DeleteApplicationKeyOperation(IApplicationEntityFactory applicationEntityFactory, IApplicationKeyEntityFactory applicationKeyEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_ApplicationKeyEntityFactory = applicationKeyEntityFactory ?? throw new ArgumentNullException(nameof(applicationKeyEntityFactory));
		}

		public (EmptyResult output, OperationError error) Execute(DeleteApplicationKeyRequest request)
		{
			var application = _ApplicationEntityFactory.GetApplicationByName(request.ApplicationName);
			if (application == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidApplicationName));
			}

			var applicationKey = _ApplicationKeyEntityFactory.GetApplicationKeyByApplicationIdAndName(application.Id, request.KeyName);
			if (applicationKey != null)
			{
				_ApplicationKeyEntityFactory.DeleteApplicationKey(applicationKey.Id);
			}

			return (new EmptyResult(), null);
		}
	}
}

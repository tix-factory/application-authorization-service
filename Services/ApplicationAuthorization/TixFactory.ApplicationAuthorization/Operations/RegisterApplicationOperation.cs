using System;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class RegisterApplicationOperation : IOperation<RegisterApplicationRequest, EmptyResult>
	{
		private const int _MaxNameLength = 50;
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;

		public RegisterApplicationOperation(IApplicationEntityFactory applicationEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
		}

		public (EmptyResult output, OperationError error) Execute(RegisterApplicationRequest request)
		{
			if (string.IsNullOrWhiteSpace(request?.Name) || request.Name.Length > _MaxNameLength)
			{
				return (null, new OperationError(ApplicationAuthorizationError.InvalidApplicationName));
			}

			var application = _ApplicationEntityFactory.GetApplicationByName(request.Name);
			if (application == null)
			{
				application = _ApplicationEntityFactory.CreateApplication(request.Name);
			}

			return (new EmptyResult(), null);
		}
	}
}

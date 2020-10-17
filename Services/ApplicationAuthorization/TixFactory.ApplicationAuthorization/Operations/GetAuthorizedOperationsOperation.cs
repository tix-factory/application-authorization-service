using System;
using System.Collections.Generic;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class GetAuthorizedOperationsOperation : IOperation<GetAuthorizedOperationsRequest, ICollection<string>>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;
		private readonly IApplicationKeyValidator _ApplicationKeyValidator;

		public GetAuthorizedOperationsOperation(
			IApplicationEntityFactory applicationEntityFactory,
			IApplicationKeyEntityFactory applicationKeyEntityFactory,
			IApplicationKeyValidator applicationKeyValidator)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_ApplicationKeyEntityFactory = applicationKeyEntityFactory ?? throw new ArgumentNullException(nameof(applicationKeyEntityFactory));
			_ApplicationKeyValidator = applicationKeyValidator ?? throw new ArgumentNullException(nameof(applicationKeyValidator));
		}

		public (ICollection<string> output, OperationError error) Execute(GetAuthorizedOperationsRequest request)
		{
			var targetApplicationKey = _ApplicationKeyEntityFactory.GetApplicationKey(request.TargetApplicationKey);
			if (targetApplicationKey == null)
			{
				return (Array.Empty<string>(), null);
			}

			var targetApplication = _ApplicationEntityFactory.GetApplicationById(targetApplicationKey.ApplicationId);
			var authorizedOperations = _ApplicationKeyValidator.GetAuthorizedOperations(targetApplication.Id, request.ApiKey);
			return (authorizedOperations, null);
		}
	}
}

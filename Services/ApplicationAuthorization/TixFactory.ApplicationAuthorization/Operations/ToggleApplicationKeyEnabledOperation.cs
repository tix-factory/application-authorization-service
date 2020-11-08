using System;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class ToggleApplicationKeyEnabledOperation : IAsyncOperation<ToggleApplicationKeyEnabledRequest, EmptyResult>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;

		public ToggleApplicationKeyEnabledOperation(IApplicationEntityFactory applicationEntityFactory, IApplicationKeyEntityFactory applicationKeyEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_ApplicationKeyEntityFactory = applicationKeyEntityFactory ?? throw new ArgumentNullException(nameof(applicationKeyEntityFactory));
		}

		public async Task<(EmptyResult output, OperationError error)> Execute(ToggleApplicationKeyEnabledRequest request, CancellationToken cancellationToken)
		{
			var application = _ApplicationEntityFactory.GetApplicationByName(request.ApplicationName);
			if (application == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidApplicationName));
			}

			var applicationKey = _ApplicationKeyEntityFactory.GetApplicationKeyByApplicationIdAndName(application.Id, request.KeyName);
			if (applicationKey == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidKeyName));
			}

			applicationKey.Enabled = request.Enabled;
			_ApplicationKeyEntityFactory.UpdateApplicationKey(applicationKey);

			return (new EmptyResult(), null);
		}
	}
}

using System;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class CreateApplicationKeyOperation : IAsyncOperation<CreateApplicationKeyRequest, Guid>
	{
		private const int _MaxNameLength = 50;
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;

		public CreateApplicationKeyOperation(IApplicationEntityFactory applicationEntityFactory, IApplicationKeyEntityFactory applicationKeyEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_ApplicationKeyEntityFactory = applicationKeyEntityFactory ?? throw new ArgumentNullException(nameof(applicationKeyEntityFactory));
		}

		public async Task<(Guid output, OperationError error)> Execute(CreateApplicationKeyRequest request, CancellationToken cancellationToken)
		{
			var application = _ApplicationEntityFactory.GetApplicationByName(request.ApplicationName);
			if (application == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidApplicationName));
			}

			if (string.IsNullOrWhiteSpace(request.KeyName) || request.KeyName.Length > _MaxNameLength)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidKeyName));
			}

			var applicationKey = await _ApplicationKeyEntityFactory.GetApplicationKeyByApplicationIdAndName(application.Id, request.KeyName, cancellationToken).ConfigureAwait(false);
			if (applicationKey != null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidKeyName));
			}

			var key = Guid.NewGuid();
			await _ApplicationKeyEntityFactory.CreateApplicationKey(application.Id, request.KeyName, key, cancellationToken).ConfigureAwait(false);

			return (key, null);
		}
	}
}

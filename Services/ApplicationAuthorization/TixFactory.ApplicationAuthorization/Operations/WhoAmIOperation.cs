using System;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class WhoAmIOperation : IOperation<Guid, WhoAmIResult>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;

		public WhoAmIOperation(
			IApplicationEntityFactory applicationEntityFactory,
			IApplicationKeyEntityFactory applicationKeyEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_ApplicationKeyEntityFactory = applicationKeyEntityFactory ?? throw new ArgumentNullException(nameof(applicationKeyEntityFactory));
		}

		public (WhoAmIResult output, OperationError error) Execute(Guid applicationKey)
		{
			var targetApplicationKey = _ApplicationKeyEntityFactory.GetApplicationKey(applicationKey);
			if (targetApplicationKey == null || !targetApplicationKey.Enabled)
			{
				return (null, null);
			}

			var targetApplication = _ApplicationEntityFactory.GetApplicationById(targetApplicationKey.ApplicationId);
			var result = new WhoAmIResult
			{
				ApplicationName = targetApplication.Name,
				ApplicationKeyName = targetApplicationKey.Name
			};

			return (result, null);
		}
	}
}

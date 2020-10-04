using System;
using TixFactory.Logging;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorizations
{
	public class ApplicationAuthorizationsOperations : IApplicationAuthorizationsOperations
	{
		public IOperation<string, ServiceResult> GetServiceOperation { get; }

		public ApplicationAuthorizationsOperations(ILogger logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			GetServiceOperation = new GetServiceOperation();
		}
	}
}

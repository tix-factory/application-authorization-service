using System;
using TixFactory.ApplicationAuthorizations.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorizations
{
	internal class GetServiceOperation : IOperation<string, ServiceResult>
	{
		private readonly IServiceEntityFactory _ServiceEntityFactory;

		public GetServiceOperation(IServiceEntityFactory serviceEntityFactory)
		{
			_ServiceEntityFactory = serviceEntityFactory ?? throw new ArgumentNullException(nameof(serviceEntityFactory));
		}

		public (ServiceResult output, OperationError error) Execute(string serviceName)
		{
			var service = _ServiceEntityFactory.GetServiceByName(serviceName);

			ServiceResult result = null;
			if (service != null)
			{
				result = new ServiceResult
				{
					Name = service.Name,
					OperationNames = Array.Empty<string>(),
					Created = service.Created,
					Updated = service.Updated
				};
			}

			return (result, null);
		}
	}
}

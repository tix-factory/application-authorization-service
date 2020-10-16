using System;
using System.Linq;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class GetServiceOperation : IOperation<string, ServiceResult>
	{
		private readonly IServiceEntityFactory _ServiceEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;

		public GetServiceOperation(IServiceEntityFactory serviceEntityFactory, IOperationEntityFactory operationEntityFactory)
		{
			_ServiceEntityFactory = serviceEntityFactory ?? throw new ArgumentNullException(nameof(serviceEntityFactory));
			_OperationEntityFactory = operationEntityFactory ?? throw new ArgumentNullException(nameof(operationEntityFactory));
		}

		public (ServiceResult output, OperationError error) Execute(string serviceName)
		{
			var service = _ServiceEntityFactory.GetServiceByName(serviceName);

			ServiceResult result = null;
			if (service != null)
			{
				var operations = _OperationEntityFactory.GetOperations(service.Id);

				result = new ServiceResult
				{
					Name = service.Name,
					Operations = operations.Select(o => new OperationResult
					{
						Name = o.Name,
						Enabled = o.Enabled,
						Created = o.Created,
						Updated = o.Updated
					}).ToArray(),
					Created = service.Created,
					Updated = service.Updated
				};
			}

			return (result, null);
		}
	}
}

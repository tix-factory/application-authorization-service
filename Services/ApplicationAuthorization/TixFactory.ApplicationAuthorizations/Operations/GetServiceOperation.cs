using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorizations
{
	internal class GetServiceOperation : IOperation<string, ServiceResult>
	{
		public (ServiceResult output, OperationError error) Execute(string serviceName)
		{
			return (null, null);
		}
	}
}

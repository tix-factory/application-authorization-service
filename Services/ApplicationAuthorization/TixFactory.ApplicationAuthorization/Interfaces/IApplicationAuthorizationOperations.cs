using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationAuthorizationOperations
	{
		IOperation<string, ServiceResult> GetServiceOperation { get; }
	}
}

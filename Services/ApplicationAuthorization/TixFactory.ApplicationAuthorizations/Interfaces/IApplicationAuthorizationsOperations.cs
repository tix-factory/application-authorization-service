using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorizations
{
	public interface IApplicationAuthorizationsOperations
	{
		IOperation<string, ServiceResult> GetServiceOperation { get; }
	}
}

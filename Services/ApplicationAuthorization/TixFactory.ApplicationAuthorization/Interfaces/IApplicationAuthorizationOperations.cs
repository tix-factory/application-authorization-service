using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationAuthorizationOperations
	{
		IOperation<string, ApplicationResult> GetApplicationOperation { get; }
	}
}

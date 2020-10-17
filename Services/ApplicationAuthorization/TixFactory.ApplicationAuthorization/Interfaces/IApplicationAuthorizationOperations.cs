using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationAuthorizationOperations
	{
		IApplicationKeyValidator ApplicationKeyValidator { get; }

		IOperation<string, ApplicationResult> GetApplicationOperation { get; }
	}
}

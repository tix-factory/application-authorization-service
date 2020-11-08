using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationKeyValidator
	{
		/// <summary>
		/// Gets names of operations an ApiKey has access to against a specified application.
		/// </summary>
		/// <param name="targetApplicationName">The specified application name.</param>
		/// <param name="key">The ApiKey (<see cref="Guid"/>).</param>
		/// <returns>The names of the operations <paramref name="key"/> has access to for the target application.</returns>
		Task<ICollection<string>> GetAuthorizedOperations(string targetApplicationName, Guid key, CancellationToken cancellationToken);

		/// <summary>
		/// Gets names of operations an ApiKey has access to against a specified application.
		/// </summary>
		/// <param name="targetApplicationId">The specified application ID.</param>
		/// <param name="key">The ApiKey (<see cref="Guid"/>).</param>
		/// <returns>The names of the operations <paramref name="key"/> has access to for the target application.</returns>
		Task<ICollection<string>> GetAuthorizedOperations(long targetApplicationId, Guid key, CancellationToken cancellationToken);
	}
}

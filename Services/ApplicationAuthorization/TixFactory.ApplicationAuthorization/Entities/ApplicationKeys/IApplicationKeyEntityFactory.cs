using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal interface IApplicationKeyEntityFactory
	{
		Task<ApplicationKey> CreateApplicationKey(long applicationId, string name, Guid key, CancellationToken cancellationToken);

		Task<ApplicationKey> GetApplicationKey(Guid key, CancellationToken cancellationToken);

		Task<ApplicationKey> GetApplicationKeyByApplicationIdAndName(long applicationId, string name, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<ApplicationKey>> GetApplicationKeysByApplicationId(long applicationId, CancellationToken cancellationToken);

		Task UpdateApplicationKey(ApplicationKey applicationKey, CancellationToken cancellationToken);

		Task DeleteApplicationKey(ApplicationKey applicationKey, CancellationToken cancellationToken);
	}
}

using System;
using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal interface IApplicationKeyEntityFactory
	{
		ApplicationKey CreateApplicationKey(long applicationId, Guid key);

		ApplicationKey GetApplicationKey(Guid key);

		IReadOnlyCollection<ApplicationKey> GetApplicationKeysByApplicationId(long applicationId);

		void UpdateApplicationKey(ApplicationKey applicationKey);

		void DeleteApplicationKey(long id);
	}
}

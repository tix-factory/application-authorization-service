using System;
using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal interface IApplicationKeyEntityFactory
	{
		ApplicationKey CreateApplicationKey(long applicationId, string name, Guid key);

		ApplicationKey GetApplicationKey(Guid key);

		ApplicationKey GetApplicationKeyByApplicationIdAndName(long applicationId, string name);

		IReadOnlyCollection<ApplicationKey> GetApplicationKeysByApplicationId(long applicationId);

		void UpdateApplicationKey(ApplicationKey applicationKey);

		void DeleteApplicationKey(long id);
	}
}

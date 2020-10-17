using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal interface IApplicationEntityFactory
	{
		Application CreateApplication(string name);

		IReadOnlyCollection<Application> GetApplications();

		Application GetApplicationByName(string name);

		void UpdateApplication(Application application);

		void DeleteApplication(long id);
	}
}

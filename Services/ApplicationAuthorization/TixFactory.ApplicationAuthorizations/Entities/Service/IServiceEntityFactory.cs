using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorizations.Entities
{
	internal interface IServiceEntityFactory
	{
		Service CreateService(string name);

		IReadOnlyCollection<Service> GetServices();

		Service GetServiceByName(string name);

		void UpdateService(Service service);

		void DeleteService(long id);
	}
}

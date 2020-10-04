using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorizations
{
	[DataContract]
	public class ServiceResult
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "operationNames")]
		public IReadOnlyCollection<string> OperationNames { get; set; }
	}
}

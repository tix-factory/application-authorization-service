using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class ServiceResult
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "operationNames")]
		public IReadOnlyCollection<string> OperationNames { get; set; }

		[DataMember(Name = "updated")]
		public DateTime Updated { get; set; }

		[DataMember(Name = "created")]
		public DateTime Created { get; set; }
	}
}

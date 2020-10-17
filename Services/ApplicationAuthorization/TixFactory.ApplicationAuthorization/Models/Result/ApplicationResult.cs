using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class ApplicationResult
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "operations")]
		public IReadOnlyCollection<OperationResult> Operations { get; set; }

		[DataMember(Name = "updated")]
		public DateTime Updated { get; set; }

		[DataMember(Name = "created")]
		public DateTime Created { get; set; }
	}
}

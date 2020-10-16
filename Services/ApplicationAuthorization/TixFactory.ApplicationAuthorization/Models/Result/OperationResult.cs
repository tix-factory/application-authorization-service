using System;
using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class OperationResult
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "enabled")]
		public bool Enabled { get; set; }

		[DataMember(Name = "updated")]
		public DateTime Updated { get; set; }

		[DataMember(Name = "created")]
		public DateTime Created { get; set; }
	}
}

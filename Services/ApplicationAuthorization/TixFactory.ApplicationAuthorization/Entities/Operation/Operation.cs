using System;
using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization.Entities
{
	[DataContract(Name = "operations")]
	internal class Operation
	{
		[DataMember(Name = "ID")]
		public long Id { get; set; }

		[DataMember(Name = "ServiceID")]
		public long ServiceId { get; set; }

		[DataMember(Name = "Name")]
		public string Name { get; set; }

		[DataMember(Name = "enabled")]
		public bool Enabled { get; set; }

		[DataMember(Name = "Updated")]
		public DateTime Updated { get; set; }

		[DataMember(Name = "Created")]
		public DateTime Created { get; set; }
	}
}

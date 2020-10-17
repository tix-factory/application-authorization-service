using System;
using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization.Entities
{
	[DataContract(Name = "applications-keys")]
	internal class ApplicationKey
	{
		[DataMember(Name = "ID")]
		public long Id { get; set; }

		[DataMember(Name = "ApplicationID")]
		public long ApplicationId { get; set; }

		[DataMember(Name = "KeyHash")]
		public string KeyHash { get; set; }

		[DataMember(Name = "enabled")]
		public bool Enabled { get; set; }

		[DataMember(Name = "Updated")]
		public DateTime Updated { get; set; }

		[DataMember(Name = "Created")]
		public DateTime Created { get; set; }
	}
}
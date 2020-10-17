using System;
using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization.Entities
{
	[DataContract(Name = "application-operation-authorizations")]
	internal class ApplicationOperationAuthorization
	{
		[DataMember(Name = "ID")]
		public long Id { get; set; }

		[DataMember(Name = "ApplicationID")]
		public long ApplicationId { get; set; }

		[DataMember(Name = "OperationID")]
		public long OperationId { get; set; }

		[DataMember(Name = "Updated")]
		public DateTime Updated { get; set; }

		[DataMember(Name = "Created")]
		public DateTime Created { get; set; }
	}
}

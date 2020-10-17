using System;
using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class GetAuthorizedOperationsRequest
	{
		[DataMember(Name = "apiKey")]
		public Guid ApiKey { get; set; }

		[DataMember(Name = "targetApplicationApiKey")]
		public Guid TargetApplicationKey { get; set; }
	}
}

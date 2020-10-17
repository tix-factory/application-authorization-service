using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class ToggleOperationEnabledRequest
	{
		[DataMember(Name = "applicationName")]
		public string ApplicationName { get; set; }

		[DataMember(Name = "operationName")]
		public string OperationName { get; set; }

		[DataMember(Name = "enabled")]
		public bool Enabled { get; set; }
	}
}

using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class ToggleOperationAuthorizationRequest
	{
		[DataMember(Name = "applicationName")]
		public string ApplicationName { get; set; }

		[DataMember(Name = "targetApplicationName")]
		public string TargetApplicationName { get; set; }

		[DataMember(Name = "targetOperationName")]
		public string TargetOperationName { get; set; }

		[DataMember(Name = "authorized")]
		public bool Authorized { get; set; }
	}
}

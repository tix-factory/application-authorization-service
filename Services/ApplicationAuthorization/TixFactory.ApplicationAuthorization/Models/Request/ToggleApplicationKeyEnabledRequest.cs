using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class ToggleApplicationKeyEnabledRequest
	{
		[DataMember(Name = "applicationName")]
		public string ApplicationName { get; set; }

		[DataMember(Name = "keyName")]
		public string KeyName { get; set; }

		[DataMember(Name = "enabled")]
		public bool Enabled { get; set; }
	}
}

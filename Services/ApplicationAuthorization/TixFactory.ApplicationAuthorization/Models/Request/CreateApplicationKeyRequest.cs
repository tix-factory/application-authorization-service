using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class CreateApplicationKeyRequest
	{
		[DataMember(Name = "applicationName")]
		public string ApplicationName { get; set; }

		[DataMember(Name = "keyName")]
		public string KeyName { get; set; }
	}
}

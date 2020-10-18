using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class WhoAmIResult
	{
		[DataMember(Name = "applicationName")]
		public string ApplicationName { get; set; }

		[DataMember(Name = "applicationKeyName")]
		public string ApplicationKeyName { get; set; }
	}
}

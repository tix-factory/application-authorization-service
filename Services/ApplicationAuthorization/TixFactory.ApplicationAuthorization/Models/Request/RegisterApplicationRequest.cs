using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class RegisterApplicationRequest
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
	}
}

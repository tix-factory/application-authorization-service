using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
	[DataContract]
	public class RegisterOperationRequest
	{
		[DataMember(Name = "applicationName")]
		public string ApplicationName { get; set; }

		[DataMember(Name = "operationName")]
		public string OperationName { get; set; }
	}
}

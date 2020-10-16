using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization.Entities
{
	[DataContract]
	internal class InsertResult<T>
	{
		[DataMember(Name = "ID")]
		public T Id { get; set; }
	}
}

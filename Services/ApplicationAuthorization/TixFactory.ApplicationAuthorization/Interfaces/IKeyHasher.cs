using System;

namespace TixFactory.ApplicationAuthorization
{
	public interface IKeyHasher
	{
		string HashKey(Guid key);
	}
}

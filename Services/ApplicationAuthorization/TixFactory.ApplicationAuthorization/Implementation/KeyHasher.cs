using System;
using System.Security.Cryptography;

namespace TixFactory.ApplicationAuthorization
{
	internal class KeyHasher : IKeyHasher
	{
		private readonly HashAlgorithm _SHA256;

		public KeyHasher()
		{
			_SHA256 = SHA256.Create();
		}

		public string HashKey(Guid key)
		{
			var keyHash = string.Empty;
			var sha256Bytes = _SHA256.ComputeHash(key.ToByteArray());

			foreach (var b in sha256Bytes)
			{
				keyHash += $"{b:X2}";
			}

			return keyHash;
		}
	}
}

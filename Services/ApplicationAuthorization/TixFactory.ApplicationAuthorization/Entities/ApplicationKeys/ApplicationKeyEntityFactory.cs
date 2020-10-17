using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using TixFactory.Collections;

namespace TixFactory.ApplicationAuthorization.Entities
{
	internal class ApplicationKeyEntityFactory : IApplicationKeyEntityFactory
	{
		private const string _InsertApplicationKeyStoredProcedure = "InsertApplicationKey";
		private const string _GetApplicationKeyByKeyHashStoredProcedureName = "GetApplicationKeyByKeyHash";
		private const string _GetApplicationKeysByApplicationIdStoredProcedureName = "GetApplicationKeysByApplicationId";
		private const string _UpdateApplicationKeyStoredProcedureName = "UpdateApplicationKey";
		private const string _DeleteApplicationKeyStoredProcedureName = "DeleteApplicationKey";
		private const int _ApplicationKeysMaxCount = 1000;
		private static readonly TimeSpan _ApplicationKeyCacheTime = TimeSpan.FromMinutes(1);
		private readonly IDatabaseConnection _DatabaseConnection;
		private readonly ExpirableDictionary<long, IReadOnlyCollection<ApplicationKey>> _ApplicationKeysByApplicationId;
		private readonly ExpirableDictionary<string, ApplicationKey> _ApplicationKeysByKeyHash;
		private readonly HashAlgorithm _SHA256;

		public ApplicationKeyEntityFactory(IDatabaseConnection databaseConnection)
		{
			_DatabaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
			_ApplicationKeysByApplicationId = new ExpirableDictionary<long, IReadOnlyCollection<ApplicationKey>>(_ApplicationKeyCacheTime, ExpirationPolicy.RenewOnWrite);
			_ApplicationKeysByKeyHash = new ExpirableDictionary<string, ApplicationKey>(_ApplicationKeyCacheTime, ExpirationPolicy.RenewOnWrite);
			_SHA256 = SHA256.Create();
		}

		public ApplicationKey CreateApplicationKey(long applicationId, Guid key)
		{
			var applicationKeys = GetApplicationKeysByApplicationId(applicationId);
			if (applicationKeys.Count >= _ApplicationKeysMaxCount)
			{
				throw new ApplicationException($"Cannot create more than {_ApplicationKeysMaxCount} application keys per application.");
			}

			var keyHash = HashKey(key);
			var applicationKeyId = _DatabaseConnection.ExecuteInsertStoredProcedure<long>(_InsertApplicationKeyStoredProcedure, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_KeyHash", keyHash),
				new MySqlParameter("@_Enabled", false)
			});

			_ApplicationKeysByApplicationId.Remove(applicationId);

			applicationKeys = GetApplicationKeysByApplicationId(applicationId);
			return applicationKeys.First(k => k.Id == applicationKeyId);
		}

		public ApplicationKey GetApplicationKey(Guid key)
		{
			var keyHash = HashKey(key);
			if (_ApplicationKeysByKeyHash.TryGetValue(keyHash, out var applicaitonKey))
			{
				return applicaitonKey;
			}

			var applicationKeys = _DatabaseConnection.ExecuteReadStoredProcedure<ApplicationKey>(_GetApplicationKeyByKeyHashStoredProcedureName, new[]
			{
				new MySqlParameter("@_KeyHash", keyHash)
			});

			_ApplicationKeysByKeyHash[keyHash] = applicaitonKey = applicationKeys.First();
			return applicaitonKey;
		}

		public IReadOnlyCollection<ApplicationKey> GetApplicationKeysByApplicationId(long applicationId)
		{
			if (_ApplicationKeysByApplicationId.TryGetValue(applicationId, out var applicationKeys))
			{
				return applicationKeys;
			}

			applicationKeys = _ApplicationKeysByApplicationId[applicationId] = _DatabaseConnection.ExecuteReadStoredProcedure<ApplicationKey>(_GetApplicationKeysByApplicationIdStoredProcedureName, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_Count", _ApplicationKeysMaxCount)
			});

			return applicationKeys;
		}

		public void UpdateApplicationKey(ApplicationKey applicationKey)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_UpdateApplicationKeyStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", applicationKey.Id),
				new MySqlParameter("@_ApplicationID", applicationKey.ApplicationId),
				new MySqlParameter("@_KeyHash", applicationKey.KeyHash),
				new MySqlParameter("@_Enabled", applicationKey.Enabled)
			});
		}

		public void DeleteApplicationKey(long id)
		{
			_DatabaseConnection.ExecuteWriteStoredProcedure(_DeleteApplicationKeyStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", id)
			});
		}

		private string HashKey(Guid key)
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

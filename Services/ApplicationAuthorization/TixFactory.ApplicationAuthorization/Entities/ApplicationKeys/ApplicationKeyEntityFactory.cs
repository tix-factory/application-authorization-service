using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TixFactory.Collections;
using TixFactory.Data.MySql;

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
		private readonly IKeyHasher _KeyHasher;
		private readonly ExpirableDictionary<long, IReadOnlyCollection<ApplicationKey>> _ApplicationKeysByApplicationId;
		private readonly ExpirableDictionary<string, ApplicationKey> _ApplicationKeysByKeyHash;

		public ApplicationKeyEntityFactory(IDatabaseConnection databaseConnection, IKeyHasher keyHasher)
		{
			_DatabaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
			_KeyHasher = keyHasher ?? throw new ArgumentNullException(nameof(keyHasher));
			_ApplicationKeysByApplicationId = new ExpirableDictionary<long, IReadOnlyCollection<ApplicationKey>>(_ApplicationKeyCacheTime, ExpirationPolicy.RenewOnWrite);
			_ApplicationKeysByKeyHash = new ExpirableDictionary<string, ApplicationKey>(_ApplicationKeyCacheTime, ExpirationPolicy.RenewOnWrite);
		}

		public async Task<ApplicationKey> CreateApplicationKey(long applicationId, string name, Guid key, CancellationToken cancellationToken)
		{
			var applicationKeys = await GetApplicationKeysByApplicationId(applicationId, cancellationToken).ConfigureAwait(false);
			if (applicationKeys.Count >= _ApplicationKeysMaxCount)
			{
				throw new ApplicationException($"Cannot create more than {_ApplicationKeysMaxCount} application keys per application.");
			}

			var keyHash = _KeyHasher.HashKey(key);
			var applicationKeyId = await _DatabaseConnection.ExecuteInsertStoredProcedureAsync<long>(_InsertApplicationKeyStoredProcedure, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_Name", name),
				new MySqlParameter("@_KeyHash", keyHash),
				new MySqlParameter("@_Enabled", true)
			}, cancellationToken).ConfigureAwait(false);

			_ApplicationKeysByApplicationId.Remove(applicationId);

			applicationKeys = await GetApplicationKeysByApplicationId(applicationId, cancellationToken).ConfigureAwait(false);
			return applicationKeys.First(k => k.Id == applicationKeyId);
		}

		public async Task<ApplicationKey> GetApplicationKey(Guid key, CancellationToken cancellationToken)
		{
			var keyHash = _KeyHasher.HashKey(key);
			if (_ApplicationKeysByKeyHash.TryGetValue(keyHash, out var applicaitonKey))
			{
				return applicaitonKey;
			}

			var applicationKeys = await _DatabaseConnection.ExecuteReadStoredProcedureAsync<ApplicationKey>(_GetApplicationKeyByKeyHashStoredProcedureName, new[]
			{
				new MySqlParameter("@_KeyHash", keyHash)
			}, cancellationToken).ConfigureAwait(false);

			_ApplicationKeysByKeyHash[keyHash] = applicaitonKey = applicationKeys.FirstOrDefault();
			return applicaitonKey;
		}

		public async Task<ApplicationKey> GetApplicationKeyByApplicationIdAndName(long applicationId, string name, CancellationToken cancellationToken)
		{
			var applicationKeys = await GetApplicationKeysByApplicationId(applicationId, cancellationToken).ConfigureAwait(false);
			return applicationKeys.FirstOrDefault(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		public async Task<IReadOnlyCollection<ApplicationKey>> GetApplicationKeysByApplicationId(long applicationId, CancellationToken cancellationToken)
		{
			if (_ApplicationKeysByApplicationId.TryGetValue(applicationId, out var applicationKeys))
			{
				return applicationKeys;
			}

			applicationKeys = _ApplicationKeysByApplicationId[applicationId] = await _DatabaseConnection.ExecuteReadStoredProcedureAsync<ApplicationKey>(_GetApplicationKeysByApplicationIdStoredProcedureName, new[]
			{
				new MySqlParameter("@_ApplicationID", applicationId),
				new MySqlParameter("@_Count", _ApplicationKeysMaxCount)
			}, cancellationToken).ConfigureAwait(false);

			return applicationKeys;
		}

		public Task UpdateApplicationKey(ApplicationKey applicationKey, CancellationToken cancellationToken)
		{
			return _DatabaseConnection.ExecuteWriteStoredProcedureAsync(_UpdateApplicationKeyStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", applicationKey.Id),
				new MySqlParameter("@_ApplicationID", applicationKey.ApplicationId),
				new MySqlParameter("@_Name", applicationKey.Name),
				new MySqlParameter("@_KeyHash", applicationKey.KeyHash),
				new MySqlParameter("@_Enabled", applicationKey.Enabled)
			}, cancellationToken);
		}

		public async Task DeleteApplicationKey(ApplicationKey applicationKey, CancellationToken cancellationToken)
		{
			await _DatabaseConnection.ExecuteWriteStoredProcedureAsync(_DeleteApplicationKeyStoredProcedureName, new[]
			{
				new MySqlParameter("@_ID", applicationKey.Id)
			}, cancellationToken).ConfigureAwait(false);

			_ApplicationKeysByApplicationId.Remove(applicationKey.ApplicationId);
			_ApplicationKeysByKeyHash.Remove(applicationKey.KeyHash);
		}
	}
}

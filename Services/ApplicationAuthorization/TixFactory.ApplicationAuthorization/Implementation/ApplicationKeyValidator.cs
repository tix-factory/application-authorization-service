﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.ApplicationAuthorization.Entities;

namespace TixFactory.ApplicationAuthorization
{
	internal class ApplicationKeyValidator : IApplicationKeyValidator
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;
		private readonly IApplicationOperationAuthorizationEntityFactory _ApplicationOperationAuthorizationEntityFactory;

		public ApplicationKeyValidator(
			IApplicationEntityFactory applicationEntityFactory,
			IOperationEntityFactory operationEntityFactory,
			IApplicationKeyEntityFactory applicationKeyEntityFactory,
			IApplicationOperationAuthorizationEntityFactory applicationOperationAuthorizationEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_OperationEntityFactory = operationEntityFactory ?? throw new ArgumentNullException(nameof(operationEntityFactory));
			_ApplicationKeyEntityFactory = applicationKeyEntityFactory ?? throw new ArgumentNullException(nameof(applicationKeyEntityFactory));
			_ApplicationOperationAuthorizationEntityFactory = applicationOperationAuthorizationEntityFactory ?? throw new ArgumentNullException(nameof(applicationOperationAuthorizationEntityFactory));
		}

		public async Task<ICollection<string>> GetAuthorizedOperations(string targetApplicationName, Guid key, CancellationToken cancellationToken)
		{
			var targetApplication = _ApplicationEntityFactory.GetApplicationByName(targetApplicationName);
			if (targetApplication == null)
			{
				return Array.Empty<string>();
			}

			return await GetAuthorizedOperations(targetApplication.Id, key, cancellationToken).ConfigureAwait(false);
		}

		public async Task<ICollection<string>> GetAuthorizedOperations(long targetApplicationId, Guid key, CancellationToken cancellationToken)
		{
			var applicationKey = await _ApplicationKeyEntityFactory.GetApplicationKey(key, cancellationToken).ConfigureAwait(false);
			if (applicationKey == null || !applicationKey.Enabled)
			{
				return Array.Empty<string>();
			}

			var targetApplicationOperations = _OperationEntityFactory.GetOperations(targetApplicationId);
			if (!targetApplicationOperations.Any())
			{
				return Array.Empty<string>();
			}

			var operationNames = new List<string>();
			var operationMap = targetApplicationOperations.Where(o => o.Enabled).ToDictionary(o => o.Id, o => o.Name);
			var applicationOperationAuthorizations = await _ApplicationOperationAuthorizationEntityFactory.GetApplicationOperationAuthorizationsByApplicationId(applicationKey.ApplicationId, cancellationToken).ConfigureAwait(false);

			foreach (var applicationOperationAuthorization in applicationOperationAuthorizations)
			{
				if (operationMap.TryGetValue(applicationOperationAuthorization.OperationId, out var operationName))
				{
					operationNames.Add(operationName);
				}
			}

			return operationNames;
		}
	}
}

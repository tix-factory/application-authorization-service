﻿using System;
using System.Collections.Generic;
using System.Linq;
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

		public ICollection<string> GetAuthorizedOperations(string targetApplicationName, Guid key)
		{
			var targetApplication = _ApplicationEntityFactory.GetApplicationByName(targetApplicationName);
			if (targetApplication == null)
			{
				return Array.Empty<string>();
			}

			var applicationKey = _ApplicationKeyEntityFactory.GetApplicationKey(key);
			if (applicationKey == null)
			{
				return Array.Empty<string>();
			}

			var targetApplicationOperations = _OperationEntityFactory.GetOperations(targetApplication.Id);
			if (!targetApplicationOperations.Any())
			{
				return Array.Empty<string>();
			}

			var operationNames = new List<string>();
			var operationMap = targetApplicationOperations.ToDictionary(o => o.Id, o => o.Name);
			var applicationOperationAuthorizations = _ApplicationOperationAuthorizationEntityFactory.GetApplicationOperationAuthorizationsByApplicationId(targetApplication.Id);

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
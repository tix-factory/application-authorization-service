﻿using System;
using System.Linq;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class GetApplicationOperation : IOperation<string, ApplicationResult>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;

		public GetApplicationOperation(IApplicationEntityFactory applicationEntityFactory, IOperationEntityFactory operationEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_OperationEntityFactory = operationEntityFactory ?? throw new ArgumentNullException(nameof(operationEntityFactory));
		}

		public (ApplicationResult output, OperationError error) Execute(string applicationName)
		{
			var application = _ApplicationEntityFactory.GetApplicationByName(applicationName);

			ApplicationResult result = null;
			if (application != null)
			{
				var operations = _OperationEntityFactory.GetOperations(application.Id);

				result = new ApplicationResult
				{
					Name = application.Name,
					Operations = operations.Select(o => new OperationResult
					{
						Name = o.Name,
						Enabled = o.Enabled,
						Created = o.Created,
						Updated = o.Updated
					}).ToArray(),
					Created = application.Created,
					Updated = application.Updated
				};
			}

			return (result, null);
		}
	}
}
﻿using System;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	internal class ToggleOperationEnabledOperation : IAsyncOperation<ToggleOperationEnabledRequest, EmptyResult>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IOperationEntityFactory _OperationEntityFactory;

		public ToggleOperationEnabledOperation(IApplicationEntityFactory applicationEntityFactory, IOperationEntityFactory operationEntityFactory)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_OperationEntityFactory = operationEntityFactory ?? throw new ArgumentNullException(nameof(operationEntityFactory));
		}

		public async Task<(EmptyResult output, OperationError error)> Execute(ToggleOperationEnabledRequest request, CancellationToken cancellationToken)
		{
			var application = _ApplicationEntityFactory.GetApplicationByName(request.ApplicationName);
			if (application == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidApplicationName));
			}

			var operation = await _OperationEntityFactory.GetOperationByName(application.Id, request.OperationName, cancellationToken).ConfigureAwait(false);
			if (operation == null)
			{
				return (default, new OperationError(ApplicationAuthorizationError.InvalidOperationName));
			}

			if (operation.Enabled != request.Enabled)
			{
				operation.Enabled = request.Enabled;
				await _OperationEntityFactory.UpdateOperation(operation, cancellationToken).ConfigureAwait(false);
			}

			return (new EmptyResult(), null);
		}
	}
}

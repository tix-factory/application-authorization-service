﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.ApplicationAuthorization.Entities;
using TixFactory.Operations;

namespace TixFactory.ApplicationAuthorization
{
	/// <summary>
	/// Retrieves names of operations authorized for a target application, for the given ApiKey.
	/// </summary>
	/// <remarks>
	/// As application (specified by ApiKey header, <see cref="GetAuthorizedOperationsRequest.TargetApplicationKey"/>),
	/// I have received a request with this ApiKey: <see cref="GetAuthorizedOperationsRequest.ApiKey"/>.
	/// Please tell me which operations the ApiKey I have received has access to.
	/// </remarks>
	internal class GetAuthorizedOperationsOperation : IAsyncOperation<GetAuthorizedOperationsRequest, ICollection<string>>
	{
		private readonly IApplicationEntityFactory _ApplicationEntityFactory;
		private readonly IApplicationKeyEntityFactory _ApplicationKeyEntityFactory;
		private readonly IApplicationKeyValidator _ApplicationKeyValidator;

		public GetAuthorizedOperationsOperation(
			IApplicationEntityFactory applicationEntityFactory,
			IApplicationKeyEntityFactory applicationKeyEntityFactory,
			IApplicationKeyValidator applicationKeyValidator)
		{
			_ApplicationEntityFactory = applicationEntityFactory ?? throw new ArgumentNullException(nameof(applicationEntityFactory));
			_ApplicationKeyEntityFactory = applicationKeyEntityFactory ?? throw new ArgumentNullException(nameof(applicationKeyEntityFactory));
			_ApplicationKeyValidator = applicationKeyValidator ?? throw new ArgumentNullException(nameof(applicationKeyValidator));
		}

		public async Task<(ICollection<string> output, OperationError error)> Execute(GetAuthorizedOperationsRequest request, CancellationToken cancellationToken)
		{
			var targetApplicationKey = await _ApplicationKeyEntityFactory.GetApplicationKey(request.TargetApplicationKey, cancellationToken).ConfigureAwait(false);
			if (targetApplicationKey == null || !targetApplicationKey.Enabled)
			{
				return (Array.Empty<string>(), null);
			}

			var targetApplication = _ApplicationEntityFactory.GetApplicationById(targetApplicationKey.ApplicationId);
			var authorizedOperations = await _ApplicationKeyValidator.GetAuthorizedOperations(targetApplication.Id, request.ApiKey, cancellationToken).ConfigureAwait(false);
			return (authorizedOperations, null);
		}
	}
}

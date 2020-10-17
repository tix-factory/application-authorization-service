using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TixFactory.Http.Service;

namespace TixFactory.ApplicationAuthorization.Service.Controllers
{
	[Route("v1/[action]")]
	public class ApplicationAuthorizationController : Controller
	{
		private const string _ApiKeyHeaderName = "Tix-Factory-Api-Key";
		private readonly IApplicationAuthorizationOperations _ApplicationAuthorizationOperations;
		private readonly IOperationExecuter _OperationExecuter;

		public ApplicationAuthorizationController(IApplicationAuthorizationOperations applicationAuthorizationOperations, IOperationExecuter operationExecuter)
		{
			_ApplicationAuthorizationOperations = applicationAuthorizationOperations ?? throw new ArgumentNullException(nameof(applicationAuthorizationOperations));
			_OperationExecuter = operationExecuter ?? throw new ArgumentNullException(nameof(operationExecuter));
		}

		[HttpPost]
		public IActionResult GetApplication([FromBody] RequestPayload<string> request)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.GetApplicationOperation, request.Data);
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult GetAuthorizedOperations([FromBody] GetAuthorizedOperationsRequest request)
		{
			if (Request.Headers.TryGetValue(_ApiKeyHeaderName, out var rawApiKey) && Guid.TryParse(rawApiKey, out var apiKey))
			{
				request.TargetApplicationKey = apiKey;
			}
			else
			{
				request.TargetApplicationKey = default;
			}

			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.GetAuthorizedOperationsOperation, request);
		}
	}
}

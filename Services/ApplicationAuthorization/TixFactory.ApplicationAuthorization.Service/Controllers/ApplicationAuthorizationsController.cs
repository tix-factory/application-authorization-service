using System;
using Microsoft.AspNetCore.Mvc;
using TixFactory.Http.Service;

namespace TixFactory.ApplicationAuthorization.Service.Controllers
{
	[Route("v1/[action]")]
	public class ApplicationAuthorizationController
	{
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
	}
}

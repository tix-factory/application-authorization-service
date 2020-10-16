using System;
using Microsoft.AspNetCore.Mvc;
using TixFactory.Http.Service;

namespace TixFactory.ApplicationAuthorization.Service.Controllers
{
	[Route("v1/[action]")]
	public class ApplicationAuthorizationsController
	{
		private readonly IApplicationAuthorizationOperations _ApplicationAuthorizationOperations;
		private readonly IOperationExecuter _OperationExecuter;

		public ApplicationAuthorizationsController(IApplicationAuthorizationOperations applicationAuthorizationsOperations, IOperationExecuter operationExecuter)
		{
			_ApplicationAuthorizationOperations = applicationAuthorizationsOperations ?? throw new ArgumentNullException(nameof(applicationAuthorizationsOperations));
			_OperationExecuter = operationExecuter ?? throw new ArgumentNullException(nameof(operationExecuter));
		}

		[HttpPost]
		public IActionResult GetService([FromBody] RequestPayload<string> request)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.GetServiceOperation, request.Data);
		}
	}
}

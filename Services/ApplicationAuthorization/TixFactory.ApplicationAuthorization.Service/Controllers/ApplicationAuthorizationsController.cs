using System;
using Microsoft.AspNetCore.Mvc;
using TixFactory.ApplicationAuthorizations;
using TixFactory.Http.Service;

namespace TixFactory.ApplicationAuthorization.Service.Controllers
{
	[Route("v1/[action]")]
	public class ApplicationAuthorizationsController
	{
		private readonly IApplicationAuthorizationsOperations _ApplicationAuthorizationsOperations;
		private readonly IOperationExecuter _OperationExecuter;

		public ApplicationAuthorizationsController(IApplicationAuthorizationsOperations applicationAuthorizationsOperations, IOperationExecuter operationExecuter)
		{
			_ApplicationAuthorizationsOperations = applicationAuthorizationsOperations ?? throw new ArgumentNullException(nameof(applicationAuthorizationsOperations));
			_OperationExecuter = operationExecuter ?? throw new ArgumentNullException(nameof(operationExecuter));
		}

		[HttpPost]
		public IActionResult GetService([FromBody] RequestPayload<string> request)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationsOperations.GetServiceOperation, request.Data);
		}
	}
}

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
		public IActionResult RegisterApplication([FromBody] RegisterApplicationRequest request)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.RegisterApplicationOperation, request);
		}

		[HttpPost]
		public IActionResult RegisterOperation([FromBody] RegisterOperationRequest request)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.RegisterOperationOperation, request);
		}
		
		[HttpPost]
		public IActionResult ToggleOperationEnabled([FromBody] ToggleOperationEnabledRequest request)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.ToggleOperationEnabledOperation, request);
		}

		[HttpPost]
		public IActionResult CreateApplicationKey([FromBody] CreateApplicationKeyRequest request)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.CreateApplicationKeyOperation, request);
		}

		[HttpPost]
		public IActionResult DeleteApplicationKey([FromBody] DeleteApplicationKeyRequest request)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.DeleteApplicationKeyOperation, request);
		}

		[HttpPost]
		public IActionResult ToggleApplicationKeyEnabled([FromBody] ToggleApplicationKeyEnabledRequest request)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.ToggleApplicationKeyEnabledOperation, request);
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

		[HttpPost]
		[AllowAnonymous]
		public IActionResult WhoAmI([FromHeader(Name = _ApiKeyHeaderName)] Guid apiKey)
		{
			return _OperationExecuter.Execute(_ApplicationAuthorizationOperations.WhoAmIOperation, apiKey);
		}
	}
}

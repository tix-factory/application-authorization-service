using System;
using System.Threading;
using System.Threading.Tasks;
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
		public Task<IActionResult> GetApplication([FromBody] RequestPayload<string> request, CancellationToken cancellationToken)
		{
			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.GetApplicationOperation, request.Data, cancellationToken);
		}

		[HttpPost]
		public Task<IActionResult> RegisterApplication([FromBody] RegisterApplicationRequest request, CancellationToken cancellationToken)
		{
			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.RegisterApplicationOperation, request, cancellationToken);
		}

		[HttpPost]
		public Task<IActionResult> RegisterOperation([FromBody] RegisterOperationRequest request, CancellationToken cancellationToken)
		{
			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.RegisterOperationOperation, request, cancellationToken);
		}

		[HttpPost]
		public Task<IActionResult> ToggleOperationEnabled([FromBody] ToggleOperationEnabledRequest request, CancellationToken cancellationToken)
		{
			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.ToggleOperationEnabledOperation, request, cancellationToken);
		}

		[HttpPost]
		public Task<IActionResult> CreateApplicationKey([FromBody] CreateApplicationKeyRequest request, CancellationToken cancellationToken)
		{
			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.CreateApplicationKeyOperation, request, cancellationToken);
		}

		[HttpPost]
		public Task<IActionResult> DeleteApplicationKey([FromBody] DeleteApplicationKeyRequest request, CancellationToken cancellationToken)
		{
			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.DeleteApplicationKeyOperation, request, cancellationToken);
		}

		[HttpPost]
		public Task<IActionResult> ToggleApplicationKeyEnabled([FromBody] ToggleApplicationKeyEnabledRequest request, CancellationToken cancellationToken)
		{
			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.ToggleApplicationKeyEnabledOperation, request, cancellationToken);
		}

		[HttpPost]
		[AllowAnonymous]
		public Task<IActionResult> GetAuthorizedOperations([FromBody] GetAuthorizedOperationsRequest request, CancellationToken cancellationToken)
		{
			if (Request.Headers.TryGetValue(_ApiKeyHeaderName, out var rawApiKey) && Guid.TryParse(rawApiKey, out var apiKey))
			{
				request.TargetApplicationKey = apiKey;
			}
			else
			{
				request.TargetApplicationKey = default;
			}

			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.GetAuthorizedOperationsOperation, request, cancellationToken);
		}

		[HttpPost]
		public Task<IActionResult> ToggleOperationAuthorization([FromBody] ToggleOperationAuthorizationRequest request, CancellationToken cancellationToken)
		{
			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.ToggleOperationAuthorizationOperation, request, cancellationToken);
		}

		[HttpPost]
		[AllowAnonymous]
		public Task<IActionResult> WhoAmI([FromHeader(Name = _ApiKeyHeaderName)] Guid apiKey, CancellationToken cancellationToken)
		{
			return _OperationExecuter.ExecuteAsync(_ApplicationAuthorizationOperations.WhoAmIOperation, apiKey, cancellationToken);
		}
	}
}

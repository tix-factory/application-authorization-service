using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using TixFactory.ApplicationAuthorization.Service.Controllers;
using TixFactory.ApplicationContext;

namespace TixFactory.ApplicationAuthorization.Service
{
	public class ValidateApiKeyAttribute : ActionFilterAttribute
	{
		private const string _ApiKeyHeaderName = "Tix-Factory-Api-Key";
		private readonly IApplicationContext _ApplicationContext;
		private readonly IApplicationKeyValidator _ApplicationKeyValidator;
		private readonly ISet<Type> _AuthenticatedControllerTypes;

		public ValidateApiKeyAttribute(IApplicationContext applicationContext, IApplicationKeyValidator applicationKeyValidator)
		{
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
			_ApplicationKeyValidator = applicationKeyValidator ?? throw new ArgumentNullException(nameof(applicationKeyValidator));

			_AuthenticatedControllerTypes = new HashSet<Type>(new[]
			{
				typeof(ApplicationAuthorizationController)
			});
		}

		public override async Task OnActionExecutionAsync(
			ActionExecutingContext context,
			ActionExecutionDelegate next)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (next == null)
			{
				throw new ArgumentNullException(nameof(next));
			}

			await OnActionExecutingAsync(context, context.HttpContext.RequestAborted).ConfigureAwait(false);
			if (context.Result == null)
			{
				OnActionExecuted(await next().ConfigureAwait(false));
			}
		}

		private async Task OnActionExecutingAsync(ActionExecutingContext actionContext, CancellationToken cancellationToken)
		{
			if (!ShouldValidateApiKey(actionContext))
			{
				// Action does not require authorization.
				return;
			}

			var validated = await TryValidateApiKey(actionContext, cancellationToken).ConfigureAwait(false);
			if (!validated)
			{
				actionContext.Result = new UnauthorizedResult();
			}
		}

		private bool ShouldValidateApiKey(ActionExecutingContext actionContext)
		{
			if (!_AuthenticatedControllerTypes.Contains(actionContext.Controller.GetType()))
			{
				// Controller does not require ApiKey validation.
				return false;
			}

			if (actionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
			{
				var allowAnonymousAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(
					attributeType: typeof(AllowAnonymousAttribute),
					inherit: true);
				return !allowAnonymousAttributes.Any();
			}

			return true;
		}

		private async Task<bool> TryValidateApiKey(ActionExecutingContext actionContext, CancellationToken cancellationToken)
		{
			if (actionContext.HttpContext.Request.Headers.TryGetValue(_ApiKeyHeaderName, out var rawApiKey)
				   && Guid.TryParse(rawApiKey, out var apiKey)
				   && actionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
			{
				var authorizedOperationNames = await _ApplicationKeyValidator.GetAuthorizedOperations(_ApplicationContext.Name, apiKey, cancellationToken).ConfigureAwait(false);
				return authorizedOperationNames.Contains(controllerActionDescriptor.ActionName);
			}

			return false;
		}
	}
}

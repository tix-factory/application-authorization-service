using System;
using System.Collections.Generic;
using System.Linq;
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

		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			if (!ShouldValidateApiKey(actionContext))
			{
				// Action does not require authorization.
				return;
			}

			if (!TryValidateApiKey(actionContext))
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
				var allowAnonymousAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(attributeType: typeof(AllowAnonymousAttribute), inherit: true);
				return !allowAnonymousAttributes.Any();
			}

			return true;
		}

		private bool TryValidateApiKey(ActionExecutingContext actionContext)
		{
			if (actionContext.HttpContext.Request.Headers.TryGetValue(_ApiKeyHeaderName, out var rawApiKey)
				   && Guid.TryParse(rawApiKey, out var apiKey)
				   && actionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
			{
				var authorizedOperationNames = _ApplicationKeyValidator.GetAuthorizedOperations(_ApplicationContext.Name, apiKey);
				return authorizedOperationNames.Contains(controllerActionDescriptor.ActionName);
			}

			return false;
		}
	}
}

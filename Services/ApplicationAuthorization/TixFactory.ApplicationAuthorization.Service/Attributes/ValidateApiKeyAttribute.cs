﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
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
			if (!TryValidateController(actionContext))
			{
				// Controller does not require authorization.
				return;
			}

			if (!TryValidateApiKey(actionContext))
			{
				actionContext.Result = new UnauthorizedResult();
			}
		}

		private bool TryValidateApiKey(ActionExecutingContext actionContext)
		{
			if (actionContext.HttpContext.Request.Headers.TryGetValue(_ApiKeyHeaderName, out var rawApiKey)
				   && Guid.TryParse(rawApiKey, out var apiKey))
			{
				var authorizedOperationNames = _ApplicationKeyValidator.GetAuthorizedOperations(_ApplicationContext.Name, apiKey);
				return authorizedOperationNames.Contains(actionContext.ActionDescriptor.DisplayName);
			}

			return false;
		}

		private bool TryValidateController(ActionExecutingContext actionContext)
		{
			return _AuthenticatedControllerTypes.Contains(actionContext.Controller.GetType());
		}
	}
}
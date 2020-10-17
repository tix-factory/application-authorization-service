﻿using System;
using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationKeyValidator
	{
		ICollection<string> GetAuthorizedOperations(string targetApplicationName, Guid key);
	}
}
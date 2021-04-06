// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk.Ion.Exceptions
{
	public class RootNodeNotFoundException : Exception
	{
		public RootNodeNotFoundException()
		{
		}

		public RootNodeNotFoundException(string message)
			: base(message)
		{
		}

		public RootNodeNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}

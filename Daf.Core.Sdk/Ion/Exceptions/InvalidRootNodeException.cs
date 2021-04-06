// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk.Ion.Exceptions
{
	public class InvalidRootNodeException : Exception
	{
		public InvalidRootNodeException()
		{
		}

		public InvalidRootNodeException(string message)
			: base(message)
		{
		}

		public InvalidRootNodeException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}

// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk.Ion.Exceptions
{
	public class InvalidNodeException : ParserException
	{
		public InvalidNodeException(int nodeLine) : base(nodeLine)
		{
		}

		public InvalidNodeException(int nodeLine, string message)
			: base(nodeLine, message)
		{
		}

		public InvalidNodeException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public InvalidNodeException()
		{
		}

		public InvalidNodeException(string message) : base(message)
		{
		}
	}
}

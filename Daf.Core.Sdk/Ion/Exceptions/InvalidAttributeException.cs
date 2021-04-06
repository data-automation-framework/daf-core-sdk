// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk.Ion.Exceptions
{
	public class InvalidAttributeException : ParserException
	{
		public InvalidAttributeException(int nodeLine) : base(nodeLine)
		{
		}

		public InvalidAttributeException(int nodeLine, string message)
			: base(nodeLine, message)
		{
		}

		public InvalidAttributeException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public InvalidAttributeException()
		{
		}

		public InvalidAttributeException(string message) : base(message)
		{
		}
	}
}

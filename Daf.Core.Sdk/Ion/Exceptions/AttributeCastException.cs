// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk.Ion.Exceptions
{
	public class AttributeCastException : ParserException
	{
		public AttributeCastException(int nodeLine) : base(nodeLine)
		{
		}

		public AttributeCastException(int nodeLine, string message)
			: base(nodeLine, message)
		{
		}

		public AttributeCastException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public AttributeCastException()
		{
		}

		public AttributeCastException(string message) : base(message)
		{
		}
	}
}

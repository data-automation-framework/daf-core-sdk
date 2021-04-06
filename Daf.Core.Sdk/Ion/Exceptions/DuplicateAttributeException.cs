// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk.Ion.Exceptions
{
	public class DuplicateAttributeException : ParserException
	{
		public DuplicateAttributeException(int nodeLine) : base(nodeLine)
		{
		}

		public DuplicateAttributeException(int nodeLine, string message)
			: base(nodeLine, message)
		{
		}

		public DuplicateAttributeException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public DuplicateAttributeException()
		{
		}

		public DuplicateAttributeException(string message) : base(message)
		{
		}
	}
}

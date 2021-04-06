// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk.Ion.Exceptions
{
	public class RequiredFieldNotFoundException : ParserException
	{
		public RequiredFieldNotFoundException(int nodeLine) : base(nodeLine)
		{
		}

		public RequiredFieldNotFoundException(int nodeLine, string message)
			: base(nodeLine, message)
		{
		}

		public RequiredFieldNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public RequiredFieldNotFoundException()
		{
		}

		public RequiredFieldNotFoundException(string message) : base(message)
		{
		}
	}
}

// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk.Ion.Exceptions
{
	public class TextFileParserException : ParserException
	{
		public TextFileParserException(int nodeLine)
			: base(nodeLine)
		{
		}

		public TextFileParserException(int nodeLine, string message)
			: base(nodeLine, message)
		{
		}

		public TextFileParserException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public TextFileParserException()
		{
		}

		public TextFileParserException(string message) : base(message)
		{
		}
	}
}

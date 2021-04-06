// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk.Ion.Exceptions
{
	public class ParserException : Exception
	{
		public int NodeLine { get; set; }
		public string? AssemblyName { get; set; }

		public ParserException(int nodeLine)
		{
			NodeLine = nodeLine;
		}

		public ParserException(int nodeLine, string message)
			: base(message + $" At document line {nodeLine}.")
		{
			NodeLine = nodeLine;
		}

		public ParserException(string message, Exception inner)
			: base(message, inner)
		{

		}

		public ParserException()
		{
		}

		public ParserException(string message) : base(message)
		{
		}
	}
}

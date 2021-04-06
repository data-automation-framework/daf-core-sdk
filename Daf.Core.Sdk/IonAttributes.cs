// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sdk
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class IsRootNodeAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IsRequiredAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class DefaultValueAttribute : Attribute
	{
		public object Value { get; }

		public DefaultValueAttribute(object value)
		{
			Value = value;
		}
	}
}

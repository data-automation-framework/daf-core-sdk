// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Sdk.Ion.Model
{
	internal class IonAttribute
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public IonAttribute(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}

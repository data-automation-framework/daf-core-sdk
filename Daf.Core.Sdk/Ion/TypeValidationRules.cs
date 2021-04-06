// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

namespace Daf.Core.Sdk.Ion
{
	internal class TypeValidationRules
	{
		public List<string> RequiredProperties { get; set; }

		public TypeValidationRules()
		{
			RequiredProperties = new List<string>();
		}
	}
}

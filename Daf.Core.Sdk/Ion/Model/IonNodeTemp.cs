// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

namespace Daf.Core.Sdk.Ion.Model
{
	internal class IonNodeTemp
	{
		public string? NodeName { get; set; }

		public int? Level { get; set; }

		public int? DocumentLine { get; set; }

		public bool? IsRootNode { get; set; }

		public List<IonAttributeTemp> Attributes { get; set; }

		public IonNodeTemp? Parent { get; set; }

		public List<IonNodeTemp> Children { get; set; }

		public bool IsValid { get; set; }

		public IonNodeTemp()
		{
			IsValid = false;
			Attributes = new List<IonAttributeTemp>();
			Children = new List<IonNodeTemp>();
		}

		public IonNodeTemp(int level, int documentLine, bool isRootNode)
		{
			IsValid = true;
			Level = level;
			DocumentLine = documentLine;
			IsRootNode = isRootNode;
			Attributes = new List<IonAttributeTemp>();
			Children = new List<IonNodeTemp>();
		}

		public override string ToString()
		{
			return NodeName + ": Parent: " + (Parent == null ? "" : Parent.NodeName) + " Children: " + Children.Count;
		}
	}
}

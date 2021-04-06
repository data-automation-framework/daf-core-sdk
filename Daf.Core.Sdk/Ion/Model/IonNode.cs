// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

namespace Daf.Core.Sdk.Ion.Model
{
	internal class IonNode
	{
		public string NodeName { get; set; }

		public int Level { get; set; }

		public int DocumentLine { get; set; }

		public bool IsRootNode { get; set; }

		public List<IonAttribute> Attributes { get; set; }

		public IonNode? Parent { get; set; }

		public List<IonNode> Children { get; set; }

		public IonNode(IonNodeTemp fromNode)
		{
			NodeName = fromNode.NodeName!;
			Level = (int)fromNode.Level!;
			DocumentLine = (int)fromNode.DocumentLine!;
			IsRootNode = (bool)fromNode.IsRootNode!;

			if (fromNode.Parent == null)
				Parent = null;
			else
				Parent = new IonNode(fromNode.Parent);

			Attributes = new List<IonAttribute>();
			foreach (IonAttributeTemp attr in fromNode.Attributes)
				Attributes.Add(new IonAttribute(attr.Name!, attr.Value!));

			Children = new List<IonNode>();
			foreach (IonNodeTemp node in fromNode.Children)
				Children.Add(new IonNode(node));
		}

		public override string ToString()
		{
			return NodeName + ": Parent: " + (Parent == null ? "" : Parent.NodeName) + " Children: " + Children.Count;
		}
	}
}

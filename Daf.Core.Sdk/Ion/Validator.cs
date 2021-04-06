// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Daf.Core.Sdk.Ion.Exceptions;
using Daf.Core.Sdk.Ion.Model;
using Daf.Core.Sdk.Ion.Reader;

namespace Daf.Core.Sdk.Ion
{
	internal static class Validator
	{
		internal static void ValidateNode(IonNode node, Type nodeType, string? assemblyName)
		{
			if (nodeType == null)
				throw new InvalidNodeException(node.DocumentLine, $"Node {node.NodeName} in document does not have a corresponding class defined in assembly {assemblyName}.");

			TypeValidationRules validationRules = GetTypeValidationRules(nodeType);

			try
			{
				ValidateRequiredProperties(node, validationRules);
				ValidateNoDuplicateAttributes(node);
				ValidateChildNodesAreProperties(node, nodeType);
				ValidateAttributesExistsInAssembly(node, nodeType);
			}
			catch (ParserException pe)
			{
				pe.AssemblyName = nodeType.Assembly.FullName;
				throw;
			}
		}

		internal static bool RootNodeExistInFile(FileToIonNodeReader textFileParser)
		{
			try
			{
				textFileParser.GetRootNodeLine();
				return true;
			}
			catch (RootNodeNotFoundException)
			{
				return false;
			}
		}

		internal static void ValidateLeadingWhitespaceNode(string nodeLine, int lineNr)
		{
			//Validate that node definition lines does not contain any other leading whitespace than tabs
			foreach (char c in nodeLine)
			{
				if (char.IsWhiteSpace(c))
				{
					if (c != '\t')
						throw new TextFileParserException(lineNr, $"Node definitions may only be indented using tabs (file line {lineNr + 1})."); //Add 1 since most file editors starts at 1 instead of 0
				}
				else
				{
					break;
				}
			}
		}

		internal static void ValidateRootNode(string rootNodeName, Assembly assembly)
		{
			int rootNodeCount = 0;
			foreach (Type t in assembly.GetTypes())
			{
				IsRootNodeAttribute? rootNodeAttribute = (IsRootNodeAttribute?)Attribute.GetCustomAttribute(t, typeof(IsRootNodeAttribute));
				if (rootNodeAttribute != null)
				{
					rootNodeCount++;
					if (t.Name != rootNodeName)
					{
						throw new InvalidRootNodeException($"Provided root node type {rootNodeName} does not match the root node of assembly {assembly.GetName().Name}. Expected root node type is {t.Name}.");
					}
				}
			}

			if (rootNodeCount != 1)
			{
				throw new InvalidRootNodeException($"A plugin must have exactly 1 root node. {rootNodeCount} root nodes were found in assembly {assembly.GetName().Name}.");
			}
		}

		private static void ValidateRequiredProperties(IonNode node, TypeValidationRules validationRules)
		{
			foreach (string requiredProperty in validationRules.RequiredProperties)
			{
				bool found = false;
				if (node.Attributes.Any(a => a.Name == requiredProperty))
				{
					found = true;
				}
				else if (node.Children.Any(c => c.NodeName == requiredProperty))
				{
					found = true;
				}

				if (!found)
					throw new RequiredFieldNotFoundException(node.DocumentLine, $"Field or child node {requiredProperty} is required but missing in node {node.NodeName}.");
			}
		}
		private static void ValidateNoDuplicateAttributes(IonNode node)
		{
			List<string> duplicates = node.Attributes.GroupBy(a => a.Name).Where(g => g.Count() > 1).Select(g => g.Key).ToList(); //g.Key is the attribute name (from group by clause)

			if (duplicates.Count > 0)
			{
				string attributes = string.Join(", ", duplicates);
				throw new DuplicateAttributeException(node.DocumentLine, $"Attributes {attributes} exists more than once in node {node.NodeName}.");
			}
		}

		private static void ValidateChildNodesAreProperties(IonNode node, Type nodeType)
		{
			if (nodeType.GetInterface("IList") == null)
			{
				foreach (IonNode child in node.Children)
				{
					bool childFound = false;

					foreach (PropertyInfo p in nodeType.GetProperties())
					{
						if (p.Name == child.NodeName)
						{
							childFound = true;
							break;
						}
					}

					if (!childFound)
						throw new InvalidNodeException(node.DocumentLine, $"Node {node.NodeName} has a child {child.NodeName} but there is no corresponding property of class {nodeType.Name} in the assembly.");
				}
			}
		}

		private static void ValidateAttributesExistsInAssembly(IonNode node, Type nodeType)
		{
			foreach (IonAttribute attr in node.Attributes)
			{
				bool attributeFound = false;
				foreach (PropertyInfo p in nodeType.GetProperties())
				{
					if (p.Name == attr.Name)
					{
						attributeFound = true;
						break;
					}
				}

				if (!attributeFound)
					throw new InvalidAttributeException(node.DocumentLine, $"Attribute {attr.Name} in {node.NodeName} is not a property of class {nodeType.Name}.");
			}
		}

		private static TypeValidationRules GetTypeValidationRules(Type nodeType)
		{
			TypeValidationRules validationRules = new();

			foreach (PropertyInfo property in nodeType.GetProperties())
			{
				IsRequiredAttribute? isRequired = (IsRequiredAttribute?)Attribute.GetCustomAttribute(property, typeof(IsRequiredAttribute));

				if (isRequired != null)
					validationRules.RequiredProperties.Add(property.Name);
			}

			return validationRules;
		}
	}
}

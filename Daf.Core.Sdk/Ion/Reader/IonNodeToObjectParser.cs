// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Daf.Core.Sdk.Ion.Exceptions;
using Daf.Core.Sdk.Ion.Model;

namespace Daf.Core.Sdk.Ion.Reader
{
	internal class IonNodeToObjectParser
	{
		private readonly Assembly assembly;

		private readonly Type[] stringTypes = new[] { typeof(string) };

		internal IonNodeToObjectParser(Assembly assembly)
		{
			this.assembly = assembly;
		}

		internal object Parse(IonNode rootNode)
		{
			return Instantiate(rootNode, null);
		}

		//Iterate recurively through the tree and instantiate and populate objects from the plugin
		private object Instantiate(IonNode node, Type? parentType)
		{
			//Hack: This is here due to a bug in the current version of .NET 5 preview that, on some computers, results in "-1" not being a valid integer when calling Int32.Parse().
			//Might have unintended side-effects. Can probably be removed once .NET 5 is released.
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			Type nodeType;
			if (node.IsRootNode)
			{
				nodeType = assembly.GetTypes().Where(t => t.Name == node.NodeName).First();    //Create root node based directly on the name in the document
			}
			else
			{
				nodeType = GetTypeByNodeName(parentType, node);
			}

			Validator.ValidateNode(node, nodeType, assembly.FullName);

			object? instance = Activator.CreateInstance(nodeType);

#pragma warning disable CA1508 // Avoid dead conditional code. Code analyser does not detect that Instance is nullable for some reason.
			if (instance == null)
				throw new InvalidNodeException(node.DocumentLine, $"Cannot instantiate node {node.NodeName}.");
#pragma warning restore CA1508 // Avoid dead conditional code

			List<string> propertiesWithValue = new(); //Keep track of which properties have been set in order to set default values on the ones that haven't

			//Set attribute values
			foreach (IonAttribute attr in node.Attributes)
			{
				foreach (PropertyInfo p in nodeType.GetProperties())
				{
					if (p.Name == attr.Name)
					{
						propertiesWithValue.Add(p.Name);
						object parsedValue = ParseStringValue(attr.Value, p.PropertyType, node);
						SetPropertyValue(nodeType, p.Name, instance, parsedValue);
						break;
					}
				}
			}

			//Instantiate and connect children recursively
			foreach (IonNode child in node.Children)
			{
				propertiesWithValue.Add(child.NodeName);
				Type childType = GetTypeByNodeName(nodeType, child);

				List<Type> childBaseTypes = GetAllBaseTypes(childType);
				object childInstance = Instantiate(child, nodeType);     //Recursive call

				//Current node is a list
				if (nodeType.GetInterface("IList") != null)
				{
					//Add child to the list
					((IList)instance).Add(childInstance);
				}
				else
				{
					foreach (PropertyInfo p in nodeType.GetProperties())
					{
						if (child.NodeName == p.Name || childBaseTypes.Any(bt => bt.Name == p.Name))
						{
							SetPropertyValue(nodeType, p.Name, instance, childInstance);
						}
					}
				}
			}

			//If any property hasn't been set via attributes or child nodes, check if it has a default value and set it
			foreach (PropertyInfo p in nodeType.GetProperties())
			{
				if (!propertiesWithValue.Contains(p.Name))
				{
					//Property is a list
					if (p.PropertyType.GetInterface("IList") != null)
					{
						//Initiate the list to an empty list
						object? emptyList = Activator.CreateInstance(p.PropertyType);
						SetPropertyValue(nodeType, p.Name, instance, emptyList);
					}
					else
					{
						object? defaultValue = GetPropertyDefaultValue(p);
						if (defaultValue != null)
						{
							SetPropertyValue(nodeType, p.Name, instance, defaultValue);
						}
					}
				}
			}

			return instance;
		}

		private static void SetPropertyValue(Type? type, string propName, object? instance, object? value)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			PropertyInfo? prop = type.GetProperty(propName);
			if (prop == null)
				throw new ParserException($"Cannot get property with name {propName}");

			if (prop.CanWrite)
			{
				prop.SetValue(instance, value);
			}
			else if (!prop.CanWrite && prop.PropertyType.GetInterface("IList") != null && value.GetType().GetInterface("IList") != null)
			{
				IList valueList = (IList)value;
				IList? propertyList = (IList?)prop.GetValue(instance);

				if (propertyList == null)
					throw new ParserException($"Cannot get value of property with name {propName}");

				foreach (object val in valueList)
				{
					propertyList.Add(val);
				}
			}
			else
			{
				throw new ParserException($"Cannot set property {propName} of type {type.FullName} since it is marked as read only.");
			}
		}

		private List<Type> GetAllBaseTypes(Type subType)
		{
			List<Type> baseTypes = new();

			if (subType.BaseType == null)
				return baseTypes;           //End recursion

			baseTypes.Add(subType.BaseType);
			baseTypes.AddRange(GetAllBaseTypes(subType.BaseType)); //Recursion, get the base type of the current base type

			return baseTypes;
		}

		private Type GetTypeByNodeName(Type? typeToSearch, IonNode node)
		{
			if (typeToSearch == null)
				throw new InvalidNodeException(node.DocumentLine, $"A type to search must be provided for node {node.NodeName}.");

			Type? nodeType = null;

			if (typeToSearch.GetInterface("IList") != null)
			{
				Type listElementType = typeToSearch.GetGenericArguments()[0];
				if (listElementType.Name == node.NodeName)
				{
					nodeType = listElementType;
				}
				else
				{
					List<Type> inheritingTypes = GetInheritingTypes(listElementType);
					foreach (Type inheritingType in inheritingTypes)
					{
						if (inheritingType.Name == node.NodeName)
						{
							nodeType = inheritingType;
							break;
						}
					}
				}
			}
			else
			{
				if (node.NodeName == null)
					throw new InvalidNodeException(node.DocumentLine, $"Node at line {node.DocumentLine} does not have a name specified.");

				PropertyInfo? prop = typeToSearch.GetProperty(node.NodeName);

				if (prop == null)
					throw new InvalidNodeException(node.DocumentLine, $"Cannot resolve property of type {typeToSearch.FullName} for node {node.NodeName}.");

				nodeType = prop.PropertyType;
			}

			if (nodeType == null)
				throw new InvalidNodeException(node.DocumentLine, $"Found no type containing a property with name {node.NodeName} or a List type containing elements of type {node.NodeName}. Search occured in type {typeToSearch.FullName}.");

			return nodeType;
		}

		private List<Type> GetInheritingTypes(Type baseType)
		{
			//Find the first level of inheriting types
			List<Type> types = assembly.GetTypes().Where(t => t.BaseType == baseType).ToList();

			//Look for additional levels of derived types
			types.AddRange(GetInheritingTypesRecursively(types));

			return types;

			IEnumerable<Type> GetInheritingTypesRecursively(List<Type> types)
			{
				List<Type> additionalTypes = new();

				//For each type, check if any of them have additional derived types
				foreach (Type type in types)
				{
					additionalTypes.AddRange(assembly.GetTypes().Where(t => t.BaseType == type));
				}

				//Can't find any additional derived types, return
				if (additionalTypes.Count == 0)
					return additionalTypes;

				//Found additional derived types, call this function again to look for more and then add the resulting list to our list
				additionalTypes.AddRange(GetInheritingTypesRecursively(additionalTypes));

				return additionalTypes;
			}
		}

		private object ParseStringValue(string value, Type type, IonNode node)
		{
			object parsedValue;

			if (type == typeof(string))
			{
				parsedValue = value;
			}
			else if (type == typeof(bool) || type == typeof(bool?))
			{
				if (value.ToUpper(CultureInfo.InvariantCulture) == "TRUE")
					parsedValue = true;
				else if (value.ToUpper(CultureInfo.InvariantCulture) == "FALSE")
					parsedValue = false;
				else
					throw new AttributeCastException(node.DocumentLine, $"Value {value} is of type bool but cannot be parsed. Valid values are true or false (case insensitive).");
			}
			else if (type.BaseType == typeof(Enum))
			{
				try
				{
					parsedValue = Enum.Parse(type, value);
				}
				catch (Exception)
				{
					throw new AttributeCastException(node.DocumentLine, $"Enumeration value {value} is not an enum field of {type.Name} in assembly {assembly.FullName}.");
				}
			}
			else if (IsNullableEnum(type))
			{
				Type nullableEnumType = Nullable.GetUnderlyingType(type)!;

				try
				{
					parsedValue = Enum.Parse(nullableEnumType, value);
				}
				catch (Exception)
				{
					throw new AttributeCastException(node.DocumentLine, $"Enumeration value {value} is not an enum field of {nullableEnumType.Name} in assembly {assembly.FullName}.");
				}
			}
			else
			{
				//Get Parse method of the provided type to cast to and invoke
				MethodInfo? parseMethod = type.GetMethod("Parse", stringTypes);
				if (parseMethod == null)
					throw new AttributeCastException(node.DocumentLine, $"Type {type.FullName} does not specify a Parse method for casting string to object value.");

				object? parsedValueTmp = parseMethod.Invoke(null, new[] { value });
				if (parsedValueTmp == null)
					throw new AttributeCastException(node.DocumentLine, $"Parse method of type {type.FullName} returned null value when trying to cast value {value}.");

				parsedValue = parsedValueTmp;
			}

			return parsedValue;
		}

		private static bool IsNullableEnum(Type type)
		{
			Type? underlyingType = Nullable.GetUnderlyingType(type);
			return (underlyingType != null) && underlyingType.IsEnum;
		}

		private static object? GetPropertyDefaultValue(PropertyInfo property)
		{
			DefaultValueAttribute? defaultValue = (DefaultValueAttribute?)Attribute.GetCustomAttribute(property, typeof(DefaultValueAttribute));

			if (defaultValue != null)
			{
				return defaultValue.Value;
			}

			return null;
		}
	}
}

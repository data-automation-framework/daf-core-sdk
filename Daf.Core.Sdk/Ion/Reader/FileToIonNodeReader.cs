// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Daf.Core.Sdk.Ion.Exceptions;
using Daf.Core.Sdk.Ion.Model;

namespace Daf.Core.Sdk.Ion.Reader
{
	internal class FileToIonNodeReader
	{
		private readonly List<string> fileLines;

		private bool inQualifiedString;
		private bool inTextBlock;

		private Context context = Context.NodeName;

		private readonly string rootNodeName;

		private const char COMMENT_SIGN = '#';

		private const char NODE_SIGN = ':';

		private const char ASSIGNMENT_SIGN = '=';

		private const char STRING_SIGN = '"';

		private const string TEXT_BLOCK_START = "<!";

		private const string TEXT_BLOCK_END = "!>";

		private const string INDENT_PUSH = "--- IndentationPush ---";

		private const string INDENT_POP = "--- IndentationPop ---";

		internal FileToIonNodeReader(string filePath, string rootNodeName)
		{
			fileLines = new List<string>(File.ReadAllLines(filePath));
			this.rootNodeName = rootNodeName;
		}

		internal IonNode Parse()
		{
			List<IonNode> nodes = ParseToFlat();            //Parse text file to IonNode and IonField objects
			BuildTree(nodes);                               //Connect the IonNodes to each other based on level to create a parent/child tree
			return nodes[0];                                //Return the root of the tree
		}

		private List<IonNode> ParseToFlat()
		{
			StringBuilder sb = new();
			List<IonNodeTemp> nodes = new();

			IonNodeTemp currentNode = new();

			IonAttributeTemp currentAttribute = new();

			bool readNodeName = false;
			bool readAttributeName = false;
			bool readAttributeValue = false;

			int rootNodeStartLine = GetRootNodeLine();

			Stack<int> extraIndents = new(); //Keeps track of indentations needed for T4 include directives

			//Reset flags before parsing
			inQualifiedString = false;
			inTextBlock = false;

			for (int i = rootNodeStartLine; i < fileLines.Count; i++)
			{
				string line = fileLines[i];

				//Execute control logic if not in text block or string content
				if (!inTextBlock && !inQualifiedString)
				{
					if (IsComment(line))
						continue;           //Skip the line if it is an ion comment

					Validator.ValidateLeadingWhitespaceNode(line, i); //Make sure that all leading whitespaces are tabs

					int level = GetLevel(line);

					if (IsParserInstruction(line))
					{
						if (IsIndentationPush(line))
							extraIndents.Push(level + GetIndentationOffset(extraIndents));
						else if (IsIndentationPop(line))
							extraIndents.Pop();

						continue;           //Skip the rest of the line if it is a parser instruction since no other data can be on the same line
					}

					level += GetIndentationOffset(extraIndents);

					if (ContainsNodeName(line))
					{
						if (level == 0 && currentNode.IsValid)  //Indicates that another root node is found, stop parsing the
							break;                              //document since only one node structure is returned per call

						if (currentNode.IsValid)
						{
							//Finish current node
							nodes.Add(currentNode);
						}
						//Prepare new node
						bool isRootNode = i == rootNodeStartLine;
						currentNode = new IonNodeTemp(level, i, isRootNode);
						context = Context.NodeName;
					}
				}

				for (int y = 0; y < line.Length; y++)
				{
					string window = GetWindow(line, y);

					if (window == TEXT_BLOCK_START && !inTextBlock)
					{
						inTextBlock = true;
						y += TEXT_BLOCK_START.Length - 1;    //Offset y to skip text block indicator

						continue;
					}
					else if (window == TEXT_BLOCK_END)
					{
						inTextBlock = false;
						y += TEXT_BLOCK_START.Length - 1;    //Offset y to skip text block indicator

						continue;
					}

					char c = line[y];

					sb.Append(c);

					if (!inTextBlock) //Start executing parsing logic if not in a text block
					{
						if (c == STRING_SIGN && !inQualifiedString)
							inQualifiedString = true;
						else if (c == STRING_SIGN && inQualifiedString)
							inQualifiedString = false;

						//Parse ion nodes and handle positional context if not in string or text block
						if (!inQualifiedString)
						{
							if (c == COMMENT_SIGN && context != Context.AttributeValue) //Don't process comments, though comment sign can be part of attribute value
							{
								sb.Clear();
								break;
							}

							if (c == NODE_SIGN && context == Context.NodeName)
								readNodeName = true;

							if (Char.IsWhiteSpace(c) && context == Context.NodeName && currentNode.NodeName != null)
							{
								context = Context.AttributeName;
							}

							if (Char.IsWhiteSpace(c) && context == Context.AttributeValue)
							{
								context = Context.AttributeName;
								readAttributeValue = true;
							}

							if (c == ASSIGNMENT_SIGN && context == Context.AttributeName)
							{
								readAttributeName = true;
								context = Context.AttributeValue;
							}
						}

						if (readAttributeName)
						{
							readAttributeName = false;
							currentAttribute = new IonAttributeTemp
							{
								Name = sb.ToString().Trim().TrimEnd(ASSIGNMENT_SIGN)
							};
							sb.Clear();
						}
						else if (readAttributeValue)
						{
							readAttributeValue = false;

							try
							{
								currentAttribute.Value = CleanAttributeString(sb.ToString());
							}
							catch (Exception ex)
							{
								throw new TextFileParserException($"Failed to trim string on line {line}.", ex);
							}

							currentNode.Attributes.Add(currentAttribute);
							sb.Clear();
						}
						else if (readNodeName)
						{
							readNodeName = false;
							currentNode.NodeName = CleanNodeName(sb.ToString().Trim().TrimEnd(NODE_SIGN), i);
							sb.Clear();
						}
					}
				}

				//End of line
				if (!inQualifiedString && !inTextBlock)
				{
					//If there is no white space at end of line, then the attribute value needs to be handled
					if (context == Context.AttributeValue)
					{
						try
						{
							currentAttribute.Value = CleanAttributeString(sb.ToString());
						}
						catch (Exception ex)
						{
							throw new TextFileParserException($"Failed to trim string on line {line}.", ex);
						}

						currentNode.Attributes.Add(currentAttribute);
						context = Context.AttributeName;
						sb.Clear();
					}
				}

				//Keep new lines if in text block
				if (inTextBlock)
					sb.Append(Environment.NewLine);
			}

			//Add the final node which was finished after the last line was read
			nodes.Add(currentNode);

			//Convert to non-nullable types
			List<IonNode> nonNullableNodes = new();
			foreach (IonNodeTemp node in nodes)
				nonNullableNodes.Add(new IonNode(node));

			return nonNullableNodes;
		}

		private static void BuildTree(List<IonNode> nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				IonNode node = nodes[i];
				if (node.Level > 0)
				{
					ConnectToParent(node, nodes, i);
				}
			}
		}

		private static void ConnectToParent(IonNode childNode, List<IonNode> prevNodes, int index)
		{
			int parentLevel = childNode.Level - 1;
			bool parentFound = false;
			for (int i = index; i >= 0; i--)            //Loop from current node and backwards to get closest parent
			{
				IonNode parentNode = prevNodes[i];
				if (parentLevel == parentNode.Level)
				{
					childNode.Parent = parentNode;
					parentNode.Children.Add(childNode);
					parentFound = true;
					break;
				}
			}

			if (!parentFound && childNode.IsRootNode == false)
				throw new TextFileParserException(childNode.DocumentLine, $"Node {childNode.NodeName} has no parent node. Verify that it is indented exactly once from the parent in the document.");
		}

		private static bool IsComment(string line)
		{
#if NETSTANDARD2_0
			if (line.TrimStart().StartsWith(COMMENT_SIGN.ToString(), StringComparison.Ordinal)) // StartsWith has no overload taking a char in .NET Standard
				return true;
#else
			if (line.TrimStart().StartsWith(COMMENT_SIGN))
				return true;
#endif

			return false;
		}

		//Count tab characters at start of string
		private static int GetLevel(string line)
		{
			int level = 0;
			foreach (char c in line)
			{
				if (c == '\t')
					level++;
				else
					break;
			}

			return level;
		}

		internal int GetRootNodeLine()
		{
			//Reset context flags since starting from line 1
			inQualifiedString = false;
			inTextBlock = false;
			for (int i = 0; i < fileLines.Count; i++)
			{
				string line = fileLines[i].TrimStart();

				for (int y = 0; y < line.Length; y++)
				{
					string window = GetWindow(line, y);

					if (window == TEXT_BLOCK_START)
					{
						inTextBlock = true;
						y += TEXT_BLOCK_START.Length - 1;    //Skip text block indicator
					}
					else if (window == TEXT_BLOCK_END)
					{
						inTextBlock = false;
						y += TEXT_BLOCK_START.Length - 1;    //Skip text block indicator
					}

					char c = line[y];

					if (!inTextBlock)    //Only execute qualified string logic if not in text block
					{

						if (c == STRING_SIGN && !inQualifiedString)
							inQualifiedString = true;
						else if (c == STRING_SIGN && inQualifiedString)
							inQualifiedString = false;
					}
				}

				//Search for root node if not in text block or qualified string
				if (!inTextBlock && !inQualifiedString)
				{
					if (ContainsNodeName(line))
					{
						string nodeName = CleanNodeName(line.Substring(0, line.IndexOf(NODE_SIGN)), i);    //Extract element name and remove potential namespaces from the node name
						if (nodeName == rootNodeName)
						{
							inQualifiedString = false;
							return i;
						}
					}
				}
			}

			throw new RootNodeNotFoundException($"Root node {rootNodeName} could not be found in document.");
		}

		private bool ContainsNodeName(string line)
		{
			bool contains = false;

#if NETSTANDARD2_0
			if (!inQualifiedString && !inTextBlock && line.Contains(NODE_SIGN.ToString())) // Contains has no overload taking a char in .NET Standard
#else
			if (!inQualifiedString && !inTextBlock && line.Contains(NODE_SIGN))
#endif
			{
#if NETSTANDARD2_0
				if (!line.Contains(STRING_SIGN.ToString())) // Contains has no overload taking a char in .NET Standard
#else
				if (!line.Contains(STRING_SIGN))
#endif
				{
					contains = true;
				}
				else
				{
					if (line.IndexOf(NODE_SIGN) < line.IndexOf(STRING_SIGN))
					{
						contains = true;
					}
					else
					{
						contains = false;
					}
				}
			}
			return contains;
		}

		private static string CleanAttributeString(string str)
		{
			string cleaned;

			try
			{
				cleaned = TrimSingle(str.TrimStart().TrimEnd(), STRING_SIGN);
			}
			catch (Exception ex)
			{
				throw new TextFileParserException($"Failed to trim string {str}.", ex);
			}

			return cleaned;
		}

		private static string TrimSingle(string str, char c)
		{
			int startIndex = 0;
			int substringLength = str.Length;
			if (str[0] == c || str[^1] == c)
			{
				if (str[0] == c)
				{
					startIndex = 1;
					substringLength--;
				}
				if (str[^1] == c)
				{
					substringLength--;
				}
				return str.Substring(startIndex, substringLength);
			}

			return str;
		}

		private static string CleanNodeName(string nodeName, int lineNr)
		{
			if (nodeName.Contains("."))
			{
				if (nodeName.LastIndexOf('.') < nodeName.Length - 1)
					return nodeName[(nodeName.LastIndexOf('.') + 1)..];
				else
					throw new TextFileParserException(lineNr, $"Node {nodeName} ends with a full stop which is not allowed for node names.");
			}

			return nodeName;
		}

		private static bool IsIndentationPush(string line)
		{
			if (line.Trim() == INDENT_PUSH)
				return true;

			return false;
		}

		private static bool IsIndentationPop(string line)
		{
			if (line.Trim() == INDENT_POP)
				return true;

			return false;
		}

		private static bool IsParserInstruction(string line)
		{
			return IsIndentationPush(line) || IsIndentationPop(line);
		}

		private static int GetIndentationOffset(Stack<int> extraIndents)
		{
			if (extraIndents.Count > 0)
				return extraIndents.Peek();

			return 0;
		}

		private static string GetWindow(string line, int position)
		{
			int windowLength = TEXT_BLOCK_START.Length;
			if (position + windowLength >= line.Length)
			{
				windowLength = line.Length - position;
			}

			return line.Substring(position, windowLength);
		}
	}
}

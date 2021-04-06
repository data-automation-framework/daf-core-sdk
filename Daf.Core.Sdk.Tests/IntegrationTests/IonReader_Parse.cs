// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.IO;
using Daf.Core.Sdk.Ion.Exceptions;
using Daf.Core.Sdk.Ion.Reader;
using Daf.Core.Sdk.Tests.Plugins;
using Xunit;

namespace Daf.Core.Sdk.Tests.IntegrationTests
{
	public class IonReader_Parse
	{
		private static readonly string resourceFolder = Path.Combine(Path.GetFullPath("IntegrationTests"), "Resources");

		[Fact]
		public void Parse_NormalFile_DoesNotThrow()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "Normal.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			//Assert that the exception wasn't thrown
			Assert.Null(ex);
		}

		[Fact]
		public void Parse_NormalFile_ReturnsExpectedStructure()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "Normal.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);
			IntegrationTestPlugin expectedStructure = ExpectedPluginStructureProvider.GetNormal();

			IntegrationTestPlugin actualStructure = reader.Parse();

			Assert.True(actualStructure.Equals(expectedStructure));
		}

		[Fact]
		public void Parse_FileWithoutRootNode_ThrowsRootNodeNotFoundException()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "NoRootNode.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			Assert.True(ex is RootNodeNotFoundException);
		}

		[Fact]
		public void Parse_ProvidedWithInvalidRootNode_ThrowsInvalidRootNodeException()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "NoRootNode.ion");
			IonReader<SsisProject> reader = new(fileToParsePath, typeof(SsisProject).Assembly); //SsisProject is not the root node of the assembly

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			Assert.True(ex is InvalidRootNodeException);
		}

		[Fact]
		public void Parse_FileWithInvalidChildNode_ThrowsInvalidNodeException()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "InvalidChildNode.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			Assert.True(ex is InvalidNodeException);
		}

		[Fact]
		public void Parse_FileWithDuplicateAttribute_ThrowsDuplicateAttributeException()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "DuplicateAttribute.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			Assert.True(ex is DuplicateAttributeException);
		}

		[Fact]
		public void Parse_FileWithMissingRequiredAttribute_ThrowsRequiredFieldNotFoundException()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "MissingRequiredAttribute.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			Assert.True(ex is RequiredFieldNotFoundException);
		}

		[Fact]
		public void Parse_FileWithInvalidAttributeType_ThrowsAttributeCastException()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "InvalidAttributeType.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			Assert.True(ex is AttributeCastException);
		}

		[Fact]
		public void Parse_FileWithInvalidAttributeName_ThrowsInvalidAttributeException()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "InvalidAttribute.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			Assert.True(ex is InvalidAttributeException);
		}

		[Fact]
		public void Parse_FileWithWrongIndentation_ThrowsTextFileParserException()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "InvalidIndentation.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			Assert.True(ex is TextFileParserException);
		}

		[Fact]
		public void Parse_FileWithNodeNameEndingInFullStop_ThrowsTextFileParserException()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "NodeEndingWithFullStop.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			//Get any exception being thrown
			Exception ex = Record.Exception(() => reader.Parse());

			Assert.True(ex is TextFileParserException);
		}

	}
}

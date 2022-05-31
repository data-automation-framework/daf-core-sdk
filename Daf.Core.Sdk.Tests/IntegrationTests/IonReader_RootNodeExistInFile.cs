// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.IO;
using Daf.Core.Sdk.Ion.Reader;
using Daf.Core.Sdk.Tests.IntegrationTests.Resources;
using Xunit;

namespace Daf.Core.Sdk.Tests.IntegrationTests
{
	public class IonReader_RootNodeExistInFile
	{
		private static readonly string resourceFolder = Path.Combine(Path.GetFullPath("IntegrationTests"), "Resources");

		[Fact]
		public void CheckRootNodeExist_NormalFile_ReturnsTrue()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "Normal.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			bool rootNodeExists = reader.RootNodeExistInFile();

			Assert.True(rootNodeExists);
		}

		[Fact]
		public void CheckRootNodeExist_NoRootNode_ReturnsFalse()
		{
			string fileToParsePath = Path.Combine(resourceFolder, "NoRootNode.ion");
			IonReader<IntegrationTestPlugin> reader = new(fileToParsePath, typeof(IntegrationTestPlugin).Assembly);

			bool rootNodeExists = reader.RootNodeExistInFile();

			Assert.False(rootNodeExists);
		}
	}
}

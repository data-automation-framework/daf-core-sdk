// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Reflection;
using Daf.Core.Sdk.Ion.Model;

[assembly: CLSCompliant(true)]
namespace Daf.Core.Sdk.Ion.Reader
{
	public class IonReader<TRootNodeType>
	{
		private readonly FileToIonNodeReader fileParser;

		private readonly IonNodeToObjectParser nodeParser;

		private readonly Assembly assembly;

		public IonReader(string filePath, Assembly assembly)
		{
			this.assembly = assembly;
			fileParser = new FileToIonNodeReader(filePath, typeof(TRootNodeType).Name);
			nodeParser = new IonNodeToObjectParser(assembly);
		}

		public TRootNodeType Parse()
		{
			Validator.ValidateRootNode(typeof(TRootNodeType).Name, assembly);
			IonNode rootNode = fileParser.Parse();
			TRootNodeType objectRootNode = (TRootNodeType)nodeParser.Parse(rootNode);
			return objectRootNode;
		}

		public bool RootNodeExistInFile()
		{
			return Validator.RootNodeExistInFile(fileParser);
		}
	}
}

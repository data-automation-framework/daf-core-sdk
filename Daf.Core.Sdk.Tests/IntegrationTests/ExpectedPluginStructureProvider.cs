// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Sdk.Tests.Plugins;

namespace Daf.Core.Sdk.Tests.IntegrationTests
{
	public static class ExpectedPluginStructureProvider
	{
		public static IntegrationTestPlugin GetNormal()
		{
			//Build structure corresponding to Resources\Normal.ion file, starting at the last node

			//ChildSequenceContainer
			ExecuteSql execSql1 = new()
			{
				Name = "TestTask3",
				//Default values
				DelayValidation = false,
				PropagateErrors = true,
				ResultSet = ExecuteSqlResultSet.None,
				TypeConversionMode = ExecuteSqlTypeConversionMode.Allowed,
				BypassPrepare = true,
				TimeOut = 0U,
				CodePage = 1252U
			};

			SequenceContainer seq1 = new()
			{
				Name = "ChildSequenceContainer",
				//Default values
				DelayValidation = false,
				PropagateErrors = true
			};
			seq1.Tasks.Add(execSql1);

			//ParentSequenceContainer
			Expression exp = new()
			{
				Name = "TestTask2",
				ExpressionValue = "Test SSIS expression",
				//Default values
				DelayValidation = false,
				PropagateErrors = true
			};

			PrecedenceConstraintList pcl = new()
			{
				//Default value
				LogicalType = LogicalOperation.And
			};
			pcl.Inputs.Add(new InputPath()
			{
				OutputPathName = "TestTask1",
				//Default values
				EvaluationOperation = TaskEvaluationOperationType.Constraint,
				EvaluationValue = TaskEvaluationOperationValue.Success
			});
			SequenceContainer seq2 = new()
			{
				Name = "ParentSequenceContainer",
				PrecedenceConstraints = pcl,
				//Default values
				DelayValidation = false,
				PropagateErrors = true
			};

			seq2.Tasks.Add(exp);    //Add child task 
			seq2.Tasks.Add(seq1);   //Add child container

			//Package
			SqlStatement sqls = new()
			{
				Value = @"SELECT A, B, C FROM ""test"".""TestTable"""
			};
			ExecuteSql es2 = new()
			{
				Name = "TestTask1",
				ConnectionName = "TestOleDbConnectionString",
				SqlStatement = sqls,
				//Default values
				DelayValidation = false,
				PropagateErrors = true,
				ResultSet = ExecuteSqlResultSet.None,
				TypeConversionMode = ExecuteSqlTypeConversionMode.Allowed,
				BypassPrepare = true,
				TimeOut = 0U,
				CodePage = 1252U
			};

			OleDbConnection oleConn = new()
			{
				ConnectionString = "TestOleDbConnectionString",
				Name = "TestDbConnection",
				//Default value
				DelayValidation = false
			};

			FlatFileColumn ffc1 = new()
			{
				Name = "TestCol1",
				DataType = DatabaseType.String,
				CodePage = 1251,
				//Default values
				InputWidth = 0,
				OutputWidth = 0,
				Precision = 0,
				Scale = 0,
				Delimiter = ",",
				TextQualified = false,
				FastParse = false
			};
			FlatFileColumn ffc2 = new()
			{
				Name = "TestCol2",
				DataType = DatabaseType.Boolean,
				//Default values
				InputWidth = 0,
				OutputWidth = 0,
				Precision = 0,
				Scale = 0,
				Delimiter = ",",
				CodePage = 0,
				TextQualified = false,
				FastParse = false
			};
			FlatFileConnection ffConn = new()
			{
				ConnectionString = "TestConnectionString",
				Name = "TestFileConnection",
				DelayValidation = true,
				TextQualifier = "\"",
				//Default values
				Format = FlatFileFormat.Delimited,
				ColumnNamesInFirstDataRow = true,
				HeaderRowsToSkip = 0,
				Unicode = true,
				CodePage = 65001,
				LocaleId = 0
			};
			ffConn.FlatFileColumns.Add(ffc1);
			ffConn.FlatFileColumns.Add(ffc2);

			Package p = new()
			{
				//Default values
				LocaleId = 0,
				DelayValidation = false
			};
			p.Connections.Add(ffConn);
			p.Connections.Add(oleConn);
			p.Tasks.Add(es2);
			p.Tasks.Add(seq2);

			//Project
			SsisProject ssisProject = new();
			ssisProject.Packages.Add(p);

			//Root
			IntegrationTestPlugin root = new();
			root.SsisProjects.Add(ssisProject);

			return root;
		}
	}
}

// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Daf.Core.Sdk.Tests.Plugins
{
	[IsRootNode]
	public class IntegrationTestPlugin : IEquatable<IntegrationTestPlugin>
	{
		/// <summary>
		/// Collection of SsisProject
		/// </summary>
		public List<SsisProject> SsisProjects { get; }

		public IntegrationTestPlugin()
		{
			SsisProjects = new List<SsisProject>();
		}

		public bool Equals([AllowNull] IntegrationTestPlugin other)
		{
			bool equal = true;

			if (other == null)
				equal = false;
			else if (SsisProjects.Count == other.SsisProjects.Count)
			{
				for (int i = 0; i < SsisProjects.Count; i++)
				{
					if (!SsisProjects[i].Equals(other.SsisProjects[i]))
					{
						equal = false;
						break;
					}
				}
			}
			else
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as IntegrationTestPlugin);
		}

		public override int GetHashCode()
		{
			return SsisProjects.GetHashCode();
		}
	}

	/// <summary>
	/// Element in collection of all the SQL Server Integration Services (SSIS) projects.
	/// </summary>
	public class SsisProject : IEquatable<SsisProject>
	{
		/// <summary>
		/// Collection of all the SQL Server Integration Services (SSIS) packages in the project.
		/// </summary>
		public List<Package> Packages { get; }

		public SsisProject()
		{
			Packages = new List<Package>();
		}

		public bool Equals([AllowNull] SsisProject other)
		{
			bool equal = true;

			if (other == null)
				equal = false;
			else if (Packages.Count == other.Packages.Count)
			{
				for (int i = 0; i < Packages.Count; i++)
				{
					if (!Packages[i].Equals(other.Packages[i]))
					{
						equal = false;
						break;
					}
				}
			}
			else
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as SsisProject);
		}

		public override int GetHashCode()
		{
			return Packages.GetHashCode();
		}
	}

	/// <summary>
	/// Element in collection of all the SQL Server Integration Services (SSIS) packages in the project.
	/// </summary>
	public class Package : IEquatable<Package>
	{
		/// <summary>
		/// Collection of all the connections in the package.
		/// </summary>
		public List<Connection> Connections { get; }

		/// <summary>
		/// Collection of all the tasks in the package.
		/// </summary>
		public List<Task> Tasks { get; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// The locale ID for the package. Uses the building operating system's regional settings if left empty, or the project-level LocaleId if it's set.
		/// </summary>
		[DefaultValue(0)]
		public int LocaleId { get; set; }

		/// <summary>
		/// Validation should be delayed until runtime.
		/// </summary>
		[DefaultValue(false)]
		public bool DelayValidation { get; set; }

		public Package()
		{
			Connections = new List<Connection>();
			Tasks = new List<Task>();
		}

		public bool Equals([AllowNull] Package other)
		{
			bool equal = true;

			if (other == null)
				equal = false;
			else if (Connections.Count == other.Connections.Count)
			{
				for (int i = 0; i < Connections.Count; i++)
				{
					if (!Connections[i].Equals(other.Connections[i]))
					{
						equal = false;
						break;
					}
				}

				if (equal && Tasks.Count == other.Tasks.Count)
				{
					for (int i = 0; i < Tasks.Count; i++)
					{
						if (!Tasks[i].Equals(other.Tasks[i]))
						{
							equal = false;
							break;
						}
					}
				}
				else
				{
					equal = false;
				}
			}
			else
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as Package);
		}

		public override int GetHashCode()
		{
			int hash = Connections.GetHashCode() ^ Tasks.GetHashCode() ^ LocaleId.GetHashCode() ^ DelayValidation.GetHashCode();
			if (Name != null)
				hash ^= Name.GetHashCode();

			return hash;
		}
	}

	/// <summary>
	/// Element in collection of child tasks.
	/// </summary>
	public abstract class Task : IEquatable<Task>
	{
		/// <summary>
		/// Collection of precedence constraints.
		/// </summary>
		public PrecedenceConstraintList? PrecedenceConstraints { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Validation should be delayed until runtime.
		/// </summary>
		[DefaultValue(false)]
		public bool DelayValidation { get; set; }

		/// <summary>
		/// If false, an OnError EventHandler with Propagate = false is created for this task.
		/// </summary>
		[DefaultValue(true)]
		public bool PropagateErrors { get; set; }

		public virtual bool Equals([AllowNull] Task other)
		{
			bool equal = true;

			if (other == null)
				equal = false;
			else if (
				Name != other.Name ||
				DelayValidation != other.DelayValidation ||
				PropagateErrors != other.PropagateErrors
				)
			{
				equal = false;
			}
			else
			{
				if (PrecedenceConstraints != null && !PrecedenceConstraints.Equals(other.PrecedenceConstraints))
					equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as Task);
		}

		public override int GetHashCode()
		{
			int hash = DelayValidation.GetHashCode() ^ PropagateErrors.GetHashCode();
			if (Name != null)
				hash ^= Name.GetHashCode();
			if (PrecedenceConstraints != null)
				hash ^= PrecedenceConstraints.GetHashCode();

			return hash;
		}
	}

	/// <summary>
	/// Element in collection of precedence constraints.
	/// </summary>
	public class PrecedenceConstraintList : IEquatable<PrecedenceConstraintList>
	{
		/// <summary>
		/// Collection of precedence constraint input paths.
		/// </summary>
		public List<InputPath> Inputs { get; }

		/// <summary>
		/// Is this an "And" or an "Or" precedence constraint list?
		/// </summary>
		[DefaultValue(LogicalOperation.And)]
		public LogicalOperation LogicalType { get; set; }

		public PrecedenceConstraintList()
		{
			Inputs = new List<InputPath>();
		}

		public bool Equals([AllowNull] PrecedenceConstraintList other)
		{
			bool equal = true;
			if (other == null)
				equal = false;
			else if (Inputs.Count == other.Inputs.Count)
			{
				for (int i = 0; i < Inputs.Count; i++)
				{
					if (!Inputs[i].Equals(other.Inputs[i]))
					{
						equal = false;
						break;
					}
				}
			}
			else
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as PrecedenceConstraintList);
		}

		public override int GetHashCode()
		{
			return Inputs.GetHashCode() ^ LogicalType.GetHashCode();
		}
	}

	/// <summary>
	/// Element in collection of precedence constraint input paths.
	/// </summary>
	public class InputPath : IEquatable<InputPath>
	{
		public string? OutputPathName { get; set; }

		/// <summary>
		/// The evaluation operation.
		/// </summary>
		[DefaultValue(TaskEvaluationOperationType.Constraint)]
		public TaskEvaluationOperationType EvaluationOperation { get; set; }

		/// <summary>
		/// The evaluation value.
		/// </summary>
		[DefaultValue(TaskEvaluationOperationValue.Success)]
		public TaskEvaluationOperationValue EvaluationValue { get; set; }

		/// <summary>
		/// The SSIS expression that must evaluate to "True" for execution to continue along this path
		/// </summary>
		public string? Expression { get; set; }

		public bool Equals([AllowNull] InputPath other)
		{
			bool equal = true;

			if (other == null)
				equal = false;
			else if (OutputPathName != other.OutputPathName ||
				EvaluationOperation != other.EvaluationOperation ||
				EvaluationValue != other.EvaluationValue ||
				Expression != other.Expression)
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as InputPath);
		}

		public override int GetHashCode()
		{
			int hash = EvaluationOperation.GetHashCode() ^ EvaluationValue.GetHashCode();
			if (OutputPathName != null)
				hash ^= OutputPathName.GetHashCode();
			if (Expression != null)
				hash ^= Expression.GetHashCode();

			return hash;
		}
	}

	/// <summary>
	/// Element in collection of all the connections in the project.
	/// </summary>
	public class Connection : IEquatable<Connection>
	{
		/// <summary>
		/// The connection string to use.
		/// </summary>
		public string? ConnectionString { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Pre-defined GUID for the connection. Use this if you want the connection to work with incremental builds, or if you want to be able to deploy packages individually.
		/// </summary>
		public string? ConnectionGuid { get; set; }

		/// <summary>
		/// Validation should be delayed until runtime.
		/// </summary>
		[DefaultValue(false)]
		public bool DelayValidation { get; set; }

		public virtual bool Equals([AllowNull] Connection other)
		{
			bool equal = true;
			if (other == null)
				equal = false;
			else if (ConnectionString != other.ConnectionString ||
				Name != other.Name ||
				ConnectionGuid != other.ConnectionGuid
				)
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as Connection);
		}

		public override int GetHashCode()
		{
			int hash = DelayValidation.GetHashCode();
			if (ConnectionString != null)
				hash ^= ConnectionString.GetHashCode();
			if (Name != null)
				hash ^= Name.GetHashCode();
			if (ConnectionGuid != null)
				hash ^= ConnectionGuid.GetHashCode();

			return hash;
		}
	}

	public class CustomConnection : Connection, IEquatable<Connection>
	{
		/// <summary>
		/// The CreationName of the custom connection.
		/// </summary>
		public string? CreationName { get; set; }

		public override bool Equals([AllowNull] Connection other)
		{
			bool equal = true;
			if (other is CustomConnection otherCustom)
			{
				if (CreationName != otherCustom.CreationName)
					equal = false;
				else if (!base.Equals(other))
					equal = false;
			}
			else
			{
				equal = false;
			}

			return equal;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class OleDbConnection : Connection, IEquatable<Connection>
	{
		public override bool Equals([AllowNull] Connection other)
		{
			return base.Equals(other);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class FlatFileConnection : Connection, IEquatable<Connection>
	{
		/// <summary>
		/// Collection of flat file columns.
		/// </summary>
		public List<FlatFileColumn> FlatFileColumns { get; }

		/// <summary>
		/// The file's format (FixedWidth, Delimited or RaggedRight).
		/// </summary>
		[DefaultValue(FlatFileFormat.Delimited)]
		public FlatFileFormat Format { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(true)]
		public bool ColumnNamesInFirstDataRow { get; set; }

		/// <summary>
		/// The number of rows to skip before reading data.
		/// </summary>
		[DefaultValue(0)]
		public int HeaderRowsToSkip { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(true)]
		public bool Unicode { get; set; }

		/// <summary>
		/// The encoding of the file.
		/// </summary>
		[DefaultValue(65001)]
		public int CodePage { get; set; }

		/// <summary>
		/// The locale ID of the file. Uses the building operating system's regional settings if left empty.
		/// </summary>
		[DefaultValue(0)]
		public int LocaleId { get; set; }

		/// <summary>
		/// The text qualifier of the file.
		/// </summary>
		public string? TextQualifier { get; set; }

		public FlatFileConnection()
		{
			FlatFileColumns = new List<FlatFileColumn>();
		}

		public override bool Equals([AllowNull] Connection other)
		{
			bool equal = true;

			if (other is FlatFileConnection otherFlatFile)
			{
				if (CodePage != otherFlatFile.CodePage ||
					ColumnNamesInFirstDataRow != otherFlatFile.ColumnNamesInFirstDataRow ||
					ConnectionString != otherFlatFile.ConnectionString ||
					DelayValidation != otherFlatFile.DelayValidation ||
					Format != otherFlatFile.Format ||
					ConnectionGuid != otherFlatFile.ConnectionGuid ||
					HeaderRowsToSkip != otherFlatFile.HeaderRowsToSkip ||
					LocaleId != otherFlatFile.LocaleId ||
					Name != otherFlatFile.Name ||
					TextQualifier != otherFlatFile.TextQualifier ||
					Unicode != otherFlatFile.Unicode
					)
				{
					equal = false;
				}
				else if (!base.Equals(other))
				{
					equal = false;
				}
				else if (FlatFileColumns.Count == otherFlatFile.FlatFileColumns.Count)
				{
					for (int i = 0; i < FlatFileColumns.Count; i++)
					{
						if (!FlatFileColumns[i].Equals(otherFlatFile.FlatFileColumns[i]))
						{
							equal = false;
							break;
						}
					}
				}
				else
				{
					equal = false;
				}
			}
			else
			{
				equal = false;
			}

			return equal;
		}
	}
	/// <summary>
	/// Element in collection of flat file columns.
	/// </summary>
	public class FlatFileColumn : IEquatable<FlatFileColumn>
	{
		/// <summary>
		/// The name of the object.
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// The column's data type.
		/// </summary>
		[DefaultValue(DatabaseType.Int32)]
		public DatabaseType DataType { get; set; }

		/// <summary>
		/// The width of the column in the source file.
		/// </summary>
		[DefaultValue(0)]
		public int InputWidth { get; set; }

		/// <summary>
		/// The width of the column in the data flow.
		/// </summary>
		[DefaultValue(0)]
		public int OutputWidth { get; set; }

		/// <summary>
		/// The precision of this column's data type. This only applies to data type definitions that accept a precision parameter, such as Decimal.
		/// </summary>
		[DefaultValue(0)]
		public int Precision { get; set; }

		/// <summary>
		/// The scale of this column's data type. This only applies to data type definitions that accept a scale parameter, such as Decimal.
		/// </summary>
		[DefaultValue(0)]
		public int Scale { get; set; }

		/// <summary>
		/// The delimiter of the column.
		/// </summary>
		[DefaultValue(",")]
		public string Delimiter { get; set; }

		/// <summary>
		/// The codepage of the column. This only applies to string-type columns.
		/// </summary>
		[DefaultValue(0)]
		public int CodePage { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(false)]
		public bool TextQualified { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(false)]
		public bool FastParse { get; set; }

		public FlatFileColumn()
		{
			Delimiter = ",";
		}

		public bool Equals([AllowNull] FlatFileColumn other)
		{
			bool equal = true;

			if (other == null)
				equal = false;
			else if (CodePage != other.CodePage ||
				DataType != other.DataType ||
				Delimiter != other.Delimiter ||
				FastParse != other.FastParse ||
				InputWidth != other.InputWidth ||
				Name != other.Name ||
				OutputWidth != other.OutputWidth ||
				Precision != other.Precision ||
				Scale != other.Scale ||
				TextQualified != other.TextQualified ||
				FastParse != other.FastParse
				)
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as FlatFileColumn);
		}

		public override int GetHashCode()
		{
			int hash = CodePage.GetHashCode() ^ DataType.GetHashCode() ^ Delimiter.GetHashCode() ^
				FastParse.GetHashCode() ^ InputWidth.GetHashCode() ^ OutputWidth.GetHashCode() ^
				Precision.GetHashCode() ^ Scale.GetHashCode() ^ TextQualified.GetHashCode();
			if (Name != null)
				hash ^= Name.GetHashCode();

			return hash;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class SequenceContainer : Task, IEquatable<Task>
	{
		/// <summary>
		/// Collection of child tasks.
		/// </summary>
		public List<Task> Tasks { get; }

		public SequenceContainer()
		{
			Tasks = new List<Task>();
		}

		public override bool Equals([AllowNull] Task other)
		{
			bool equal = true;

			if (other is SequenceContainer otherSeq)
			{
				if (!base.Equals(other))
					equal = false;
				else if (Tasks.Count == otherSeq.Tasks.Count)
				{
					for (int i = 0; i < Tasks.Count; i++)
					{
						if (!Tasks[i].Equals(otherSeq.Tasks[i]))
						{
							equal = false;
							break;
						}
					}
				}

			}
			else
			{
				equal = false;
			}

			return equal;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class ExecuteSql : Task, IEquatable<Task>
	{
		/// <summary>
		/// The SQL statement to execute.
		/// </summary>
		public SqlStatement? SqlStatement { get; set; }

		/// <summary>
		/// Collection of result-to-variable mappings for SQL query results.
		/// </summary>
		public List<Result> Results { get; }

		/// <summary>
		/// Collection of variable-to-parameter mappings for SQL query parameters.
		/// </summary>
		public List<SqlParameter> SqlParameters { get; }

		/// <summary>
		/// Specifies the connection used to access the data.
		/// </summary>
		public string? ConnectionName { get; set; }

		/// <summary>
		/// Specifies the format of the query results.
		/// </summary>
		[DefaultValue(ExecuteSqlResultSet.None)]
		public ExecuteSqlResultSet ResultSet { get; set; }

		/// <summary>
		/// Specifies how the task will perform value to variable type conversions.
		/// </summary>
		[DefaultValue(ExecuteSqlTypeConversionMode.Allowed)]
		public ExecuteSqlTypeConversionMode TypeConversionMode { get; set; }

		/// <summary>
		/// Indicates wheter the task should prepare the query before executing it.
		/// </summary>
		[DefaultValue(true)]
		public bool BypassPrepare { get; set; }

		/// <summary>
		/// Specifies the time-out value.
		/// </summary>
		[DefaultValue(0U)]
		public uint TimeOut { get; set; }

		/// <summary>
		/// Specifies the code page value.
		/// </summary>
		[DefaultValue(1252U)]
		public uint CodePage { get; set; }

		public ExecuteSql()
		{
			Results = new List<Result>();
			SqlParameters = new List<SqlParameter>();
		}

		public override bool Equals([AllowNull] Task other)
		{
			bool equal = true;

			if (other is ExecuteSql otherSql)
			{
				if (!base.Equals(other))
					equal = false;
				else if (BypassPrepare != otherSql.BypassPrepare ||
					CodePage != otherSql.CodePage ||
					ConnectionName != otherSql.ConnectionName ||
					DelayValidation != otherSql.DelayValidation ||
					Name != otherSql.Name ||
					PropagateErrors != otherSql.PropagateErrors ||
					ResultSet != otherSql.ResultSet ||
					TimeOut != otherSql.TimeOut ||
					TypeConversionMode != otherSql.TypeConversionMode
					)
				{
					equal = false;
				}
				else if (SqlStatement != null && !SqlStatement.Equals(otherSql.SqlStatement))
				{
					equal = false;
				}
				else if (Results.Count == otherSql.Results.Count)
				{
					for (int i = 0; i < Results.Count; i++)
					{
						if (!Results[i].Equals(otherSql.Results[i]))
						{
							equal = false;
							break;
						}
					}

					if (equal && SqlParameters.Count == otherSql.SqlParameters.Count)
					{
						for (int i = 0; i < SqlParameters.Count; i++)
						{
							if (!SqlParameters[i].Equals(otherSql.SqlParameters[i]))
							{
								equal = false;
								break;
							}
						}
					}
					else
						equal = false;
				}
				else
					equal = false;
			}

			return equal;
		}
	}

	/// <summary>
	/// Element in collection of result-to-variable mappings for SQL query results.
	/// </summary>
	public class Result : IEquatable<Result>
	{
		/// <summary>
		/// The name of the result to map into a variable.
		/// </summary>
		public string? ResultName { get; set; }

		/// <summary>
		/// The variable to map the result into.
		/// </summary>
		public string? VariableName { get; set; }

		public bool Equals([AllowNull] Result other)
		{
			bool equal = true;

			if (other == null)
				equal = false;
			else if (ResultName != other.ResultName ||
				VariableName != other.VariableName)
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as Result);
		}

		public override int GetHashCode()
		{
			int hash = 0;

			if (ResultName != null && VariableName != null)
				hash = ResultName.GetHashCode() ^ VariableName.GetHashCode();
			else if (ResultName != null)
				hash = ResultName.GetHashCode();
			else if (VariableName != null)
				hash = VariableName.GetHashCode();

			return hash;
		}
	}

	public class SqlStatement : IEquatable<SqlStatement>
	{
		public string? Value { get; set; }

		public bool Equals([AllowNull] SqlStatement other)
		{
			bool equal = true;

			if (other == null)
				equal = false;
			else if (Value != other.Value)
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as SqlStatement);
		}

		public override int GetHashCode()
		{
			int hash = 0;
			if (Value != null)
				hash = Value.GetHashCode();

			return hash;
		}
	}

	/// <summary>
	/// Element in collection of variable-to-parameter mappings for SQL query parameters.
	/// </summary>
	public class SqlParameter : IEquatable<SqlParameter>
	{
		/// <summary>
		/// The variable to map into the specified query parameter.
		/// </summary>
		public string? VariableName { get; set; }

		/// <summary>
		/// The name of the query parameter to map the variable name against.
		/// </summary>
		public string? ParameterName { get; set; }

		/// <summary>
		/// The data type of the query parameter.
		/// </summary>
		public DatabaseType DataType { get; set; }

		/// <summary>
		/// The direction of the query parameter.
		/// </summary>
		[DefaultValue(ParameterDirection.Input)]
		public ParameterDirection Direction { get; set; }

		/// <summary>
		/// The size (length) of the mapped parameter value. Only relevant when used with an applicable data type (string, binary etc).
		/// </summary>
		[DefaultValue("0")]
		public string Size { get; set; }

		public SqlParameter()
		{
			Size = "0";
		}

		public bool Equals([AllowNull] SqlParameter other)
		{
			bool equal = true;

			if (other == null)
				equal = false;
			else if (DataType != other.DataType ||
				Direction != other.Direction ||
				ParameterName != other.ParameterName ||
				Size != other.Size ||
				VariableName != other.VariableName
				)
			{
				equal = false;
			}

			return equal;
		}

		public override bool Equals([AllowNull] object obj)
		{
			return Equals(obj as SqlParameter);
		}

		public override int GetHashCode()
		{
			int hash = DataType.GetHashCode() ^ Direction.GetHashCode() ^ Size.GetHashCode();
			if (VariableName != null)
				hash ^= VariableName.GetHashCode();
			if (ParameterName != null)
				hash ^= ParameterName.GetHashCode();

			return hash;
		}
	}

	public enum ExecuteSqlTypeConversionMode
	{
		None,
		Allowed,
	}

	public enum ExecuteSqlResultSet
	{
		None,
		SingleRow,
		Full,
		Xml,
	}

	/// <summary>
	/// 
	/// </summary>
	public class Expression : Task, IEquatable<Task>
	{
		/// <summary>
		/// Expression.
		/// </summary>
		[IsRequired]
		public string? ExpressionValue { get; set; }

		public override bool Equals([AllowNull] Task other)
		{
			bool equal = true;

			if (other is Expression otherExp)
			{
				if (!base.Equals(other))
				{
					equal = false;
				}
				else if (ExpressionValue != otherExp.ExpressionValue)
				{
					equal = false;
				}
			}
			else
			{
				equal = false;
			}

			return equal;
		}
	}

	public enum ParameterDirection
	{
		Input,
		Output,
		ReturnValue,
	}


	public enum TaskEvaluationOperationType
	{
		Constraint,
		Expression,
		ExpressionAndConstraint,
		ExpressionOrConstraint,
	}

	public enum TaskEvaluationOperationValue
	{
		Success,
		Failure,
		Completion,
	}

	public enum LogicalOperation
	{
		And,
		Or,
	}

	public enum FlatFileFormat
	{
		Delimited,
		FixedWidth,
		RaggedRight,
	}

	[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "These are enums of data types")]
	public enum DatabaseType
	{
		AnsiString,
		AnsiStringFixedLength,
		Binary,
		Byte,
		Boolean,
		Currency,
		Date,
		DateTime,
		DateTime2,
		DateTimeOffset,
		Decimal,
		Double,
		Guid,
		Int16,
		Int32,
		Int64,
		Object,
		SByte,
		Single,
		String,
		StringFixedLength,
		Time,
		UInt16,
		UInt32,
		UInt64,
		VarNumeric,
		Xml,
	}
}

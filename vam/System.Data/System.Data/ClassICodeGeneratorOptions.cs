using System.CodeDom.Compiler;

namespace System.Data;

internal class ClassICodeGeneratorOptions : ClassGeneratorOptions
{
	private ICodeGenerator gen;

	public CodeNamingMethod CreateDataSetName;

	public CodeNamingMethod CreateTableTypeName;

	public CodeNamingMethod CreateTableMemberName;

	public CodeNamingMethod CreateTableColumnName;

	public CodeNamingMethod CreateColumnName;

	public CodeNamingMethod CreateRowName;

	public CodeNamingMethod CreateRelationName;

	public CodeNamingMethod CreateTableDelegateName;

	public CodeNamingMethod CreateEventArgsName;

	public CodeNamingMethod CreateTableAdapterNSName;

	public CodeNamingMethod CreateTableAdapterName;

	public ClassICodeGeneratorOptions(ICodeGenerator codeGen)
	{
		gen = codeGen;
	}

	internal override string DataSetName(string source)
	{
		if (CreateDataSetName != null)
		{
			return CreateDataSetName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen);
	}

	internal override string TableTypeName(string source)
	{
		if (CreateTableTypeName != null)
		{
			return CreateTableTypeName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen) + "DataTable";
	}

	internal override string TableMemberName(string source)
	{
		if (CreateTableMemberName != null)
		{
			return CreateTableMemberName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen);
	}

	internal override string TableColName(string source)
	{
		if (CreateTableColumnName != null)
		{
			return CreateTableColumnName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen);
	}

	internal override string TableDelegateName(string source)
	{
		if (CreateTableDelegateName != null)
		{
			return CreateTableDelegateName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen) + "RowChangedEventHandler";
	}

	internal override string EventArgsName(string source)
	{
		if (CreateEventArgsName != null)
		{
			return CreateEventArgsName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen) + "RowChangedEventArgs";
	}

	internal override string ColumnName(string source)
	{
		if (CreateColumnName != null)
		{
			return CreateColumnName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen);
	}

	internal override string RowName(string source)
	{
		if (CreateRowName != null)
		{
			return CreateRowName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen) + "Row";
	}

	internal override string RelationName(string source)
	{
		if (CreateRelationName != null)
		{
			return CreateRelationName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen) + "Relation";
	}

	internal override string TableAdapterNSName(string source)
	{
		if (CreateTableAdapterNSName != null)
		{
			return CreateTableAdapterNSName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen) + "TableAdapters";
	}

	internal override string TableAdapterName(string source)
	{
		if (CreateTableAdapterName != null)
		{
			return CreateTableAdapterName(source, gen);
		}
		return CustomDataClassGenerator.MakeSafeName(source, gen);
	}
}

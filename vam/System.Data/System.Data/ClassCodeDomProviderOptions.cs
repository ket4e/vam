using System.CodeDom.Compiler;

namespace System.Data;

internal class ClassCodeDomProviderOptions : ClassGeneratorOptions
{
	private CodeDomProvider provider;

	public CodeDomNamingMethod CreateDataSetName;

	public CodeDomNamingMethod CreateTableTypeName;

	public CodeDomNamingMethod CreateTableMemberName;

	public CodeDomNamingMethod CreateTableColumnName;

	public CodeDomNamingMethod CreateColumnName;

	public CodeDomNamingMethod CreateRowName;

	public CodeDomNamingMethod CreateRelationName;

	public CodeDomNamingMethod CreateTableDelegateName;

	public CodeDomNamingMethod CreateEventArgsName;

	public CodeDomNamingMethod CreateTableAdapterNSName;

	public CodeDomNamingMethod CreateTableAdapterName;

	public ClassCodeDomProviderOptions(CodeDomProvider codeProvider)
	{
		provider = codeProvider;
	}

	internal override string DataSetName(string source)
	{
		if (CreateDataSetName != null)
		{
			return CreateDataSetName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider);
	}

	internal override string TableTypeName(string source)
	{
		if (CreateTableTypeName != null)
		{
			return CreateTableTypeName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider) + "DataTable";
	}

	internal override string TableMemberName(string source)
	{
		if (CreateTableMemberName != null)
		{
			return CreateTableMemberName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider);
	}

	internal override string TableColName(string source)
	{
		if (CreateTableColumnName != null)
		{
			return CreateTableColumnName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider);
	}

	internal override string TableDelegateName(string source)
	{
		if (CreateTableDelegateName != null)
		{
			return CreateTableDelegateName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider) + "RowChangedEventHandler";
	}

	internal override string EventArgsName(string source)
	{
		if (CreateEventArgsName != null)
		{
			return CreateEventArgsName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider) + "RowChangedEventArgs";
	}

	internal override string ColumnName(string source)
	{
		if (CreateColumnName != null)
		{
			return CreateColumnName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider);
	}

	internal override string RowName(string source)
	{
		if (CreateRowName != null)
		{
			return CreateRowName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider) + "Row";
	}

	internal override string RelationName(string source)
	{
		if (CreateRelationName != null)
		{
			return CreateRelationName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider) + "Relation";
	}

	internal override string TableAdapterNSName(string source)
	{
		if (CreateTableAdapterNSName != null)
		{
			return CreateTableAdapterNSName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider) + "TableAdapters";
	}

	internal override string TableAdapterName(string source)
	{
		if (CreateTableAdapterName != null)
		{
			return CreateTableAdapterName(source, provider);
		}
		return CustomDataClassGenerator.MakeSafeName(source, provider);
	}
}

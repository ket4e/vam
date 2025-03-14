using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data;

internal class Generator
{
	private DataSet ds;

	private CodeNamespace cns;

	private ClassGeneratorOptions opts;

	private CodeCompileUnit cunit;

	private CodeTypeDeclaration dsType;

	public Generator(DataSet ds, CodeNamespace cns, ICodeGenerator codeGen, ClassGeneratorOptions options)
	{
		this.ds = ds;
		this.cns = cns;
		opts = options;
		cunit = null;
		if (opts == null)
		{
			opts = new ClassICodeGeneratorOptions(codeGen);
		}
	}

	public Generator(DataSet ds, CodeNamespace cns, CodeDomProvider codeProvider, ClassGeneratorOptions options)
	{
		this.ds = ds;
		this.cns = cns;
		opts = options;
		cunit = null;
		if (opts == null)
		{
			opts = new ClassCodeDomProviderOptions(codeProvider);
		}
	}

	public Generator(DataSet ds, CodeCompileUnit cunit, CodeNamespace cns, CodeDomProvider codeProvider, ClassGeneratorOptions options)
	{
		this.ds = ds;
		this.cns = cns;
		opts = options;
		this.cunit = cunit;
		if (opts == null)
		{
			opts = new ClassCodeDomProviderOptions(codeProvider);
		}
	}

	public void Run()
	{
		cns.Imports.Add(new CodeNamespaceImport("System"));
		cns.Imports.Add(new CodeNamespaceImport("System.Collections"));
		cns.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
		cns.Imports.Add(new CodeNamespaceImport("System.Data"));
		cns.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
		cns.Imports.Add(new CodeNamespaceImport("System.Xml"));
		CodeTypeDeclaration codeTypeDeclaration = GenerateDataSetType();
		cns.Types.Add(codeTypeDeclaration);
		foreach (DataTable table in ds.Tables)
		{
			CodeTypeDeclaration value = GenerateDataTableType(table);
			CodeTypeDeclaration value2 = GenerateDataRowType(table);
			CodeTypeDelegate codeTypeDelegate = new CodeTypeDelegate(opts.TableDelegateName(table.TableName));
			codeTypeDelegate.Parameters.Add(Param(typeof(object), "o"));
			codeTypeDelegate.Parameters.Add(Param(opts.EventArgsName(table.TableName), "e"));
			CodeTypeDeclaration value3 = GenerateEventType(table);
			if (opts.MakeClassesInsideDataSet)
			{
				codeTypeDeclaration.Members.Add(value);
				codeTypeDeclaration.Members.Add(value2);
				codeTypeDeclaration.Members.Add(codeTypeDelegate);
				codeTypeDeclaration.Members.Add(value3);
			}
			else
			{
				cns.Types.Add(value);
				cns.Types.Add(value2);
				cns.Types.Add(codeTypeDelegate);
				cns.Types.Add(value3);
			}
		}
		if (cunit != null)
		{
			TableAdapterSchemaInfo tableAdapterSchemaData = ds.TableAdapterSchemaData;
			if (tableAdapterSchemaData != null)
			{
				CodeNamespace codeNamespace = new CodeNamespace(opts.TableAdapterNSName(opts.DataSetName(ds.DataSetName)));
				CodeTypeDeclaration value4 = GenerateTableAdapterType(tableAdapterSchemaData);
				codeNamespace.Types.Add(value4);
				cunit.Namespaces.Add(codeNamespace);
			}
		}
	}

	private CodeThisReferenceExpression This()
	{
		return new CodeThisReferenceExpression();
	}

	private CodeBaseReferenceExpression Base()
	{
		return new CodeBaseReferenceExpression();
	}

	private CodePrimitiveExpression Const(object value)
	{
		return new CodePrimitiveExpression(value);
	}

	private CodeTypeReference TypeRef(Type t)
	{
		return new CodeTypeReference(t);
	}

	private CodeTypeReference TypeRef(string name)
	{
		return new CodeTypeReference(name);
	}

	private CodeTypeReference TypeRefArray(Type t, int dimension)
	{
		return new CodeTypeReference(TypeRef(t), dimension);
	}

	private CodeTypeReference TypeRefArray(string name, int dimension)
	{
		return new CodeTypeReference(TypeRef(name), dimension);
	}

	private CodeParameterDeclarationExpression Param(string t, string name)
	{
		return new CodeParameterDeclarationExpression(t, name);
	}

	private CodeParameterDeclarationExpression Param(Type t, string name)
	{
		return new CodeParameterDeclarationExpression(t, name);
	}

	private CodeParameterDeclarationExpression Param(CodeTypeReference t, string name)
	{
		return new CodeParameterDeclarationExpression(t, name);
	}

	private CodeArgumentReferenceExpression ParamRef(string name)
	{
		return new CodeArgumentReferenceExpression(name);
	}

	private CodeCastExpression Cast(string t, CodeExpression exp)
	{
		return new CodeCastExpression(t, exp);
	}

	private CodeCastExpression Cast(Type t, CodeExpression exp)
	{
		return new CodeCastExpression(t, exp);
	}

	private CodeCastExpression Cast(CodeTypeReference t, CodeExpression exp)
	{
		return new CodeCastExpression(t, exp);
	}

	private CodeExpression New(Type t, params CodeExpression[] parameters)
	{
		return new CodeObjectCreateExpression(t, parameters);
	}

	private CodeExpression New(string t, params CodeExpression[] parameters)
	{
		return new CodeObjectCreateExpression(TypeRef(t), parameters);
	}

	private CodeExpression NewArray(Type t, params CodeExpression[] parameters)
	{
		return new CodeArrayCreateExpression(t, parameters);
	}

	private CodeExpression NewArray(Type t, int size)
	{
		return new CodeArrayCreateExpression(t, size);
	}

	private CodeVariableReferenceExpression Local(string name)
	{
		return new CodeVariableReferenceExpression(name);
	}

	private CodeFieldReferenceExpression FieldRef(string name)
	{
		return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), name);
	}

	private CodeFieldReferenceExpression FieldRef(CodeExpression exp, string name)
	{
		return new CodeFieldReferenceExpression(exp, name);
	}

	private CodePropertyReferenceExpression PropRef(string name)
	{
		return new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name);
	}

	private CodePropertyReferenceExpression PropRef(CodeExpression target, string name)
	{
		return new CodePropertyReferenceExpression(target, name);
	}

	private CodeIndexerExpression IndexerRef(CodeExpression target, CodeExpression parameters)
	{
		return new CodeIndexerExpression(target, parameters);
	}

	private CodeIndexerExpression IndexerRef(CodeExpression param)
	{
		return new CodeIndexerExpression(new CodeThisReferenceExpression(), param);
	}

	private CodeEventReferenceExpression EventRef(string name)
	{
		return new CodeEventReferenceExpression(new CodeThisReferenceExpression(), name);
	}

	private CodeEventReferenceExpression EventRef(CodeExpression target, string name)
	{
		return new CodeEventReferenceExpression(target, name);
	}

	private CodeMethodInvokeExpression MethodInvoke(string name, params CodeExpression[] parameters)
	{
		return new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), name, parameters);
	}

	private CodeMethodInvokeExpression MethodInvoke(CodeExpression target, string name, params CodeExpression[] parameters)
	{
		return new CodeMethodInvokeExpression(target, name, parameters);
	}

	private CodeBinaryOperatorExpression EqualsValue(CodeExpression exp1, CodeExpression exp2)
	{
		return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.ValueEquality, exp2);
	}

	private CodeBinaryOperatorExpression Equals(CodeExpression exp1, CodeExpression exp2)
	{
		return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.IdentityEquality, exp2);
	}

	private CodeBinaryOperatorExpression Inequals(CodeExpression exp1, CodeExpression exp2)
	{
		return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.IdentityInequality, exp2);
	}

	private CodeBinaryOperatorExpression GreaterThan(CodeExpression exp1, CodeExpression exp2)
	{
		return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.GreaterThan, exp2);
	}

	private CodeBinaryOperatorExpression LessThan(CodeExpression exp1, CodeExpression exp2)
	{
		return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.LessThan, exp2);
	}

	private CodeBinaryOperatorExpression Compute(CodeExpression exp1, CodeExpression exp2, CodeBinaryOperatorType ops)
	{
		if (ops >= CodeBinaryOperatorType.Add && ops < CodeBinaryOperatorType.Assign)
		{
			return new CodeBinaryOperatorExpression(exp1, ops, exp2);
		}
		return null;
	}

	private CodeBinaryOperatorExpression BitOps(CodeExpression exp1, CodeExpression exp2, CodeBinaryOperatorType ops)
	{
		if (ops >= CodeBinaryOperatorType.BitwiseOr && ops <= CodeBinaryOperatorType.BitwiseAnd)
		{
			return new CodeBinaryOperatorExpression(exp1, ops, exp2);
		}
		return null;
	}

	private CodeBinaryOperatorExpression BooleanOps(CodeExpression exp1, CodeExpression exp2, CodeBinaryOperatorType ops)
	{
		if (ops >= CodeBinaryOperatorType.BooleanOr && ops <= CodeBinaryOperatorType.BooleanAnd)
		{
			return new CodeBinaryOperatorExpression(exp1, ops, exp2);
		}
		return null;
	}

	private CodeTypeReferenceExpression TypeRefExp(Type t)
	{
		return new CodeTypeReferenceExpression(t);
	}

	private CodeTypeOfExpression TypeOfRef(string name)
	{
		return new CodeTypeOfExpression(TypeRef(name));
	}

	private CodeExpressionStatement Eval(CodeExpression exp)
	{
		return new CodeExpressionStatement(exp);
	}

	private CodeAssignStatement Let(CodeExpression exp, CodeExpression value)
	{
		return new CodeAssignStatement(exp, value);
	}

	private CodeMethodReturnStatement Return(CodeExpression exp)
	{
		return new CodeMethodReturnStatement(exp);
	}

	private CodeVariableDeclarationStatement VarDecl(Type t, string name, CodeExpression init)
	{
		return new CodeVariableDeclarationStatement(t, name, init);
	}

	private CodeVariableDeclarationStatement VarDecl(string t, string name, CodeExpression init)
	{
		return new CodeVariableDeclarationStatement(t, name, init);
	}

	private CodeCommentStatement Comment(string comment)
	{
		return new CodeCommentStatement(comment);
	}

	private CodeThrowExceptionStatement Throw(CodeExpression exp)
	{
		return new CodeThrowExceptionStatement(exp);
	}

	private CodeTypeDeclaration GenerateDataSetType()
	{
		dsType = new CodeTypeDeclaration(opts.DataSetName(ds.DataSetName));
		dsType.BaseTypes.Add(TypeRef(typeof(DataSet)));
		dsType.BaseTypes.Add(TypeRef(typeof(IXmlSerializable)));
		dsType.Members.Add(CreateDataSetDefaultCtor());
		dsType.Members.Add(CreateDataSetSerializationCtor());
		dsType.Members.Add(CreateDataSetCloneMethod(dsType));
		dsType.Members.Add(CreateDataSetGetSchemaSerializable());
		dsType.Members.Add(CreateDataSetGetSchema());
		dsType.Members.Add(CreateDataSetInitializeClass());
		dsType.Members.Add(CreateDataSetInitializeFields());
		dsType.Members.Add(CreateDataSetSchemaChanged());
		foreach (DataTable table in ds.Tables)
		{
			CreateDataSetTableMembers(dsType, table);
		}
		foreach (DataRelation relation in ds.Relations)
		{
			CreateDataSetRelationMembers(dsType, relation);
		}
		return dsType;
	}

	private CodeConstructor CreateDataSetDefaultCtor()
	{
		CodeConstructor codeConstructor = new CodeConstructor();
		codeConstructor.Attributes = MemberAttributes.Public;
		codeConstructor.Statements.Add(Eval(MethodInvoke("InitializeClass")));
		CodeVariableDeclarationStatement value = VarDecl(typeof(CollectionChangeEventHandler), "handler", New(typeof(CollectionChangeEventHandler), new CodeDelegateCreateExpression(new CodeTypeReference(typeof(CollectionChangeEventHandler)), new CodeThisReferenceExpression(), "SchemaChanged")));
		codeConstructor.Statements.Add(value);
		codeConstructor.Statements.Add(new CodeAttachEventStatement(EventRef(PropRef("Tables"), "CollectionChanged"), Local("handler")));
		codeConstructor.Statements.Add(new CodeAttachEventStatement(EventRef(PropRef("Relations"), "CollectionChanged"), Local("handler")));
		return codeConstructor;
	}

	private CodeConstructor CreateDataSetSerializationCtor()
	{
		CodeConstructor codeConstructor = new CodeConstructor();
		codeConstructor.Attributes = MemberAttributes.Family;
		codeConstructor.Parameters.Add(Param(typeof(SerializationInfo), "info"));
		codeConstructor.Parameters.Add(Param(typeof(StreamingContext), "ctx"));
		codeConstructor.Statements.Add(Comment("TODO: implement"));
		codeConstructor.Statements.Add(Throw(New(typeof(NotImplementedException))));
		return codeConstructor;
	}

	private CodeMemberMethod CreateDataSetCloneMethod(CodeTypeDeclaration dsType)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.ReturnType = TypeRef(typeof(DataSet));
		codeMemberMethod.Attributes = (MemberAttributes)24580;
		codeMemberMethod.Name = "Clone";
		CodeVariableReferenceExpression codeVariableReferenceExpression = Local("set");
		codeMemberMethod.Statements.Add(VarDecl(dsType.Name, "set", Cast(dsType.Name, MethodInvoke(Base(), "Clone"))));
		codeMemberMethod.Statements.Add(Eval(MethodInvoke(codeVariableReferenceExpression, "InitializeFields")));
		codeMemberMethod.Statements.Add(Return(codeVariableReferenceExpression));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateDataSetGetSchema()
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.PrivateImplementationType = TypeRef(typeof(IXmlSerializable));
		codeMemberMethod.Name = "GetSchema";
		codeMemberMethod.ReturnType = TypeRef(typeof(XmlSchema));
		codeMemberMethod.Statements.Add(Return(MethodInvoke("GetSchemaSerializable")));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateDataSetGetSchemaSerializable()
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Attributes = (MemberAttributes)12292;
		codeMemberMethod.Name = "GetSchemaSerializable";
		codeMemberMethod.ReturnType = TypeRef(typeof(XmlSchema));
		codeMemberMethod.Statements.Add(VarDecl(typeof(StringWriter), "sw", New(typeof(StringWriter))));
		codeMemberMethod.Statements.Add(Eval(MethodInvoke("WriteXmlSchema", Local("sw"))));
		codeMemberMethod.Statements.Add(Return(MethodInvoke(TypeRefExp(typeof(XmlSchema)), "Read", New(typeof(XmlTextReader), New(typeof(StringReader), MethodInvoke(Local("sw"), "ToString"))), Const(null))));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateDataSetInitializeClass()
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "InitializeClass";
		codeMemberMethod.Attributes = MemberAttributes.Assembly;
		codeMemberMethod.Statements.Add(Let(PropRef("DataSetName"), Const(ds.DataSetName)));
		codeMemberMethod.Statements.Add(Let(PropRef("Prefix"), Const(ds.Prefix)));
		codeMemberMethod.Statements.Add(Let(PropRef("Namespace"), Const(ds.Namespace)));
		codeMemberMethod.Statements.Add(Let(PropRef("Locale"), New(typeof(CultureInfo), Const(ds.Locale.Name))));
		codeMemberMethod.Statements.Add(Let(PropRef("CaseSensitive"), Const(ds.CaseSensitive)));
		codeMemberMethod.Statements.Add(Let(PropRef("EnforceConstraints"), Const(ds.EnforceConstraints)));
		foreach (DataTable table in ds.Tables)
		{
			string name = "__table" + opts.TableMemberName(table.TableName);
			string t = opts.TableTypeName(table.TableName);
			codeMemberMethod.Statements.Add(Let(FieldRef(name), New(t)));
			codeMemberMethod.Statements.Add(Eval(MethodInvoke(PropRef("Tables"), "Add", FieldRef(name))));
		}
		bool flag = false;
		bool flag2 = false;
		foreach (DataTable table2 in ds.Tables)
		{
			string tableField = "__table" + opts.TableMemberName(table2.TableName);
			foreach (Constraint constraint3 in table2.Constraints)
			{
				if (constraint3 is UniqueConstraint uc)
				{
					if (!flag2)
					{
						codeMemberMethod.Statements.Add(VarDecl(typeof(UniqueConstraint), "uc", null));
						flag2 = true;
					}
					CreateUniqueKeyStatements(codeMemberMethod, uc, tableField);
				}
			}
		}
		foreach (DataTable table3 in ds.Tables)
		{
			string tableField2 = "__table" + opts.TableMemberName(table3.TableName);
			foreach (Constraint constraint4 in table3.Constraints)
			{
				if (constraint4 is ForeignKeyConstraint foreignKeyConstraint)
				{
					if (!flag)
					{
						codeMemberMethod.Statements.Add(VarDecl(typeof(ForeignKeyConstraint), "fkc", null));
						flag = true;
					}
					string rtableField = "__table" + opts.TableMemberName(foreignKeyConstraint.RelatedTable.TableName);
					CreateForeignKeyStatements(codeMemberMethod, foreignKeyConstraint, tableField2, rtableField);
				}
			}
		}
		foreach (DataRelation relation in ds.Relations)
		{
			string text = opts.RelationName(relation.RelationName);
			ArrayList arrayList = new ArrayList();
			DataColumn[] parentColumns = relation.ParentColumns;
			foreach (DataColumn dataColumn in parentColumns)
			{
				arrayList.Add(IndexerRef(PropRef(FieldRef("__table" + opts.TableMemberName(relation.ParentTable.TableName)), "Columns"), Const(dataColumn.ColumnName)));
			}
			ArrayList arrayList2 = new ArrayList();
			DataColumn[] childColumns = relation.ChildColumns;
			foreach (DataColumn dataColumn2 in childColumns)
			{
				arrayList2.Add(IndexerRef(PropRef(FieldRef("__table" + opts.TableMemberName(relation.ChildTable.TableName)), "Columns"), Const(dataColumn2.ColumnName)));
			}
			string name2 = "__relation" + text;
			codeMemberMethod.Statements.Add(Let(FieldRef(name2), New(typeof(DataRelation), Const(relation.RelationName), NewArray(typeof(DataColumn), arrayList.ToArray(typeof(CodeExpression)) as CodeExpression[]), NewArray(typeof(DataColumn), arrayList2.ToArray(typeof(CodeExpression)) as CodeExpression[]), Const(false))));
			codeMemberMethod.Statements.Add(Let(PropRef(FieldRef(name2), "Nested"), Const(relation.Nested)));
			codeMemberMethod.Statements.Add(MethodInvoke(PropRef("Relations"), "Add", FieldRef(name2)));
		}
		return codeMemberMethod;
	}

	private void CreateUniqueKeyStatements(CodeMemberMethod m, UniqueConstraint uc, string tableField)
	{
		ArrayList arrayList = new ArrayList();
		DataColumn[] columns = uc.Columns;
		foreach (DataColumn dataColumn in columns)
		{
			arrayList.Add(IndexerRef(PropRef(FieldRef(tableField), "Columns"), Const(dataColumn.ColumnName)));
		}
		m.Statements.Add(Let(Local("uc"), New(typeof(UniqueConstraint), Const(uc.ConstraintName), NewArray(typeof(DataColumn), arrayList.ToArray(typeof(CodeExpression)) as CodeExpression[]), Const(uc.IsPrimaryKey))));
		m.Statements.Add(MethodInvoke(PropRef(FieldRef(tableField), "Constraints"), "Add", Local("uc")));
	}

	private void CreateForeignKeyStatements(CodeMemberMethod m, ForeignKeyConstraint fkc, string tableField, string rtableField)
	{
		ArrayList arrayList = new ArrayList();
		DataColumn[] relatedColumns = fkc.RelatedColumns;
		foreach (DataColumn dataColumn in relatedColumns)
		{
			arrayList.Add(IndexerRef(PropRef(FieldRef(rtableField), "Columns"), Const(dataColumn.ColumnName)));
		}
		ArrayList arrayList2 = new ArrayList();
		DataColumn[] columns = fkc.Columns;
		foreach (DataColumn dataColumn2 in columns)
		{
			arrayList2.Add(IndexerRef(PropRef(FieldRef(tableField), "Columns"), Const(dataColumn2.ColumnName)));
		}
		m.Statements.Add(Let(Local("fkc"), New(typeof(ForeignKeyConstraint), Const(fkc.ConstraintName), NewArray(typeof(DataColumn), arrayList.ToArray(typeof(CodeExpression)) as CodeExpression[]), NewArray(typeof(DataColumn), arrayList2.ToArray(typeof(CodeExpression)) as CodeExpression[]))));
		m.Statements.Add(Let(PropRef(Local("fkc"), "AcceptRejectRule"), FieldRef(TypeRefExp(typeof(AcceptRejectRule)), Enum.GetName(typeof(AcceptRejectRule), fkc.AcceptRejectRule))));
		m.Statements.Add(Let(PropRef(Local("fkc"), "DeleteRule"), FieldRef(TypeRefExp(typeof(Rule)), Enum.GetName(typeof(Rule), fkc.DeleteRule))));
		m.Statements.Add(Let(PropRef(Local("fkc"), "UpdateRule"), FieldRef(TypeRefExp(typeof(Rule)), Enum.GetName(typeof(Rule), fkc.UpdateRule))));
		m.Statements.Add(MethodInvoke(PropRef(FieldRef(tableField), "Constraints"), "Add", Local("fkc")));
	}

	private CodeMemberMethod CreateDataSetInitializeFields()
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Attributes = MemberAttributes.Assembly;
		codeMemberMethod.Name = "InitializeFields";
		foreach (DataTable table in ds.Tables)
		{
			codeMemberMethod.Statements.Add(Eval(MethodInvoke(FieldRef("__table" + opts.TableMemberName(table.TableName)), "InitializeFields")));
		}
		foreach (DataRelation relation in ds.Relations)
		{
			codeMemberMethod.Statements.Add(Let(FieldRef("__relation" + opts.RelationName(relation.RelationName)), IndexerRef(PropRef("Relations"), Const(relation.RelationName))));
		}
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateDataSetSchemaChanged()
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "SchemaChanged";
		codeMemberMethod.Parameters.Add(Param(typeof(object), "sender"));
		codeMemberMethod.Parameters.Add(Param(typeof(CollectionChangeEventArgs), "e"));
		codeMemberMethod.Statements.Add(new CodeConditionStatement(EqualsValue(PropRef(ParamRef("e"), "Action"), FieldRef(TypeRefExp(typeof(CollectionChangeAction)), "Remove")), new CodeStatement[1] { Eval(MethodInvoke("InitializeFields")) }, new CodeStatement[0]));
		return codeMemberMethod;
	}

	private void CreateDataSetTableMembers(CodeTypeDeclaration dsType, DataTable table)
	{
		string name = opts.TableTypeName(table.TableName);
		string text = opts.TableMemberName(table.TableName);
		CodeMemberField codeMemberField = new CodeMemberField();
		codeMemberField.Type = TypeRef(name);
		codeMemberField.Name = "__table" + text;
		dsType.Members.Add(codeMemberField);
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Type = TypeRef(name);
		codeMemberProperty.Attributes = MemberAttributes.Public;
		codeMemberProperty.Name = ((!(text == table.TableName)) ? text : ("_" + text));
		codeMemberProperty.HasSet = false;
		codeMemberProperty.GetStatements.Add(Return(FieldRef("__table" + text)));
		dsType.Members.Add(codeMemberProperty);
	}

	private void CreateDataSetRelationMembers(CodeTypeDeclaration dsType, DataRelation relation)
	{
		string text = opts.RelationName(relation.RelationName);
		string name = "__relation" + text;
		CodeMemberField codeMemberField = new CodeMemberField();
		codeMemberField.Type = TypeRef(typeof(DataRelation));
		codeMemberField.Name = name;
		dsType.Members.Add(codeMemberField);
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Type = TypeRef(typeof(DataRelation));
		codeMemberProperty.Attributes = MemberAttributes.Public;
		codeMemberProperty.Name = text;
		codeMemberProperty.HasSet = false;
		codeMemberProperty.GetStatements.Add(Return(FieldRef(name)));
		dsType.Members.Add(codeMemberProperty);
	}

	private CodeTypeDeclaration GenerateDataTableType(DataTable dt)
	{
		CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
		codeTypeDeclaration.Name = opts.TableTypeName(dt.TableName);
		codeTypeDeclaration.BaseTypes.Add(TypeRef(typeof(DataTable)));
		codeTypeDeclaration.BaseTypes.Add(TypeRef(typeof(IEnumerable)));
		codeTypeDeclaration.Members.Add(CreateTableCtor1(dt));
		codeTypeDeclaration.Members.Add(CreateTableCtor2(dt));
		codeTypeDeclaration.Members.Add(CreateTableCount(dt));
		codeTypeDeclaration.Members.Add(CreateTableIndexer(dt));
		codeTypeDeclaration.Members.Add(CreateTableInitializeClass(dt));
		codeTypeDeclaration.Members.Add(CreateTableInitializeFields(dt));
		codeTypeDeclaration.Members.Add(CreateTableGetEnumerator(dt));
		codeTypeDeclaration.Members.Add(CreateTableClone(dt));
		codeTypeDeclaration.Members.Add(CreateTableCreateInstance(dt));
		codeTypeDeclaration.Members.Add(CreateTableAddRow1(dt));
		codeTypeDeclaration.Members.Add(CreateTableAddRow2(dt));
		codeTypeDeclaration.Members.Add(CreateTableNewRow(dt));
		codeTypeDeclaration.Members.Add(CreateTableNewRowFromBuilder(dt));
		codeTypeDeclaration.Members.Add(CreateTableRemoveRow(dt));
		codeTypeDeclaration.Members.Add(CreateTableGetRowType(dt));
		codeTypeDeclaration.Members.Add(CreateTableEventStarter(dt, "Changing"));
		codeTypeDeclaration.Members.Add(CreateTableEventStarter(dt, "Changed"));
		codeTypeDeclaration.Members.Add(CreateTableEventStarter(dt, "Deleting"));
		codeTypeDeclaration.Members.Add(CreateTableEventStarter(dt, "Deleted"));
		codeTypeDeclaration.Members.Add(CreateTableEvent(dt, "RowChanging"));
		codeTypeDeclaration.Members.Add(CreateTableEvent(dt, "RowChanged"));
		codeTypeDeclaration.Members.Add(CreateTableEvent(dt, "RowDeleting"));
		codeTypeDeclaration.Members.Add(CreateTableEvent(dt, "RowDeleted"));
		foreach (DataColumn column in dt.Columns)
		{
			codeTypeDeclaration.Members.Add(CreateTableColumnField(dt, column));
			codeTypeDeclaration.Members.Add(CreateTableColumnProperty(dt, column));
		}
		return codeTypeDeclaration;
	}

	private CodeConstructor CreateTableCtor1(DataTable dt)
	{
		CodeConstructor codeConstructor = new CodeConstructor();
		codeConstructor.Attributes = MemberAttributes.Assembly;
		codeConstructor.BaseConstructorArgs.Add(Const(dt.TableName));
		codeConstructor.Statements.Add(Eval(MethodInvoke("InitializeClass")));
		codeConstructor.Statements.Add(Eval(MethodInvoke("InitializeFields")));
		return codeConstructor;
	}

	private CodeConstructor CreateTableCtor2(DataTable dt)
	{
		CodeConstructor codeConstructor = new CodeConstructor();
		codeConstructor.Attributes = MemberAttributes.Assembly;
		codeConstructor.Parameters.Add(Param(typeof(DataTable), GetRowTableFieldName(dt)));
		codeConstructor.BaseConstructorArgs.Add(PropRef(ParamRef(GetRowTableFieldName(dt)), "TableName"));
		codeConstructor.Statements.Add(Comment("TODO: implement"));
		codeConstructor.Statements.Add(Throw(New(typeof(NotImplementedException))));
		return codeConstructor;
	}

	private CodeMemberMethod CreateTableInitializeClass(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "InitializeClass";
		foreach (DataColumn column in dt.Columns)
		{
			codeMemberMethod.Statements.Add(Eval(MethodInvoke(PropRef("Columns"), "Add", New(typeof(DataColumn), Const(column.ColumnName), new CodeTypeOfExpression(column.DataType)))));
		}
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableInitializeFields(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "InitializeFields";
		codeMemberMethod.Attributes = MemberAttributes.Assembly;
		foreach (DataColumn column in dt.Columns)
		{
			string name = $"__column{opts.TableColName(column.ColumnName)}";
			codeMemberMethod.Statements.Add(Let(FieldRef(name), IndexerRef(PropRef("Columns"), Const(column.ColumnName))));
			if (!column.AllowDBNull)
			{
				codeMemberMethod.Statements.Add(Let(FieldRef(PropRef(name), "AllowDBNull"), Const(column.AllowDBNull)));
			}
			if (column.DefaultValue != null && column.DefaultValue.GetType() != typeof(DBNull))
			{
				codeMemberMethod.Statements.Add(Let(FieldRef(PropRef(name), "DefaultValue"), Const(column.DefaultValue)));
			}
			if (column.AutoIncrement)
			{
				codeMemberMethod.Statements.Add(Let(FieldRef(PropRef(name), "AutoIncrement"), Const(column.AutoIncrement)));
			}
			if (column.AutoIncrementSeed != 0L)
			{
				codeMemberMethod.Statements.Add(Let(FieldRef(PropRef(name), "AutoIncrementSeed"), Const(column.AutoIncrementSeed)));
			}
			if (column.AutoIncrementStep != 1)
			{
				codeMemberMethod.Statements.Add(Let(FieldRef(PropRef(name), "AutoIncrementStep"), Const(column.AutoIncrementStep)));
			}
			if (column.ReadOnly)
			{
				codeMemberMethod.Statements.Add(Let(FieldRef(PropRef(name), "ReadOnly"), Const(column.ReadOnly)));
			}
		}
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableClone(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "Clone";
		codeMemberMethod.Attributes = (MemberAttributes)24580;
		codeMemberMethod.ReturnType = TypeRef(typeof(DataTable));
		string t = opts.TableTypeName(dt.TableName);
		codeMemberMethod.Statements.Add(VarDecl(t, "t", Cast(t, MethodInvoke(Base(), "Clone"))));
		codeMemberMethod.Statements.Add(Eval(MethodInvoke(Local("t"), "InitializeFields")));
		codeMemberMethod.Statements.Add(Return(Local("t")));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableGetEnumerator(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "GetEnumerator";
		codeMemberMethod.Attributes = MemberAttributes.Public;
		codeMemberMethod.ReturnType = TypeRef(typeof(IEnumerator));
		codeMemberMethod.Statements.Add(Return(MethodInvoke(PropRef("Rows"), "GetEnumerator")));
		codeMemberMethod.ImplementationTypes.Add(TypeRef(typeof(IEnumerable)));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableCreateInstance(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "CreateInstance";
		codeMemberMethod.Attributes = (MemberAttributes)12292;
		codeMemberMethod.ReturnType = TypeRef(typeof(DataTable));
		codeMemberMethod.Statements.Add(Return(New(opts.TableTypeName(dt.TableName))));
		return codeMemberMethod;
	}

	private CodeMemberField CreateTableColumnField(DataTable dt, DataColumn col)
	{
		CodeMemberField codeMemberField = new CodeMemberField();
		codeMemberField.Name = "__column" + opts.ColumnName(col.ColumnName);
		codeMemberField.Type = TypeRef(typeof(DataColumn));
		return codeMemberField;
	}

	private CodeMemberProperty CreateTableColumnProperty(DataTable dt, DataColumn col)
	{
		string text = opts.ColumnName(col.ColumnName);
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = text + "Column";
		codeMemberProperty.Attributes = MemberAttributes.Assembly;
		codeMemberProperty.Type = TypeRef(typeof(DataColumn));
		codeMemberProperty.HasSet = false;
		codeMemberProperty.GetStatements.Add(Return(FieldRef("__column" + text)));
		return codeMemberProperty;
	}

	private CodeMemberProperty CreateTableCount(DataTable dt)
	{
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = "Count";
		codeMemberProperty.Attributes = MemberAttributes.Public;
		codeMemberProperty.Type = TypeRef(typeof(int));
		codeMemberProperty.HasSet = false;
		codeMemberProperty.GetStatements.Add(Return(PropRef(PropRef("Rows"), "Count")));
		return codeMemberProperty;
	}

	private CodeMemberProperty CreateTableIndexer(DataTable dt)
	{
		string text = opts.RowName(dt.TableName);
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = "Item";
		codeMemberProperty.Attributes = MemberAttributes.Public;
		codeMemberProperty.Type = TypeRef(text);
		codeMemberProperty.Parameters.Add(Param(typeof(int), "i"));
		codeMemberProperty.HasSet = false;
		codeMemberProperty.GetStatements.Add(Return(Cast(text, IndexerRef(PropRef("Rows"), ParamRef("i")))));
		return codeMemberProperty;
	}

	private CodeMemberMethod CreateTableAddRow1(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		string text = opts.RowName(dt.TableName);
		codeMemberMethod.Name = "Add" + text;
		codeMemberMethod.Attributes = MemberAttributes.Public;
		codeMemberMethod.Parameters.Add(Param(TypeRef(text), "row"));
		codeMemberMethod.Statements.Add(Eval(MethodInvoke(PropRef("Rows"), "Add", ParamRef("row"))));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableAddRow2(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		string text = opts.RowName(dt.TableName);
		codeMemberMethod.Name = "Add" + text;
		codeMemberMethod.ReturnType = TypeRef(text);
		codeMemberMethod.Attributes = MemberAttributes.Public;
		codeMemberMethod.Statements.Add(VarDecl(text, "row", MethodInvoke("New" + text)));
		foreach (DataColumn column in dt.Columns)
		{
			if (column.ColumnMapping == MappingType.Hidden)
			{
				foreach (DataRelation relation in dt.DataSet.Relations)
				{
					if (relation.ChildTable == dt)
					{
						string text2 = opts.RowName(relation.ParentTable.TableName);
						string name = text2;
						codeMemberMethod.Parameters.Add(Param(text2, name));
						codeMemberMethod.Statements.Add(Eval(MethodInvoke(Local("row"), "SetParentRow", ParamRef(name), IndexerRef(PropRef(PropRef("DataSet"), "Relations"), Const(relation.RelationName)))));
						break;
					}
				}
			}
			else
			{
				string text3 = opts.ColumnName(column.ColumnName);
				codeMemberMethod.Parameters.Add(Param(column.DataType, text3));
				codeMemberMethod.Statements.Add(Let(IndexerRef(Local("row"), Const(text3)), ParamRef(text3)));
			}
		}
		codeMemberMethod.Statements.Add(MethodInvoke(PropRef("Rows"), "Add", Local("row")));
		codeMemberMethod.Statements.Add(Return(Local("row")));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableNewRow(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		string text = opts.RowName(dt.TableName);
		codeMemberMethod.Name = "New" + text;
		codeMemberMethod.ReturnType = TypeRef(text);
		codeMemberMethod.Attributes = MemberAttributes.Public;
		codeMemberMethod.Statements.Add(Return(Cast(text, MethodInvoke("NewRow"))));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableNewRowFromBuilder(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "NewRowFromBuilder";
		codeMemberMethod.Attributes = (MemberAttributes)12292;
		codeMemberMethod.ReturnType = TypeRef(typeof(DataRow));
		codeMemberMethod.Parameters.Add(Param(typeof(DataRowBuilder), "builder"));
		codeMemberMethod.Statements.Add(Return(New(opts.RowName(dt.TableName), ParamRef("builder"))));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableRemoveRow(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		string text = opts.RowName(dt.TableName);
		codeMemberMethod.Name = "Remove" + text;
		codeMemberMethod.Attributes = MemberAttributes.Public;
		codeMemberMethod.Parameters.Add(Param(TypeRef(text), "row"));
		codeMemberMethod.Statements.Add(Eval(MethodInvoke(PropRef("Rows"), "Remove", ParamRef("row"))));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableGetRowType(DataTable dt)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "GetRowType";
		codeMemberMethod.Attributes = (MemberAttributes)12292;
		codeMemberMethod.ReturnType = TypeRef(typeof(Type));
		codeMemberMethod.Statements.Add(Return(new CodeTypeOfExpression(opts.RowName(dt.TableName))));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateTableEventStarter(DataTable dt, string type)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "OnRow" + type;
		codeMemberMethod.Attributes = (MemberAttributes)12292;
		codeMemberMethod.Parameters.Add(Param(typeof(DataRowChangeEventArgs), "e"));
		codeMemberMethod.Statements.Add(Eval(MethodInvoke(Base(), codeMemberMethod.Name, ParamRef("e"))));
		string text = opts.TableMemberName(dt.TableName) + "Row" + type;
		CodeStatement codeStatement = Eval(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(This(), text), This(), New(opts.EventArgsName(dt.TableName), Cast(opts.RowName(dt.TableName), PropRef(ParamRef("e"), "Row")), PropRef(ParamRef("e"), "Action"))));
		codeMemberMethod.Statements.Add(new CodeConditionStatement(Inequals(EventRef(text), Const(null)), new CodeStatement[1] { codeStatement }, new CodeStatement[0]));
		return codeMemberMethod;
	}

	private CodeMemberEvent CreateTableEvent(DataTable dt, string nameSuffix)
	{
		CodeMemberEvent codeMemberEvent = new CodeMemberEvent();
		codeMemberEvent.Attributes = MemberAttributes.Public;
		codeMemberEvent.Name = opts.TableMemberName(dt.TableName) + nameSuffix;
		codeMemberEvent.Type = TypeRef(opts.TableDelegateName(dt.TableName));
		return codeMemberEvent;
	}

	public CodeTypeDeclaration GenerateDataRowType(DataTable dt)
	{
		CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
		codeTypeDeclaration.Name = opts.RowName(dt.TableName);
		codeTypeDeclaration.BaseTypes.Add(TypeRef(typeof(DataRow)));
		codeTypeDeclaration.Members.Add(CreateRowCtor(dt));
		codeTypeDeclaration.Members.Add(CreateRowTableField(dt));
		foreach (DataColumn column in dt.Columns)
		{
			if (column.ColumnMapping != MappingType.Hidden)
			{
				codeTypeDeclaration.Members.Add(CreateRowColumnProperty(dt, column));
				codeTypeDeclaration.Members.Add(CreateRowColumnIsNull(dt, column));
				codeTypeDeclaration.Members.Add(CreateRowColumnSetNull(dt, column));
			}
		}
		foreach (DataRelation parentRelation in dt.ParentRelations)
		{
			codeTypeDeclaration.Members.Add(CreateRowParentRowProperty(dt, parentRelation));
		}
		foreach (DataRelation childRelation in dt.ChildRelations)
		{
			codeTypeDeclaration.Members.Add(CreateRowGetChildRows(dt, childRelation));
		}
		return codeTypeDeclaration;
	}

	private CodeConstructor CreateRowCtor(DataTable dt)
	{
		CodeConstructor codeConstructor = new CodeConstructor();
		codeConstructor.Attributes = MemberAttributes.Assembly;
		codeConstructor.Parameters.Add(Param(typeof(DataRowBuilder), "builder"));
		codeConstructor.BaseConstructorArgs.Add(ParamRef("builder"));
		codeConstructor.Statements.Add(Let(FieldRef(GetRowTableFieldName(dt)), Cast(opts.TableTypeName(dt.TableName), PropRef("Table"))));
		return codeConstructor;
	}

	private string GetRowTableFieldName(DataTable dt)
	{
		return "table" + dt.TableName;
	}

	private CodeMemberField CreateRowTableField(DataTable dt)
	{
		CodeMemberField codeMemberField = new CodeMemberField();
		codeMemberField.Name = GetRowTableFieldName(dt);
		codeMemberField.Type = TypeRef(opts.TableTypeName(dt.TableName));
		return codeMemberField;
	}

	private CodeMemberProperty CreateRowColumnProperty(DataTable dt, DataColumn col)
	{
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = opts.ColumnName(col.ColumnName);
		codeMemberProperty.Type = TypeRef(col.DataType);
		codeMemberProperty.Attributes = MemberAttributes.Public;
		codeMemberProperty.GetStatements.Add(VarDecl(typeof(object), "ret", IndexerRef(PropRef(PropRef(GetRowTableFieldName(dt)), opts.TableColName(col.ColumnName) + "Column"))));
		codeMemberProperty.GetStatements.Add(new CodeConditionStatement(Equals(Local("ret"), PropRef(TypeRefExp(typeof(DBNull)), "Value")), new CodeStatement[1] { Throw(New(typeof(StrongTypingException), Const("Cannot get strong typed value since it is DB null."), Const(null))) }, new CodeStatement[1] { Return(Cast(col.DataType, Local("ret"))) }));
		codeMemberProperty.SetStatements.Add(Let(IndexerRef(PropRef(PropRef(GetRowTableFieldName(dt)), opts.TableColName(col.ColumnName) + "Column")), new CodePropertySetValueReferenceExpression()));
		return codeMemberProperty;
	}

	private CodeMemberMethod CreateRowColumnIsNull(DataTable dt, DataColumn col)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "Is" + opts.ColumnName(col.ColumnName) + "Null";
		codeMemberMethod.Attributes = MemberAttributes.Public;
		codeMemberMethod.ReturnType = TypeRef(typeof(bool));
		codeMemberMethod.Statements.Add(Return(MethodInvoke("IsNull", PropRef(PropRef(GetRowTableFieldName(dt)), opts.TableColName(col.ColumnName) + "Column"))));
		return codeMemberMethod;
	}

	private CodeMemberMethod CreateRowColumnSetNull(DataTable dt, DataColumn col)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "Set" + opts.ColumnName(col.ColumnName) + "Null";
		codeMemberMethod.Attributes = MemberAttributes.Public;
		codeMemberMethod.Statements.Add(Let(IndexerRef(PropRef(PropRef(GetRowTableFieldName(dt)), opts.TableColName(col.ColumnName) + "Column")), PropRef(TypeRefExp(typeof(DBNull)), "Value")));
		return codeMemberMethod;
	}

	private CodeMemberProperty CreateRowParentRowProperty(DataTable dt, DataRelation rel)
	{
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = opts.TableMemberName(rel.ParentTable.TableName) + "Row" + ((!(rel.ParentTable.TableName == rel.ChildTable.TableName)) ? string.Empty : "Parent");
		codeMemberProperty.Attributes = MemberAttributes.Public;
		codeMemberProperty.Type = TypeRef(opts.RowName(rel.ParentTable.TableName));
		codeMemberProperty.GetStatements.Add(Return(Cast(codeMemberProperty.Type, MethodInvoke("GetParentRow", IndexerRef(PropRef(PropRef(PropRef("Table"), "DataSet"), "Relations"), Const(rel.RelationName))))));
		codeMemberProperty.SetStatements.Add(Eval(MethodInvoke("SetParentRow", new CodePropertySetValueReferenceExpression(), IndexerRef(PropRef(PropRef(PropRef("Table"), "DataSet"), "Relations"), Const(rel.RelationName)))));
		return codeMemberProperty;
	}

	private CodeMemberMethod CreateRowGetChildRows(DataTable dt, DataRelation rel)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "Get" + opts.TableMemberName(rel.ChildTable.TableName) + "Rows";
		codeMemberMethod.Attributes = MemberAttributes.Public;
		codeMemberMethod.ReturnType = new CodeTypeReference(opts.RowName(rel.ChildTable.TableName), 1);
		codeMemberMethod.Statements.Add(Return(Cast(codeMemberMethod.ReturnType, MethodInvoke("GetChildRows", IndexerRef(PropRef(PropRef(PropRef("Table"), "DataSet"), "Relations"), Const(rel.RelationName))))));
		return codeMemberMethod;
	}

	private CodeTypeDeclaration GenerateEventType(DataTable dt)
	{
		CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
		codeTypeDeclaration.Name = opts.EventArgsName(dt.TableName);
		codeTypeDeclaration.BaseTypes.Add(TypeRef(typeof(EventArgs)));
		codeTypeDeclaration.Attributes = MemberAttributes.Public;
		codeTypeDeclaration.Members.Add(new CodeMemberField(TypeRef(opts.RowName(dt.TableName)), "eventRow"));
		codeTypeDeclaration.Members.Add(new CodeMemberField(TypeRef(typeof(DataRowAction)), "eventAction"));
		codeTypeDeclaration.Members.Add(CreateEventCtor(dt));
		codeTypeDeclaration.Members.Add(CreateEventRow(dt));
		codeTypeDeclaration.Members.Add(CreateEventAction(dt));
		return codeTypeDeclaration;
	}

	private CodeConstructor CreateEventCtor(DataTable dt)
	{
		CodeConstructor codeConstructor = new CodeConstructor();
		codeConstructor.Attributes = MemberAttributes.Public;
		codeConstructor.Parameters.Add(Param(TypeRef(opts.RowName(dt.TableName)), "r"));
		codeConstructor.Parameters.Add(Param(TypeRef(typeof(DataRowAction)), "a"));
		codeConstructor.Statements.Add(Let(FieldRef("eventRow"), ParamRef("r")));
		codeConstructor.Statements.Add(Let(FieldRef("eventAction"), ParamRef("a")));
		return codeConstructor;
	}

	private CodeMemberProperty CreateEventRow(DataTable dt)
	{
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = "Row";
		codeMemberProperty.Attributes = (MemberAttributes)24578;
		codeMemberProperty.Type = TypeRef(opts.RowName(dt.TableName));
		codeMemberProperty.HasSet = false;
		codeMemberProperty.GetStatements.Add(Return(FieldRef("eventRow")));
		return codeMemberProperty;
	}

	private CodeMemberProperty CreateEventAction(DataTable dt)
	{
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = "Action";
		codeMemberProperty.Attributes = (MemberAttributes)24578;
		codeMemberProperty.Type = TypeRef(typeof(DataRowAction));
		codeMemberProperty.HasSet = false;
		codeMemberProperty.GetStatements.Add(Return(FieldRef("eventAction")));
		return codeMemberProperty;
	}

	private CodeTypeDeclaration GenerateTableAdapterType(TableAdapterSchemaInfo taInfo)
	{
		Type type = null;
		int num = -1;
		CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
		codeTypeDeclaration.Name = opts.TableAdapterName(taInfo.Name);
		codeTypeDeclaration.BaseTypes.Add(TypeRef(taInfo.BaseClass));
		codeTypeDeclaration.Members.Add(CreateTableAdapterDefaultCtor());
		CreateDBAdapterFieldAndProperty(codeTypeDeclaration, taInfo.Adapter);
		CreateDBConnectionFieldAndProperty(codeTypeDeclaration, taInfo.Connection);
		DbCommand dbCommand = null;
		dbCommand = ((taInfo.Commands.Count <= 0) ? taInfo.Provider.CreateCommand() : ((DbCommandInfo)taInfo.Commands[0]).Command);
		CreateDBCommandCollectionFieldAndProperty(codeTypeDeclaration, dbCommand);
		CreateAdapterClearBeforeFillFieldAndProperty(codeTypeDeclaration);
		CreateAdapterInitializeMethod(codeTypeDeclaration, taInfo);
		CreateConnectionInitializeMethod(codeTypeDeclaration, taInfo);
		CreateCommandCollectionInitializeMethod(codeTypeDeclaration, taInfo);
		CreateDbSourceMethods(codeTypeDeclaration, taInfo);
		if (taInfo.ShortCommands)
		{
			CreateShortCommandMethods(codeTypeDeclaration, taInfo);
		}
		return codeTypeDeclaration;
	}

	private CodeConstructor CreateTableAdapterDefaultCtor()
	{
		CodeConstructor codeConstructor = new CodeConstructor();
		codeConstructor.Attributes = MemberAttributes.Public;
		codeConstructor.Statements.Add(Let(PropRef("ClearBeforeFill"), Const(true)));
		return codeConstructor;
	}

	private void CreateDBAdapterFieldAndProperty(CodeTypeDeclaration t, DbDataAdapter adapter)
	{
		CodeMemberField codeMemberField = new CodeMemberField();
		codeMemberField.Name = "_adapter";
		codeMemberField.Type = TypeRef(adapter.GetType());
		t.Members.Add(codeMemberField);
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = "Adapter";
		codeMemberProperty.Attributes = MemberAttributes.Private;
		codeMemberProperty.Type = codeMemberField.Type;
		codeMemberProperty.HasSet = false;
		CodeExpression codeExpression = FieldRef("_adapter");
		CodeStatement codeStatement = Eval(MethodInvoke("InitAdapter"));
		CodeStatement value = new CodeConditionStatement(Equals(codeExpression, Const(null)), new CodeStatement[1] { codeStatement }, new CodeStatement[0]);
		codeMemberProperty.GetStatements.Add(value);
		codeMemberProperty.GetStatements.Add(Return(codeExpression));
		t.Members.Add(codeMemberProperty);
	}

	private void CreateDBConnectionFieldAndProperty(CodeTypeDeclaration t, DbConnection conn)
	{
		CodeMemberField codeMemberField = new CodeMemberField();
		codeMemberField.Name = "_connection";
		codeMemberField.Type = TypeRef(conn.GetType());
		t.Members.Add(codeMemberField);
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = "Connection";
		codeMemberProperty.Attributes = MemberAttributes.Assembly;
		codeMemberProperty.Type = codeMemberField.Type;
		CodeExpression codeExpression = FieldRef("_connection");
		CodeStatement codeStatement = Eval(MethodInvoke("InitConnection"));
		CodeStatement value = new CodeConditionStatement(Equals(codeExpression, Const(null)), new CodeStatement[1] { codeStatement }, new CodeStatement[0]);
		codeMemberProperty.GetStatements.Add(value);
		codeMemberProperty.GetStatements.Add(Return(codeExpression));
		codeMemberProperty.SetStatements.Add(Let(codeExpression, new CodePropertySetValueReferenceExpression()));
		string name = "InsertCommand";
		string name2 = "Connection";
		codeStatement = null;
		value = null;
		codeExpression = null;
		codeExpression = PropRef(PropRef("Adapter"), name);
		codeStatement = Let(PropRef(codeExpression, name2), new CodePropertySetValueReferenceExpression());
		value = new CodeConditionStatement(Inequals(codeExpression, Const(null)), new CodeStatement[1] { codeStatement }, new CodeStatement[0]);
		codeMemberProperty.SetStatements.Add(value);
		codeStatement = null;
		value = null;
		codeExpression = null;
		name = "DeleteCommand";
		codeExpression = PropRef(PropRef("Adapter"), name);
		codeStatement = Let(PropRef(codeExpression, name2), new CodePropertySetValueReferenceExpression());
		value = new CodeConditionStatement(Inequals(codeExpression, Const(null)), new CodeStatement[1] { codeStatement }, new CodeStatement[0]);
		codeMemberProperty.SetStatements.Add(value);
		codeStatement = null;
		value = null;
		name = "UpdateCommand";
		codeExpression = PropRef(PropRef("Adapter"), name);
		codeStatement = Let(PropRef(codeExpression, name2), new CodePropertySetValueReferenceExpression());
		value = new CodeConditionStatement(Inequals(codeExpression, Const(null)), new CodeStatement[1] { codeStatement }, new CodeStatement[0]);
		codeMemberProperty.SetStatements.Add(value);
		codeStatement = null;
		codeExpression = null;
		value = null;
		codeStatement = VarDecl(typeof(int), "i", Const(0));
		codeExpression = LessThan(Local("i"), PropRef(PropRef("CommandCollection"), "Length"));
		value = Let(Local("i"), Compute(Local("i"), Const(1), CodeBinaryOperatorType.Add));
		CodeExpression codeExpression2 = IndexerRef(PropRef("CommandCollection"), Local("i"));
		CodeStatement codeStatement2 = Let(PropRef(codeExpression2, "Connection"), new CodePropertySetValueReferenceExpression());
		CodeStatement codeStatement3 = new CodeConditionStatement(Inequals(codeExpression2, Const(null)), new CodeStatement[1] { codeStatement2 }, new CodeStatement[0]);
		CodeIterationStatement value2 = new CodeIterationStatement(codeStatement, codeExpression, value, codeStatement3);
		codeMemberProperty.SetStatements.Add(value2);
		t.Members.Add(codeMemberProperty);
	}

	private void CreateDBCommandCollectionFieldAndProperty(CodeTypeDeclaration t, DbCommand cmd)
	{
		CodeMemberField codeMemberField = new CodeMemberField();
		codeMemberField.Name = "_commandCollection";
		codeMemberField.Type = TypeRefArray(cmd.GetType(), 1);
		t.Members.Add(codeMemberField);
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = "CommandCollection";
		codeMemberProperty.Attributes = MemberAttributes.Family;
		codeMemberProperty.Type = codeMemberField.Type;
		codeMemberProperty.HasSet = false;
		CodeExpression codeExpression = FieldRef("_commandCollection");
		CodeStatement codeStatement = Eval(MethodInvoke("InitCommandCollection"));
		CodeStatement value = new CodeConditionStatement(Equals(codeExpression, Const(null)), new CodeStatement[1] { codeStatement }, new CodeStatement[0]);
		codeMemberProperty.GetStatements.Add(value);
		codeMemberProperty.GetStatements.Add(Return(codeExpression));
		t.Members.Add(codeMemberProperty);
	}

	private void CreateAdapterClearBeforeFillFieldAndProperty(CodeTypeDeclaration t)
	{
		CodeMemberField codeMemberField = new CodeMemberField();
		codeMemberField.Name = "_clearBeforeFill";
		codeMemberField.Type = TypeRef(typeof(bool));
		t.Members.Add(codeMemberField);
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = "ClearBeforeFill";
		codeMemberProperty.Attributes = MemberAttributes.Public;
		codeMemberProperty.Type = codeMemberField.Type;
		codeMemberProperty.SetStatements.Add(Let(FieldRef("_clearBeforeFill"), new CodePropertySetValueReferenceExpression()));
		codeMemberProperty.GetStatements.Add(Return(FieldRef("_clearBeforeFill")));
		t.Members.Add(codeMemberProperty);
	}

	private void CreateAdapterInitializeMethod(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "InitAdapter";
		codeMemberMethod.Attributes = MemberAttributes.Private;
		CodeExpression exp = FieldRef("_adapter");
		CodeStatement value = Let(exp, New(taInfo.Adapter.GetType()));
		codeMemberMethod.Statements.Add(value);
		value = VarDecl(typeof(DataTableMapping), "tableMapping", null);
		codeMemberMethod.Statements.Add(value);
		foreach (DataTableMapping tableMapping in taInfo.Adapter.TableMappings)
		{
			exp = Local("tableMapping");
			value = Let(exp, New(tableMapping.GetType()));
			codeMemberMethod.Statements.Add(value);
			value = Let(PropRef(exp, "SourceTable"), Const(tableMapping.SourceTable));
			codeMemberMethod.Statements.Add(value);
			value = Let(PropRef(exp, "DataSetTable"), Const(tableMapping.DataSetTable));
			codeMemberMethod.Statements.Add(value);
			foreach (DataColumnMapping columnMapping in tableMapping.ColumnMappings)
			{
				value = Eval(MethodInvoke(PropRef(exp, "ColumnMappings"), "Add", Const(columnMapping.SourceColumn), Const(columnMapping.DataSetColumn)));
				codeMemberMethod.Statements.Add(value);
			}
			exp = PropRef(FieldRef("_adapter"), "TableMappings");
			value = Eval(MethodInvoke(exp, "Add", Local("tableMapping")));
			codeMemberMethod.Statements.Add(value);
		}
		exp = PropRef(FieldRef("_adapter"), "DeleteCommand");
		DbCommand deleteCommand = taInfo.Adapter.DeleteCommand;
		AddDbCommandStatements(codeMemberMethod, exp, deleteCommand);
		exp = PropRef(FieldRef("_adapter"), "InsertCommand");
		deleteCommand = taInfo.Adapter.InsertCommand;
		AddDbCommandStatements(codeMemberMethod, exp, deleteCommand);
		exp = PropRef(FieldRef("_adapter"), "UpdateCommand");
		deleteCommand = taInfo.Adapter.UpdateCommand;
		AddDbCommandStatements(codeMemberMethod, exp, deleteCommand);
		t.Members.Add(codeMemberMethod);
	}

	private void AddDbCommandStatements(CodeMemberMethod m, CodeExpression expr, DbCommand cmd)
	{
		if (cmd == null)
		{
			return;
		}
		CodeStatement value = Let(expr, New(cmd.GetType()));
		m.Statements.Add(value);
		value = Let(PropRef(expr, "Connection"), PropRef("Connection"));
		m.Statements.Add(value);
		value = Let(PropRef(expr, "CommandText"), Const(cmd.CommandText));
		m.Statements.Add(value);
		CodeExpression value2 = PropRef(Local(typeof(CommandType).FullName), cmd.CommandType.ToString());
		value = Let(PropRef(expr, "CommandType"), value2);
		m.Statements.Add(value);
		value2 = PropRef(expr, "Parameters");
		foreach (DbParameter parameter in cmd.Parameters)
		{
			AddDbParameterStatements(m, value2, parameter);
		}
	}

	private void AddDbParameterStatements(CodeMemberMethod m, CodeExpression expr, DbParameter param)
	{
		object frameworkDbType = param.FrameworkDbType;
		string value = null;
		if (param.SourceColumn != string.Empty)
		{
			value = param.SourceColumn;
		}
		CodeExpression[] parameters = new CodeExpression[10]
		{
			Const(param.ParameterName),
			PropRef(Local(frameworkDbType.GetType().FullName), frameworkDbType.ToString()),
			Const(param.Size),
			PropRef(Local(typeof(ParameterDirection).FullName), param.Direction.ToString()),
			Const(param.IsNullable),
			Const(((IDbDataParameter)param).Precision),
			Const(((IDbDataParameter)param).Scale),
			Const(value),
			PropRef(Local(typeof(DataRowVersion).FullName), param.SourceVersion.ToString()),
			Const(param.Value)
		};
		m.Statements.Add(Eval(MethodInvoke(expr, "Add", New(param.GetType(), parameters))));
	}

	private void CreateConnectionInitializeMethod(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "InitConnection";
		codeMemberMethod.Attributes = MemberAttributes.Private;
		CodeExpression exp = FieldRef("_connection");
		CodeStatement value = Let(exp, New(taInfo.Connection.GetType()));
		codeMemberMethod.Statements.Add(value);
		exp = PropRef(FieldRef("_connection"), "ConnectionString");
		CodeExpression target = IndexerRef(PropRef(Local(typeof(ConfigurationManager).ToString()), "ConnectionStrings"), Const(taInfo.ConnectionString));
		value = Let(exp, PropRef(target, "ConnectionString"));
		codeMemberMethod.Statements.Add(value);
		t.Members.Add(codeMemberMethod);
	}

	private void CreateCommandCollectionInitializeMethod(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
	{
		CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
		codeMemberMethod.Name = "InitCommandCollection";
		codeMemberMethod.Attributes = MemberAttributes.Private;
		Type type = ((DbCommandInfo)taInfo.Commands[0]).Command.GetType();
		CodeExpression codeExpression = FieldRef("_commandCollection");
		CodeStatement value = Let(codeExpression, NewArray(type, taInfo.Commands.Count));
		codeMemberMethod.Statements.Add(value);
		for (int i = 0; i < taInfo.Commands.Count; i++)
		{
			DbCommand command = ((DbCommandInfo)taInfo.Commands[i]).Command;
			CodeExpression codeExpression2 = IndexerRef(codeExpression, Const(i));
			value = Let(codeExpression2, New(type));
			codeMemberMethod.Statements.Add(value);
			value = Let(PropRef(codeExpression2, "Connection"), PropRef("Connection"));
			codeMemberMethod.Statements.Add(value);
			value = Let(PropRef(codeExpression2, "CommandText"), Const(command.CommandText));
			codeMemberMethod.Statements.Add(value);
			value = Let(PropRef(codeExpression2, "CommandType"), PropRef(Local(typeof(CommandType).FullName), command.CommandType.ToString()));
			codeMemberMethod.Statements.Add(value);
			codeExpression2 = PropRef(codeExpression2, "Parameters");
			foreach (DbParameter parameter in command.Parameters)
			{
				AddDbParameterStatements(codeMemberMethod, codeExpression2, parameter);
			}
		}
		t.Members.Add(codeMemberMethod);
	}

	private void CreateDbSourceMethods(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
	{
		string text = null;
		CodeMemberMethod codeMemberMethod = null;
		string text2 = null;
		CodeExpression codeExpression = PropRef(PropRef("Adapter"), "SelectCommand");
		for (int i = 0; i < taInfo.Commands.Count; i++)
		{
			DbCommandInfo dbCommandInfo = (DbCommandInfo)taInfo.Commands[i];
			DbSourceMethodInfo[] methods = dbCommandInfo.Methods;
			foreach (DbSourceMethodInfo dbSourceMethodInfo in methods)
			{
				if (dbSourceMethodInfo.MethodType == GenerateMethodsType.Fill)
				{
					continue;
				}
				codeMemberMethod = new CodeMemberMethod();
				codeMemberMethod.Name = dbSourceMethodInfo.Name;
				CodeStatement value = Let(codeExpression, IndexerRef(PropRef("CommandCollection"), Const(i)));
				codeMemberMethod.Statements.Add(value);
				switch ((MemberAttributes)(int)Enum.Parse(typeof(MemberAttributes), dbSourceMethodInfo.Modifier))
				{
				case MemberAttributes.Public:
					codeMemberMethod.Attributes = MemberAttributes.Public;
					break;
				case MemberAttributes.Private:
					codeMemberMethod.Attributes = MemberAttributes.Private;
					break;
				case MemberAttributes.Assembly:
					codeMemberMethod.Attributes = MemberAttributes.Assembly;
					break;
				case MemberAttributes.Family:
					codeMemberMethod.Attributes = MemberAttributes.Family;
					break;
				}
				QueryType queryType = (QueryType)(int)Enum.Parse(typeof(QueryType), dbSourceMethodInfo.QueryType);
				switch (queryType)
				{
				case QueryType.NoData:
				case QueryType.Scalar:
				{
					codeMemberMethod.ReturnType = TypeRef(typeof(int));
					AddGeneratedMethodParametersAndStatements(codeMemberMethod, codeExpression, dbCommandInfo.Command);
					text = typeof(ConnectionState).FullName;
					CodeExpression target = PropRef(Local("command"), "Connection");
					CodeExpression target2 = PropRef(PropRef(Local("System"), "Data"), "ConnectionState");
					value = VarDecl(text, "previousConnectionState", PropRef(target, "State"));
					codeMemberMethod.Statements.Add(value);
					CodeExpression exp = BitOps(PropRef(target, "State"), PropRef(target2, "Open"), CodeBinaryOperatorType.BitwiseAnd);
					value = new CodeConditionStatement(Inequals(exp, PropRef(target2, "Open")), new CodeStatement[1] { Eval(MethodInvoke(target, "Open")) }, new CodeStatement[0]);
					codeMemberMethod.Statements.Add(value);
					CodeTryCatchFinallyStatement codeTryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
					if (queryType == QueryType.NoData)
					{
						codeMemberMethod.Statements.Add(VarDecl(typeof(int), "returnValue", null));
						exp = MethodInvoke(Local("command"), "ExecuteNonQuery");
					}
					else
					{
						text2 = dbSourceMethodInfo.ScalarCallRetval.Substring(0, dbSourceMethodInfo.ScalarCallRetval.IndexOf(','));
						codeMemberMethod.Statements.Add(VarDecl(TypeRef(text2).BaseType, "returnValue", null));
						exp = MethodInvoke(Local("command"), "ExecuteScalar");
					}
					codeTryCatchFinallyStatement.TryStatements.Add(Let(Local("returnValue"), exp));
					value = new CodeConditionStatement(Equals(Local("previousConnectionState"), PropRef(target2, "Closed")), new CodeStatement[1] { Eval(MethodInvoke(target, "Close")) }, new CodeStatement[0]);
					codeTryCatchFinallyStatement.FinallyStatements.Add(value);
					codeMemberMethod.Statements.Add(codeTryCatchFinallyStatement);
					if (queryType == QueryType.NoData)
					{
						codeMemberMethod.Statements.Add(Return(Local("returnValue")));
						break;
					}
					target2 = Equals(Local("returnValue"), Const(null));
					exp = Equals(MethodInvoke(Local("returnValue"), "GetType"), TypeOfRef("System.DBNull"));
					value = new CodeConditionStatement(BooleanOps(target2, exp, CodeBinaryOperatorType.BooleanOr), new CodeStatement[1] { Return(Const(null)) }, new CodeStatement[1] { Return(Cast(text2, Local("returnValue"))) });
					codeMemberMethod.Statements.Add(value);
					break;
				}
				case QueryType.Rowset:
					text = opts.DataSetName(ds.DataSetName) + "." + opts.TableTypeName(ds.Tables[0].TableName);
					codeMemberMethod.ReturnType = TypeRef(text);
					AddGeneratedMethodParametersAndStatements(codeMemberMethod, codeExpression, dbCommandInfo.Command);
					value = VarDecl(text, "dataTable", New(text));
					codeMemberMethod.Statements.Add(value);
					codeExpression = PropRef("Adapter");
					value = Eval(MethodInvoke(codeExpression, "Fill", Local("dataTable")));
					codeMemberMethod.Statements.Add(value);
					codeMemberMethod.Statements.Add(Return(Local("dataTable")));
					break;
				}
				t.Members.Add(codeMemberMethod);
			}
		}
	}

	private void AddGeneratedMethodParametersAndStatements(CodeMemberMethod m, CodeExpression expr, DbCommand cmd)
	{
		int num = 0;
		DbType dbType = DbType.DateTime;
		foreach (DbParameter parameter in cmd.Parameters)
		{
			if (parameter.Direction != ParameterDirection.ReturnValue)
			{
				string name = ((parameter.ParameterName[0] != '@') ? parameter.ParameterName : parameter.ParameterName.Substring(1));
				if (parameter.SystemType != null)
				{
					m.Parameters.Add(Param(TypeRef(parameter.SystemType), name));
				}
				CodeExpression exp = IndexerRef(PropRef(expr, "Parameters"), Const(num));
				CodeStatement value = Let(exp, ParamRef(name));
				m.Statements.Add(value);
			}
			num++;
		}
	}

	private void CreateShortCommandMethods(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
	{
	}
}

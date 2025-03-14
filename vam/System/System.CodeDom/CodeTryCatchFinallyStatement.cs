using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeTryCatchFinallyStatement : CodeStatement
{
	private CodeStatementCollection tryStatements;

	private CodeStatementCollection finallyStatements;

	private CodeCatchClauseCollection catchClauses;

	public CodeStatementCollection FinallyStatements
	{
		get
		{
			if (finallyStatements == null)
			{
				finallyStatements = new CodeStatementCollection();
			}
			return finallyStatements;
		}
	}

	public CodeStatementCollection TryStatements
	{
		get
		{
			if (tryStatements == null)
			{
				tryStatements = new CodeStatementCollection();
			}
			return tryStatements;
		}
	}

	public CodeCatchClauseCollection CatchClauses
	{
		get
		{
			if (catchClauses == null)
			{
				catchClauses = new CodeCatchClauseCollection();
			}
			return catchClauses;
		}
	}

	public CodeTryCatchFinallyStatement()
	{
	}

	public CodeTryCatchFinallyStatement(CodeStatement[] tryStatements, CodeCatchClause[] catchClauses)
	{
		TryStatements.AddRange(tryStatements);
		CatchClauses.AddRange(catchClauses);
	}

	public CodeTryCatchFinallyStatement(CodeStatement[] tryStatements, CodeCatchClause[] catchClauses, CodeStatement[] finallyStatements)
	{
		TryStatements.AddRange(tryStatements);
		CatchClauses.AddRange(catchClauses);
		FinallyStatements.AddRange(finallyStatements);
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

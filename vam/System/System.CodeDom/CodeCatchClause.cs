using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeCatchClause
{
	private CodeTypeReference catchExceptionType;

	private string localName;

	private CodeStatementCollection statements;

	public CodeTypeReference CatchExceptionType
	{
		get
		{
			if (catchExceptionType == null)
			{
				catchExceptionType = new CodeTypeReference(typeof(Exception));
			}
			return catchExceptionType;
		}
		set
		{
			catchExceptionType = value;
		}
	}

	public string LocalName
	{
		get
		{
			if (localName == null)
			{
				return string.Empty;
			}
			return localName;
		}
		set
		{
			localName = value;
		}
	}

	public CodeStatementCollection Statements
	{
		get
		{
			if (statements == null)
			{
				statements = new CodeStatementCollection();
			}
			return statements;
		}
	}

	public CodeCatchClause()
	{
	}

	public CodeCatchClause(string localName)
	{
		this.localName = localName;
	}

	public CodeCatchClause(string localName, CodeTypeReference catchExceptionType)
	{
		this.localName = localName;
		this.catchExceptionType = catchExceptionType;
	}

	public CodeCatchClause(string localName, CodeTypeReference catchExceptionType, params CodeStatement[] statements)
	{
		this.localName = localName;
		this.catchExceptionType = catchExceptionType;
		Statements.AddRange(statements);
	}
}

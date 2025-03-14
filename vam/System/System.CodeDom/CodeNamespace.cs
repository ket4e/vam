using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeNamespace : CodeObject
{
	private CodeCommentStatementCollection comments;

	private CodeNamespaceImportCollection imports;

	private CodeNamespaceCollection namespaces;

	private CodeTypeDeclarationCollection classes;

	private string name;

	private int populated;

	public CodeCommentStatementCollection Comments
	{
		get
		{
			if (comments == null)
			{
				comments = new CodeCommentStatementCollection();
				if (this.PopulateComments != null)
				{
					this.PopulateComments(this, EventArgs.Empty);
				}
			}
			return comments;
		}
	}

	public CodeNamespaceImportCollection Imports
	{
		get
		{
			if (imports == null)
			{
				imports = new CodeNamespaceImportCollection();
				if (this.PopulateImports != null)
				{
					this.PopulateImports(this, EventArgs.Empty);
				}
			}
			return imports;
		}
	}

	public string Name
	{
		get
		{
			if (name == null)
			{
				return string.Empty;
			}
			return name;
		}
		set
		{
			name = value;
		}
	}

	public CodeTypeDeclarationCollection Types
	{
		get
		{
			if (classes == null)
			{
				classes = new CodeTypeDeclarationCollection();
				if (this.PopulateTypes != null)
				{
					this.PopulateTypes(this, EventArgs.Empty);
				}
			}
			return classes;
		}
	}

	public event EventHandler PopulateComments;

	public event EventHandler PopulateImports;

	public event EventHandler PopulateTypes;

	public CodeNamespace()
	{
	}

	public CodeNamespace(string name)
	{
		this.name = name;
	}
}

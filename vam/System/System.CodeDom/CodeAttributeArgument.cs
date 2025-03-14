using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeAttributeArgument
{
	private string name;

	private CodeExpression value;

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

	public CodeExpression Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public CodeAttributeArgument()
	{
	}

	public CodeAttributeArgument(CodeExpression value)
	{
		this.value = value;
	}

	public CodeAttributeArgument(string name, CodeExpression value)
	{
		this.name = name;
		this.value = value;
	}
}

using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeArrayCreateExpression : CodeExpression
{
	private CodeTypeReference createType;

	private CodeExpressionCollection initializers;

	private CodeExpression sizeExpression;

	private int size;

	public CodeTypeReference CreateType
	{
		get
		{
			if (createType == null)
			{
				createType = new CodeTypeReference(typeof(void));
			}
			return createType;
		}
		set
		{
			createType = value;
		}
	}

	public CodeExpressionCollection Initializers
	{
		get
		{
			if (initializers == null)
			{
				initializers = new CodeExpressionCollection();
			}
			return initializers;
		}
	}

	public CodeExpression SizeExpression
	{
		get
		{
			return sizeExpression;
		}
		set
		{
			sizeExpression = value;
		}
	}

	public int Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
		}
	}

	public CodeArrayCreateExpression()
	{
	}

	public CodeArrayCreateExpression(CodeTypeReference createType, CodeExpression size)
	{
		this.createType = createType;
		sizeExpression = size;
	}

	public CodeArrayCreateExpression(CodeTypeReference createType, params CodeExpression[] initializers)
	{
		this.createType = createType;
		Initializers.AddRange(initializers);
	}

	public CodeArrayCreateExpression(CodeTypeReference createType, int size)
	{
		this.createType = createType;
		this.size = size;
	}

	public CodeArrayCreateExpression(string createType, CodeExpression size)
	{
		this.createType = new CodeTypeReference(createType);
		sizeExpression = size;
	}

	public CodeArrayCreateExpression(string createType, params CodeExpression[] initializers)
	{
		this.createType = new CodeTypeReference(createType);
		Initializers.AddRange(initializers);
	}

	public CodeArrayCreateExpression(string createType, int size)
	{
		this.createType = new CodeTypeReference(createType);
		this.size = size;
	}

	public CodeArrayCreateExpression(Type createType, CodeExpression size)
	{
		this.createType = new CodeTypeReference(createType);
		sizeExpression = size;
	}

	public CodeArrayCreateExpression(Type createType, params CodeExpression[] initializers)
	{
		this.createType = new CodeTypeReference(createType);
		Initializers.AddRange(initializers);
	}

	public CodeArrayCreateExpression(Type createType, int size)
	{
		this.createType = new CodeTypeReference(createType);
		this.size = size;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

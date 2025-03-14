using System.Runtime.InteropServices;
using System.Text;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeTypeReference : CodeObject
{
	private string baseType;

	private CodeTypeReference arrayElementType;

	private int arrayRank;

	private bool isInterface;

	private bool needsFixup;

	private CodeTypeReferenceCollection typeArguments;

	private CodeTypeReferenceOptions referenceOptions;

	public CodeTypeReference ArrayElementType
	{
		get
		{
			return arrayElementType;
		}
		set
		{
			arrayElementType = value;
		}
	}

	public int ArrayRank
	{
		get
		{
			return arrayRank;
		}
		set
		{
			arrayRank = value;
		}
	}

	public string BaseType
	{
		get
		{
			if (arrayElementType != null && arrayRank > 0)
			{
				return arrayElementType.BaseType;
			}
			if (baseType == null)
			{
				return string.Empty;
			}
			return baseType;
		}
		set
		{
			baseType = value;
		}
	}

	internal bool IsInterface => isInterface;

	[ComVisible(false)]
	public CodeTypeReferenceOptions Options
	{
		get
		{
			return referenceOptions;
		}
		set
		{
			referenceOptions = value;
		}
	}

	[ComVisible(false)]
	public CodeTypeReferenceCollection TypeArguments
	{
		get
		{
			if (typeArguments == null)
			{
				typeArguments = new CodeTypeReferenceCollection();
			}
			return typeArguments;
		}
	}

	public CodeTypeReference()
	{
	}

	[System.MonoTODO("We should parse basetype from right to left in 2.0 profile.")]
	public CodeTypeReference(string baseType)
	{
		Parse(baseType);
	}

	[System.MonoTODO("We should parse basetype from right to left in 2.0 profile.")]
	public CodeTypeReference(Type baseType)
	{
		if (baseType == null)
		{
			throw new ArgumentNullException("baseType");
		}
		if (baseType.IsGenericParameter)
		{
			this.baseType = baseType.Name;
			referenceOptions = CodeTypeReferenceOptions.GenericTypeParameter;
		}
		else if (baseType.IsGenericTypeDefinition)
		{
			this.baseType = baseType.FullName;
		}
		else if (baseType.IsGenericType)
		{
			this.baseType = baseType.GetGenericTypeDefinition().FullName;
			Type[] genericArguments = baseType.GetGenericArguments();
			foreach (Type type in genericArguments)
			{
				if (type.IsGenericParameter)
				{
					TypeArguments.Add(new CodeTypeReference(new CodeTypeParameter(type.Name)));
				}
				else
				{
					TypeArguments.Add(new CodeTypeReference(type));
				}
			}
		}
		else if (baseType.IsArray)
		{
			arrayRank = baseType.GetArrayRank();
			arrayElementType = new CodeTypeReference(baseType.GetElementType());
			this.baseType = arrayElementType.BaseType;
		}
		else
		{
			Parse(baseType.FullName);
		}
		isInterface = baseType.IsInterface;
	}

	public CodeTypeReference(CodeTypeReference arrayElementType, int arrayRank)
	{
		baseType = null;
		this.arrayRank = arrayRank;
		this.arrayElementType = arrayElementType;
	}

	[System.MonoTODO("We should parse basetype from right to left in 2.0 profile.")]
	public CodeTypeReference(string baseType, int arrayRank)
		: this(new CodeTypeReference(baseType), arrayRank)
	{
	}

	public CodeTypeReference(CodeTypeParameter typeParameter)
		: this(typeParameter.Name)
	{
		referenceOptions = CodeTypeReferenceOptions.GenericTypeParameter;
	}

	public CodeTypeReference(string typeName, CodeTypeReferenceOptions referenceOptions)
		: this(typeName)
	{
		this.referenceOptions = referenceOptions;
	}

	public CodeTypeReference(Type type, CodeTypeReferenceOptions referenceOptions)
		: this(type)
	{
		this.referenceOptions = referenceOptions;
	}

	public CodeTypeReference(string typeName, params CodeTypeReference[] typeArguments)
		: this(typeName)
	{
		TypeArguments.AddRange(typeArguments);
		if (baseType.IndexOf('`') < 0)
		{
			baseType = baseType + "`" + TypeArguments.Count;
		}
	}

	private void Parse(string baseType)
	{
		if (baseType == null || baseType.Length == 0)
		{
			this.baseType = typeof(void).FullName;
			return;
		}
		int num = baseType.IndexOf('[');
		if (num == -1)
		{
			this.baseType = baseType;
			return;
		}
		int num2 = baseType.LastIndexOf(']');
		if (num2 < num)
		{
			this.baseType = baseType;
			return;
		}
		int num3 = baseType.LastIndexOf('>');
		if (num3 != -1 && num3 > num2)
		{
			this.baseType = baseType;
			return;
		}
		string[] array = baseType.Substring(num + 1, num2 - num - 1).Split(',');
		if (num2 - num != array.Length)
		{
			this.baseType = baseType.Substring(0, num);
			int num4 = 0;
			int i = num;
			StringBuilder stringBuilder = new StringBuilder();
			for (; i < baseType.Length; i++)
			{
				char c = baseType[i];
				switch (c)
				{
				case '[':
					if (num4 > 1 && stringBuilder.Length > 0)
					{
						stringBuilder.Append(c);
					}
					num4++;
					break;
				case ']':
					num4--;
					if (num4 > 1 && stringBuilder.Length > 0)
					{
						stringBuilder.Append(c);
					}
					if (stringBuilder.Length != 0 && num4 % 2 == 0)
					{
						TypeArguments.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
					}
					break;
				case ',':
					if (num4 > 1)
					{
						for (; i + 1 < baseType.Length && baseType[i + 1] != ']'; i++)
						{
						}
					}
					else if (stringBuilder.Length > 0)
					{
						CodeTypeReference value = new CodeTypeReference(stringBuilder.ToString());
						TypeArguments.Add(value);
						stringBuilder.Length = 0;
					}
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
			}
		}
		else
		{
			arrayElementType = new CodeTypeReference(baseType.Substring(0, num));
			arrayRank = array.Length;
		}
	}
}

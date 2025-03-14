namespace System.ComponentModel.Design;

[Serializable]
[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
public sealed class HelpKeywordAttribute : Attribute
{
	public static readonly HelpKeywordAttribute Default;

	private string contextKeyword;

	public string HelpKeyword => contextKeyword;

	public HelpKeywordAttribute()
	{
	}

	public HelpKeywordAttribute(string keyword)
	{
		contextKeyword = keyword;
	}

	public HelpKeywordAttribute(Type t)
	{
		if (t == null)
		{
			throw new ArgumentNullException("t");
		}
		contextKeyword = t.FullName;
	}

	public override bool Equals(object other)
	{
		if (other == null)
		{
			return false;
		}
		if (!(other is HelpKeywordAttribute helpKeywordAttribute))
		{
			return false;
		}
		return helpKeywordAttribute.contextKeyword == contextKeyword;
	}

	public override int GetHashCode()
	{
		return (contextKeyword != null) ? contextKeyword.GetHashCode() : 0;
	}

	public override bool IsDefaultAttribute()
	{
		return contextKeyword == null;
	}
}

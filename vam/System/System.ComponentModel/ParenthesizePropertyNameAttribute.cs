namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class ParenthesizePropertyNameAttribute : Attribute
{
	private bool parenthesis;

	public static readonly ParenthesizePropertyNameAttribute Default = new ParenthesizePropertyNameAttribute();

	public bool NeedParenthesis => parenthesis;

	public ParenthesizePropertyNameAttribute()
	{
		parenthesis = false;
	}

	public ParenthesizePropertyNameAttribute(bool needParenthesis)
	{
		parenthesis = needParenthesis;
	}

	public override bool Equals(object o)
	{
		if (!(o is ParenthesizePropertyNameAttribute))
		{
			return false;
		}
		if (o == this)
		{
			return true;
		}
		return ((ParenthesizePropertyNameAttribute)o).NeedParenthesis == parenthesis;
	}

	public override int GetHashCode()
	{
		return parenthesis.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return parenthesis == Default.NeedParenthesis;
	}
}

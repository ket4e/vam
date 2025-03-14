namespace System.Drawing;

public struct CharacterRange
{
	private int first;

	private int length;

	public int First
	{
		get
		{
			return first;
		}
		set
		{
			first = value;
		}
	}

	public int Length
	{
		get
		{
			return length;
		}
		set
		{
			length = value;
		}
	}

	public CharacterRange(int First, int Length)
	{
		first = First;
		length = Length;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CharacterRange characterRange))
		{
			return false;
		}
		return this == characterRange;
	}

	public override int GetHashCode()
	{
		return first ^ length;
	}

	public static bool operator ==(CharacterRange cr1, CharacterRange cr2)
	{
		return cr1.first == cr2.first && cr1.length == cr2.length;
	}

	public static bool operator !=(CharacterRange cr1, CharacterRange cr2)
	{
		return cr1.first != cr2.first || cr1.length != cr2.length;
	}
}

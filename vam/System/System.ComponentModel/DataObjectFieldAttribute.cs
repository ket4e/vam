namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DataObjectFieldAttribute : Attribute
{
	private bool primary_key;

	private bool is_identity;

	private bool is_nullable;

	private int length = -1;

	public bool IsIdentity => is_identity;

	public bool IsNullable => is_nullable;

	public int Length => length;

	public bool PrimaryKey => primary_key;

	public DataObjectFieldAttribute(bool primaryKey)
	{
		primary_key = primaryKey;
	}

	public DataObjectFieldAttribute(bool primaryKey, bool isIdentity)
	{
		primary_key = primaryKey;
		is_identity = isIdentity;
	}

	public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable)
	{
		primary_key = primaryKey;
		is_identity = isIdentity;
		is_nullable = isNullable;
	}

	public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable, int length)
	{
		primary_key = primaryKey;
		is_identity = isIdentity;
		is_nullable = isNullable;
		this.length = length;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DataObjectFieldAttribute dataObjectFieldAttribute))
		{
			return false;
		}
		return dataObjectFieldAttribute.primary_key == primary_key && dataObjectFieldAttribute.is_identity == is_identity && dataObjectFieldAttribute.is_nullable == is_nullable && dataObjectFieldAttribute.length == length;
	}

	public override int GetHashCode()
	{
		return ((primary_key ? 1 : 0) | (is_identity ? 2 : 0) | (is_nullable ? 4 : 0)) ^ length;
	}
}

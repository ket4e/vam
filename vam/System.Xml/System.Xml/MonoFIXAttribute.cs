namespace System.Xml;

[AttributeUsage(AttributeTargets.All)]
internal class MonoFIXAttribute : Attribute
{
	private string comment;

	public string Comment => comment;

	public MonoFIXAttribute()
	{
	}

	public MonoFIXAttribute(string comment)
	{
		this.comment = comment;
	}
}

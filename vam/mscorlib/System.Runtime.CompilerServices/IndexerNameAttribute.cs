using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[Serializable]
[ComVisible(true)]
[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public sealed class IndexerNameAttribute : Attribute
{
	public IndexerNameAttribute(string indexerName)
	{
	}
}

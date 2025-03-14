using System.ComponentModel;

namespace System.Data;

[EditorBrowsable(EditorBrowsableState.Never)]
[Flags]
[Obsolete]
public enum PropertyAttributes
{
	NotSupported = 0,
	Required = 1,
	Optional = 2,
	Read = 0x200,
	Write = 0x400
}

using System.Runtime.InteropServices;

namespace System.ComponentModel.Design;

[Flags]
[ComVisible(true)]
public enum SelectionTypes
{
	Auto = 1,
	[Obsolete("This value has been deprecated. Use SelectionTypes.Auto instead.")]
	Normal = 1,
	Replace = 2,
	[Obsolete("This value has been deprecated. It is no longer supported.")]
	MouseDown = 4,
	[Obsolete("This value has been deprecated. It is no longer supported.")]
	MouseUp = 8,
	[Obsolete("This value has been deprecated. Use SelectionTypes.Primary instead.")]
	Click = 0x10,
	Primary = 0x10,
	[Obsolete("This value has been deprecated. It is no longer supported.")]
	Valid = 0x1F,
	Toggle = 0x20,
	Add = 0x40,
	Remove = 0x80
}

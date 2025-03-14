using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public enum ScrollEventType
{
	SmallDecrement,
	SmallIncrement,
	LargeDecrement,
	LargeIncrement,
	ThumbPosition,
	ThumbTrack,
	First,
	Last,
	EndScroll
}

using System;

namespace UnityEngine.CSSLayout;

internal enum CSSDirection
{
	Inherit = 0,
	LTR = 1,
	RTL = 2,
	[Obsolete("Use LTR instead")]
	LeftToRight = 1,
	[Obsolete("Use RTL instead")]
	RightToLeft = 2
}

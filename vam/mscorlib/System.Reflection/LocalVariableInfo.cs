using System.Runtime.InteropServices;

namespace System.Reflection;

[ComVisible(true)]
public class LocalVariableInfo
{
	internal Type type;

	internal bool is_pinned;

	internal ushort position;

	public virtual bool IsPinned => is_pinned;

	public virtual int LocalIndex => position;

	public virtual Type LocalType => type;

	internal LocalVariableInfo()
	{
	}

	public override string ToString()
	{
		if (is_pinned)
		{
			return $"{type} ({position}) (pinned)";
		}
		return $"{type} ({position})";
	}
}

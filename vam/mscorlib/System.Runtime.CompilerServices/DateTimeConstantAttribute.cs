using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[Serializable]
[ComVisible(true)]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
public sealed class DateTimeConstantAttribute : CustomConstantAttribute
{
	private long ticks;

	internal long Ticks => ticks;

	public override object Value => ticks;

	public DateTimeConstantAttribute(long ticks)
	{
		this.ticks = ticks;
	}
}

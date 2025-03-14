namespace System.Windows.Forms.CarbonInternal;

internal struct EventTypeSpec
{
	public uint eventClass;

	public uint eventKind;

	public EventTypeSpec(uint eventClass, uint eventKind)
	{
		this.eventClass = eventClass;
		this.eventKind = eventKind;
	}
}

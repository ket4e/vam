namespace System.Windows.Forms.CarbonInternal;

internal struct EventRecord
{
	internal ushort what;

	internal uint message;

	internal uint when;

	internal QDPoint mouse;

	internal ushort modifiers;
}

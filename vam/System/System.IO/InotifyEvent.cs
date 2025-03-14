namespace System.IO;

internal struct InotifyEvent
{
	public static readonly System.IO.InotifyEvent Default = default(System.IO.InotifyEvent);

	public int WatchDescriptor;

	public System.IO.InotifyMask Mask;

	public string Name;

	public override string ToString()
	{
		return $"[Descriptor: {WatchDescriptor} Mask: {Mask} Name: {Name}]";
	}
}

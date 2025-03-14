namespace System.Windows.Forms.RTF;

internal class DestinationCallback
{
	private DestinationDelegate[] callbacks;

	public DestinationDelegate this[Minor c]
	{
		get
		{
			return callbacks[(int)c];
		}
		set
		{
			callbacks[(int)c] = value;
		}
	}

	public DestinationCallback()
	{
		callbacks = new DestinationDelegate[Enum.GetValues(typeof(Minor)).Length];
	}
}

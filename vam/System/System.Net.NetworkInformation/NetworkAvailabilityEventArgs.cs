namespace System.Net.NetworkInformation;

public class NetworkAvailabilityEventArgs : EventArgs
{
	private bool available;

	public bool IsAvailable => available;

	internal NetworkAvailabilityEventArgs(bool available)
	{
		this.available = available;
	}
}

namespace System.ComponentModel;

public class ProgressChangedEventArgs : EventArgs
{
	private int progress;

	private object state;

	public int ProgressPercentage => progress;

	public object UserState => state;

	public ProgressChangedEventArgs(int progressPercentage, object userState)
	{
		progress = progressPercentage;
		state = userState;
	}
}

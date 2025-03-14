namespace System.Windows.Forms;

public class CacheVirtualItemsEventArgs : EventArgs
{
	private int start_index;

	private int end_index;

	public int StartIndex => start_index;

	public int EndIndex => end_index;

	public CacheVirtualItemsEventArgs(int startIndex, int endIndex)
	{
		start_index = startIndex;
		end_index = endIndex;
	}
}

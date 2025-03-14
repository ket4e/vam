namespace UnityEngine.Experimental.UIElements;

internal class CursorManager : ICursorManager
{
	public void SetCursor(CursorStyle cursor)
	{
		Debug.LogError("UIElements cursors are not yet supported outside of the editor.");
	}

	public void ResetCursor()
	{
		Debug.LogError("UIElements cursors are not yet supported outside of the editor.");
	}
}

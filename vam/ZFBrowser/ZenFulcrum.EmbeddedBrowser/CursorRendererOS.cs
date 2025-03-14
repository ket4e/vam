using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class CursorRendererOS : CursorRendererBase
{
	[Tooltip("If true, the mouse cursor should be visible when it's not on a browser.")]
	public bool cursorNormallyVisible = true;

	protected override void CursorChange()
	{
		if (!cursor.HasMouse)
		{
			Cursor.visible = cursorNormallyVisible;
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		else if (cursor.Texture != null)
		{
			Cursor.visible = true;
			Cursor.SetCursor(cursor.Texture, cursor.Hotspot, CursorMode.Auto);
		}
		else
		{
			Cursor.visible = false;
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}
}

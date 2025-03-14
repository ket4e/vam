using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class CursorRendererOverlay : CursorRendererBase
{
	[Tooltip("How large should we render the cursor?")]
	public float scale = 0.5f;

	protected override void CursorChange()
	{
	}

	public void OnGUI()
	{
		if (cursor != null && cursor.HasMouse && (bool)cursor.Texture)
		{
			Texture2D texture = cursor.Texture;
			Rect position = new Rect((float)Screen.width / 2f, (float)Screen.height / 2f, (float)texture.width * scale, (float)texture.height * scale);
			position.x -= cursor.Hotspot.x * scale;
			position.y -= cursor.Hotspot.y * scale;
			GUI.DrawTexture(position, texture);
		}
	}
}

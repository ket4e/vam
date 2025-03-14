namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Script interface for VisualElement cursor style property IStyle.cursor.</para>
/// </summary>
public struct CursorStyle
{
	/// <summary>
	///   <para>The texture to use for the cursor style. To use a texture as a cursor, import the texture with "Read/Write enabled" in the texture importer (or using the "Cursor" defaults).</para>
	/// </summary>
	public Texture2D texture { get; set; }

	/// <summary>
	///   <para>The offset from the top left of the texture to use as the target point (must be within the bounds of the cursor).</para>
	/// </summary>
	public Vector2 hotspot { get; set; }

	internal int defaultCursorId { get; set; }

	public override int GetHashCode()
	{
		return texture.GetHashCode() ^ hotspot.GetHashCode() ^ defaultCursorId.GetHashCode();
	}

	public override bool Equals(object other)
	{
		if (!(other is CursorStyle cursorStyle))
		{
			return false;
		}
		return texture.Equals(cursorStyle.texture) && hotspot.Equals(cursorStyle.hotspot) && defaultCursorId == cursorStyle.defaultCursorId;
	}

	public static bool operator ==(CursorStyle lhs, CursorStyle rhs)
	{
		return lhs.texture == rhs.texture && lhs.hotspot == rhs.hotspot;
	}

	public static bool operator !=(CursorStyle lhs, CursorStyle rhs)
	{
		return !(lhs == rhs);
	}
}

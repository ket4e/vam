using System;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Specification for how to render a character from the font texture. See Font.characterInfo.</para>
/// </summary>
[UsedByNativeCode]
public struct CharacterInfo
{
	/// <summary>
	///   <para>Unicode value of the character.</para>
	/// </summary>
	public int index;

	/// <summary>
	///   <para>UV coordinates for the character in the texture.</para>
	/// </summary>
	[Obsolete("CharacterInfo.uv is deprecated. Use uvBottomLeft, uvBottomRight, uvTopRight or uvTopLeft instead.")]
	public Rect uv;

	/// <summary>
	///   <para>Screen coordinates for the character in generated text meshes.</para>
	/// </summary>
	[Obsolete("CharacterInfo.vert is deprecated. Use minX, maxX, minY, maxY instead.")]
	public Rect vert;

	/// <summary>
	///   <para>How far to advance between the beginning of this charcater and the next.</para>
	/// </summary>
	[Obsolete("CharacterInfo.width is deprecated. Use advance instead.")]
	public float width;

	/// <summary>
	///   <para>The size of the character or 0 if it is the default font size.</para>
	/// </summary>
	public int size;

	/// <summary>
	///   <para>The style of the character.</para>
	/// </summary>
	public FontStyle style;

	/// <summary>
	///   <para>Is the character flipped?</para>
	/// </summary>
	[Obsolete("CharacterInfo.flipped is deprecated. Use uvBottomLeft, uvBottomRight, uvTopRight or uvTopLeft instead, which will be correct regardless of orientation.")]
	public bool flipped;

	/// <summary>
	///   <para>The horizontal distance, rounded to the nearest integer, from the origin of this character to the origin of the next character.</para>
	/// </summary>
	public int advance
	{
		get
		{
			return (int)Math.Round(width, MidpointRounding.AwayFromZero);
		}
		set
		{
			width = value;
		}
	}

	/// <summary>
	///   <para>The width of the glyph image.</para>
	/// </summary>
	public int glyphWidth
	{
		get
		{
			return (int)vert.width;
		}
		set
		{
			vert.width = value;
		}
	}

	/// <summary>
	///   <para>The height of the glyph image.</para>
	/// </summary>
	public int glyphHeight
	{
		get
		{
			return (int)(0f - vert.height);
		}
		set
		{
			float height = vert.height;
			vert.height = -value;
			vert.y += height - vert.height;
		}
	}

	/// <summary>
	///   <para>The horizontal distance from the origin of this glyph to the begining of the glyph image.</para>
	/// </summary>
	public int bearing
	{
		get
		{
			return (int)vert.x;
		}
		set
		{
			vert.x = value;
		}
	}

	/// <summary>
	///   <para>The minimum extend of the glyph image in the y-axis.</para>
	/// </summary>
	public int minY
	{
		get
		{
			return (int)(vert.y + vert.height);
		}
		set
		{
			vert.height = (float)value - vert.y;
		}
	}

	/// <summary>
	///   <para>The maximum extend of the glyph image in the y-axis.</para>
	/// </summary>
	public int maxY
	{
		get
		{
			return (int)vert.y;
		}
		set
		{
			float y = vert.y;
			vert.y = value;
			vert.height += y - vert.y;
		}
	}

	/// <summary>
	///   <para>The minium extend of the glyph image in the x-axis.</para>
	/// </summary>
	public int minX
	{
		get
		{
			return (int)vert.x;
		}
		set
		{
			float x = vert.x;
			vert.x = value;
			vert.width += x - vert.x;
		}
	}

	/// <summary>
	///   <para>The maximum extend of the glyph image in the x-axis.</para>
	/// </summary>
	public int maxX
	{
		get
		{
			return (int)(vert.x + vert.width);
		}
		set
		{
			vert.width = (float)value - vert.x;
		}
	}

	internal Vector2 uvBottomLeftUnFlipped
	{
		get
		{
			return new Vector2(uv.x, uv.y);
		}
		set
		{
			Vector2 vector = uvTopRightUnFlipped;
			uv.x = value.x;
			uv.y = value.y;
			uv.width = vector.x - uv.x;
			uv.height = vector.y - uv.y;
		}
	}

	internal Vector2 uvBottomRightUnFlipped
	{
		get
		{
			return new Vector2(uv.x + uv.width, uv.y);
		}
		set
		{
			Vector2 vector = uvTopRightUnFlipped;
			uv.width = value.x - uv.x;
			uv.y = value.y;
			uv.height = vector.y - uv.y;
		}
	}

	internal Vector2 uvTopRightUnFlipped
	{
		get
		{
			return new Vector2(uv.x + uv.width, uv.y + uv.height);
		}
		set
		{
			uv.width = value.x - uv.x;
			uv.height = value.y - uv.y;
		}
	}

	internal Vector2 uvTopLeftUnFlipped
	{
		get
		{
			return new Vector2(uv.x, uv.y + uv.height);
		}
		set
		{
			Vector2 vector = uvTopRightUnFlipped;
			uv.x = value.x;
			uv.height = value.y - uv.y;
			uv.width = vector.x - uv.x;
		}
	}

	/// <summary>
	///   <para>The uv coordinate matching the bottom left of the glyph image in the font texture.</para>
	/// </summary>
	public Vector2 uvBottomLeft
	{
		get
		{
			return uvBottomLeftUnFlipped;
		}
		set
		{
			uvBottomLeftUnFlipped = value;
		}
	}

	/// <summary>
	///   <para>The uv coordinate matching the bottom right of the glyph image in the font texture.</para>
	/// </summary>
	public Vector2 uvBottomRight
	{
		get
		{
			return (!flipped) ? uvBottomRightUnFlipped : uvTopLeftUnFlipped;
		}
		set
		{
			if (flipped)
			{
				uvTopLeftUnFlipped = value;
			}
			else
			{
				uvBottomRightUnFlipped = value;
			}
		}
	}

	/// <summary>
	///   <para>The uv coordinate matching the top right of the glyph image in the font texture.</para>
	/// </summary>
	public Vector2 uvTopRight
	{
		get
		{
			return uvTopRightUnFlipped;
		}
		set
		{
			uvTopRightUnFlipped = value;
		}
	}

	/// <summary>
	///   <para>The uv coordinate matching the top left of the glyph image in the font texture.</para>
	/// </summary>
	public Vector2 uvTopLeft
	{
		get
		{
			return (!flipped) ? uvTopLeftUnFlipped : uvBottomRightUnFlipped;
		}
		set
		{
			if (flipped)
			{
				uvBottomRightUnFlipped = value;
			}
			else
			{
				uvTopLeftUnFlipped = value;
			}
		}
	}
}

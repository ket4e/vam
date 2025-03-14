using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A Splat prototype is just a texture that is used by the TerrainData.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[UsedByNativeCode]
public sealed class SplatPrototype
{
	internal Texture2D m_Texture;

	internal Texture2D m_NormalMap;

	internal Vector2 m_TileSize = new Vector2(15f, 15f);

	internal Vector2 m_TileOffset = new Vector2(0f, 0f);

	internal Vector4 m_SpecularMetallic = new Vector4(0f, 0f, 0f, 0f);

	internal float m_Smoothness = 0f;

	/// <summary>
	///   <para>Texture of the splat applied to the Terrain.</para>
	/// </summary>
	public Texture2D texture
	{
		get
		{
			return m_Texture;
		}
		set
		{
			m_Texture = value;
		}
	}

	/// <summary>
	///   <para>Normal map of the splat applied to the Terrain.</para>
	/// </summary>
	public Texture2D normalMap
	{
		get
		{
			return m_NormalMap;
		}
		set
		{
			m_NormalMap = value;
		}
	}

	/// <summary>
	///   <para>Size of the tile used in the texture of the SplatPrototype.</para>
	/// </summary>
	public Vector2 tileSize
	{
		get
		{
			return m_TileSize;
		}
		set
		{
			m_TileSize = value;
		}
	}

	/// <summary>
	///   <para>Offset of the tile texture of the SplatPrototype.</para>
	/// </summary>
	public Vector2 tileOffset
	{
		get
		{
			return m_TileOffset;
		}
		set
		{
			m_TileOffset = value;
		}
	}

	public Color specular
	{
		get
		{
			return new Color(m_SpecularMetallic.x, m_SpecularMetallic.y, m_SpecularMetallic.z);
		}
		set
		{
			m_SpecularMetallic.x = value.r;
			m_SpecularMetallic.y = value.g;
			m_SpecularMetallic.z = value.b;
		}
	}

	/// <summary>
	///   <para>The metallic value of the splat layer.</para>
	/// </summary>
	public float metallic
	{
		get
		{
			return m_SpecularMetallic.w;
		}
		set
		{
			m_SpecularMetallic.w = value;
		}
	}

	/// <summary>
	///   <para>The smoothness value of the splat layer when the main texture has no alpha channel.</para>
	/// </summary>
	public float smoothness
	{
		get
		{
			return m_Smoothness;
		}
		set
		{
			m_Smoothness = value;
		}
	}
}

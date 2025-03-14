using System;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps;

/// <summary>
///   <para>Class for a default tile in the Tilemap.</para>
/// </summary>
[Serializable]
[RequiredByNativeCode]
public class Tile : TileBase
{
	/// <summary>
	///   <para>Enum for determining what collider shape is generated for this Tile by the TilemapCollider2D.</para>
	/// </summary>
	public enum ColliderType
	{
		/// <summary>
		///   <para>No collider shape is generated for the Tile by the TilemapCollider2D.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>The Sprite outline is used as the collider shape for the Tile by the TilemapCollider2D.</para>
		/// </summary>
		Sprite,
		/// <summary>
		///   <para>The grid layout boundary outline is used as the collider shape for the Tile by the TilemapCollider2D.</para>
		/// </summary>
		Grid
	}

	[SerializeField]
	private Sprite m_Sprite;

	[SerializeField]
	private Color m_Color = Color.white;

	[SerializeField]
	private Matrix4x4 m_Transform = Matrix4x4.identity;

	[SerializeField]
	private GameObject m_InstancedGameObject;

	[SerializeField]
	private TileFlags m_Flags = TileFlags.LockColor;

	[SerializeField]
	private ColliderType m_ColliderType = ColliderType.Sprite;

	/// <summary>
	///   <para>Sprite to be rendered at the Tile.</para>
	/// </summary>
	public Sprite sprite
	{
		get
		{
			return m_Sprite;
		}
		set
		{
			m_Sprite = value;
		}
	}

	/// <summary>
	///   <para>Color of the Tile.</para>
	/// </summary>
	public Color color
	{
		get
		{
			return m_Color;
		}
		set
		{
			m_Color = value;
		}
	}

	/// <summary>
	///   <para>Matrix4x4|Transform matrix of the Tile.</para>
	/// </summary>
	public Matrix4x4 transform
	{
		get
		{
			return m_Transform;
		}
		set
		{
			m_Transform = value;
		}
	}

	/// <summary>
	///   <para>GameObject of the Tile.</para>
	/// </summary>
	public GameObject gameObject
	{
		get
		{
			return m_InstancedGameObject;
		}
		set
		{
			m_InstancedGameObject = value;
		}
	}

	/// <summary>
	///   <para>TileFlags of the Tile.</para>
	/// </summary>
	public TileFlags flags
	{
		get
		{
			return m_Flags;
		}
		set
		{
			m_Flags = value;
		}
	}

	public ColliderType colliderType
	{
		get
		{
			return m_ColliderType;
		}
		set
		{
			m_ColliderType = value;
		}
	}

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		tileData.sprite = m_Sprite;
		tileData.color = m_Color;
		tileData.transform = m_Transform;
		tileData.gameObject = m_InstancedGameObject;
		tileData.flags = m_Flags;
		tileData.colliderType = m_ColliderType;
	}
}

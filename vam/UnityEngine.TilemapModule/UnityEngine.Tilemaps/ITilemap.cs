using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps;

/// <summary>
///   <para>Class passed onto when information is queried from the tiles.</para>
/// </summary>
[RequiredByNativeCode]
public class ITilemap
{
	internal static ITilemap s_Instance;

	internal Tilemap m_Tilemap;

	/// <summary>
	///   <para>The origin of the Tilemap in cell position.</para>
	/// </summary>
	public Vector3Int origin => m_Tilemap.origin;

	/// <summary>
	///   <para>The size of the Tilemap in cells.</para>
	/// </summary>
	public Vector3Int size => m_Tilemap.size;

	/// <summary>
	///   <para>Returns the boundaries of the Tilemap in local space size.</para>
	/// </summary>
	public Bounds localBounds => m_Tilemap.localBounds;

	/// <summary>
	///   <para>Returns the boundaries of the Tilemap in cell size.</para>
	/// </summary>
	public BoundsInt cellBounds => m_Tilemap.cellBounds;

	internal ITilemap()
	{
	}

	internal void SetTilemapInstance(Tilemap tilemap)
	{
		m_Tilemap = tilemap;
	}

	/// <summary>
	///   <para>Gets the.</para>
	/// </summary>
	/// <param name="position">Position of the Tile on the Tilemap.</param>
	/// <returns>
	///   <para>Sprite at the XY coordinate.</para>
	/// </returns>
	public virtual Sprite GetSprite(Vector3Int position)
	{
		return m_Tilemap.GetSprite(position);
	}

	/// <summary>
	///   <para>Gets the color of a.</para>
	/// </summary>
	/// <param name="position">Position of the Tile on the Tilemap.</param>
	/// <returns>
	///   <para>Color of the at the XY coordinate.</para>
	/// </returns>
	public virtual Color GetColor(Vector3Int position)
	{
		return m_Tilemap.GetColor(position);
	}

	/// <summary>
	///   <para>Gets the transform matrix of a.</para>
	/// </summary>
	/// <param name="position">Position of the Tile on the Tilemap.</param>
	/// <returns>
	///   <para>The transform matrix.</para>
	/// </returns>
	public virtual Matrix4x4 GetTransformMatrix(Vector3Int position)
	{
		return m_Tilemap.GetTransformMatrix(position);
	}

	/// <summary>
	///   <para>Gets the TileFlags of the Tile at the given position.</para>
	/// </summary>
	/// <param name="position">Position of the Tile on the Tilemap.</param>
	/// <returns>
	///   <para>TileFlags from the Tile.</para>
	/// </returns>
	public virtual TileFlags GetTileFlags(Vector3Int position)
	{
		return m_Tilemap.GetTileFlags(position);
	}

	/// <summary>
	///   <para>Gets the.</para>
	/// </summary>
	/// <param name="position">Position of the Tile on the Tilemap.</param>
	/// <returns>
	///   <para> placed at the cell.</para>
	/// </returns>
	public virtual TileBase GetTile(Vector3Int position)
	{
		return m_Tilemap.GetTile(position);
	}

	public virtual T GetTile<T>(Vector3Int position) where T : TileBase
	{
		return m_Tilemap.GetTile<T>(position);
	}

	/// <summary>
	///   <para>Refreshes a.</para>
	/// </summary>
	/// <param name="position">Position of the Tile on the Tilemap.</param>
	public void RefreshTile(Vector3Int position)
	{
		m_Tilemap.RefreshTile(position);
	}

	public T GetComponent<T>()
	{
		return m_Tilemap.GetComponent<T>();
	}

	[RequiredByNativeCode]
	private static ITilemap CreateInstance()
	{
		s_Instance = new ITilemap();
		return s_Instance;
	}
}

using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The TerrainData class stores heightmaps, detail mesh positions, tree instances, and terrain texture alpha maps.</para>
/// </summary>
[NativeHeader("Modules/Terrain/Public/TerrainDataScriptingInterface.h")]
[NativeHeader("TerrainScriptingClasses.h")]
public sealed class TerrainData : Object
{
	private enum BoundaryValueType
	{
		MaxHeightmapRes,
		MinDetailResPerPatch,
		MaxDetailResPerPatch,
		MaxDetailPatchCount,
		MinAlphamapRes,
		MaxAlphamapRes,
		MinBaseMapRes,
		MaxBaseMapRes
	}

	private const string k_ScriptingInterfaceName = "TerrainDataScriptingInterface";

	private const string k_ScriptingInterfacePrefix = "TerrainDataScriptingInterface::";

	private const string k_HeightmapPrefix = "GetHeightmap().";

	private const string k_DetailDatabasePrefix = "GetDetailDatabase().";

	private const string k_TreeDatabasePrefix = "GetTreeDatabase().";

	private const string k_SplatDatabasePrefix = "GetSplatDatabase().";

	private static readonly int k_MaximumResolution = GetBoundaryValue(BoundaryValueType.MaxHeightmapRes);

	private static readonly int k_MinimumDetailResolutionPerPatch = GetBoundaryValue(BoundaryValueType.MinDetailResPerPatch);

	private static readonly int k_MaximumDetailResolutionPerPatch = GetBoundaryValue(BoundaryValueType.MaxDetailResPerPatch);

	private static readonly int k_MaximumDetailPatchCount = GetBoundaryValue(BoundaryValueType.MaxDetailPatchCount);

	private static readonly int k_MinimumAlphamapResolution = GetBoundaryValue(BoundaryValueType.MinAlphamapRes);

	private static readonly int k_MaximumAlphamapResolution = GetBoundaryValue(BoundaryValueType.MaxAlphamapRes);

	private static readonly int k_MinimumBaseMapResolution = GetBoundaryValue(BoundaryValueType.MinBaseMapRes);

	private static readonly int k_MaximumBaseMapResolution = GetBoundaryValue(BoundaryValueType.MaxBaseMapRes);

	/// <summary>
	///   <para>Width of the terrain in samples (Read Only).</para>
	/// </summary>
	public extern int heightmapWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetHeightmap().GetWidth")]
		get;
	}

	/// <summary>
	///   <para>Height of the terrain in samples (Read Only).</para>
	/// </summary>
	public extern int heightmapHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetHeightmap().GetHeight")]
		get;
	}

	/// <summary>
	///   <para>Resolution of the heightmap.</para>
	/// </summary>
	public int heightmapResolution
	{
		get
		{
			return internalHeightmapResolution;
		}
		set
		{
			int num = value;
			if (value < 0 || value > k_MaximumResolution)
			{
				Debug.LogWarning("heightmapResolution is clamped to the range of [0, " + k_MaximumResolution + "].");
				num = Math.Min(k_MaximumResolution, Math.Max(value, 0));
			}
			internalHeightmapResolution = num;
		}
	}

	private extern int internalHeightmapResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetHeightmap().GetResolution")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetHeightmap().SetResolution")]
		set;
	}

	/// <summary>
	///   <para>The size of each heightmap sample.</para>
	/// </summary>
	public Vector3 heightmapScale
	{
		[NativeName("GetHeightmap().GetScale")]
		get
		{
			get_heightmapScale_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>The total size in world units of the terrain.</para>
	/// </summary>
	public Vector3 size
	{
		[NativeName("GetHeightmap().GetSize")]
		get
		{
			get_size_Injected(out var ret);
			return ret;
		}
		[NativeName("GetHeightmap().SetSize")]
		set
		{
			set_size_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The local bounding box of the TerrainData object.</para>
	/// </summary>
	public Bounds bounds
	{
		[NativeName("GetHeightmap().CalculateBounds")]
		get
		{
			get_bounds_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>The thickness of the terrain used for collision detection.</para>
	/// </summary>
	public extern float thickness
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetHeightmap().GetThickness")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetHeightmap().SetThickness")]
		set;
	}

	/// <summary>
	///   <para>Strength of the waving grass in the terrain.</para>
	/// </summary>
	public extern float wavingGrassStrength
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetDetailDatabase().GetWavingGrassStrength")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassStrength", HasExplicitThis = true)]
		set;
	}

	/// <summary>
	///   <para>Amount of waving grass in the terrain.</para>
	/// </summary>
	public extern float wavingGrassAmount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetDetailDatabase().GetWavingGrassAmount")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassAmount", HasExplicitThis = true)]
		set;
	}

	/// <summary>
	///   <para>Speed of the waving grass.</para>
	/// </summary>
	public extern float wavingGrassSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetDetailDatabase().GetWavingGrassSpeed")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassSpeed", HasExplicitThis = true)]
		set;
	}

	/// <summary>
	///   <para>Color of the waving grass that the terrain has.</para>
	/// </summary>
	public Color wavingGrassTint
	{
		[NativeName("GetDetailDatabase().GetWavingGrassTint")]
		get
		{
			get_wavingGrassTint_Injected(out var ret);
			return ret;
		}
		[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassTint", HasExplicitThis = true)]
		set
		{
			set_wavingGrassTint_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Detail width of the TerrainData.</para>
	/// </summary>
	public extern int detailWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetDetailDatabase().GetWidth")]
		get;
	}

	/// <summary>
	///   <para>Detail height of the TerrainData.</para>
	/// </summary>
	public extern int detailHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetDetailDatabase().GetHeight")]
		get;
	}

	/// <summary>
	///   <para>Detail Resolution of the TerrainData.</para>
	/// </summary>
	public extern int detailResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetDetailDatabase().GetResolution")]
		get;
	}

	internal extern int detailResolutionPerPatch
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetDetailDatabase().GetResolutionPerPatch")]
		get;
	}

	/// <summary>
	///   <para>Contains the detail texture/meshes that the terrain has.</para>
	/// </summary>
	public extern DetailPrototype[] detailPrototypes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TerrainDataScriptingInterface::GetDetailPrototypes", HasExplicitThis = true)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TerrainDataScriptingInterface::SetDetailPrototypes", HasExplicitThis = true)]
		set;
	}

	/// <summary>
	///   <para>Contains the current trees placed in the terrain.</para>
	/// </summary>
	public TreeInstance[] treeInstances
	{
		get
		{
			return Internal_GetTreeInstances();
		}
		set
		{
			Internal_SetTreeInstances(value);
		}
	}

	/// <summary>
	///   <para>Returns the number of tree instances.</para>
	/// </summary>
	public extern int treeInstanceCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetTreeDatabase().GetInstances().size")]
		get;
	}

	/// <summary>
	///   <para>The list of tree prototypes this are the ones available in the inspector.</para>
	/// </summary>
	public extern TreePrototype[] treePrototypes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TerrainDataScriptingInterface::GetTreePrototypes", HasExplicitThis = true)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TerrainDataScriptingInterface::SetTreePrototypes", HasExplicitThis = true)]
		set;
	}

	/// <summary>
	///   <para>Number of alpha map layers.</para>
	/// </summary>
	public extern int alphamapLayers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetSplatDatabase().GetDepth")]
		get;
	}

	/// <summary>
	///   <para>Resolution of the alpha map.</para>
	/// </summary>
	public int alphamapResolution
	{
		get
		{
			return Internal_alphamapResolution;
		}
		set
		{
			int internal_alphamapResolution = value;
			if (value < k_MinimumAlphamapResolution || value > k_MaximumAlphamapResolution)
			{
				Debug.LogWarning("alphamapResolution is clamped to the range of [" + k_MinimumAlphamapResolution + ", " + k_MaximumAlphamapResolution + "].");
				internal_alphamapResolution = Math.Min(k_MaximumAlphamapResolution, Math.Max(value, k_MinimumAlphamapResolution));
			}
			Internal_alphamapResolution = internal_alphamapResolution;
		}
	}

	private extern int Internal_alphamapResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetSplatDatabase().GetAlphamapResolution")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetSplatDatabase().SetAlphamapResolution")]
		set;
	}

	/// <summary>
	///   <para>Width of the alpha map.</para>
	/// </summary>
	public int alphamapWidth => alphamapResolution;

	/// <summary>
	///   <para>Height of the alpha map.</para>
	/// </summary>
	public int alphamapHeight => alphamapResolution;

	/// <summary>
	///   <para>Resolution of the base map used for rendering far patches on the terrain.</para>
	/// </summary>
	public int baseMapResolution
	{
		get
		{
			return Internal_baseMapResolution;
		}
		set
		{
			int internal_baseMapResolution = value;
			if (value < k_MinimumBaseMapResolution || value > k_MaximumBaseMapResolution)
			{
				Debug.LogWarning("baseMapResolution is clamped to the range of [" + k_MinimumBaseMapResolution + ", " + k_MaximumBaseMapResolution + "].");
				internal_baseMapResolution = Math.Min(k_MaximumBaseMapResolution, Math.Max(value, k_MinimumBaseMapResolution));
			}
			Internal_baseMapResolution = internal_baseMapResolution;
		}
	}

	private extern int Internal_baseMapResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetSplatDatabase().GetBaseMapResolution")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetSplatDatabase().SetBaseMapResolution")]
		set;
	}

	private extern int alphamapTextureCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetSplatDatabase().GetAlphaTextureCount")]
		get;
	}

	/// <summary>
	///   <para>Alpha map textures used by the Terrain. Used by Terrain Inspector for undo.</para>
	/// </summary>
	public Texture2D[] alphamapTextures
	{
		get
		{
			Texture2D[] array = new Texture2D[alphamapTextureCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GetAlphamapTexture(i);
			}
			return array;
		}
	}

	/// <summary>
	///   <para>Splat texture used by the terrain.</para>
	/// </summary>
	public extern SplatPrototype[] splatPrototypes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TerrainDataScriptingInterface::GetSplatPrototypes", HasExplicitThis = true)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TerrainDataScriptingInterface::SetSplatPrototypes", HasExplicitThis = true)]
		set;
	}

	public TerrainData()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[StaticAccessor("TerrainDataScriptingInterface", StaticAccessorType.DoubleColon)]
	private static extern int GetBoundaryValue(BoundaryValueType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::Create")]
	private static extern void Internal_Create([Writable] TerrainData terrainData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern bool HasUser(GameObject user);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void AddUser(GameObject user);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void RemoveUser(GameObject user);

	/// <summary>
	///   <para>Gets the height at a certain point x,y.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetHeightmap().GetHeight")]
	public extern float GetHeight(int x, int y);

	/// <summary>
	///   <para>Gets an interpolated height at a point x,y.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetHeightmap().GetInterpolatedHeight")]
	public extern float GetInterpolatedHeight(float x, float y);

	/// <summary>
	///   <para>Get an array of heightmap samples.</para>
	/// </summary>
	/// <param name="xBase">First x index of heightmap samples to retrieve.</param>
	/// <param name="yBase">First y index of heightmap samples to retrieve.</param>
	/// <param name="width">Number of samples to retrieve along the heightmap's x axis.</param>
	/// <param name="height">Number of samples to retrieve along the heightmap's y axis.</param>
	public float[,] GetHeights(int xBase, int yBase, int width, int height)
	{
		if (xBase < 0 || yBase < 0 || xBase + width < 0 || yBase + height < 0 || xBase + width > heightmapWidth || yBase + height > heightmapHeight)
		{
			throw new ArgumentException("Trying to access out-of-bounds terrain height information.");
		}
		return Internal_GetHeights(xBase, yBase, width, height);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::GetHeights", HasExplicitThis = true)]
	private extern float[,] Internal_GetHeights(int xBase, int yBase, int width, int height);

	public void SetHeights(int xBase, int yBase, float[,] heights)
	{
		if (heights == null)
		{
			throw new NullReferenceException();
		}
		if (xBase + heights.GetLength(1) > heightmapWidth || xBase + heights.GetLength(1) < 0 || yBase + heights.GetLength(0) < 0 || xBase < 0 || yBase < 0 || yBase + heights.GetLength(0) > heightmapHeight)
		{
			throw new ArgumentException(UnityString.Format("X or Y base out of bounds. Setting up to {0}x{1} while map size is {2}x{3}", xBase + heights.GetLength(1), yBase + heights.GetLength(0), heightmapWidth, heightmapHeight));
		}
		Internal_SetHeights(xBase, yBase, heights.GetLength(1), heights.GetLength(0), heights);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::SetHeights", HasExplicitThis = true)]
	private extern void Internal_SetHeights(int xBase, int yBase, int width, int height, float[,] heights);

	public void SetHeightsDelayLOD(int xBase, int yBase, float[,] heights)
	{
		if (heights == null)
		{
			throw new ArgumentNullException("heights");
		}
		int length = heights.GetLength(0);
		int length2 = heights.GetLength(1);
		if (xBase < 0 || xBase + length2 < 0 || xBase + length2 > heightmapWidth)
		{
			throw new ArgumentException(UnityString.Format("X out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", xBase, xBase + length2, heightmapWidth));
		}
		if (yBase < 0 || yBase + length < 0 || yBase + length > heightmapHeight)
		{
			throw new ArgumentException(UnityString.Format("Y out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", yBase, yBase + length, heightmapHeight));
		}
		Internal_SetHeightsDelayLOD(xBase, yBase, length2, length, heights);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::SetHeightsDelayLOD", HasExplicitThis = true)]
	private extern void Internal_SetHeightsDelayLOD(int xBase, int yBase, int width, int height, float[,] heights);

	/// <summary>
	///   <para>Gets the gradient of the terrain at point (x,y).</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetHeightmap().GetSteepness")]
	public extern float GetSteepness(float x, float y);

	/// <summary>
	///   <para>Get an interpolated normal at a given location.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	[NativeName("GetHeightmap().GetInterpolatedNormal")]
	public Vector3 GetInterpolatedNormal(float x, float y)
	{
		GetInterpolatedNormal_Injected(x, y, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetHeightmap().GetAdjustedSize")]
	internal extern int GetAdjustedSize(int size);

	/// <summary>
	///   <para>Set the resolution of the detail map.</para>
	/// </summary>
	/// <param name="detailResolution">Specifies the number of pixels in the detail resolution map. A larger detailResolution, leads to more accurate detail object painting.</param>
	/// <param name="resolutionPerPatch">Specifies the size in pixels of each individually rendered detail patch. A larger number reduces draw calls, but might increase triangle count since detail patches are culled on a per batch basis. A recommended value is 16. If you use a very large detail object distance and your grass is very sparse, it makes sense to increase the value.</param>
	public void SetDetailResolution(int detailResolution, int resolutionPerPatch)
	{
		if (detailResolution < 0)
		{
			Debug.LogWarning("detailResolution must not be negative.");
			detailResolution = 0;
		}
		if (resolutionPerPatch < k_MinimumDetailResolutionPerPatch || resolutionPerPatch > k_MaximumDetailResolutionPerPatch)
		{
			Debug.LogWarning("resolutionPerPatch is clamped to the range of [" + k_MinimumDetailResolutionPerPatch + ", " + k_MaximumDetailResolutionPerPatch + "].");
			resolutionPerPatch = Math.Min(k_MaximumDetailResolutionPerPatch, Math.Max(resolutionPerPatch, k_MinimumDetailResolutionPerPatch));
		}
		int num = detailResolution / resolutionPerPatch;
		if (num > k_MaximumDetailPatchCount)
		{
			Debug.LogWarning("Patch count (detailResolution / resolutionPerPatch) is clamped to the range of [0, " + k_MaximumDetailPatchCount + "].");
			num = Math.Min(k_MaximumDetailPatchCount, Math.Max(num, 0));
		}
		Internal_SetDetailResolution(num, resolutionPerPatch);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetDetailDatabase().SetDetailResolution")]
	private extern void Internal_SetDetailResolution(int patchCount, int resolutionPerPatch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetDetailDatabase().ResetDirtyDetails")]
	internal extern void ResetDirtyDetails();

	/// <summary>
	///   <para>Reloads all the values of the available prototypes (ie, detail mesh assets) in the TerrainData Object.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::RefreshPrototypes", HasExplicitThis = true)]
	public extern void RefreshPrototypes();

	/// <summary>
	///   <para>Returns an array of all supported detail layer indices in the area.</para>
	/// </summary>
	/// <param name="xBase"></param>
	/// <param name="yBase"></param>
	/// <param name="totalWidth"></param>
	/// <param name="totalHeight"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::GetSupportedLayers", HasExplicitThis = true)]
	public extern int[] GetSupportedLayers(int xBase, int yBase, int totalWidth, int totalHeight);

	/// <summary>
	///   <para>Returns a 2D array of the detail object density in the specific location.</para>
	/// </summary>
	/// <param name="xBase"></param>
	/// <param name="yBase"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="layer"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::GetDetailLayer", HasExplicitThis = true)]
	public extern int[,] GetDetailLayer(int xBase, int yBase, int width, int height, int layer);

	public void SetDetailLayer(int xBase, int yBase, int layer, int[,] details)
	{
		Internal_SetDetailLayer(xBase, yBase, details.GetLength(1), details.GetLength(0), layer, details);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::SetDetailLayer", HasExplicitThis = true)]
	private extern void Internal_SetDetailLayer(int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, int[,] data);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetTreeDatabase().GetInstances")]
	private extern TreeInstance[] Internal_GetTreeInstances();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::SetTreeInstances", HasExplicitThis = true)]
	private extern void Internal_SetTreeInstances([NotNull] TreeInstance[] instances);

	/// <summary>
	///   <para>Get the tree instance at the specified index. It is used as a faster version of treeInstances[index] as this function doesn't create the entire tree instances array.</para>
	/// </summary>
	/// <param name="index">The index of the tree instance.</param>
	public TreeInstance GetTreeInstance(int index)
	{
		if (index < 0 || index >= treeInstanceCount)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		return Internal_GetTreeInstance(index);
	}

	[FreeFunction("TerrainDataScriptingInterface::GetTreeInstance", HasExplicitThis = true)]
	private TreeInstance Internal_GetTreeInstance(int index)
	{
		Internal_GetTreeInstance_Injected(index, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Set the tree instance with new parameters at the specified index. However, TreeInstance.prototypeIndex and TreeInstance.position can not be changed otherwise an ArgumentException will be thrown.</para>
	/// </summary>
	/// <param name="index">The index of the tree instance.</param>
	/// <param name="instance">The new TreeInstance value.</param>
	[FreeFunction("TerrainDataScriptingInterface::SetTreeInstance", HasExplicitThis = true)]
	[NativeThrows]
	public void SetTreeInstance(int index, TreeInstance instance)
	{
		SetTreeInstance_Injected(index, ref instance);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetTreeDatabase().RemoveTreePrototype")]
	internal extern void RemoveTreePrototype(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetTreeDatabase().RecalculateTreePositions")]
	internal extern void RecalculateTreePositions();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetDetailDatabase().RemoveDetailPrototype")]
	internal extern void RemoveDetailPrototype(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetTreeDatabase().NeedUpgradeScaledPrototypes")]
	internal extern bool NeedUpgradeScaledTreePrototypes();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::UpgradeScaledTreePrototype", HasExplicitThis = true)]
	internal extern void UpgradeScaledTreePrototype();

	/// <summary>
	///   <para>Returns the alpha map at a position x, y given a width and height.</para>
	/// </summary>
	/// <param name="x">The x offset to read from.</param>
	/// <param name="y">The y offset to read from.</param>
	/// <param name="width">The width of the alpha map area to read.</param>
	/// <param name="height">The height of the alpha map area to read.</param>
	/// <returns>
	///   <para>A 3D array of floats, where the 3rd dimension represents the mixing weight of each splatmap at each x,y coordinate.</para>
	/// </returns>
	public float[,,] GetAlphamaps(int x, int y, int width, int height)
	{
		if (x < 0 || y < 0 || width < 0 || height < 0)
		{
			throw new ArgumentException("Invalid argument for GetAlphaMaps");
		}
		return Internal_GetAlphamaps(x, y, width, height);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::GetAlphamaps", HasExplicitThis = true)]
	private extern float[,,] Internal_GetAlphamaps(int x, int y, int width, int height);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[RequiredByNativeCode]
	[NativeName("GetSplatDatabase().GetAlphamapResolution")]
	internal extern float GetAlphamapResolutionInternal();

	public void SetAlphamaps(int x, int y, float[,,] map)
	{
		if (map.GetLength(2) != alphamapLayers)
		{
			throw new Exception(UnityString.Format("Float array size wrong (layers should be {0})", alphamapLayers));
		}
		Internal_SetAlphamaps(x, y, map.GetLength(1), map.GetLength(0), map);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::SetAlphamaps", HasExplicitThis = true)]
	private extern void Internal_SetAlphamaps(int x, int y, int width, int height, float[,,] map);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetSplatDatabase().RecalculateBasemapIfDirty")]
	internal extern void RecalculateBasemapIfDirty();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetSplatDatabase().SetBasemapDirty")]
	internal extern void SetBasemapDirty(bool dirty);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetSplatDatabase().GetAlphaTexture")]
	private extern Texture2D GetAlphamapTexture(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetTreeDatabase().AddTree")]
	internal extern void AddTree(ref TreeInstance tree);

	[NativeName("GetTreeDatabase().RemoveTrees")]
	internal int RemoveTrees(Vector2 position, float radius, int prototypeIndex)
	{
		return RemoveTrees_Injected(ref position, radius, prototypeIndex);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_heightmapScale_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_size_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_size_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_bounds_Injected(out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetInterpolatedNormal_Injected(float x, float y, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_wavingGrassTint_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_wavingGrassTint_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void Internal_GetTreeInstance_Injected(int index, out TreeInstance ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTreeInstance_Injected(int index, ref TreeInstance instance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int RemoveTrees_Injected(ref Vector2 position, float radius, int prototypeIndex);
}

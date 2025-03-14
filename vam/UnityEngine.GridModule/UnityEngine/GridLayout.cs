using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>An abstract class that defines a grid layout.</para>
/// </summary>
[NativeType(Header = "Modules/Grid/Public/Grid.h")]
[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
[RequireComponent(typeof(Transform))]
public class GridLayout : Behaviour
{
	/// <summary>
	///   <para>The layout of the GridLayout.</para>
	/// </summary>
	public enum CellLayout
	{
		/// <summary>
		///   <para>Rectangular layout for cells in the GridLayout.</para>
		/// </summary>
		Rectangle
	}

	/// <summary>
	///   <para>Swizzles cell positions to other positions.</para>
	/// </summary>
	public enum CellSwizzle
	{
		/// <summary>
		///   <para>Keeps the cell positions at XYZ.</para>
		/// </summary>
		XYZ,
		/// <summary>
		///   <para>Swizzles the cell positions from XYZ to XZY.</para>
		/// </summary>
		XZY,
		/// <summary>
		///   <para>Swizzles the cell positions from XYZ to YXZ.</para>
		/// </summary>
		YXZ,
		/// <summary>
		///   <para>Swizzles the cell positions from XYZ to YZX.</para>
		/// </summary>
		YZX,
		/// <summary>
		///   <para>Swizzles the cell positions from XYZ to ZXY.</para>
		/// </summary>
		ZXY,
		/// <summary>
		///   <para>Swizzles the cell positions from XYZ to ZYX.</para>
		/// </summary>
		ZYX
	}

	/// <summary>
	///   <para>The size of each cell in the layout.</para>
	/// </summary>
	public Vector3 cellSize
	{
		[FreeFunction("GridLayoutBindings::GetCellSize", HasExplicitThis = true)]
		get
		{
			get_cellSize_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>The size of the gap between each cell in the layout.</para>
	/// </summary>
	public Vector3 cellGap
	{
		[FreeFunction("GridLayoutBindings::GetCellGap", HasExplicitThis = true)]
		get
		{
			get_cellGap_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>The layout of the cells.</para>
	/// </summary>
	public extern CellLayout cellLayout
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>The cell swizzle for the layout.</para>
	/// </summary>
	public extern CellSwizzle cellSwizzle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Returns the local bounds for a cell at the location.</para>
	/// </summary>
	/// <param name="localPosition">Location of the cell.</param>
	/// <param name="cellPosition"></param>
	/// <returns>
	///   <para>Local bounds of cell at the position.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::GetBoundsLocal", HasExplicitThis = true)]
	public Bounds GetBoundsLocal(Vector3Int cellPosition)
	{
		GetBoundsLocal_Injected(ref cellPosition, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Converts a cell position to local position space.</para>
	/// </summary>
	/// <param name="cellPosition">Cell position to convert.</param>
	/// <returns>
	///   <para>Local position of the cell position.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::CellToLocal", HasExplicitThis = true)]
	public Vector3 CellToLocal(Vector3Int cellPosition)
	{
		CellToLocal_Injected(ref cellPosition, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Converts a local position to cell position.</para>
	/// </summary>
	/// <param name="localPosition">Local Position to convert.</param>
	/// <returns>
	///   <para>Cell position of the local position.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::LocalToCell", HasExplicitThis = true)]
	public Vector3Int LocalToCell(Vector3 localPosition)
	{
		LocalToCell_Injected(ref localPosition, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Converts an interpolated cell position in floats to local position space.</para>
	/// </summary>
	/// <param name="cellPosition">Interpolated cell position to convert.</param>
	/// <returns>
	///   <para>Local position of the cell position.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::CellToLocalInterpolated", HasExplicitThis = true)]
	public Vector3 CellToLocalInterpolated(Vector3 cellPosition)
	{
		CellToLocalInterpolated_Injected(ref cellPosition, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Converts a local position to cell position.</para>
	/// </summary>
	/// <param name="localPosition">Local Position to convert.</param>
	/// <returns>
	///   <para>Interpolated cell position of the local position.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::LocalToCellInterpolated", HasExplicitThis = true)]
	public Vector3 LocalToCellInterpolated(Vector3 localPosition)
	{
		LocalToCellInterpolated_Injected(ref localPosition, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Converts a cell position to world position space.</para>
	/// </summary>
	/// <param name="cellPosition">Cell position to convert.</param>
	/// <returns>
	///   <para>World position of the cell position.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::CellToWorld", HasExplicitThis = true)]
	public Vector3 CellToWorld(Vector3Int cellPosition)
	{
		CellToWorld_Injected(ref cellPosition, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Converts a world position to cell position.</para>
	/// </summary>
	/// <param name="worldPosition">World Position to convert.</param>
	/// <returns>
	///   <para>Cell position of the world position.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::WorldToCell", HasExplicitThis = true)]
	public Vector3Int WorldToCell(Vector3 worldPosition)
	{
		WorldToCell_Injected(ref worldPosition, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Converts a local position to world position.</para>
	/// </summary>
	/// <param name="localPosition">Local Position to convert.</param>
	/// <returns>
	///   <para>World position of the local position.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::LocalToWorld", HasExplicitThis = true)]
	public Vector3 LocalToWorld(Vector3 localPosition)
	{
		LocalToWorld_Injected(ref localPosition, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Converts a world position to local position.</para>
	/// </summary>
	/// <param name="worldPosition">World Position to convert.</param>
	/// <returns>
	///   <para>Local position of the world position.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::WorldToLocal", HasExplicitThis = true)]
	public Vector3 WorldToLocal(Vector3 worldPosition)
	{
		WorldToLocal_Injected(ref worldPosition, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Get the default center coordinate of a cell for the set layout of the Grid.</para>
	/// </summary>
	/// <returns>
	///   <para>Cell Center coordinate.</para>
	/// </returns>
	[FreeFunction("GridLayoutBindings::GetLayoutCellCenter", HasExplicitThis = true)]
	public Vector3 GetLayoutCellCenter()
	{
		GetLayoutCellCenter_Injected(out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_cellSize_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_cellGap_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetBoundsLocal_Injected(ref Vector3Int cellPosition, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void CellToLocal_Injected(ref Vector3Int cellPosition, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void LocalToCell_Injected(ref Vector3 localPosition, out Vector3Int ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void CellToLocalInterpolated_Injected(ref Vector3 cellPosition, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void LocalToCellInterpolated_Injected(ref Vector3 localPosition, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void CellToWorld_Injected(ref Vector3Int cellPosition, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void WorldToCell_Injected(ref Vector3 worldPosition, out Vector3Int ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void LocalToWorld_Injected(ref Vector3 localPosition, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void WorldToLocal_Injected(ref Vector3 worldPosition, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetLayoutCellCenter_Injected(out Vector3 ret);
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A collection of common line functions.</para>
/// </summary>
public sealed class LineUtility
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void GeneratePointsToKeep3D(object pointsList, float tolerance, object pointsToKeepList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void GeneratePointsToKeep2D(object pointsList, float tolerance, object pointsToKeepList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void GenerateSimplifiedPoints3D(object pointsList, float tolerance, object simplifiedPoints);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void GenerateSimplifiedPoints2D(object pointsList, float tolerance, object simplifiedPoints);

	public static void Simplify(List<Vector3> points, float tolerance, List<int> pointsToKeep)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (pointsToKeep == null)
		{
			throw new ArgumentNullException("pointsToKeep");
		}
		GeneratePointsToKeep3D(points, tolerance, pointsToKeep);
	}

	public static void Simplify(List<Vector3> points, float tolerance, List<Vector3> simplifiedPoints)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (simplifiedPoints == null)
		{
			throw new ArgumentNullException("simplifiedPoints");
		}
		GenerateSimplifiedPoints3D(points, tolerance, simplifiedPoints);
	}

	public static void Simplify(List<Vector2> points, float tolerance, List<int> pointsToKeep)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (pointsToKeep == null)
		{
			throw new ArgumentNullException("pointsToKeep");
		}
		GeneratePointsToKeep2D(points, tolerance, pointsToKeep);
	}

	public static void Simplify(List<Vector2> points, float tolerance, List<Vector2> simplifiedPoints)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (simplifiedPoints == null)
		{
			throw new ArgumentNullException("simplifiedPoints");
		}
		GenerateSimplifiedPoints2D(points, tolerance, simplifiedPoints);
	}
}

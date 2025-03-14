using System;

namespace UnityEngine.Experimental.AI;

/// <summary>
///   <para>Represents a compact identifier for the data of a NavMesh node.</para>
/// </summary>
public struct PolygonId : IEquatable<PolygonId>
{
	internal ulong polyRef;

	/// <summary>
	///   <para>Returns true if the PolygonId has been created empty and has never pointed to any node in the NavMesh.</para>
	/// </summary>
	public bool IsNull()
	{
		return polyRef == 0;
	}

	public static bool operator ==(PolygonId x, PolygonId y)
	{
		return x.polyRef == y.polyRef;
	}

	public static bool operator !=(PolygonId x, PolygonId y)
	{
		return x.polyRef != y.polyRef;
	}

	/// <summary>
	///   <para>Returns the hash code for use in collections.</para>
	/// </summary>
	public override int GetHashCode()
	{
		return polyRef.GetHashCode();
	}

	/// <summary>
	///   <para>Returns true if two PolygonId objects refer to the same NavMesh node.</para>
	/// </summary>
	/// <param name="rhs"></param>
	/// <param name="obj"></param>
	public bool Equals(PolygonId rhs)
	{
		return rhs == this;
	}

	/// <summary>
	///   <para>Returns true if two PolygonId objects refer to the same NavMesh node.</para>
	/// </summary>
	/// <param name="rhs"></param>
	/// <param name="obj"></param>
	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is PolygonId polygonId))
		{
			return false;
		}
		return polygonId == this;
	}
}

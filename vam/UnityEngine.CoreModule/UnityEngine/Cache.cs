using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Data structure for cache. Please refer to See Also:Caching.AddCache for more information.</para>
/// </summary>
[NativeHeader("Runtime/Misc/Cache.h")]
[StaticAccessor("CacheWrapper", StaticAccessorType.DoubleColon)]
public struct Cache
{
	private int m_Handle;

	internal int handle => m_Handle;

	/// <summary>
	///   <para>Returns true if the cache is valid.</para>
	/// </summary>
	public bool valid => Cache_IsValid(m_Handle);

	/// <summary>
	///   <para>Returns true if the cache is ready.</para>
	/// </summary>
	public bool ready => Cache_IsReady(m_Handle);

	/// <summary>
	///   <para>Returns true if the cache is readonly.</para>
	/// </summary>
	public bool readOnly => Cache_IsReadonly(m_Handle);

	/// <summary>
	///   <para>Returns the path of the cache.</para>
	/// </summary>
	public string path => Cache_GetPath(m_Handle);

	/// <summary>
	///   <para>Returns the index of the cache in the cache list.</para>
	/// </summary>
	public int index => Cache_GetIndex(m_Handle);

	/// <summary>
	///   <para>Returns the number of currently unused bytes in the cache.</para>
	/// </summary>
	public long spaceFree => Cache_GetSpaceFree(m_Handle);

	/// <summary>
	///   <para>Allows you to specify the total number of bytes that can be allocated for the cache.</para>
	/// </summary>
	public long maximumAvailableStorageSpace
	{
		get
		{
			return Cache_GetMaximumDiskSpaceAvailable(m_Handle);
		}
		set
		{
			Cache_SetMaximumDiskSpaceAvailable(m_Handle, value);
		}
	}

	/// <summary>
	///   <para>Returns the used disk space in bytes.</para>
	/// </summary>
	public long spaceOccupied => Cache_GetCachingDiskSpaceUsed(m_Handle);

	/// <summary>
	///   <para>The number of seconds that an AssetBundle may remain unused in the cache before it is automatically deleted.</para>
	/// </summary>
	public int expirationDelay
	{
		get
		{
			return Cache_GetExpirationDelay(m_Handle);
		}
		set
		{
			Cache_SetExpirationDelay(m_Handle, value);
		}
	}

	public static bool operator ==(Cache lhs, Cache rhs)
	{
		return lhs.handle == rhs.handle;
	}

	public static bool operator !=(Cache lhs, Cache rhs)
	{
		return lhs.handle != rhs.handle;
	}

	public override int GetHashCode()
	{
		return m_Handle;
	}

	public override bool Equals(object other)
	{
		if (!(other is Cache cache))
		{
			return false;
		}
		return handle == cache.handle;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool Cache_IsValid(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern bool Cache_IsReady(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern bool Cache_IsReadonly(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern string Cache_GetPath(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int Cache_GetIndex(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern long Cache_GetSpaceFree(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern long Cache_GetMaximumDiskSpaceAvailable(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern void Cache_SetMaximumDiskSpaceAvailable(int handle, long value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern long Cache_GetCachingDiskSpaceUsed(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern int Cache_GetExpirationDelay(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern void Cache_SetExpirationDelay(int handle, int value);

	/// <summary>
	///   <para>Removes all cached content in the cache that has been cached by the current application.</para>
	/// </summary>
	/// <param name="expiration">The number of seconds that AssetBundles may remain unused in the cache.</param>
	/// <returns>
	///   <para>Returns True when cache clearing succeeded.</para>
	/// </returns>
	public bool ClearCache()
	{
		return Cache_ClearCache(m_Handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern bool Cache_ClearCache(int handle);

	/// <summary>
	///   <para>Removes all cached content in the cache that has been cached by the current application.</para>
	/// </summary>
	/// <param name="expiration">The number of seconds that AssetBundles may remain unused in the cache.</param>
	/// <returns>
	///   <para>Returns True when cache clearing succeeded.</para>
	/// </returns>
	public bool ClearCache(int expiration)
	{
		return Cache_ClearCache_Expiration(m_Handle, expiration);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern bool Cache_ClearCache_Expiration(int handle, int expiration);
}

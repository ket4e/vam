using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The Caching class lets you manage cached AssetBundles, downloaded using UnityWebRequestAssetBundle.GetAssetBundle().</para>
/// </summary>
[NativeHeader("Runtime/Misc/CachingManager.h")]
[StaticAccessor("GetCachingManager()", StaticAccessorType.Dot)]
public sealed class Caching
{
	[Obsolete("this API is not for public use.")]
	public static extern CacheIndex[] index
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Controls compression of cache data. Enabled by default.</para>
	/// </summary>
	public static extern bool compressionEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns true if Caching system is ready for use.</para>
	/// </summary>
	public static extern bool ready
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetIsReady")]
		get;
	}

	[Obsolete("Please use use Cache.spaceOccupied to get used bytes per cache.")]
	public static int spaceUsed => (int)spaceOccupied;

	[Obsolete("This property is only used for the current cache, use Cache.spaceOccupied to get used bytes per cache.")]
	public static extern long spaceOccupied
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
		[NativeName("GetCachingDiskSpaceUsed")]
		get;
	}

	[Obsolete("Please use use Cache.spaceOccupied to get used bytes per cache.")]
	public static int spaceAvailable => (int)spaceFree;

	[Obsolete("This property is only used for the current cache, use Cache.spaceFree to get unused bytes per cache.")]
	public static extern long spaceFree
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
		[NativeName("GetCachingDiskSpaceFree")]
		get;
	}

	[Obsolete("This property is only used for the current cache, use Cache.maximumAvailableStorageSpace to access the maximum available storage space per cache.")]
	[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
	public static extern long maximumAvailableDiskSpace
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetMaximumDiskSpaceAvailable")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetMaximumDiskSpaceAvailable")]
		set;
	}

	[Obsolete("This property is only used for the current cache, use Cache.expirationDelay to access the expiration delay per cache.")]
	[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
	public static extern int expirationDelay
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns the cache count in the cache list.</para>
	/// </summary>
	public static extern int cacheCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Returns the default cache which is added by Unity internally.</para>
	/// </summary>
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	public static Cache defaultCache
	{
		[NativeName("Caching_GetDefaultCacheHandle")]
		get
		{
			get_defaultCache_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Gets or sets the current cache in which AssetBundles should be cached.</para>
	/// </summary>
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	public static Cache currentCacheForWriting
	{
		[NativeName("Caching_GetCurrentCacheHandle")]
		get
		{
			get_currentCacheForWriting_Injected(out var ret);
			return ret;
		}
		[NativeName("Caching_SetCurrentCacheByHandle")]
		[NativeThrows]
		set
		{
			set_currentCacheForWriting_Injected(ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern Hash128[] GetCachedVersions(string assetBundleName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void GetCachedVersionsInternal(string assetBundleName, object cachedVersions);

	/// <summary>
	///   <para>Removes all AssetBundle content that has been cached by the current application.</para>
	/// </summary>
	/// <param name="expiration">The number of seconds that AssetBundles may remain unused in the cache.</param>
	/// <returns>
	///   <para>True when cache clearing succeeded, false if cache was in use.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool ClearCache();

	/// <summary>
	///   <para>Removes all AssetBundle content that has been cached by the current application.</para>
	/// </summary>
	/// <param name="expiration">The number of seconds that AssetBundles may remain unused in the cache.</param>
	/// <returns>
	///   <para>True when cache clearing succeeded, false if cache was in use.</para>
	/// </returns>
	public static bool ClearCache(int expiration)
	{
		return ClearCache_Int(expiration);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ClearCache")]
	internal static extern bool ClearCache_Int(int expiration);

	/// <summary>
	///   <para>Removes the given version of the AssetBundle.</para>
	/// </summary>
	/// <param name="assetBundleName">The AssetBundle name.</param>
	/// <param name="hash">Version needs to be cleaned.</param>
	/// <returns>
	///   <para>Returns true when cache clearing succeeded.  Can return false if any cached bundle is in use.</para>
	/// </returns>
	public static bool ClearCachedVersion(string assetBundleName, Hash128 hash)
	{
		if (string.IsNullOrEmpty(assetBundleName))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return ClearCachedVersionInternal(assetBundleName, hash);
	}

	[NativeName("ClearCachedVersion")]
	internal static bool ClearCachedVersionInternal(string assetBundleName, Hash128 hash)
	{
		return ClearCachedVersionInternal_Injected(assetBundleName, ref hash);
	}

	/// <summary>
	///   <para>Removes all the cached versions of the AssetBundle from the cache, except for the specified version.</para>
	/// </summary>
	/// <param name="assetBundleName">The AssetBundle name.</param>
	/// <param name="hash">Version needs to be kept.</param>
	/// <returns>
	///   <para>Returns true when cache clearing succeeded.</para>
	/// </returns>
	public static bool ClearOtherCachedVersions(string assetBundleName, Hash128 hash)
	{
		if (string.IsNullOrEmpty(assetBundleName))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return ClearCachedVersions(assetBundleName, hash, keepInputVersion: true);
	}

	/// <summary>
	///   <para>Removes all the cached versions of the given AssetBundle from the cache.</para>
	/// </summary>
	/// <param name="assetBundleName">The AssetBundle name.</param>
	/// <returns>
	///   <para>Returns true when cache clearing succeeded.</para>
	/// </returns>
	public static bool ClearAllCachedVersions(string assetBundleName)
	{
		if (string.IsNullOrEmpty(assetBundleName))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return ClearCachedVersions(assetBundleName, default(Hash128), keepInputVersion: false);
	}

	internal static bool ClearCachedVersions(string assetBundleName, Hash128 hash, bool keepInputVersion)
	{
		return ClearCachedVersions_Injected(assetBundleName, ref hash, keepInputVersion);
	}

	public static void GetCachedVersions(string assetBundleName, List<Hash128> outCachedVersions)
	{
		if (string.IsNullOrEmpty(assetBundleName))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		if (outCachedVersions == null)
		{
			throw new ArgumentNullException("Input outCachedVersions cannot be null.");
		}
		GetCachedVersionsInternal(assetBundleName, outCachedVersions);
	}

	/// <summary>
	///   <para>Checks if an AssetBundle is cached.</para>
	/// </summary>
	/// <param name="string">Url The filename of the AssetBundle. Domain and path information are stripped from this string automatically.</param>
	/// <param name="int">Version The version number of the AssetBundle to check for. Negative values are not allowed.</param>
	/// <param name="url"></param>
	/// <param name="version"></param>
	/// <returns>
	///   <para>True if an AssetBundle matching the url and version parameters has previously been loaded using UnityWebRequestAssetBundle.GetAssetBundle() and is currently stored in the cache. Returns false if the AssetBundle is not in cache, either because it has been flushed from the cache or was never loaded using the Caching API.</para>
	/// </returns>
	[Obsolete("Please use IsVersionCached with Hash128 instead.")]
	public static bool IsVersionCached(string url, int version)
	{
		return IsVersionCached(url, new Hash128(0u, 0u, 0u, (uint)version));
	}

	public static bool IsVersionCached(string url, Hash128 hash)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentException("Input AssetBundle url cannot be null or empty.");
		}
		return IsVersionCached(url, "", hash);
	}

	public static bool IsVersionCached(CachedAssetBundle cachedBundle)
	{
		if (string.IsNullOrEmpty(cachedBundle.name))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return IsVersionCached("", cachedBundle.name, cachedBundle.hash);
	}

	[NativeName("IsCached")]
	internal static bool IsVersionCached(string url, string assetBundleName, Hash128 hash)
	{
		return IsVersionCached_Injected(url, assetBundleName, ref hash);
	}

	/// <summary>
	///   <para>Bumps the timestamp of a cached file to be the current time.</para>
	/// </summary>
	/// <param name="url"></param>
	/// <param name="version"></param>
	[Obsolete("Please use MarkAsUsed with Hash128 instead.")]
	public static bool MarkAsUsed(string url, int version)
	{
		return MarkAsUsed(url, new Hash128(0u, 0u, 0u, (uint)version));
	}

	public static bool MarkAsUsed(string url, Hash128 hash)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentException("Input AssetBundle url cannot be null or empty.");
		}
		return MarkAsUsed(url, "", hash);
	}

	public static bool MarkAsUsed(CachedAssetBundle cachedBundle)
	{
		if (string.IsNullOrEmpty(cachedBundle.name))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return MarkAsUsed("", cachedBundle.name, cachedBundle.hash);
	}

	internal static bool MarkAsUsed(string url, string assetBundleName, Hash128 hash)
	{
		return MarkAsUsed_Injected(url, assetBundleName, ref hash);
	}

	[Obsolete("This function is obsolete and will always return -1. Use IsVersionCached instead.")]
	public static int GetVersionFromCache(string url)
	{
		return -1;
	}

	/// <summary>
	///   <para>Add a cache with the given path.</para>
	/// </summary>
	/// <param name="cachePath">Path to the cache folder.</param>
	public static Cache AddCache(string cachePath)
	{
		if (string.IsNullOrEmpty(cachePath))
		{
			throw new ArgumentNullException("Cache path cannot be null or empty.");
		}
		bool isReadonly = false;
		if (cachePath.Replace('\\', '/').StartsWith(Application.streamingAssetsPath))
		{
			isReadonly = true;
		}
		else
		{
			if (!Directory.Exists(cachePath))
			{
				throw new ArgumentException("Cache path '" + cachePath + "' doesn't exist.");
			}
			if ((File.GetAttributes(cachePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				isReadonly = true;
			}
		}
		if (GetCacheByPath(cachePath).valid)
		{
			throw new InvalidOperationException("Cache with path '" + cachePath + "' has already been added.");
		}
		return AddCache(cachePath, isReadonly);
	}

	[NativeName("AddCachePath")]
	internal static Cache AddCache(string cachePath, bool isReadonly)
	{
		AddCache_Injected(cachePath, isReadonly, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Returns the Cache at the given position in the cache list.</para>
	/// </summary>
	/// <param name="cacheIndex">Index of the cache to get.</param>
	/// <returns>
	///   <para>A reference to the Cache at the index specified.</para>
	/// </returns>
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	[NativeName("Caching_GetCacheHandleAt")]
	[NativeThrows]
	public static Cache GetCacheAt(int cacheIndex)
	{
		GetCacheAt_Injected(cacheIndex, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Returns the Cache that has the given cache path.</para>
	/// </summary>
	/// <param name="cachePath">The cache path.</param>
	/// <returns>
	///   <para>A reference to the Cache with the given path.</para>
	/// </returns>
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	[NativeName("Caching_GetCacheHandleByPath")]
	[NativeThrows]
	public static Cache GetCacheByPath(string cachePath)
	{
		GetCacheByPath_Injected(cachePath, out var ret);
		return ret;
	}

	public static void GetAllCachePaths(List<string> cachePaths)
	{
		cachePaths.Clear();
		for (int i = 0; i < cacheCount; i++)
		{
			cachePaths.Add(GetCacheAt(i).path);
		}
	}

	/// <summary>
	///   <para>Removes the Cache from cache list.</para>
	/// </summary>
	/// <param name="cache">The Cache to be removed.</param>
	/// <returns>
	///   <para>Returns true if the Cache is removed.</para>
	/// </returns>
	[NativeThrows]
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	[NativeName("Caching_RemoveCacheByHandle")]
	public static bool RemoveCache(Cache cache)
	{
		return RemoveCache_Injected(ref cache);
	}

	/// <summary>
	///   <para>Moves the source Cache before the destination Cache in the cache list.</para>
	/// </summary>
	/// <param name="src">The Cache to move.</param>
	/// <param name="dst">The Cache which should come after the source Cache in the cache list.</param>
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	[NativeName("Caching_MoveCacheBeforeByHandle")]
	[NativeThrows]
	public static void MoveCacheBefore(Cache src, Cache dst)
	{
		MoveCacheBefore_Injected(ref src, ref dst);
	}

	/// <summary>
	///   <para>Moves the source Cache after the destination Cache in the cache list.</para>
	/// </summary>
	/// <param name="src">The Cache to move.</param>
	/// <param name="dst">The Cache which should come before the source Cache in the cache list.</param>
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	[NativeName("Caching_MoveCacheAfterByHandle")]
	[NativeThrows]
	public static void MoveCacheAfter(Cache src, Cache dst)
	{
		MoveCacheAfter_Injected(ref src, ref dst);
	}

	[Obsolete("This function is obsolete. Please use ClearCache.  (UnityUpgradable) -> ClearCache()")]
	public static bool CleanCache()
	{
		return ClearCache();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ClearCachedVersionInternal_Injected(string assetBundleName, ref Hash128 hash);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ClearCachedVersions_Injected(string assetBundleName, ref Hash128 hash, bool keepInputVersion);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsVersionCached_Injected(string url, string assetBundleName, ref Hash128 hash);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool MarkAsUsed_Injected(string url, string assetBundleName, ref Hash128 hash);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddCache_Injected(string cachePath, bool isReadonly, out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCacheAt_Injected(int cacheIndex, out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCacheByPath_Injected(string cachePath, out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveCache_Injected(ref Cache cache);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveCacheBefore_Injected(ref Cache src, ref Cache dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveCacheAfter_Injected(ref Cache src, ref Cache dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_defaultCache_Injected(out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_currentCacheForWriting_Injected(out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_currentCacheForWriting_Injected(ref Cache value);
}

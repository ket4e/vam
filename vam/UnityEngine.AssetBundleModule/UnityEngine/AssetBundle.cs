using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

/// <summary>
///   <para>AssetBundles let you stream additional assets via the UnityWebRequest class and instantiate them at runtime. AssetBundles are created via BuildPipeline.BuildAssetBundle.</para>
/// </summary>
[ExcludeFromPreset]
public sealed class AssetBundle : Object
{
	/// <summary>
	///   <para>Main asset that was supplied when building the asset bundle (Read Only).</para>
	/// </summary>
	public extern Object mainAsset
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Return true if the AssetBundle is a streamed scene AssetBundle.</para>
	/// </summary>
	public extern bool isStreamedSceneAssetBundle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	private AssetBundle()
	{
	}

	/// <summary>
	///   <para>Unloads all currently loaded Asset Bundles.</para>
	/// </summary>
	/// <param name="unloadAllObjects">Determines whether the current instances of objects loaded from Asset Bundles will also be unloaded.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void UnloadAllAssetBundles(bool unloadAllObjects);

	/// <summary>
	///   <para>To use when you need to get a list of all the currently loaded Asset Bundles.</para>
	/// </summary>
	/// <returns>
	///   <para>Returns IEnumerable&lt;AssetBundle&gt; of all currently loaded Asset Bundles.</para>
	/// </returns>
	public static IEnumerable<AssetBundle> GetAllLoadedAssetBundles()
	{
		return GetAllLoadedAssetBundles_Internal();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern AssetBundle[] GetAllLoadedAssetBundles_Internal();

	/// <summary>
	///   <para>Asynchronously loads an AssetBundle from a file on disk.</para>
	/// </summary>
	/// <param name="path">Path of the file on disk.</param>
	/// <param name="crc">An optional CRC-32 checksum of the uncompressed content. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match.</param>
	/// <param name="offset">An optional byte offset. This value specifies where to start reading the AssetBundle from.</param>
	/// <returns>
	///   <para>Asynchronous create request for an AssetBundle. Use AssetBundleCreateRequest.assetBundle property to get an AssetBundle once it is loaded.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern AssetBundleCreateRequest LoadFromFileAsync(string path, [DefaultValue("0")] uint crc, [DefaultValue("0")] ulong offset);

	[ExcludeFromDocs]
	public static AssetBundleCreateRequest LoadFromFileAsync(string path, uint crc)
	{
		ulong offset = 0uL;
		return LoadFromFileAsync(path, crc, offset);
	}

	[ExcludeFromDocs]
	public static AssetBundleCreateRequest LoadFromFileAsync(string path)
	{
		ulong offset = 0uL;
		uint crc = 0u;
		return LoadFromFileAsync(path, crc, offset);
	}

	/// <summary>
	///   <para>Synchronously loads an AssetBundle from a file on disk.</para>
	/// </summary>
	/// <param name="path">Path of the file on disk.</param>
	/// <param name="crc">An optional CRC-32 checksum of the uncompressed content. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match.</param>
	/// <param name="offset">An optional byte offset. This value specifies where to start reading the AssetBundle from.</param>
	/// <returns>
	///   <para>Loaded AssetBundle object or null if failed.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern AssetBundle LoadFromFile(string path, [DefaultValue("0")] uint crc, [DefaultValue("0")] ulong offset);

	[ExcludeFromDocs]
	public static AssetBundle LoadFromFile(string path, uint crc)
	{
		ulong offset = 0uL;
		return LoadFromFile(path, crc, offset);
	}

	[ExcludeFromDocs]
	public static AssetBundle LoadFromFile(string path)
	{
		ulong offset = 0uL;
		uint crc = 0u;
		return LoadFromFile(path, crc, offset);
	}

	/// <summary>
	///   <para>Asynchronously create an AssetBundle from a memory region.</para>
	/// </summary>
	/// <param name="binary">Array of bytes with the AssetBundle data.</param>
	/// <param name="crc">An optional CRC-32 checksum of the uncompressed content. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match.</param>
	/// <returns>
	///   <para>Asynchronous create request for an AssetBundle. Use AssetBundleCreateRequest.assetBundle property to get an AssetBundle once it is loaded.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary, [DefaultValue("0")] uint crc);

	[ExcludeFromDocs]
	public static AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary)
	{
		uint crc = 0u;
		return LoadFromMemoryAsync(binary, crc);
	}

	/// <summary>
	///   <para>Synchronously create an AssetBundle from a memory region.</para>
	/// </summary>
	/// <param name="binary">Array of bytes with the AssetBundle data.</param>
	/// <param name="crc">An optional CRC-32 checksum of the uncompressed content. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match.</param>
	/// <returns>
	///   <para>Loaded AssetBundle object or null if failed.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern AssetBundle LoadFromMemory(byte[] binary, [DefaultValue("0")] uint crc);

	[ExcludeFromDocs]
	public static AssetBundle LoadFromMemory(byte[] binary)
	{
		uint crc = 0u;
		return LoadFromMemory(binary, crc);
	}

	internal static void ValidateLoadFromStream(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("ManagedStream object must be non-null", "stream");
		}
		if (!stream.CanRead)
		{
			throw new ArgumentException("ManagedStream object must be readable (stream.CanRead must return true)", "stream");
		}
		if (!stream.CanSeek)
		{
			throw new ArgumentException("ManagedStream object must be seekable (stream.CanSeek must return true)", "stream");
		}
	}

	[ExcludeFromDocs]
	public static AssetBundleCreateRequest LoadFromStreamAsync(Stream stream, uint crc)
	{
		uint managedReadBufferSize = 0u;
		return LoadFromStreamAsync(stream, crc, managedReadBufferSize);
	}

	[ExcludeFromDocs]
	public static AssetBundleCreateRequest LoadFromStreamAsync(Stream stream)
	{
		uint managedReadBufferSize = 0u;
		uint crc = 0u;
		return LoadFromStreamAsync(stream, crc, managedReadBufferSize);
	}

	/// <summary>
	///   <para>Asynchronously loads an AssetBundle from a managed Stream.</para>
	/// </summary>
	/// <param name="stream">The managed Stream object. Unity calls Read(), Seek() and the Length property on this object to load the AssetBundle data.</param>
	/// <param name="crc">An optional CRC-32 checksum of the uncompressed content.</param>
	/// <param name="managedReadBufferSize">An optional overide for the size of the read buffer size used whilst loading data. The default size is 32KB.</param>
	/// <returns>
	///   <para>Asynchronous create request for an AssetBundle. Use AssetBundleCreateRequest.assetBundle property to get an AssetBundle once it is loaded.</para>
	/// </returns>
	public static AssetBundleCreateRequest LoadFromStreamAsync(Stream stream, [DefaultValue("0")] uint crc, [DefaultValue("0")] uint managedReadBufferSize)
	{
		ValidateLoadFromStream(stream);
		return LoadFromStreamAsyncInternal(stream, crc, managedReadBufferSize);
	}

	[ExcludeFromDocs]
	public static AssetBundle LoadFromStream(Stream stream, uint crc)
	{
		uint managedReadBufferSize = 0u;
		return LoadFromStream(stream, crc, managedReadBufferSize);
	}

	[ExcludeFromDocs]
	public static AssetBundle LoadFromStream(Stream stream)
	{
		uint managedReadBufferSize = 0u;
		uint crc = 0u;
		return LoadFromStream(stream, crc, managedReadBufferSize);
	}

	/// <summary>
	///   <para>Synchronously loads an AssetBundle from a managed Stream.</para>
	/// </summary>
	/// <param name="stream">The managed Stream object. Unity calls Read(), Seek() and the Length property on this object to load the AssetBundle data.</param>
	/// <param name="crc">An optional CRC-32 checksum of the uncompressed content.</param>
	/// <param name="managedReadBufferSize">An optional overide for the size of the read buffer size used whilst loading data. The default size is 32KB.</param>
	/// <returns>
	///   <para>The loaded AssetBundle object or null when the object fails to load.</para>
	/// </returns>
	public static AssetBundle LoadFromStream(Stream stream, [DefaultValue("0")] uint crc, [DefaultValue("0")] uint managedReadBufferSize)
	{
		ValidateLoadFromStream(stream);
		return LoadFromStreamInternal(stream, crc, managedReadBufferSize);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern AssetBundleCreateRequest LoadFromStreamAsyncInternal(Stream stream, uint crc, [DefaultValue("0")] uint managedReadBufferSize);

	[ExcludeFromDocs]
	internal static AssetBundleCreateRequest LoadFromStreamAsyncInternal(Stream stream, uint crc)
	{
		uint managedReadBufferSize = 0u;
		return LoadFromStreamAsyncInternal(stream, crc, managedReadBufferSize);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern AssetBundle LoadFromStreamInternal(Stream stream, uint crc, [DefaultValue("0")] uint managedReadBufferSize);

	[ExcludeFromDocs]
	internal static AssetBundle LoadFromStreamInternal(Stream stream, uint crc)
	{
		uint managedReadBufferSize = 0u;
		return LoadFromStreamInternal(stream, crc, managedReadBufferSize);
	}

	/// <summary>
	///   <para>Check if an AssetBundle contains a specific object.</para>
	/// </summary>
	/// <param name="name"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool Contains(string name);

	[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
	public Object Load(string name)
	{
		return null;
	}

	[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
	public T Load<T>(string name) where T : Object
	{
		return (T)null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
	[GeneratedByOldBindingsGenerator]
	public extern Object Load(string name, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("Method LoadAsync has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAssetAsync instead and check the documentation for details.", true)]
	[GeneratedByOldBindingsGenerator]
	public extern AssetBundleRequest LoadAsync(string name, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
	[GeneratedByOldBindingsGenerator]
	public extern Object[] LoadAll(Type type);

	[Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
	public Object[] LoadAll()
	{
		return null;
	}

	[Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
	public T[] LoadAll<T>() where T : Object
	{
		return null;
	}

	/// <summary>
	///   <para>Loads asset with name of type T from the bundle.</para>
	/// </summary>
	/// <param name="name"></param>
	public Object LoadAsset(string name)
	{
		return LoadAsset(name, typeof(Object));
	}

	public T LoadAsset<T>(string name) where T : Object
	{
		return (T)LoadAsset(name, typeof(T));
	}

	/// <summary>
	///   <para>Loads asset with name of a given type from the bundle.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
	public Object LoadAsset(string name, Type type)
	{
		if (name == null)
		{
			throw new NullReferenceException("The input asset name cannot be null.");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("The input asset name cannot be empty.");
		}
		if (type == null)
		{
			throw new NullReferenceException("The input type cannot be null.");
		}
		return LoadAsset_Internal(name, type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
	private extern Object LoadAsset_Internal(string name, Type type);

	/// <summary>
	///   <para>Asynchronously loads asset with name of a given T from the bundle.</para>
	/// </summary>
	/// <param name="name"></param>
	public AssetBundleRequest LoadAssetAsync(string name)
	{
		return LoadAssetAsync(name, typeof(Object));
	}

	public AssetBundleRequest LoadAssetAsync<T>(string name)
	{
		return LoadAssetAsync(name, typeof(T));
	}

	/// <summary>
	///   <para>Asynchronously loads asset with name of a given type from the bundle.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	public AssetBundleRequest LoadAssetAsync(string name, Type type)
	{
		if (name == null)
		{
			throw new NullReferenceException("The input asset name cannot be null.");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("The input asset name cannot be empty.");
		}
		if (type == null)
		{
			throw new NullReferenceException("The input type cannot be null.");
		}
		return LoadAssetAsync_Internal(name, type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern AssetBundleRequest LoadAssetAsync_Internal(string name, Type type);

	/// <summary>
	///   <para>Loads asset and sub assets with name from the bundle.</para>
	/// </summary>
	/// <param name="name"></param>
	public Object[] LoadAssetWithSubAssets(string name)
	{
		return LoadAssetWithSubAssets(name, typeof(Object));
	}

	internal static T[] ConvertObjects<T>(Object[] rawObjects) where T : Object
	{
		if (rawObjects == null)
		{
			return null;
		}
		T[] array = new T[rawObjects.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (T)rawObjects[i];
		}
		return array;
	}

	public T[] LoadAssetWithSubAssets<T>(string name) where T : Object
	{
		return ConvertObjects<T>(LoadAssetWithSubAssets(name, typeof(T)));
	}

	/// <summary>
	///   <para>Loads asset and sub assets with name of a given type from the bundle.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	public Object[] LoadAssetWithSubAssets(string name, Type type)
	{
		if (name == null)
		{
			throw new NullReferenceException("The input asset name cannot be null.");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("The input asset name cannot be empty.");
		}
		if (type == null)
		{
			throw new NullReferenceException("The input type cannot be null.");
		}
		return LoadAssetWithSubAssets_Internal(name, type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern Object[] LoadAssetWithSubAssets_Internal(string name, Type type);

	/// <summary>
	///   <para>Loads asset with sub assets with name of type T from the bundle asynchronously.</para>
	/// </summary>
	/// <param name="name"></param>
	public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name)
	{
		return LoadAssetWithSubAssetsAsync(name, typeof(Object));
	}

	public AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name)
	{
		return LoadAssetWithSubAssetsAsync(name, typeof(T));
	}

	/// <summary>
	///   <para>Loads asset with sub assets with name of a given type from the bundle asynchronously.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, Type type)
	{
		if (name == null)
		{
			throw new NullReferenceException("The input asset name cannot be null.");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("The input asset name cannot be empty.");
		}
		if (type == null)
		{
			throw new NullReferenceException("The input type cannot be null.");
		}
		return LoadAssetWithSubAssetsAsync_Internal(name, type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern AssetBundleRequest LoadAssetWithSubAssetsAsync_Internal(string name, Type type);

	/// <summary>
	///   <para>Loads all assets contained in the asset bundle that inherit from type T.</para>
	/// </summary>
	public Object[] LoadAllAssets()
	{
		return LoadAllAssets(typeof(Object));
	}

	public T[] LoadAllAssets<T>() where T : Object
	{
		return ConvertObjects<T>(LoadAllAssets(typeof(T)));
	}

	/// <summary>
	///   <para>Loads all assets contained in the asset bundle that inherit from type.</para>
	/// </summary>
	/// <param name="type"></param>
	public Object[] LoadAllAssets(Type type)
	{
		if (type == null)
		{
			throw new NullReferenceException("The input type cannot be null.");
		}
		return LoadAssetWithSubAssets_Internal("", type);
	}

	/// <summary>
	///   <para>Loads all assets contained in the asset bundle that inherit from T asynchronously.</para>
	/// </summary>
	public AssetBundleRequest LoadAllAssetsAsync()
	{
		return LoadAllAssetsAsync(typeof(Object));
	}

	public AssetBundleRequest LoadAllAssetsAsync<T>()
	{
		return LoadAllAssetsAsync(typeof(T));
	}

	/// <summary>
	///   <para>Loads all assets contained in the asset bundle that inherit from type asynchronously.</para>
	/// </summary>
	/// <param name="type"></param>
	public AssetBundleRequest LoadAllAssetsAsync(Type type)
	{
		if (type == null)
		{
			throw new NullReferenceException("The input type cannot be null.");
		}
		return LoadAssetWithSubAssetsAsync_Internal("", type);
	}

	/// <summary>
	///   <para>Unloads all assets in the bundle.</para>
	/// </summary>
	/// <param name="unloadAllLoadedObjects"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Unload(bool unloadAllLoadedObjects);

	[Obsolete("This method is deprecated. Use GetAllAssetNames() instead.")]
	public string[] AllAssetNames()
	{
		return GetAllAssetNames();
	}

	/// <summary>
	///   <para>Return all asset names in the AssetBundle.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern string[] GetAllAssetNames();

	/// <summary>
	///   <para>Return all the scene asset paths (paths to *.unity assets) in the AssetBundle.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern string[] GetAllScenePaths();
}

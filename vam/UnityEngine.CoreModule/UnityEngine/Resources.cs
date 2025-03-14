using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

/// <summary>
///   <para>The Resources class allows you to find and access Objects including assets.</para>
/// </summary>
public sealed class Resources
{
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

	/// <summary>
	///   <para>Returns a list of all objects of Type type.</para>
	/// </summary>
	/// <param name="type">Type of the class to match while searching.</param>
	/// <returns>
	///   <para>An array of objects whose class is type or is derived from type.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
	[GeneratedByOldBindingsGenerator]
	public static extern Object[] FindObjectsOfTypeAll(Type type);

	public static T[] FindObjectsOfTypeAll<T>() where T : Object
	{
		return ConvertObjects<T>(FindObjectsOfTypeAll(typeof(T)));
	}

	/// <summary>
	///   <para>Loads an asset stored at path in a Resources folder.</para>
	/// </summary>
	/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
	public static Object Load(string path)
	{
		return Load(path, typeof(Object));
	}

	public static T Load<T>(string path) where T : Object
	{
		return (T)Load(path, typeof(T));
	}

	/// <summary>
	///   <para>Loads an asset stored at path in a Resources folder.</para>
	/// </summary>
	/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
	/// <param name="systemTypeInstance">Type filter for objects returned.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
	[GeneratedByOldBindingsGenerator]
	public static extern Object Load(string path, Type systemTypeInstance);

	/// <summary>
	///   <para>Asynchronously loads an asset stored at path in a Resources folder.</para>
	/// </summary>
	/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
	/// <param name="systemTypeInstance">Type filter for objects returned.</param>
	/// <param name="type"></param>
	public static ResourceRequest LoadAsync(string path)
	{
		return LoadAsync(path, typeof(Object));
	}

	public static ResourceRequest LoadAsync<T>(string path) where T : Object
	{
		return LoadAsync(path, typeof(T));
	}

	/// <summary>
	///   <para>Asynchronously loads an asset stored at path in a Resources folder.</para>
	/// </summary>
	/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
	/// <param name="systemTypeInstance">Type filter for objects returned.</param>
	/// <param name="type"></param>
	public static ResourceRequest LoadAsync(string path, Type type)
	{
		ResourceRequest resourceRequest = LoadAsyncInternal(path, type);
		resourceRequest.m_Path = path;
		resourceRequest.m_Type = type;
		return resourceRequest;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern ResourceRequest LoadAsyncInternal(string path, Type type);

	/// <summary>
	///   <para>Loads all assets in a folder or file at path in a Resources folder.</para>
	/// </summary>
	/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
	/// <param name="systemTypeInstance">Type filter for objects returned.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern Object[] LoadAll(string path, Type systemTypeInstance);

	/// <summary>
	///   <para>Loads all assets in a folder or file at path in a Resources folder.</para>
	/// </summary>
	/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
	public static Object[] LoadAll(string path)
	{
		return LoadAll(path, typeof(Object));
	}

	public static T[] LoadAll<T>(string path) where T : Object
	{
		return ConvertObjects<T>(LoadAll(path, typeof(T)));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	[GeneratedByOldBindingsGenerator]
	public static extern Object GetBuiltinResource(Type type, string path);

	public static T GetBuiltinResource<T>(string path) where T : Object
	{
		return (T)GetBuiltinResource(typeof(T), path);
	}

	/// <summary>
	///   <para>Unloads assetToUnload from memory.</para>
	/// </summary>
	/// <param name="assetToUnload"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void UnloadAsset(Object assetToUnload);

	/// <summary>
	///   <para>Unloads assets that are not used.</para>
	/// </summary>
	/// <returns>
	///   <para>Object on which you can yield to wait until the operation completes.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern AsyncOperation UnloadUnusedAssets();
}

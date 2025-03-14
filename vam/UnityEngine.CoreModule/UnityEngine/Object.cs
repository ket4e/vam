using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

/// <summary>
///   <para>Base class for all objects Unity can reference.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
[GenerateManagedProxy]
[NativeHeader("Runtime/Export/UnityEngineObject.bindings.h")]
[NativeHeader("Runtime/GameCode/CloneObject.h")]
public class Object
{
	private IntPtr m_CachedPtr;

	internal static int OffsetOfInstanceIDInCPlusPlusObject = -1;

	/// <summary>
	///   <para>The name of the object.</para>
	/// </summary>
	public string name
	{
		get
		{
			return GetName(this);
		}
		set
		{
			SetName(this, value);
		}
	}

	/// <summary>
	///   <para>Should the object be hidden, saved with the scene or modifiable by the user?</para>
	/// </summary>
	public extern HideFlags hideFlags
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns the instance id of the object.</para>
	/// </summary>
	[SecuritySafeCritical]
	public unsafe int GetInstanceID()
	{
		if (m_CachedPtr == IntPtr.Zero)
		{
			return 0;
		}
		if (OffsetOfInstanceIDInCPlusPlusObject == -1)
		{
			OffsetOfInstanceIDInCPlusPlusObject = GetOffsetOfInstanceIDInCPlusPlusObject();
		}
		return *(int*)(void*)new IntPtr(m_CachedPtr.ToInt64() + OffsetOfInstanceIDInCPlusPlusObject);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object other)
	{
		Object @object = other as Object;
		if (@object == null && other != null && !(other is Object))
		{
			return false;
		}
		return CompareBaseObjects(this, @object);
	}

	public static implicit operator bool(Object exists)
	{
		return !CompareBaseObjects(exists, null);
	}

	private static bool CompareBaseObjects(Object lhs, Object rhs)
	{
		bool flag = (object)lhs == null;
		bool flag2 = (object)rhs == null;
		if (flag2 && flag)
		{
			return true;
		}
		if (flag2)
		{
			return !IsNativeObjectAlive(lhs);
		}
		if (flag)
		{
			return !IsNativeObjectAlive(rhs);
		}
		return object.ReferenceEquals(lhs, rhs);
	}

	private void EnsureRunningOnMainThread()
	{
		if (!CurrentThreadIsMainThread())
		{
			throw new InvalidOperationException("EnsureRunningOnMainThread can only be called from the main thread");
		}
	}

	private static bool IsNativeObjectAlive(Object o)
	{
		return o.GetCachedPtr() != IntPtr.Zero;
	}

	private IntPtr GetCachedPtr()
	{
		return m_CachedPtr;
	}

	/// <summary>
	///   <para>Clones the object original and returns the clone.</para>
	/// </summary>
	/// <param name="original">An existing object that you want to make a copy of.</param>
	/// <param name="position">Position for the new object.</param>
	/// <param name="rotation">Orientation of the new object.</param>
	/// <param name="parent">Parent that will be assigned to the new object.</param>
	/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
	/// <returns>
	///   <para>The instantiated clone.</para>
	/// </returns>
	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original, Vector3 position, Quaternion rotation)
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		if (original is ScriptableObject)
		{
			throw new ArgumentException("Cannot instantiate a ScriptableObject with a position and rotation");
		}
		return Internal_InstantiateSingle(original, position, rotation);
	}

	/// <summary>
	///   <para>Clones the object original and returns the clone.</para>
	/// </summary>
	/// <param name="original">An existing object that you want to make a copy of.</param>
	/// <param name="position">Position for the new object.</param>
	/// <param name="rotation">Orientation of the new object.</param>
	/// <param name="parent">Parent that will be assigned to the new object.</param>
	/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
	/// <returns>
	///   <para>The instantiated clone.</para>
	/// </returns>
	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent)
	{
		if (parent == null)
		{
			return Internal_InstantiateSingle(original, position, rotation);
		}
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		return Internal_InstantiateSingleWithParent(original, parent, position, rotation);
	}

	/// <summary>
	///   <para>Clones the object original and returns the clone.</para>
	/// </summary>
	/// <param name="original">An existing object that you want to make a copy of.</param>
	/// <param name="position">Position for the new object.</param>
	/// <param name="rotation">Orientation of the new object.</param>
	/// <param name="parent">Parent that will be assigned to the new object.</param>
	/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
	/// <returns>
	///   <para>The instantiated clone.</para>
	/// </returns>
	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original)
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		return Internal_CloneSingle(original);
	}

	/// <summary>
	///   <para>Clones the object original and returns the clone.</para>
	/// </summary>
	/// <param name="original">An existing object that you want to make a copy of.</param>
	/// <param name="position">Position for the new object.</param>
	/// <param name="rotation">Orientation of the new object.</param>
	/// <param name="parent">Parent that will be assigned to the new object.</param>
	/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
	/// <returns>
	///   <para>The instantiated clone.</para>
	/// </returns>
	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original, Transform parent)
	{
		return Instantiate(original, parent, instantiateInWorldSpace: false);
	}

	/// <summary>
	///   <para>Clones the object original and returns the clone.</para>
	/// </summary>
	/// <param name="original">An existing object that you want to make a copy of.</param>
	/// <param name="position">Position for the new object.</param>
	/// <param name="rotation">Orientation of the new object.</param>
	/// <param name="parent">Parent that will be assigned to the new object.</param>
	/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
	/// <returns>
	///   <para>The instantiated clone.</para>
	/// </returns>
	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original, Transform parent, bool instantiateInWorldSpace)
	{
		if (parent == null)
		{
			return Internal_CloneSingle(original);
		}
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		return Internal_CloneSingleWithParent(original, parent, instantiateInWorldSpace);
	}

	public static T Instantiate<T>(T original) where T : Object
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		return (T)Internal_CloneSingle(original);
	}

	public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
	{
		return (T)Instantiate((Object)original, position, rotation);
	}

	public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
	{
		return (T)Instantiate((Object)original, position, rotation, parent);
	}

	public static T Instantiate<T>(T original, Transform parent) where T : Object
	{
		return Instantiate(original, parent, worldPositionStays: false);
	}

	public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
	{
		return (T)Instantiate((Object)original, parent, worldPositionStays);
	}

	/// <summary>
	///   <para>Removes a gameobject, component or asset.</para>
	/// </summary>
	/// <param name="obj">The object to destroy.</param>
	/// <param name="t">The optional amount of time to delay before destroying the object.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::DestroyObjectFromScripting")]
	public static extern void Destroy(Object obj, [DefaultValue("0.0F")] float t);

	/// <summary>
	///   <para>Removes a gameobject, component or asset.</para>
	/// </summary>
	/// <param name="obj">The object to destroy.</param>
	/// <param name="t">The optional amount of time to delay before destroying the object.</param>
	[ExcludeFromDocs]
	public static void Destroy(Object obj)
	{
		float t = 0f;
		Destroy(obj, t);
	}

	/// <summary>
	///   <para>Destroys the object obj immediately. You are strongly recommended to use Destroy instead.</para>
	/// </summary>
	/// <param name="obj">Object to be destroyed.</param>
	/// <param name="allowDestroyingAssets">Set to true to allow assets to be destroyed.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::DestroyObjectFromScriptingImmediate")]
	public static extern void DestroyImmediate(Object obj, [DefaultValue("false")] bool allowDestroyingAssets);

	/// <summary>
	///   <para>Destroys the object obj immediately. You are strongly recommended to use Destroy instead.</para>
	/// </summary>
	/// <param name="obj">Object to be destroyed.</param>
	/// <param name="allowDestroyingAssets">Set to true to allow assets to be destroyed.</param>
	[ExcludeFromDocs]
	public static void DestroyImmediate(Object obj)
	{
		bool allowDestroyingAssets = false;
		DestroyImmediate(obj, allowDestroyingAssets);
	}

	/// <summary>
	///   <para>Returns a list of all active loaded objects of Type type.</para>
	/// </summary>
	/// <param name="type">The type of object to find.</param>
	/// <returns>
	///   <para>The array of objects found matching the type specified.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
	[FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
	public static extern Object[] FindObjectsOfType(Type type);

	/// <summary>
	///   <para>Makes the object target not be destroyed automatically when loading a new scene.</para>
	/// </summary>
	/// <param name="target">The object which is not destroyed on scene change.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetSceneManager().DontDestroyOnLoad")]
	public static extern void DontDestroyOnLoad(Object target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("use Object.Destroy instead.")]
	[FreeFunction("Scripting::DestroyObjectFromScripting")]
	public static extern void DestroyObject(Object obj, [DefaultValue("0.0F")] float t);

	[Obsolete("use Object.Destroy instead.")]
	[ExcludeFromDocs]
	public static void DestroyObject(Object obj)
	{
		float t = 0f;
		DestroyObject(obj, t);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("warning use Object.FindObjectsOfType instead.")]
	[FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
	public static extern Object[] FindSceneObjectsOfType(Type type);

	/// <summary>
	///   <para>Returns a list of all active and inactive loaded objects of Type type, including assets.</para>
	/// </summary>
	/// <param name="type">The type of object or asset to find.</param>
	/// <returns>
	///   <para>The array of objects and assets found matching the type specified.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
	[FreeFunction("UnityEngineObjectBindings::FindObjectsOfTypeIncludingAssets")]
	public static extern Object[] FindObjectsOfTypeIncludingAssets(Type type);

	public static T[] FindObjectsOfType<T>() where T : Object
	{
		return Resources.ConvertObjects<T>(FindObjectsOfType(typeof(T)));
	}

	public static T FindObjectOfType<T>() where T : Object
	{
		return (T)FindObjectOfType(typeof(T));
	}

	/// <summary>
	///   <para>Returns a list of all active and inactive loaded objects of Type type.</para>
	/// </summary>
	/// <param name="type">The type of object to find.</param>
	/// <returns>
	///   <para>The array of objects found matching the type specified.</para>
	/// </returns>
	[Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
	public static Object[] FindObjectsOfTypeAll(Type type)
	{
		return Resources.FindObjectsOfTypeAll(type);
	}

	private static void CheckNullArgument(object arg, string message)
	{
		if (arg == null)
		{
			throw new ArgumentException(message);
		}
	}

	/// <summary>
	///   <para>Returns the first active loaded object of Type type.</para>
	/// </summary>
	/// <param name="type">The type of object to find.</param>
	/// <returns>
	///   <para>This returns the  Object that matches the specified type. It returns null if no Object matches the type.</para>
	/// </returns>
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public static Object FindObjectOfType(Type type)
	{
		Object[] array = FindObjectsOfType(type);
		if (array.Length > 0)
		{
			return array[0];
		}
		return null;
	}

	/// <summary>
	///   <para>Returns the name of the GameObject.</para>
	/// </summary>
	/// <returns>
	///   <para>The name returned by ToString.</para>
	/// </returns>
	public override string ToString()
	{
		return ToString(this);
	}

	public static bool operator ==(Object x, Object y)
	{
		return CompareBaseObjects(x, y);
	}

	public static bool operator !=(Object x, Object y)
	{
		return !CompareBaseObjects(x, y);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "Object::GetOffsetOfInstanceIdMember", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern int GetOffsetOfInstanceIDInCPlusPlusObject();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "Thread::CurrentThreadIsMainThread", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern bool CurrentThreadIsMainThread();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CloneObject")]
	private static extern Object Internal_CloneSingle(Object data);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CloneObject")]
	private static extern Object Internal_CloneSingleWithParent(Object data, Transform parent, bool worldPositionStays);

	[FreeFunction("InstantiateObject")]
	private static Object Internal_InstantiateSingle(Object data, Vector3 pos, Quaternion rot)
	{
		return Internal_InstantiateSingle_Injected(data, ref pos, ref rot);
	}

	[FreeFunction("InstantiateObject")]
	private static Object Internal_InstantiateSingleWithParent(Object data, Transform parent, Vector3 pos, Quaternion rot)
	{
		return Internal_InstantiateSingleWithParent_Injected(data, parent, ref pos, ref rot);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("UnityEngineObjectBindings::ToString")]
	private static extern string ToString(Object obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("UnityEngineObjectBindings::GetName")]
	private static extern string GetName(Object obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("UnityEngineObjectBindings::SetName")]
	private static extern void SetName(Object obj, string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "UnityEngineObjectBindings::DoesObjectWithInstanceIDExist", IsFreeFunction = true, IsThreadSafe = true)]
	internal static extern bool DoesObjectWithInstanceIDExist(int instanceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules]
	[FreeFunction("UnityEngineObjectBindings::FindObjectFromInstanceID")]
	internal static extern Object FindObjectFromInstanceID(int instanceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Object Internal_InstantiateSingle_Injected(Object data, ref Vector3 pos, ref Quaternion rot);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Object Internal_InstantiateSingleWithParent_Injected(Object data, Transform parent, ref Vector3 pos, ref Quaternion rot);
}

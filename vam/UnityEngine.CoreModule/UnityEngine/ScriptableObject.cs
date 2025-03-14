using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A class you can derive from if you want to create objects that don't need to be attached to game objects.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
[NativeClass(null)]
[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
public class ScriptableObject : Object
{
	public ScriptableObject()
	{
		CreateScriptableObject(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("ENABLE_MONO")]
	[Obsolete("Use EditorUtility.SetDirty instead")]
	public extern void SetDirty();

	/// <summary>
	///   <para>Creates an instance of a scriptable object.</para>
	/// </summary>
	/// <param name="className">The type of the ScriptableObject to create, as the name of the type.</param>
	/// <param name="type">The type of the ScriptableObject to create, as a System.Type instance.</param>
	/// <returns>
	///   <para>The created ScriptableObject.</para>
	/// </returns>
	public static ScriptableObject CreateInstance(string className)
	{
		return CreateScriptableObjectInstanceFromName(className);
	}

	/// <summary>
	///   <para>Creates an instance of a scriptable object.</para>
	/// </summary>
	/// <param name="className">The type of the ScriptableObject to create, as the name of the type.</param>
	/// <param name="type">The type of the ScriptableObject to create, as a System.Type instance.</param>
	/// <returns>
	///   <para>The created ScriptableObject.</para>
	/// </returns>
	public static ScriptableObject CreateInstance(Type type)
	{
		return CreateScriptableObjectInstanceFromType(type);
	}

	public static T CreateInstance<T>() where T : ScriptableObject
	{
		return (T)CreateInstance(typeof(T));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private static extern void CreateScriptableObject([Writable] ScriptableObject self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::CreateScriptableObject")]
	private static extern ScriptableObject CreateScriptableObjectInstanceFromName(string className);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::CreateScriptableObjectWithType")]
	private static extern ScriptableObject CreateScriptableObjectInstanceFromType(Type type);
}

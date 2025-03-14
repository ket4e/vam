using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Shader scripts used for all rendering.</para>
/// </summary>
[NativeHeader("Runtime/Shaders/Shader.h")]
[NativeHeader("Runtime/Shaders/ShaderNameRegistry.h")]
[NativeHeader("Runtime/Shaders/GpuPrograms/ShaderVariantCollection.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Misc/ResourceManager.h")]
[NativeHeader("Runtime/Shaders/ComputeShader.h")]
public sealed class Shader : Object
{
	/// <summary>
	///   <para>Shader hardware tier classification for current device.</para>
	/// </summary>
	[Obsolete("Use Graphics.activeTier instead (UnityUpgradable) -> UnityEngine.Graphics.activeTier", false)]
	public static ShaderHardwareTier globalShaderHardwareTier
	{
		get
		{
			return (ShaderHardwareTier)Graphics.activeTier;
		}
		set
		{
			Graphics.activeTier = (GraphicsTier)value;
		}
	}

	/// <summary>
	///   <para>Shader LOD level for this shader.</para>
	/// </summary>
	[NativeProperty("MaximumShaderLOD")]
	public extern int maximumLOD
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Shader LOD level for all shaders.</para>
	/// </summary>
	[NativeProperty("GlobalMaximumShaderLOD")]
	public static extern int globalMaximumLOD
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Can this shader run on the end-users graphics card? (Read Only)</para>
	/// </summary>
	public extern bool isSupported
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsSupported")]
		get;
	}

	/// <summary>
	///   <para>Render pipeline currently in use.</para>
	/// </summary>
	public static extern string globalRenderPipeline
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Render queue of this shader. (Read Only)</para>
	/// </summary>
	public extern int renderQueue
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("ShaderScripting::GetRenderQueue", HasExplicitThis = true)]
		get;
	}

	internal extern DisableBatchingType disableBatching
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("ShaderScripting::GetDisableBatchingType", HasExplicitThis = true)]
		get;
	}

	private Shader()
	{
	}

	/// <summary>
	///   <para>Gets unique identifier for a shader property name.</para>
	/// </summary>
	/// <param name="name">Shader property name.</param>
	/// <returns>
	///   <para>Unique integer for the name.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern int PropertyToID(string name);

	/// <summary>
	///   <para>Finds a shader with the given name.</para>
	/// </summary>
	/// <param name="name"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetScriptMapper().FindShader")]
	public static extern Shader Find(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetBuiltinResource<Shader>")]
	internal static extern Shader FindBuiltin(string name);

	/// <summary>
	///   <para>Set a global shader keyword.</para>
	/// </summary>
	/// <param name="keyword"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::EnableKeyword")]
	public static extern void EnableKeyword(string keyword);

	/// <summary>
	///   <para>Unset a global shader keyword.</para>
	/// </summary>
	/// <param name="keyword"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::DisableKeyword")]
	public static extern void DisableKeyword(string keyword);

	/// <summary>
	///   <para>Is global shader keyword enabled?</para>
	/// </summary>
	/// <param name="keyword"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::IsKeywordEnabled")]
	public static extern bool IsKeywordEnabled(string keyword);

	/// <summary>
	///   <para>Fully load all shaders to prevent future performance hiccups.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void WarmupAllShaders();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::TagToID")]
	internal static extern int TagToID(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::IDToTag")]
	internal static extern string IDToTag(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::SetGlobalFloat")]
	private static extern void SetGlobalFloatImpl(int name, float value);

	[FreeFunction("ShaderScripting::SetGlobalVector")]
	private static void SetGlobalVectorImpl(int name, Vector4 value)
	{
		SetGlobalVectorImpl_Injected(name, ref value);
	}

	[FreeFunction("ShaderScripting::SetGlobalMatrix")]
	private static void SetGlobalMatrixImpl(int name, Matrix4x4 value)
	{
		SetGlobalMatrixImpl_Injected(name, ref value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::SetGlobalTexture")]
	private static extern void SetGlobalTextureImpl(int name, Texture value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::SetGlobalBuffer")]
	private static extern void SetGlobalBufferImpl(int name, ComputeBuffer value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalFloat")]
	private static extern float GetGlobalFloatImpl(int name);

	[FreeFunction("ShaderScripting::GetGlobalVector")]
	private static Vector4 GetGlobalVectorImpl(int name)
	{
		GetGlobalVectorImpl_Injected(name, out var ret);
		return ret;
	}

	[FreeFunction("ShaderScripting::GetGlobalMatrix")]
	private static Matrix4x4 GetGlobalMatrixImpl(int name)
	{
		GetGlobalMatrixImpl_Injected(name, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalTexture")]
	private static extern Texture GetGlobalTextureImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::SetGlobalFloatArray")]
	private static extern void SetGlobalFloatArrayImpl(int name, float[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::SetGlobalVectorArray")]
	private static extern void SetGlobalVectorArrayImpl(int name, Vector4[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::SetGlobalMatrixArray")]
	private static extern void SetGlobalMatrixArrayImpl(int name, Matrix4x4[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalFloatArray")]
	private static extern float[] GetGlobalFloatArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalVectorArray")]
	private static extern Vector4[] GetGlobalVectorArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalMatrixArray")]
	private static extern Matrix4x4[] GetGlobalMatrixArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalFloatArrayCount")]
	private static extern int GetGlobalFloatArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalVectorArrayCount")]
	private static extern int GetGlobalVectorArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalMatrixArrayCount")]
	private static extern int GetGlobalMatrixArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::ExtractGlobalFloatArray")]
	private static extern void ExtractGlobalFloatArrayImpl(int name, [Out] float[] val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::ExtractGlobalVectorArray")]
	private static extern void ExtractGlobalVectorArrayImpl(int name, [Out] Vector4[] val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::ExtractGlobalMatrixArray")]
	private static extern void ExtractGlobalMatrixArrayImpl(int name, [Out] Matrix4x4[] val);

	private static void SetGlobalFloatArray(int name, float[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetGlobalFloatArrayImpl(name, values, count);
	}

	private static void SetGlobalVectorArray(int name, Vector4[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetGlobalVectorArrayImpl(name, values, count);
	}

	private static void SetGlobalMatrixArray(int name, Matrix4x4[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetGlobalMatrixArrayImpl(name, values, count);
	}

	private static void ExtractGlobalFloatArray(int name, List<float> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int globalFloatArrayCountImpl = GetGlobalFloatArrayCountImpl(name);
		if (globalFloatArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, globalFloatArrayCountImpl);
			ExtractGlobalFloatArrayImpl(name, (float[])NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private static void ExtractGlobalVectorArray(int name, List<Vector4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int globalVectorArrayCountImpl = GetGlobalVectorArrayCountImpl(name);
		if (globalVectorArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, globalVectorArrayCountImpl);
			ExtractGlobalVectorArrayImpl(name, (Vector4[])NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private static void ExtractGlobalMatrixArray(int name, List<Matrix4x4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int globalMatrixArrayCountImpl = GetGlobalMatrixArrayCountImpl(name);
		if (globalMatrixArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, globalMatrixArrayCountImpl);
			ExtractGlobalMatrixArrayImpl(name, (Matrix4x4[])NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	/// <summary>
	///   <para>Sets a global float property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalFloat(string name, float value)
	{
		SetGlobalFloatImpl(PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a global float property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalFloat(int name, float value)
	{
		SetGlobalFloatImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a global int property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalInt(string name, int value)
	{
		SetGlobalFloatImpl(PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a global int property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalInt(int name, int value)
	{
		SetGlobalFloatImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a global vector property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalVector(string name, Vector4 value)
	{
		SetGlobalVectorImpl(PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a global vector property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalVector(int name, Vector4 value)
	{
		SetGlobalVectorImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a global color property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalColor(string name, Color value)
	{
		SetGlobalVectorImpl(PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a global color property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalColor(int name, Color value)
	{
		SetGlobalVectorImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a global matrix property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalMatrix(string name, Matrix4x4 value)
	{
		SetGlobalMatrixImpl(PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a global matrix property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalMatrix(int name, Matrix4x4 value)
	{
		SetGlobalMatrixImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a global texture property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalTexture(string name, Texture value)
	{
		SetGlobalTextureImpl(PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a global texture property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalTexture(int name, Texture value)
	{
		SetGlobalTextureImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a global compute buffer property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalBuffer(string name, ComputeBuffer value)
	{
		SetGlobalBufferImpl(PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a global compute buffer property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	public static void SetGlobalBuffer(int name, ComputeBuffer value)
	{
		SetGlobalBufferImpl(name, value);
	}

	public static void SetGlobalFloatArray(string name, List<float> values)
	{
		SetGlobalFloatArray(PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	public static void SetGlobalFloatArray(int name, List<float> values)
	{
		SetGlobalFloatArray(name, NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	/// <summary>
	///   <para>Sets a global float array property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="values"></param>
	public static void SetGlobalFloatArray(string name, float[] values)
	{
		SetGlobalFloatArray(PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Sets a global float array property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="values"></param>
	public static void SetGlobalFloatArray(int name, float[] values)
	{
		SetGlobalFloatArray(name, values, values.Length);
	}

	public static void SetGlobalVectorArray(string name, List<Vector4> values)
	{
		SetGlobalVectorArray(PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	public static void SetGlobalVectorArray(int name, List<Vector4> values)
	{
		SetGlobalVectorArray(name, NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	/// <summary>
	///   <para>Sets a global vector array property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="values"></param>
	public static void SetGlobalVectorArray(string name, Vector4[] values)
	{
		SetGlobalVectorArray(PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Sets a global vector array property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="values"></param>
	public static void SetGlobalVectorArray(int name, Vector4[] values)
	{
		SetGlobalVectorArray(name, values, values.Length);
	}

	public static void SetGlobalMatrixArray(string name, List<Matrix4x4> values)
	{
		SetGlobalMatrixArray(PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	public static void SetGlobalMatrixArray(int name, List<Matrix4x4> values)
	{
		SetGlobalMatrixArray(name, NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	/// <summary>
	///   <para>Sets a global matrix array property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="values"></param>
	public static void SetGlobalMatrixArray(string name, Matrix4x4[] values)
	{
		SetGlobalMatrixArray(PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Sets a global matrix array property for all shaders.</para>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="values"></param>
	public static void SetGlobalMatrixArray(int name, Matrix4x4[] values)
	{
		SetGlobalMatrixArray(name, values, values.Length);
	}

	/// <summary>
	///   <para>Gets a global float property for all shaders previously set using SetGlobalFloat.</para>
	/// </summary>
	/// <param name="name"></param>
	public static float GetGlobalFloat(string name)
	{
		return GetGlobalFloatImpl(PropertyToID(name));
	}

	/// <summary>
	///   <para>Gets a global float property for all shaders previously set using SetGlobalFloat.</para>
	/// </summary>
	/// <param name="name"></param>
	public static float GetGlobalFloat(int name)
	{
		return GetGlobalFloatImpl(name);
	}

	/// <summary>
	///   <para>Gets a global int property for all shaders previously set using SetGlobalInt.</para>
	/// </summary>
	/// <param name="name"></param>
	public static int GetGlobalInt(string name)
	{
		return (int)GetGlobalFloatImpl(PropertyToID(name));
	}

	/// <summary>
	///   <para>Gets a global int property for all shaders previously set using SetGlobalInt.</para>
	/// </summary>
	/// <param name="name"></param>
	public static int GetGlobalInt(int name)
	{
		return (int)GetGlobalFloatImpl(name);
	}

	/// <summary>
	///   <para>Gets a global vector property for all shaders previously set using SetGlobalVector.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Vector4 GetGlobalVector(string name)
	{
		return GetGlobalVectorImpl(PropertyToID(name));
	}

	/// <summary>
	///   <para>Gets a global vector property for all shaders previously set using SetGlobalVector.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Vector4 GetGlobalVector(int name)
	{
		return GetGlobalVectorImpl(name);
	}

	/// <summary>
	///   <para>Gets a global color property for all shaders previously set using SetGlobalColor.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Color GetGlobalColor(string name)
	{
		return GetGlobalVectorImpl(PropertyToID(name));
	}

	/// <summary>
	///   <para>Gets a global color property for all shaders previously set using SetGlobalColor.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Color GetGlobalColor(int name)
	{
		return GetGlobalVectorImpl(name);
	}

	/// <summary>
	///   <para>Gets a global matrix property for all shaders previously set using SetGlobalMatrix.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Matrix4x4 GetGlobalMatrix(string name)
	{
		return GetGlobalMatrixImpl(PropertyToID(name));
	}

	/// <summary>
	///   <para>Gets a global matrix property for all shaders previously set using SetGlobalMatrix.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Matrix4x4 GetGlobalMatrix(int name)
	{
		return GetGlobalMatrixImpl(name);
	}

	/// <summary>
	///   <para>Gets a global texture property for all shaders previously set using SetGlobalTexture.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Texture GetGlobalTexture(string name)
	{
		return GetGlobalTextureImpl(PropertyToID(name));
	}

	/// <summary>
	///   <para>Gets a global texture property for all shaders previously set using SetGlobalTexture.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Texture GetGlobalTexture(int name)
	{
		return GetGlobalTextureImpl(name);
	}

	/// <summary>
	///   <para>Gets a global float array for all shaders previously set using SetGlobalFloatArray.</para>
	/// </summary>
	/// <param name="name"></param>
	public static float[] GetGlobalFloatArray(string name)
	{
		return GetGlobalFloatArray(PropertyToID(name));
	}

	/// <summary>
	///   <para>Gets a global float array for all shaders previously set using SetGlobalFloatArray.</para>
	/// </summary>
	/// <param name="name"></param>
	public static float[] GetGlobalFloatArray(int name)
	{
		return (GetGlobalFloatArrayCountImpl(name) == 0) ? null : GetGlobalFloatArrayImpl(name);
	}

	/// <summary>
	///   <para>Gets a global vector array for all shaders previously set using SetGlobalVectorArray.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Vector4[] GetGlobalVectorArray(string name)
	{
		return GetGlobalVectorArray(PropertyToID(name));
	}

	/// <summary>
	///   <para>Gets a global vector array for all shaders previously set using SetGlobalVectorArray.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Vector4[] GetGlobalVectorArray(int name)
	{
		return (GetGlobalVectorArrayCountImpl(name) == 0) ? null : GetGlobalVectorArrayImpl(name);
	}

	/// <summary>
	///   <para>Gets a global matrix array for all shaders previously set using SetGlobalMatrixArray.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Matrix4x4[] GetGlobalMatrixArray(string name)
	{
		return GetGlobalMatrixArray(PropertyToID(name));
	}

	/// <summary>
	///   <para>Gets a global matrix array for all shaders previously set using SetGlobalMatrixArray.</para>
	/// </summary>
	/// <param name="name"></param>
	public static Matrix4x4[] GetGlobalMatrixArray(int name)
	{
		return (GetGlobalMatrixArrayCountImpl(name) == 0) ? null : GetGlobalMatrixArrayImpl(name);
	}

	public static void GetGlobalFloatArray(string name, List<float> values)
	{
		ExtractGlobalFloatArray(PropertyToID(name), values);
	}

	public static void GetGlobalFloatArray(int name, List<float> values)
	{
		ExtractGlobalFloatArray(name, values);
	}

	public static void GetGlobalVectorArray(string name, List<Vector4> values)
	{
		ExtractGlobalVectorArray(PropertyToID(name), values);
	}

	public static void GetGlobalVectorArray(int name, List<Vector4> values)
	{
		ExtractGlobalVectorArray(name, values);
	}

	public static void GetGlobalMatrixArray(string name, List<Matrix4x4> values)
	{
		ExtractGlobalMatrixArray(PropertyToID(name), values);
	}

	public static void GetGlobalMatrixArray(int name, List<Matrix4x4> values)
	{
		ExtractGlobalMatrixArray(name, values);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalVectorImpl_Injected(int name, ref Vector4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalMatrixImpl_Injected(int name, ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGlobalVectorImpl_Injected(int name, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGlobalMatrixImpl_Injected(int name, out Matrix4x4 ret);
}

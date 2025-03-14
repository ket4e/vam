using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

/// <summary>
///   <para>A block of material values to apply.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Shaders/ShaderPropertySheet.h")]
[NativeHeader("Runtime/Math/SphericalHarmonicsL2.h")]
[NativeHeader("Runtime/Shaders/ComputeShader.h")]
public sealed class MaterialPropertyBlock
{
	internal IntPtr m_Ptr;

	/// <summary>
	///   <para>Is the material property block empty? (Read Only)</para>
	/// </summary>
	public extern bool isEmpty
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("IsEmpty")]
		get;
	}

	public MaterialPropertyBlock()
	{
		m_Ptr = CreateImpl();
	}

	[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
	public void AddFloat(string name, float value)
	{
		SetFloat(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
	public void AddFloat(int nameID, float value)
	{
		SetFloat(nameID, value);
	}

	[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
	public void AddVector(string name, Vector4 value)
	{
		SetVector(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
	public void AddVector(int nameID, Vector4 value)
	{
		SetVector(nameID, value);
	}

	[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
	public void AddColor(string name, Color value)
	{
		SetColor(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
	public void AddColor(int nameID, Color value)
	{
		SetColor(nameID, value);
	}

	[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
	public void AddMatrix(string name, Matrix4x4 value)
	{
		SetMatrix(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
	public void AddMatrix(int nameID, Matrix4x4 value)
	{
		SetMatrix(nameID, value);
	}

	[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
	public void AddTexture(string name, Texture value)
	{
		SetTexture(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
	public void AddTexture(int nameID, Texture value)
	{
		SetTexture(nameID, value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetFloatFromScript")]
	private extern float GetFloatImpl(int name);

	[NativeName("GetVectorFromScript")]
	private Vector4 GetVectorImpl(int name)
	{
		GetVectorImpl_Injected(name, out var ret);
		return ret;
	}

	[NativeName("GetColorFromScript")]
	private Color GetColorImpl(int name)
	{
		GetColorImpl_Injected(name, out var ret);
		return ret;
	}

	[NativeName("GetMatrixFromScript")]
	private Matrix4x4 GetMatrixImpl(int name)
	{
		GetMatrixImpl_Injected(name, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetTextureFromScript")]
	private extern Texture GetTextureImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetFloatFromScript")]
	private extern void SetFloatImpl(int name, float value);

	[NativeName("SetVectorFromScript")]
	private void SetVectorImpl(int name, Vector4 value)
	{
		SetVectorImpl_Injected(name, ref value);
	}

	[NativeName("SetColorFromScript")]
	private void SetColorImpl(int name, Color value)
	{
		SetColorImpl_Injected(name, ref value);
	}

	[NativeName("SetMatrixFromScript")]
	private void SetMatrixImpl(int name, Matrix4x4 value)
	{
		SetMatrixImpl_Injected(name, ref value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetTextureFromScript")]
	private extern void SetTextureImpl(int name, [NotNull] Texture value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetBufferFromScript")]
	private extern void SetBufferImpl(int name, ComputeBuffer value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetFloatArrayFromScript")]
	private extern void SetFloatArrayImpl(int name, float[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetVectorArrayFromScript")]
	private extern void SetVectorArrayImpl(int name, Vector4[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetMatrixArrayFromScript")]
	private extern void SetMatrixArrayImpl(int name, Matrix4x4[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetFloatArrayFromScript")]
	private extern float[] GetFloatArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetVectorArrayFromScript")]
	private extern Vector4[] GetVectorArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetMatrixArrayFromScript")]
	private extern Matrix4x4[] GetMatrixArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetFloatArrayCountFromScript")]
	private extern int GetFloatArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetVectorArrayCountFromScript")]
	private extern int GetVectorArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetMatrixArrayCountFromScript")]
	private extern int GetMatrixArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ExtractFloatArrayFromScript")]
	private extern void ExtractFloatArrayImpl(int name, [Out] float[] val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ExtractVectorArrayFromScript")]
	private extern void ExtractVectorArrayImpl(int name, [Out] Vector4[] val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ExtractMatrixArrayFromScript")]
	private extern void ExtractMatrixArrayImpl(int name, [Out] Matrix4x4[] val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ConvertAndCopySHCoefficientArraysToPropertySheetFromScript")]
	internal static extern void Internal_CopySHCoefficientArraysFrom(MaterialPropertyBlock properties, SphericalHarmonicsL2[] lightProbes, int sourceStart, int destStart, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CopyProbeOcclusionArrayToPropertySheetFromScript")]
	internal static extern void Internal_CopyProbeOcclusionArrayFrom(MaterialPropertyBlock properties, Vector4[] occlusionProbes, int sourceStart, int destStart, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "MaterialPropertyBlockScripting::Create", IsFreeFunction = true)]
	private static extern IntPtr CreateImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "MaterialPropertyBlockScripting::Destroy", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern void DestroyImpl(IntPtr mpb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void Clear(bool keepMemory);

	/// <summary>
	///   <para>Clear material property values.</para>
	/// </summary>
	public void Clear()
	{
		Clear(keepMemory: true);
	}

	private void SetFloatArray(int name, float[] values, int count)
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
		SetFloatArrayImpl(name, values, count);
	}

	private void SetVectorArray(int name, Vector4[] values, int count)
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
		SetVectorArrayImpl(name, values, count);
	}

	private void SetMatrixArray(int name, Matrix4x4[] values, int count)
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
		SetMatrixArrayImpl(name, values, count);
	}

	private void ExtractFloatArray(int name, List<float> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int floatArrayCountImpl = GetFloatArrayCountImpl(name);
		if (floatArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, floatArrayCountImpl);
			ExtractFloatArrayImpl(name, (float[])NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private void ExtractVectorArray(int name, List<Vector4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int vectorArrayCountImpl = GetVectorArrayCountImpl(name);
		if (vectorArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, vectorArrayCountImpl);
			ExtractVectorArrayImpl(name, (Vector4[])NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private void ExtractMatrixArray(int name, List<Matrix4x4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int matrixArrayCountImpl = GetMatrixArrayCountImpl(name);
		if (matrixArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, matrixArrayCountImpl);
			ExtractMatrixArrayImpl(name, (Matrix4x4[])NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	~MaterialPropertyBlock()
	{
		Dispose();
	}

	private void Dispose()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			DestroyImpl(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///   <para>Set a float property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The float value to set.</param>
	public void SetFloat(string name, float value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Set a float property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The float value to set.</param>
	public void SetFloat(int name, float value)
	{
		SetFloatImpl(name, value);
	}

	/// <summary>
	///   <para>Set a vector property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The Vector4 value to set.</param>
	public void SetVector(string name, Vector4 value)
	{
		SetVectorImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Set a vector property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The Vector4 value to set.</param>
	public void SetVector(int name, Vector4 value)
	{
		SetVectorImpl(name, value);
	}

	/// <summary>
	///   <para>Set a color property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The Color value to set.</param>
	public void SetColor(string name, Color value)
	{
		SetColorImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Set a color property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The Color value to set.</param>
	public void SetColor(int name, Color value)
	{
		SetColorImpl(name, value);
	}

	/// <summary>
	///   <para>Set a matrix property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The matrix value to set.</param>
	public void SetMatrix(string name, Matrix4x4 value)
	{
		SetMatrixImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Set a matrix property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The matrix value to set.</param>
	public void SetMatrix(int name, Matrix4x4 value)
	{
		SetMatrixImpl(name, value);
	}

	/// <summary>
	///   <para>Set a ComputeBuffer property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The ComputeBuffer to set.</param>
	public void SetBuffer(string name, ComputeBuffer value)
	{
		SetBufferImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Set a ComputeBuffer property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The ComputeBuffer to set.</param>
	public void SetBuffer(int name, ComputeBuffer value)
	{
		SetBufferImpl(name, value);
	}

	/// <summary>
	///   <para>Set a texture property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The Texture to set.</param>
	public void SetTexture(string name, Texture value)
	{
		SetTextureImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Set a texture property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="value">The Texture to set.</param>
	public void SetTexture(int name, Texture value)
	{
		SetTextureImpl(name, value);
	}

	public void SetFloatArray(string name, List<float> values)
	{
		SetFloatArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	public void SetFloatArray(int name, List<float> values)
	{
		SetFloatArray(name, NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	/// <summary>
	///   <para>Set a float array property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="values">The array to set.</param>
	public void SetFloatArray(string name, float[] values)
	{
		SetFloatArray(Shader.PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Set a float array property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="values">The array to set.</param>
	public void SetFloatArray(int name, float[] values)
	{
		SetFloatArray(name, values, values.Length);
	}

	public void SetVectorArray(string name, List<Vector4> values)
	{
		SetVectorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	public void SetVectorArray(int name, List<Vector4> values)
	{
		SetVectorArray(name, NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	/// <summary>
	///   <para>Set a vector array property.</para>
	/// </summary>
	/// <param name="nameID">The name of the property.</param>
	/// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The array to set.</param>
	public void SetVectorArray(string name, Vector4[] values)
	{
		SetVectorArray(Shader.PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Set a vector array property.</para>
	/// </summary>
	/// <param name="nameID">The name of the property.</param>
	/// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The array to set.</param>
	public void SetVectorArray(int name, Vector4[] values)
	{
		SetVectorArray(name, values, values.Length);
	}

	public void SetMatrixArray(string name, List<Matrix4x4> values)
	{
		SetMatrixArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	public void SetMatrixArray(int name, List<Matrix4x4> values)
	{
		SetMatrixArray(name, NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	/// <summary>
	///   <para>Set a matrix array property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="nameID">The array to set.</param>
	public void SetMatrixArray(string name, Matrix4x4[] values)
	{
		SetMatrixArray(Shader.PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Set a matrix array property.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="nameID">The array to set.</param>
	public void SetMatrixArray(int name, Matrix4x4[] values)
	{
		SetMatrixArray(name, values, values.Length);
	}

	/// <summary>
	///   <para>Get a float from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public float GetFloat(string name)
	{
		return GetFloatImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a float from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public float GetFloat(int name)
	{
		return GetFloatImpl(name);
	}

	/// <summary>
	///   <para>Get a vector from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Vector4 GetVector(string name)
	{
		return GetVectorImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a vector from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Vector4 GetVector(int name)
	{
		return GetVectorImpl(name);
	}

	/// <summary>
	///   <para>Get a color from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Color GetColor(string name)
	{
		return GetColorImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a color from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Color GetColor(int name)
	{
		return GetColorImpl(name);
	}

	/// <summary>
	///   <para>Get a matrix from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Matrix4x4 GetMatrix(string name)
	{
		return GetMatrixImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a matrix from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Matrix4x4 GetMatrix(int name)
	{
		return GetMatrixImpl(name);
	}

	/// <summary>
	///   <para>Get a texture from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Texture GetTexture(string name)
	{
		return GetTextureImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a texture from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Texture GetTexture(int name)
	{
		return GetTextureImpl(name);
	}

	/// <summary>
	///   <para>Get a float array from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public float[] GetFloatArray(string name)
	{
		return GetFloatArray(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a float array from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public float[] GetFloatArray(int name)
	{
		return (GetFloatArrayCountImpl(name) == 0) ? null : GetFloatArrayImpl(name);
	}

	/// <summary>
	///   <para>Get a vector array from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Vector4[] GetVectorArray(string name)
	{
		return GetVectorArray(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a vector array from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Vector4[] GetVectorArray(int name)
	{
		return (GetVectorArrayCountImpl(name) == 0) ? null : GetVectorArrayImpl(name);
	}

	/// <summary>
	///   <para>Get a matrix array from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Matrix4x4[] GetMatrixArray(string name)
	{
		return GetMatrixArray(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a matrix array from the property block.</para>
	/// </summary>
	/// <param name="name"></param>
	public Matrix4x4[] GetMatrixArray(int name)
	{
		return (GetMatrixArrayCountImpl(name) == 0) ? null : GetMatrixArrayImpl(name);
	}

	public void GetFloatArray(string name, List<float> values)
	{
		ExtractFloatArray(Shader.PropertyToID(name), values);
	}

	public void GetFloatArray(int name, List<float> values)
	{
		ExtractFloatArray(name, values);
	}

	public void GetVectorArray(string name, List<Vector4> values)
	{
		ExtractVectorArray(Shader.PropertyToID(name), values);
	}

	public void GetVectorArray(int name, List<Vector4> values)
	{
		ExtractVectorArray(name, values);
	}

	public void GetMatrixArray(string name, List<Matrix4x4> values)
	{
		ExtractMatrixArray(Shader.PropertyToID(name), values);
	}

	public void GetMatrixArray(int name, List<Matrix4x4> values)
	{
		ExtractMatrixArray(name, values);
	}

	public void CopySHCoefficientArraysFrom(List<SphericalHarmonicsL2> lightProbes)
	{
		if (lightProbes == null)
		{
			throw new ArgumentNullException("lightProbes");
		}
		CopySHCoefficientArraysFrom(NoAllocHelpers.ExtractArrayFromListT(lightProbes), 0, 0, lightProbes.Count);
	}

	/// <summary>
	///   <para>This function converts and copies the entire source array into 7 Vector4 property arrays named unity_SHAr, unity_SHAg, unity_SHAb, unity_SHBr, unity_SHBg, unity_SHBb and unity_SHC for use with instanced rendering.</para>
	/// </summary>
	/// <param name="lightProbes">The array of SH values to copy from.</param>
	public void CopySHCoefficientArraysFrom(SphericalHarmonicsL2[] lightProbes)
	{
		if (lightProbes == null)
		{
			throw new ArgumentNullException("lightProbes");
		}
		CopySHCoefficientArraysFrom(lightProbes, 0, 0, lightProbes.Length);
	}

	public void CopySHCoefficientArraysFrom(List<SphericalHarmonicsL2> lightProbes, int sourceStart, int destStart, int count)
	{
		CopySHCoefficientArraysFrom(NoAllocHelpers.ExtractArrayFromListT(lightProbes), sourceStart, destStart, count);
	}

	/// <summary>
	///   <para>This function converts and copies the source array into 7 Vector4 property arrays named unity_SHAr, unity_SHAg, unity_SHAb, unity_SHBr, unity_SHBg, unity_SHBb and unity_SHC with the specified source and destination range for use with instanced rendering.</para>
	/// </summary>
	/// <param name="lightProbes">The array of SH values to copy from.</param>
	/// <param name="sourceStart">The index of the first element in the source array to copy from.</param>
	/// <param name="destStart">The index of the first element in the destination MaterialPropertyBlock array to copy to.</param>
	/// <param name="count">The number of elements to copy.</param>
	public void CopySHCoefficientArraysFrom(SphericalHarmonicsL2[] lightProbes, int sourceStart, int destStart, int count)
	{
		if (lightProbes == null)
		{
			throw new ArgumentNullException("lightProbes");
		}
		if (sourceStart < 0)
		{
			throw new ArgumentOutOfRangeException("sourceStart", "Argument sourceStart must not be negative.");
		}
		if (destStart < 0)
		{
			throw new ArgumentOutOfRangeException("sourceStart", "Argument destStart must not be negative.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Argument count must not be negative.");
		}
		if (lightProbes.Length < sourceStart + count)
		{
			throw new ArgumentOutOfRangeException("The specified source start index or count is out of the range.");
		}
		Internal_CopySHCoefficientArraysFrom(this, lightProbes, sourceStart, destStart, count);
	}

	public void CopyProbeOcclusionArrayFrom(List<Vector4> occlusionProbes)
	{
		if (occlusionProbes == null)
		{
			throw new ArgumentNullException("occlusionProbes");
		}
		CopyProbeOcclusionArrayFrom(NoAllocHelpers.ExtractArrayFromListT(occlusionProbes), 0, 0, occlusionProbes.Count);
	}

	/// <summary>
	///   <para>This function copies the entire source array into a Vector4 property array named unity_ProbesOcclusion for use with instanced rendering.</para>
	/// </summary>
	/// <param name="occlusionProbes">The array of probe occlusion values to copy from.</param>
	public void CopyProbeOcclusionArrayFrom(Vector4[] occlusionProbes)
	{
		if (occlusionProbes == null)
		{
			throw new ArgumentNullException("occlusionProbes");
		}
		CopyProbeOcclusionArrayFrom(occlusionProbes, 0, 0, occlusionProbes.Length);
	}

	public void CopyProbeOcclusionArrayFrom(List<Vector4> occlusionProbes, int sourceStart, int destStart, int count)
	{
		CopyProbeOcclusionArrayFrom(NoAllocHelpers.ExtractArrayFromListT(occlusionProbes), sourceStart, destStart, count);
	}

	/// <summary>
	///   <para>This function copies the source array into a Vector4 property array named unity_ProbesOcclusion with the specified source and destination range for use with instanced rendering.</para>
	/// </summary>
	/// <param name="occlusionProbes">The array of probe occlusion values to copy from.</param>
	/// <param name="sourceStart">The index of the first element in the source array to copy from.</param>
	/// <param name="destStart">The index of the first element in the destination MaterialPropertyBlock array to copy to.</param>
	/// <param name="count">The number of elements to copy.</param>
	public void CopyProbeOcclusionArrayFrom(Vector4[] occlusionProbes, int sourceStart, int destStart, int count)
	{
		if (occlusionProbes == null)
		{
			throw new ArgumentNullException("occlusionProbes");
		}
		if (sourceStart < 0)
		{
			throw new ArgumentOutOfRangeException("sourceStart", "Argument sourceStart must not be negative.");
		}
		if (destStart < 0)
		{
			throw new ArgumentOutOfRangeException("sourceStart", "Argument destStart must not be negative.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Argument count must not be negative.");
		}
		if (occlusionProbes.Length < sourceStart + count)
		{
			throw new ArgumentOutOfRangeException("The specified source start index or count is out of the range.");
		}
		Internal_CopyProbeOcclusionArrayFrom(this, occlusionProbes, sourceStart, destStart, count);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetVectorImpl_Injected(int name, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetColorImpl_Injected(int name, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetMatrixImpl_Injected(int name, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetVectorImpl_Injected(int name, ref Vector4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetColorImpl_Injected(int name, ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetMatrixImpl_Injected(int name, ref Matrix4x4 value);
}

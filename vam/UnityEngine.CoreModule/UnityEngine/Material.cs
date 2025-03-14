using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The material class.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Shaders/Material.h")]
public class Material : Object
{
	/// <summary>
	///   <para>Additional shader keywords set by this material.</para>
	/// </summary>
	public extern string[] shaderKeywords
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The shader used by the material.</para>
	/// </summary>
	public extern Shader shader
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The main material's color.</para>
	/// </summary>
	public Color color
	{
		get
		{
			return GetColor("_Color");
		}
		set
		{
			SetColor("_Color", value);
		}
	}

	/// <summary>
	///   <para>The material's texture.</para>
	/// </summary>
	public Texture mainTexture
	{
		get
		{
			return GetTexture("_MainTex");
		}
		set
		{
			SetTexture("_MainTex", value);
		}
	}

	/// <summary>
	///   <para>The texture offset of the main texture.</para>
	/// </summary>
	public Vector2 mainTextureOffset
	{
		get
		{
			return GetTextureOffset("_MainTex");
		}
		set
		{
			SetTextureOffset("_MainTex", value);
		}
	}

	/// <summary>
	///   <para>The texture scale of the main texture.</para>
	/// </summary>
	public Vector2 mainTextureScale
	{
		get
		{
			return GetTextureScale("_MainTex");
		}
		set
		{
			SetTextureScale("_MainTex", value);
		}
	}

	/// <summary>
	///   <para>Render queue of this material.</para>
	/// </summary>
	public extern int renderQueue
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetActualRenderQueue")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetCustomRenderQueue")]
		set;
	}

	/// <summary>
	///   <para>Defines how the material should interact with lightmaps and lightprobes.</para>
	/// </summary>
	public extern MaterialGlobalIlluminationFlags globalIlluminationFlags
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Gets and sets whether the Double Sided Global Illumination setting is enabled for this material.</para>
	/// </summary>
	public extern bool doubleSidedGI
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Gets and sets whether GPU instancing is enabled for this material.</para>
	/// </summary>
	[NativeProperty("EnableInstancingVariants")]
	public extern bool enableInstancing
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>How many passes are in this material (Read Only).</para>
	/// </summary>
	public extern int passCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Create a temporary Material.</para>
	/// </summary>
	/// <param name="shader">Create a material with a given Shader.</param>
	/// <param name="source">Create a material by copying all properties from another material.</param>
	public Material(Shader shader)
	{
		CreateWithShader(this, shader);
	}

	/// <summary>
	///   <para>Create a temporary Material.</para>
	/// </summary>
	/// <param name="shader">Create a material with a given Shader.</param>
	/// <param name="source">Create a material by copying all properties from another material.</param>
	public Material(Material source)
	{
		CreateWithMaterial(this, source);
	}

	/// <summary>
	///   <para></para>
	/// </summary>
	/// <param name="contents"></param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Creating materials from shader source string is no longer supported. Use Shader assets instead.", false)]
	public Material(string contents)
	{
		CreateWithString(this);
	}

	/// <summary>
	///   <para>Interpolate properties between two materials.</para>
	/// </summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <param name="t"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Lerp(Material start, Material end, float t);

	/// <summary>
	///   <para>Activate the given pass for rendering.</para>
	/// </summary>
	/// <param name="pass">Shader pass number to setup.</param>
	/// <returns>
	///   <para>If false is returned, no rendering should be done.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool SetPass(int pass);

	[Obsolete("Creating materials from shader source string will be removed in the future. Use Shader assets instead.")]
	public static Material Create(string scriptContents)
	{
		return new Material(scriptContents);
	}

	/// <summary>
	///   <para>Copy properties from other material into this material.</para>
	/// </summary>
	/// <param name="mat"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void CopyPropertiesFromMaterial(Material mat);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MaterialScripting::CreateWithShader")]
	private static extern void CreateWithShader([Writable] Material self, [NotNull] Shader shader);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MaterialScripting::CreateWithMaterial")]
	private static extern void CreateWithMaterial([Writable] Material self, [NotNull] Material source);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MaterialScripting::CreateWithString")]
	private static extern void CreateWithString([Writable] Material self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern Material GetDefaultMaterial();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern Material GetDefaultParticleMaterial();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern Material GetDefaultLineMaterial();

	/// <summary>
	///   <para>Checks if material's shader has a property of a given name.</para>
	/// </summary>
	/// <param name="name"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("HasPropertyFromScript")]
	public extern bool HasProperty(int name);

	/// <summary>
	///   <para>Checks if material's shader has a property of a given name.</para>
	/// </summary>
	/// <param name="name"></param>
	public bool HasProperty(string name)
	{
		return HasProperty(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Sets a shader keyword that is enabled by this material.</para>
	/// </summary>
	/// <param name="keyword"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void EnableKeyword(string keyword);

	/// <summary>
	///   <para>Unset a shader keyword.</para>
	/// </summary>
	/// <param name="keyword"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void DisableKeyword(string keyword);

	/// <summary>
	///   <para>Is the shader keyword enabled on this material?</para>
	/// </summary>
	/// <param name="keyword"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern bool IsKeywordEnabled(string keyword);

	/// <summary>
	///   <para>Enables or disables a Shader pass on a per-Material level.</para>
	/// </summary>
	/// <param name="passName">Shader pass name (case insensitive).</param>
	/// <param name="enabled">Flag indicating whether this Shader pass should be enabled.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MaterialScripting::SetShaderPassEnabled", HasExplicitThis = true)]
	public extern void SetShaderPassEnabled(string passName, bool enabled);

	/// <summary>
	///   <para>Checks whether a given Shader pass is enabled on this Material.</para>
	/// </summary>
	/// <param name="passName">Shader pass name (case insensitive).</param>
	/// <returns>
	///   <para>True if the Shader pass is enabled.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MaterialScripting::GetShaderPassEnabled", HasExplicitThis = true)]
	public extern bool GetShaderPassEnabled(string passName);

	/// <summary>
	///   <para>Returns the name of the shader pass at index pass.</para>
	/// </summary>
	/// <param name="pass"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern string GetPassName(int pass);

	/// <summary>
	///   <para>Returns the index of the pass passName.</para>
	/// </summary>
	/// <param name="passName"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern int FindPass(string passName);

	/// <summary>
	///   <para>Sets an override tag/value on the material.</para>
	/// </summary>
	/// <param name="tag">Name of the tag to set.</param>
	/// <param name="val">Name of the value to set. Empty string to clear the override flag.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetOverrideTag(string tag, string val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetTag")]
	private extern string GetTagImpl(string tag, bool currentSubShaderOnly, string defaultValue);

	/// <summary>
	///   <para>Get the value of material's shader tag.</para>
	/// </summary>
	/// <param name="tag"></param>
	/// <param name="searchFallbacks"></param>
	/// <param name="defaultValue"></param>
	public string GetTag(string tag, bool searchFallbacks, string defaultValue)
	{
		return GetTagImpl(tag, !searchFallbacks, defaultValue);
	}

	/// <summary>
	///   <para>Get the value of material's shader tag.</para>
	/// </summary>
	/// <param name="tag"></param>
	/// <param name="searchFallbacks"></param>
	/// <param name="defaultValue"></param>
	public string GetTag(string tag, bool searchFallbacks)
	{
		return GetTagImpl(tag, !searchFallbacks, "");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetFloatFromScript")]
	private extern void SetFloatImpl(int name, float value);

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
	private extern void SetTextureImpl(int name, Texture value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetBufferFromScript")]
	private extern void SetBufferImpl(int name, ComputeBuffer value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetFloatFromScript")]
	private extern float GetFloatImpl(int name);

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
	[FreeFunction(Name = "MaterialScripting::SetFloatArray", HasExplicitThis = true)]
	private extern void SetFloatArrayImpl(int name, float[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::SetVectorArray", HasExplicitThis = true)]
	private extern void SetVectorArrayImpl(int name, Vector4[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::SetColorArray", HasExplicitThis = true)]
	private extern void SetColorArrayImpl(int name, Color[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::SetMatrixArray", HasExplicitThis = true)]
	private extern void SetMatrixArrayImpl(int name, Matrix4x4[] values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::GetFloatArray", HasExplicitThis = true)]
	private extern float[] GetFloatArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::GetVectorArray", HasExplicitThis = true)]
	private extern Vector4[] GetVectorArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::GetColorArray", HasExplicitThis = true)]
	private extern Color[] GetColorArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::GetMatrixArray", HasExplicitThis = true)]
	private extern Matrix4x4[] GetMatrixArrayImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::GetFloatArrayCount", HasExplicitThis = true)]
	private extern int GetFloatArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::GetVectorArrayCount", HasExplicitThis = true)]
	private extern int GetVectorArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::GetColorArrayCount", HasExplicitThis = true)]
	private extern int GetColorArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::GetMatrixArrayCount", HasExplicitThis = true)]
	private extern int GetMatrixArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::ExtractFloatArray", HasExplicitThis = true)]
	private extern void ExtractFloatArrayImpl(int name, [Out] float[] val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::ExtractVectorArray", HasExplicitThis = true)]
	private extern void ExtractVectorArrayImpl(int name, [Out] Vector4[] val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::ExtractColorArray", HasExplicitThis = true)]
	private extern void ExtractColorArrayImpl(int name, [Out] Color[] val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MaterialScripting::ExtractMatrixArray", HasExplicitThis = true)]
	private extern void ExtractMatrixArrayImpl(int name, [Out] Matrix4x4[] val);

	[NativeName("GetTextureScaleAndOffsetFromScript")]
	private Vector4 GetTextureScaleAndOffsetImpl(int name)
	{
		GetTextureScaleAndOffsetImpl_Injected(name, out var ret);
		return ret;
	}

	[NativeName("SetTextureOffsetFromScript")]
	private void SetTextureOffsetImpl(int name, Vector2 offset)
	{
		SetTextureOffsetImpl_Injected(name, ref offset);
	}

	[NativeName("SetTextureScaleFromScript")]
	private void SetTextureScaleImpl(int name, Vector2 scale)
	{
		SetTextureScaleImpl_Injected(name, ref scale);
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

	private void SetColorArray(int name, Color[] values, int count)
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
		SetColorArrayImpl(name, values, count);
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

	private void ExtractColorArray(int name, List<Color> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int colorArrayCountImpl = GetColorArrayCountImpl(name);
		if (colorArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, colorArrayCountImpl);
			ExtractColorArrayImpl(name, (Color[])NoAllocHelpers.ExtractArrayFromList(values));
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

	/// <summary>
	///   <para>Sets a named float value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="value">Float value to set.</param>
	/// <param name="name">Property name, e.g. "_Glossiness".</param>
	public void SetFloat(string name, float value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a named float value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="value">Float value to set.</param>
	/// <param name="name">Property name, e.g. "_Glossiness".</param>
	public void SetFloat(int name, float value)
	{
		SetFloatImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a named integer value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="value">Integer value to set.</param>
	/// <param name="name">Property name, e.g. "_SrcBlend".</param>
	public void SetInt(string name, int value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a named integer value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="value">Integer value to set.</param>
	/// <param name="name">Property name, e.g. "_SrcBlend".</param>
	public void SetInt(int name, int value)
	{
		SetFloatImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a named color value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_Color".</param>
	/// <param name="value">Color value to set.</param>
	public void SetColor(string name, Color value)
	{
		SetColorImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a named color value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_Color".</param>
	/// <param name="value">Color value to set.</param>
	public void SetColor(int name, Color value)
	{
		SetColorImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a named vector value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_WaveAndDistance".</param>
	/// <param name="value">Vector value to set.</param>
	public void SetVector(string name, Vector4 value)
	{
		SetColorImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a named vector value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_WaveAndDistance".</param>
	/// <param name="value">Vector value to set.</param>
	public void SetVector(int name, Vector4 value)
	{
		SetColorImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a named matrix for the shader.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_CubemapRotation".</param>
	/// <param name="value">Matrix value to set.</param>
	public void SetMatrix(string name, Matrix4x4 value)
	{
		SetMatrixImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a named matrix for the shader.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_CubemapRotation".</param>
	/// <param name="value">Matrix value to set.</param>
	public void SetMatrix(int name, Matrix4x4 value)
	{
		SetMatrixImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a named texture.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_MainTex".</param>
	/// <param name="value">Texture to set.</param>
	public void SetTexture(string name, Texture value)
	{
		SetTextureImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a named texture.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_MainTex".</param>
	/// <param name="value">Texture to set.</param>
	public void SetTexture(int name, Texture value)
	{
		SetTextureImpl(name, value);
	}

	/// <summary>
	///   <para>Sets a named ComputeBuffer value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name.</param>
	/// <param name="value">The ComputeBuffer value to set.</param>
	public void SetBuffer(string name, ComputeBuffer value)
	{
		SetBufferImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets a named ComputeBuffer value.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name.</param>
	/// <param name="value">The ComputeBuffer value to set.</param>
	public void SetBuffer(int name, ComputeBuffer value)
	{
		SetBufferImpl(name, value);
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
	///   <para>Sets a float array property.</para>
	/// </summary>
	/// <param name="name">Property name.</param>
	/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
	/// <param name="values">Array of values to set.</param>
	public void SetFloatArray(string name, float[] values)
	{
		SetFloatArray(Shader.PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Sets a float array property.</para>
	/// </summary>
	/// <param name="name">Property name.</param>
	/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
	/// <param name="values">Array of values to set.</param>
	public void SetFloatArray(int name, float[] values)
	{
		SetFloatArray(name, values, values.Length);
	}

	public void SetColorArray(string name, List<Color> values)
	{
		SetColorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	public void SetColorArray(int name, List<Color> values)
	{
		SetColorArray(name, NoAllocHelpers.ExtractArrayFromListT(values), values.Count);
	}

	/// <summary>
	///   <para>Sets a color array property.</para>
	/// </summary>
	/// <param name="name">Property name.</param>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="values">Array of values to set.</param>
	public void SetColorArray(string name, Color[] values)
	{
		SetColorArray(Shader.PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Sets a color array property.</para>
	/// </summary>
	/// <param name="name">Property name.</param>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="values">Array of values to set.</param>
	public void SetColorArray(int name, Color[] values)
	{
		SetColorArray(name, values, values.Length);
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
	///   <para>Sets a vector array property.</para>
	/// </summary>
	/// <param name="name">Property name.</param>
	/// <param name="values">Array of values to set.</param>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	public void SetVectorArray(string name, Vector4[] values)
	{
		SetVectorArray(Shader.PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Sets a vector array property.</para>
	/// </summary>
	/// <param name="name">Property name.</param>
	/// <param name="values">Array of values to set.</param>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
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
	///   <para>Sets a matrix array property.</para>
	/// </summary>
	/// <param name="name">Property name.</param>
	/// <param name="values">Array of values to set.</param>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	public void SetMatrixArray(string name, Matrix4x4[] values)
	{
		SetMatrixArray(Shader.PropertyToID(name), values, values.Length);
	}

	/// <summary>
	///   <para>Sets a matrix array property.</para>
	/// </summary>
	/// <param name="name">Property name.</param>
	/// <param name="values">Array of values to set.</param>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	public void SetMatrixArray(int name, Matrix4x4[] values)
	{
		SetMatrixArray(name, values, values.Length);
	}

	/// <summary>
	///   <para>Get a named float value.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public float GetFloat(string name)
	{
		return GetFloatImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named float value.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public float GetFloat(int name)
	{
		return GetFloatImpl(name);
	}

	/// <summary>
	///   <para>Get a named integer value.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public int GetInt(string name)
	{
		return (int)GetFloatImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named integer value.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public int GetInt(int name)
	{
		return (int)GetFloatImpl(name);
	}

	/// <summary>
	///   <para>Get a named color value.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Color GetColor(string name)
	{
		return GetColorImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named color value.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Color GetColor(int name)
	{
		return GetColorImpl(name);
	}

	/// <summary>
	///   <para>Get a named vector value.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Vector4 GetVector(string name)
	{
		return GetColorImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named vector value.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Vector4 GetVector(int name)
	{
		return GetColorImpl(name);
	}

	/// <summary>
	///   <para>Get a named matrix value from the shader.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Matrix4x4 GetMatrix(string name)
	{
		return GetMatrixImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named matrix value from the shader.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Matrix4x4 GetMatrix(int name)
	{
		return GetMatrixImpl(name);
	}

	/// <summary>
	///   <para>Get a named texture.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Texture GetTexture(string name)
	{
		return GetTextureImpl(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named texture.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Texture GetTexture(int name)
	{
		return GetTextureImpl(name);
	}

	/// <summary>
	///   <para>Get a named float array.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	public float[] GetFloatArray(string name)
	{
		return GetFloatArray(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named float array.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	public float[] GetFloatArray(int name)
	{
		return (GetFloatArrayCountImpl(name) == 0) ? null : GetFloatArrayImpl(name);
	}

	/// <summary>
	///   <para>Get a named color array.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Color[] GetColorArray(string name)
	{
		return GetColorArray(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named color array.</para>
	/// </summary>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	/// <param name="name">The name of the property.</param>
	public Color[] GetColorArray(int name)
	{
		return (GetColorArrayCountImpl(name) == 0) ? null : GetColorArrayImpl(name);
	}

	/// <summary>
	///   <para>Get a named vector array.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	public Vector4[] GetVectorArray(string name)
	{
		return GetVectorArray(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named vector array.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	public Vector4[] GetVectorArray(int name)
	{
		return (GetVectorArrayCountImpl(name) == 0) ? null : GetVectorArrayImpl(name);
	}

	/// <summary>
	///   <para>Get a named matrix array.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
	public Matrix4x4[] GetMatrixArray(string name)
	{
		return GetMatrixArray(Shader.PropertyToID(name));
	}

	/// <summary>
	///   <para>Get a named matrix array.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
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

	public void GetColorArray(string name, List<Color> values)
	{
		ExtractColorArray(Shader.PropertyToID(name), values);
	}

	public void GetColorArray(int name, List<Color> values)
	{
		ExtractColorArray(name, values);
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

	/// <summary>
	///   <para>Sets the placement offset of texture propertyName.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, for example: "_MainTex".</param>
	/// <param name="value">Texture placement offset.</param>
	public void SetTextureOffset(string name, Vector2 value)
	{
		SetTextureOffsetImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets the placement offset of texture propertyName.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, for example: "_MainTex".</param>
	/// <param name="value">Texture placement offset.</param>
	public void SetTextureOffset(int name, Vector2 value)
	{
		SetTextureOffsetImpl(name, value);
	}

	/// <summary>
	///   <para>Sets the placement scale of texture propertyName.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_MainTex".</param>
	/// <param name="value">Texture placement scale.</param>
	public void SetTextureScale(string name, Vector2 value)
	{
		SetTextureScaleImpl(Shader.PropertyToID(name), value);
	}

	/// <summary>
	///   <para>Sets the placement scale of texture propertyName.</para>
	/// </summary>
	/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
	/// <param name="name">Property name, e.g. "_MainTex".</param>
	/// <param name="value">Texture placement scale.</param>
	public void SetTextureScale(int name, Vector2 value)
	{
		SetTextureScaleImpl(name, value);
	}

	/// <summary>
	///   <para>Gets the placement offset of texture propertyName.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	public Vector2 GetTextureOffset(string name)
	{
		return GetTextureOffset(Shader.PropertyToID(name));
	}

	public Vector2 GetTextureOffset(int name)
	{
		Vector4 textureScaleAndOffsetImpl = GetTextureScaleAndOffsetImpl(name);
		return new Vector2(textureScaleAndOffsetImpl.z, textureScaleAndOffsetImpl.w);
	}

	/// <summary>
	///   <para>Gets the placement scale of texture propertyName.</para>
	/// </summary>
	/// <param name="name">The name of the property.</param>
	public Vector2 GetTextureScale(string name)
	{
		return GetTextureScale(Shader.PropertyToID(name));
	}

	public Vector2 GetTextureScale(int name)
	{
		Vector4 textureScaleAndOffsetImpl = GetTextureScaleAndOffsetImpl(name);
		return new Vector2(textureScaleAndOffsetImpl.x, textureScaleAndOffsetImpl.y);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetColorImpl_Injected(int name, ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetMatrixImpl_Injected(int name, ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetColorImpl_Injected(int name, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetMatrixImpl_Injected(int name, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetTextureScaleAndOffsetImpl_Injected(int name, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTextureOffsetImpl_Injected(int name, ref Vector2 offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTextureScaleImpl_Injected(int name, ref Vector2 scale);
}

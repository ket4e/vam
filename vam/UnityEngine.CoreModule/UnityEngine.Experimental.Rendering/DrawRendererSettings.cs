using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Settings for ScriptableRenderContext.DrawRenderers.</para>
/// </summary>
public struct DrawRendererSettings
{
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	[UnsafeValueType]
	[CompilerGenerated]
	public struct _003CshaderPassNames_003E__FixedBuffer0
	{
		public int FixedElementField;
	}

	private const int kMaxShaderPasses = 16;

	/// <summary>
	///   <para>The maxiumum number of passes that can be rendered in 1 DrawRenderers call.</para>
	/// </summary>
	public static readonly int maxShaderPasses = 16;

	/// <summary>
	///   <para>How to sort objects during rendering.</para>
	/// </summary>
	public DrawRendererSortSettings sorting;

	private _003CshaderPassNames_003E__FixedBuffer0 shaderPassNames;

	/// <summary>
	///   <para>What kind of per-object data to setup during rendering.</para>
	/// </summary>
	public RendererConfiguration rendererConfiguration;

	/// <summary>
	///   <para>Other flags controlling object rendering.</para>
	/// </summary>
	public DrawRendererFlags flags;

	private int m_OverrideMaterialInstanceId;

	private int m_OverrideMaterialPassIdx;

	/// <summary>
	///   <para>Create a draw settings struct.</para>
	/// </summary>
	/// <param name="camera">Camera to use. Camera's transparency sort mode is used to determine whether to use orthographic or distance based sorting.</param>
	/// <param name="shaderPassName">Shader pass to use.</param>
	public unsafe DrawRendererSettings(Camera camera, ShaderPassName shaderPassName)
	{
		rendererConfiguration = RendererConfiguration.None;
		flags = DrawRendererFlags.EnableInstancing;
		m_OverrideMaterialInstanceId = 0;
		m_OverrideMaterialPassIdx = 0;
		fixed (int* ptr = &shaderPassNames.FixedElementField)
		{
			for (int i = 0; i < maxShaderPasses; i++)
			{
				System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, i) = -1;
			}
		}
		fixed (int* ptr2 = &shaderPassNames.FixedElementField)
		{
			*ptr2 = shaderPassName.nameIndex;
		}
		rendererConfiguration = RendererConfiguration.None;
		flags = DrawRendererFlags.EnableInstancing;
		InitializeSortSettings(camera, out sorting);
	}

	/// <summary>
	///   <para>Set the Material to use for all drawers that would render in this group.</para>
	/// </summary>
	/// <param name="mat">Override material.</param>
	/// <param name="passIndex">Pass to use in the material.</param>
	public void SetOverrideMaterial(Material mat, int passIndex)
	{
		if (mat == null)
		{
			m_OverrideMaterialInstanceId = 0;
		}
		else
		{
			m_OverrideMaterialInstanceId = mat.GetInstanceID();
		}
		m_OverrideMaterialPassIdx = passIndex;
	}

	/// <summary>
	///   <para>Set the shader passes that this draw call can render.</para>
	/// </summary>
	/// <param name="index">Index of the shader pass to use.</param>
	/// <param name="shaderPassName">Name of the shader pass.</param>
	public unsafe void SetShaderPassName(int index, ShaderPassName shaderPassName)
	{
		if (index >= maxShaderPasses || index < 0)
		{
			throw new ArgumentOutOfRangeException("index", $"Index should range from 0 - DrawRendererSettings.maxShaderPasses ({maxShaderPasses}), was {index}");
		}
		fixed (int* ptr = &shaderPassNames.FixedElementField)
		{
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index) = shaderPassName.nameIndex;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void InitializeSortSettings(Camera camera, out DrawRendererSortSettings sortSettings);
}

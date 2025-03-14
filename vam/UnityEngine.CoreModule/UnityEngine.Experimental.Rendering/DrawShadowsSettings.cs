using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Settings for RenderLoop.DrawShadows.</para>
/// </summary>
[UsedByNativeCode]
public struct DrawShadowsSettings
{
	private IntPtr _cullResults;

	/// <summary>
	///   <para>The index of the shadow-casting light to be rendered.</para>
	/// </summary>
	public int lightIndex;

	/// <summary>
	///   <para>The split data.</para>
	/// </summary>
	public ShadowSplitData splitData;

	/// <summary>
	///   <para>Culling results to use.</para>
	/// </summary>
	public CullResults cullResults
	{
		set
		{
			_cullResults = value.cullResults;
		}
	}

	/// <summary>
	///   <para>Create a shadow settings object.</para>
	/// </summary>
	/// <param name="cullResults">The cull results for this light.</param>
	/// <param name="lightIndex">The light index.</param>
	public DrawShadowsSettings(CullResults cullResults, int lightIndex)
	{
		_cullResults = cullResults.cullResults;
		this.lightIndex = lightIndex;
		splitData.cullingPlaneCount = 0;
		splitData.cullingSphere = Vector4.zero;
	}
}

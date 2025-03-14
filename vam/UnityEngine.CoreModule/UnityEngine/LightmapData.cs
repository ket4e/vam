using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Data of a lightmap.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[UsedByNativeCode]
public sealed class LightmapData
{
	internal Texture2D m_Light;

	internal Texture2D m_Dir;

	internal Texture2D m_ShadowMask;

	[Obsolete("Use lightmapColor property (UnityUpgradable) -> lightmapColor", false)]
	public Texture2D lightmapLight
	{
		get
		{
			return m_Light;
		}
		set
		{
			m_Light = value;
		}
	}

	/// <summary>
	///   <para>Lightmap storing color of incoming light.</para>
	/// </summary>
	public Texture2D lightmapColor
	{
		get
		{
			return m_Light;
		}
		set
		{
			m_Light = value;
		}
	}

	/// <summary>
	///   <para>Lightmap storing dominant direction of incoming light.</para>
	/// </summary>
	public Texture2D lightmapDir
	{
		get
		{
			return m_Dir;
		}
		set
		{
			m_Dir = value;
		}
	}

	/// <summary>
	///   <para>Texture storing occlusion mask per light (ShadowMask, up to four lights).</para>
	/// </summary>
	public Texture2D shadowMask
	{
		get
		{
			return m_ShadowMask;
		}
		set
		{
			m_ShadowMask = value;
		}
	}
}

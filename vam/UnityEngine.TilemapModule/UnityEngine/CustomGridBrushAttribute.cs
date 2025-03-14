using System;

namespace UnityEngine;

/// <summary>
///   <para>Attribute to define the class as a grid brush and to make it available in the palette window.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CustomGridBrushAttribute : Attribute
{
	private bool m_HideAssetInstances;

	private bool m_HideDefaultInstance;

	private bool m_DefaultBrush;

	private string m_DefaultName;

	/// <summary>
	///   <para>Hide all asset instances of this brush in the tile palette window.</para>
	/// </summary>
	public bool hideAssetInstances => m_HideAssetInstances;

	/// <summary>
	///   <para>Hide the default instance of brush in the tile palette window.</para>
	/// </summary>
	public bool hideDefaultInstance => m_HideDefaultInstance;

	/// <summary>
	///   <para>If set to true, brush will replace Unity built-in brush as the default brush in palette window.
	///
	/// Only one class at any one time should set defaultBrush to true.</para>
	/// </summary>
	public bool defaultBrush => m_DefaultBrush;

	/// <summary>
	///   <para>Name of the default instance of this brush.</para>
	/// </summary>
	public string defaultName => m_DefaultName;

	/// <summary>
	///   <para>Attribute to define the class as a grid brush and to make it available in the palette window.</para>
	/// </summary>
	/// <param name="defaultBrush">If set to true, brush will replace Unity built-in brush as the default brush in palette window.</param>
	/// <param name="defaultName">Name of the default instance of this brush.</param>
	/// <param name="hideAssetInstanes">Hide all asset instances of this brush in the tile palette window.</param>
	/// <param name="hideDefaultInstance">Hide the default instance of brush in the tile palette window.</param>
	/// <param name="hideAssetInstances"></param>
	public CustomGridBrushAttribute()
	{
		m_HideAssetInstances = false;
		m_HideDefaultInstance = false;
		m_DefaultBrush = false;
		m_DefaultName = "";
	}

	/// <summary>
	///   <para>Attribute to define the class as a grid brush and to make it available in the palette window.</para>
	/// </summary>
	/// <param name="defaultBrush">If set to true, brush will replace Unity built-in brush as the default brush in palette window.</param>
	/// <param name="defaultName">Name of the default instance of this brush.</param>
	/// <param name="hideAssetInstanes">Hide all asset instances of this brush in the tile palette window.</param>
	/// <param name="hideDefaultInstance">Hide the default instance of brush in the tile palette window.</param>
	/// <param name="hideAssetInstances"></param>
	public CustomGridBrushAttribute(bool hideAssetInstances, bool hideDefaultInstance, bool defaultBrush, string defaultName)
	{
		m_HideAssetInstances = hideAssetInstances;
		m_HideDefaultInstance = hideDefaultInstance;
		m_DefaultBrush = defaultBrush;
		m_DefaultName = defaultName;
	}
}

using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A texture image used in a 2D GUI.</para>
/// </summary>
[Obsolete("This component is part of the legacy UI system and will be removed in a future release.")]
public sealed class GUITexture : GUIElement
{
	/// <summary>
	///   <para>The color of the GUI texture.</para>
	/// </summary>
	public Color color
	{
		get
		{
			INTERNAL_get_color(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_color(ref value);
		}
	}

	/// <summary>
	///   <para>The texture used for drawing.</para>
	/// </summary>
	public extern Texture texture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Pixel inset used for pixel adjustments for size and position.</para>
	/// </summary>
	public Rect pixelInset
	{
		get
		{
			INTERNAL_get_pixelInset(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_pixelInset(ref value);
		}
	}

	/// <summary>
	///   <para>The border defines the number of pixels from the edge that are not affected by scale.</para>
	/// </summary>
	public extern RectOffset border
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_color(out Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_color(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_pixelInset(out Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_pixelInset(ref Rect value);
}

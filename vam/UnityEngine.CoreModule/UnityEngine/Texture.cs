using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Base class for texture handling. Contains functionality that is common to both Texture2D and RenderTexture classes.</para>
/// </summary>
[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/Texture.h")]
public class Texture : Object
{
	/// <summary>
	///   <para>This counter is incremented when the texture is updated.</para>
	/// </summary>
	public extern uint updateCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern int masterTextureLimit
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("AnisoLimit")]
	public static extern AnisotropicFiltering anisotropicFiltering
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Width of the texture in pixels. (Read Only)</para>
	/// </summary>
	public virtual int width
	{
		get
		{
			return GetDataWidth();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	///   <para>Height of the texture in pixels. (Read Only)</para>
	/// </summary>
	public virtual int height
	{
		get
		{
			return GetDataHeight();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	///   <para>Dimensionality (type) of the texture (Read Only).</para>
	/// </summary>
	public virtual TextureDimension dimension
	{
		get
		{
			return GetDimension();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	///   <para>Texture coordinate wrapping mode.</para>
	/// </summary>
	public extern TextureWrapMode wrapMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetWrapModeU")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Texture U coordinate wrapping mode.</para>
	/// </summary>
	public extern TextureWrapMode wrapModeU
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Texture V coordinate wrapping mode.</para>
	/// </summary>
	public extern TextureWrapMode wrapModeV
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Texture W coordinate wrapping mode for Texture3D.</para>
	/// </summary>
	public extern TextureWrapMode wrapModeW
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Filtering mode of the texture.</para>
	/// </summary>
	public extern FilterMode filterMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Anisotropic filtering level of the texture.</para>
	/// </summary>
	public extern int anisoLevel
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Mip map bias of the texture.</para>
	/// </summary>
	public extern float mipMapBias
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Vector2 texelSize
	{
		[NativeName("GetNpotTexelSize")]
		get
		{
			get_texelSize_Injected(out var ret);
			return ret;
		}
	}

	protected Texture()
	{
	}

	/// <summary>
	///   <para>Increment the update counter.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void IncrementUpdateCount();

	/// <summary>
	///   <para>Sets Anisotropic limits.</para>
	/// </summary>
	/// <param name="forcedMin"></param>
	/// <param name="globalMax"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetGlobalAnisoLimits")]
	public static extern void SetGlobalAnisotropicFilteringLimits(int forcedMin, int globalMax);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetDataWidth();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetDataHeight();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern TextureDimension GetDimension();

	/// <summary>
	///   <para>Retrieve a native (underlying graphics API) pointer to the texture resource.</para>
	/// </summary>
	/// <returns>
	///   <para>Pointer to an underlying graphics API texture resource.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern IntPtr GetNativeTexturePtr();

	[Obsolete("Use GetNativeTexturePtr instead.", false)]
	public int GetNativeTextureID()
	{
		return (int)GetNativeTexturePtr();
	}

	internal UnityException CreateNonReadableException(Texture t)
	{
		return new UnityException($"Texture '{t.name}' is not readable, the texture memory can not be accessed from scripts. You can make the texture readable in the Texture Import Settings.");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_texelSize_Injected(out Vector2 ret);
}

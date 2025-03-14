using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Render textures are textures that can be rendered to.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Camera/Camera.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/RenderTexture.h")]
public class RenderTexture : Texture
{
	/// <summary>
	///   <para>The precision of the render texture's depth buffer in bits (0, 16, 24/32 are supported).</para>
	/// </summary>
	public extern int depth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Color buffer of the render texture (Read Only).</para>
	/// </summary>
	public RenderBuffer colorBuffer
	{
		get
		{
			GetColorBuffer(out var res);
			return res;
		}
	}

	/// <summary>
	///   <para>Depth/stencil buffer of the render texture (Read Only).</para>
	/// </summary>
	public RenderBuffer depthBuffer
	{
		get
		{
			GetDepthBuffer(out var res);
			return res;
		}
	}

	/// <summary>
	///   <para>Currently active render texture.</para>
	/// </summary>
	public static extern RenderTexture active
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("RenderTexture.enabled is always now, no need to use it")]
	public static extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The width of the render texture in pixels.</para>
	/// </summary>
	public override extern int width
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The height of the render texture in pixels.</para>
	/// </summary>
	public override extern int height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Dimensionality (type) of the render texture.</para>
	/// </summary>
	public override extern TextureDimension dimension
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Render texture has mipmaps when this flag is set.</para>
	/// </summary>
	[NativeProperty("MipMap")]
	public extern bool useMipMap
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Does this render texture use sRGB read/write conversions (Read Only).</para>
	/// </summary>
	[NativeProperty("SRGBReadWrite")]
	public extern bool sRGB
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>The color format of the render texture.</para>
	/// </summary>
	[NativeProperty("ColorFormat")]
	public extern RenderTextureFormat format
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>If this RenderTexture is a VR eye texture used in stereoscopic rendering, this property decides what special rendering occurs, if any.</para>
	/// </summary>
	[NativeProperty("VRUsage")]
	public extern VRTextureUsage vrUsage
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The render texture memoryless mode property.</para>
	/// </summary>
	[NativeProperty("Memoryless")]
	public extern RenderTextureMemoryless memorylessMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Mipmap levels are generated automatically when this flag is set.</para>
	/// </summary>
	public extern bool autoGenerateMips
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Volume extent of a 3D render texture or number of slices of array texture.</para>
	/// </summary>
	public extern int volumeDepth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The antialiasing level for the RenderTexture.</para>
	/// </summary>
	public extern int antiAliasing
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>If true and antiAliasing is greater than 1, the render texture will not be resolved by default.  Use this if the render texture needs to be bound as a multisampled texture in a shader.</para>
	/// </summary>
	public extern bool bindTextureMS
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Enable random access write into this render texture on Shader Model 5.0 level shaders.</para>
	/// </summary>
	public extern bool enableRandomWrite
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Is the render texture marked to be scaled by the Dynamic Resolution system.</para>
	/// </summary>
	public extern bool useDynamicScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public bool isPowerOfTwo
	{
		get
		{
			return GetIsPowerOfTwo();
		}
		set
		{
		}
	}

	[Obsolete("Use RenderTexture.dimension instead.", false)]
	public bool isCubemap
	{
		get
		{
			return dimension == TextureDimension.Cube;
		}
		set
		{
			dimension = ((!value) ? TextureDimension.Tex2D : TextureDimension.Cube);
		}
	}

	/// <summary>
	///   <para>If enabled, this Render Texture will be used as a Texture3D.</para>
	/// </summary>
	[Obsolete("Use RenderTexture.dimension instead.", false)]
	public bool isVolume
	{
		get
		{
			return dimension == TextureDimension.Tex3D;
		}
		set
		{
			dimension = ((!value) ? TextureDimension.Tex2D : TextureDimension.Tex3D);
		}
	}

	/// <summary>
	///   <para>This struct contains all the information required to create a RenderTexture. It can be copied, cached, and reused to easily create RenderTextures that all share the same properties.</para>
	/// </summary>
	public RenderTextureDescriptor descriptor
	{
		get
		{
			return GetDescriptor();
		}
		set
		{
			ValidateRenderTextureDesc(value);
			SetRenderTextureDescriptor(value);
		}
	}

	protected internal RenderTexture()
	{
	}

	/// <summary>
	///   <para>Creates a new RenderTexture object.</para>
	/// </summary>
	/// <param name="width">Texture width in pixels.</param>
	/// <param name="height">Texture height in pixels.</param>
	/// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
	/// <param name="format">Texture color format.</param>
	/// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
	/// <param name="desc">Create the RenderTexture with the settings in the RenderTextureDescriptor.</param>
	/// <param name="textureToCopy">Copy the settings from another RenderTexture.</param>
	public RenderTexture(RenderTextureDescriptor desc)
	{
		ValidateRenderTextureDesc(desc);
		Internal_Create(this);
		SetRenderTextureDescriptor(desc);
	}

	/// <summary>
	///   <para>Creates a new RenderTexture object.</para>
	/// </summary>
	/// <param name="width">Texture width in pixels.</param>
	/// <param name="height">Texture height in pixels.</param>
	/// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
	/// <param name="format">Texture color format.</param>
	/// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
	/// <param name="desc">Create the RenderTexture with the settings in the RenderTextureDescriptor.</param>
	/// <param name="textureToCopy">Copy the settings from another RenderTexture.</param>
	public RenderTexture(RenderTexture textureToCopy)
	{
		if (textureToCopy == null)
		{
			throw new ArgumentNullException("textureToCopy");
		}
		ValidateRenderTextureDesc(textureToCopy.descriptor);
		Internal_Create(this);
		SetRenderTextureDescriptor(textureToCopy.descriptor);
	}

	/// <summary>
	///   <para>Creates a new RenderTexture object.</para>
	/// </summary>
	/// <param name="width">Texture width in pixels.</param>
	/// <param name="height">Texture height in pixels.</param>
	/// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
	/// <param name="format">Texture color format.</param>
	/// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
	/// <param name="desc">Create the RenderTexture with the settings in the RenderTextureDescriptor.</param>
	/// <param name="textureToCopy">Copy the settings from another RenderTexture.</param>
	public RenderTexture(int width, int height, int depth, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
	{
		Internal_Create(this);
		this.width = width;
		this.height = height;
		this.depth = depth;
		this.format = format;
		bool flag = QualitySettings.activeColorSpace == ColorSpace.Linear;
		SetSRGBReadWrite((readWrite != 0) ? (readWrite == RenderTextureReadWrite.sRGB) : flag);
	}

	/// <summary>
	///   <para>Creates a new RenderTexture object.</para>
	/// </summary>
	/// <param name="width">Texture width in pixels.</param>
	/// <param name="height">Texture height in pixels.</param>
	/// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
	/// <param name="format">Texture color format.</param>
	/// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
	/// <param name="desc">Create the RenderTexture with the settings in the RenderTextureDescriptor.</param>
	/// <param name="textureToCopy">Copy the settings from another RenderTexture.</param>
	public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
		: this(width, height, depth, format, RenderTextureReadWrite.Default)
	{
	}

	/// <summary>
	///   <para>Creates a new RenderTexture object.</para>
	/// </summary>
	/// <param name="width">Texture width in pixels.</param>
	/// <param name="height">Texture height in pixels.</param>
	/// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
	/// <param name="format">Texture color format.</param>
	/// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
	/// <param name="desc">Create the RenderTexture with the settings in the RenderTextureDescriptor.</param>
	/// <param name="textureToCopy">Copy the settings from another RenderTexture.</param>
	public RenderTexture(int width, int height, int depth)
		: this(width, height, depth, RenderTextureFormat.Default, RenderTextureReadWrite.Default)
	{
	}

	private void SetRenderTextureDescriptor(RenderTextureDescriptor desc)
	{
		INTERNAL_CALL_SetRenderTextureDescriptor(this, ref desc);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SetRenderTextureDescriptor(RenderTexture self, ref RenderTextureDescriptor desc);

	private RenderTextureDescriptor GetDescriptor()
	{
		INTERNAL_CALL_GetDescriptor(this, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetDescriptor(RenderTexture self, out RenderTextureDescriptor value);

	private static RenderTexture GetTemporary_Internal(RenderTextureDescriptor desc)
	{
		return INTERNAL_CALL_GetTemporary_Internal(ref desc);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern RenderTexture INTERNAL_CALL_GetTemporary_Internal(ref RenderTextureDescriptor desc);

	/// <summary>
	///   <para>Release a temporary texture allocated with GetTemporary.</para>
	/// </summary>
	/// <param name="temp"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void ReleaseTemporary(RenderTexture temp);

	/// <summary>
	///   <para>Force an antialiased render texture to be resolved.</para>
	/// </summary>
	/// <param name="target">The render texture to resolve into.  If set, the target render texture must have the same dimensions and format as the source.</param>
	public void ResolveAntiAliasedSurface()
	{
		Internal_ResolveAntiAliasedSurface(null);
	}

	/// <summary>
	///   <para>Force an antialiased render texture to be resolved.</para>
	/// </summary>
	/// <param name="target">The render texture to resolve into.  If set, the target render texture must have the same dimensions and format as the source.</param>
	public void ResolveAntiAliasedSurface(RenderTexture target)
	{
		Internal_ResolveAntiAliasedSurface(target);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_ResolveAntiAliasedSurface(RenderTexture target);

	/// <summary>
	///   <para>Hint the GPU driver that the contents of the RenderTexture will not be used.</para>
	/// </summary>
	/// <param name="discardColor">Should the colour buffer be discarded?</param>
	/// <param name="discardDepth">Should the depth buffer be discarded?</param>
	public void DiscardContents()
	{
		INTERNAL_CALL_DiscardContents(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DiscardContents(RenderTexture self);

	/// <summary>
	///   <para>Hint the GPU driver that the contents of the RenderTexture will not be used.</para>
	/// </summary>
	/// <param name="discardColor">Should the colour buffer be discarded?</param>
	/// <param name="discardDepth">Should the depth buffer be discarded?</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void DiscardContents(bool discardColor, bool discardDepth);

	/// <summary>
	///   <para>Indicate that there's a RenderTexture restore operation expected.</para>
	/// </summary>
	public void MarkRestoreExpected()
	{
		INTERNAL_CALL_MarkRestoreExpected(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_MarkRestoreExpected(RenderTexture self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void GetColorBuffer(out RenderBuffer res);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void GetDepthBuffer(out RenderBuffer res);

	/// <summary>
	///   <para>Retrieve a native (underlying graphics API) pointer to the depth buffer resource.</para>
	/// </summary>
	/// <returns>
	///   <para>Pointer to an underlying graphics API depth buffer resource.</para>
	/// </returns>
	public IntPtr GetNativeDepthBufferPtr()
	{
		INTERNAL_CALL_GetNativeDepthBufferPtr(this, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetNativeDepthBufferPtr(RenderTexture self, out IntPtr value);

	/// <summary>
	///   <para>Assigns this RenderTexture as a global shader property named propertyName.</para>
	/// </summary>
	/// <param name="propertyName"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetGlobalShaderProperty(string propertyName);

	[Obsolete("GetTexelOffset always returns zero now, no point in using it.")]
	public Vector2 GetTexelOffset()
	{
		return Vector2.zero;
	}

	/// <summary>
	///   <para>Does a RenderTexture have stencil buffer?</para>
	/// </summary>
	/// <param name="rt">Render texture, or null for main screen.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool SupportsStencil(RenderTexture rt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern VRTextureUsage GetActiveVRUsage();

	[Obsolete("SetBorderColor is no longer supported.", true)]
	public void SetBorderColor(Color color)
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool GetIsPowerOfTwo();

	/// <summary>
	///   <para>Actually creates the RenderTexture.</para>
	/// </summary>
	/// <returns>
	///   <para>True if the texture is created, else false.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern bool Create();

	/// <summary>
	///   <para>Releases the RenderTexture.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Release();

	/// <summary>
	///   <para>Is the render texture actually created?</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern bool IsCreated();

	/// <summary>
	///   <para>Generate mipmap levels of a render texture.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void GenerateMips();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ConvertToEquirect(RenderTexture equirect, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void SetSRGBReadWrite(bool srgb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("RenderTextureScripting::Create")]
	private static extern void Internal_Create([Writable] RenderTexture rt);

	private static void ValidateRenderTextureDesc(RenderTextureDescriptor desc)
	{
		if (desc.width <= 0)
		{
			throw new ArgumentException("RenderTextureDesc width must be greater than zero.", "desc.width");
		}
		if (desc.height <= 0)
		{
			throw new ArgumentException("RenderTextureDesc height must be greater than zero.", "desc.height");
		}
		if (desc.volumeDepth <= 0)
		{
			throw new ArgumentException("RenderTextureDesc volumeDepth must be greater than zero.", "desc.volumeDepth");
		}
		if (desc.msaaSamples != 1 && desc.msaaSamples != 2 && desc.msaaSamples != 4 && desc.msaaSamples != 8)
		{
			throw new ArgumentException("RenderTextureDesc msaaSamples must be 1, 2, 4, or 8.", "desc.msaaSamples");
		}
		if (desc.depthBufferBits != 0 && desc.depthBufferBits != 16 && desc.depthBufferBits != 24)
		{
			throw new ArgumentException("RenderTextureDesc depthBufferBits must be 0, 16, or 24.", "desc.depthBufferBits");
		}
	}

	/// <summary>
	///   <para>Allocate a temporary render texture.</para>
	/// </summary>
	/// <param name="width">Width in pixels.</param>
	/// <param name="height">Height in pixels.</param>
	/// <param name="depthBuffer">Depth buffer bits (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
	/// <param name="format">Render texture format.</param>
	/// <param name="readWrite">Color space conversion mode.</param>
	/// <param name="msaaSamples">Number of antialiasing samples to store in the texture. Valid values are 1, 2, 4, and 8. Throws an exception if any other value is passed.</param>
	/// <param name="memorylessMode">Render texture memoryless mode.</param>
	/// <param name="desc">Use this RenderTextureDesc for the settings when creating the temporary RenderTexture.</param>
	/// <param name="antiAliasing"></param>
	public static RenderTexture GetTemporary(RenderTextureDescriptor desc)
	{
		ValidateRenderTextureDesc(desc);
		desc.createdFromScript = true;
		return GetTemporary_Internal(desc);
	}

	private static RenderTexture GetTemporaryImpl(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int antiAliasing = 1, RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, bool useDynamicScale = false)
	{
		RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height, format, depthBuffer);
		desc.sRGB = readWrite != RenderTextureReadWrite.Linear;
		desc.msaaSamples = antiAliasing;
		desc.memoryless = memorylessMode;
		desc.vrUsage = vrUsage;
		desc.useDynamicScale = useDynamicScale;
		return GetTemporary(desc);
	}

	public static RenderTexture GetTemporary(int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing, [DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [DefaultValue("false")] bool useDynamicScale)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage, useDynamicScale);
	}

	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage);
	}

	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode);
	}

	/// <summary>
	///   <para>Allocate a temporary render texture.</para>
	/// </summary>
	/// <param name="width">Width in pixels.</param>
	/// <param name="height">Height in pixels.</param>
	/// <param name="depthBuffer">Depth buffer bits (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
	/// <param name="format">Render texture format.</param>
	/// <param name="readWrite">Color space conversion mode.</param>
	/// <param name="msaaSamples">Number of antialiasing samples to store in the texture. Valid values are 1, 2, 4, and 8. Throws an exception if any other value is passed.</param>
	/// <param name="memorylessMode">Render texture memoryless mode.</param>
	/// <param name="desc">Use this RenderTextureDesc for the settings when creating the temporary RenderTexture.</param>
	/// <param name="antiAliasing"></param>
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format, readWrite, antiAliasing);
	}

	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format, readWrite);
	}

	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format);
	}

	public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
	{
		return GetTemporaryImpl(width, height, depthBuffer);
	}

	public static RenderTexture GetTemporary(int width, int height)
	{
		return GetTemporaryImpl(width, height);
	}
}

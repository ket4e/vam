using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

/// <summary>
///   <para>Access system and hardware information.</para>
/// </summary>
[NativeHeader("Runtime/Misc/SystemInfo.h")]
[NativeHeader("Runtime/Shaders/GraphicsCapsScriptBindings.h")]
[NativeHeader("Runtime/Camera/RenderLoops/MotionVectorRenderLoop.h")]
[NativeHeader("Runtime/Input/GetInput.h")]
public sealed class SystemInfo
{
	/// <summary>
	///   <para>Value returned by SystemInfo string properties which are not supported on the current platform.</para>
	/// </summary>
	public const string unsupportedIdentifier = "n/a";

	/// <summary>
	///   <para>The current battery level (Read Only).</para>
	/// </summary>
	[NativeProperty]
	public static float batteryLevel => GetBatteryLevel();

	/// <summary>
	///   <para>Returns the current status of the device's battery (Read Only).</para>
	/// </summary>
	public static BatteryStatus batteryStatus => GetBatteryStatus();

	/// <summary>
	///   <para>Operating system name with version (Read Only).</para>
	/// </summary>
	public static string operatingSystem => GetOperatingSystem();

	/// <summary>
	///   <para>Returns the operating system family the game is running on (Read Only).</para>
	/// </summary>
	public static OperatingSystemFamily operatingSystemFamily => GetOperatingSystemFamily();

	/// <summary>
	///   <para>Processor name (Read Only).</para>
	/// </summary>
	public static string processorType => GetProcessorType();

	/// <summary>
	///   <para>Processor frequency in MHz (Read Only).</para>
	/// </summary>
	public static int processorFrequency => GetProcessorFrequencyMHz();

	/// <summary>
	///   <para>Number of processors present (Read Only).</para>
	/// </summary>
	public static int processorCount => GetProcessorCount();

	/// <summary>
	///   <para>Amount of system memory present (Read Only).</para>
	/// </summary>
	public static int systemMemorySize => GetPhysicalMemoryMB();

	/// <summary>
	///   <para>A unique device identifier. It is guaranteed to be unique for every device (Read Only).</para>
	/// </summary>
	public static string deviceUniqueIdentifier => GetDeviceUniqueIdentifier();

	/// <summary>
	///   <para>The user defined name of the device (Read Only).</para>
	/// </summary>
	public static string deviceName => GetDeviceName();

	/// <summary>
	///   <para>The model of the device (Read Only).</para>
	/// </summary>
	public static string deviceModel => GetDeviceModel();

	/// <summary>
	///   <para>Is an accelerometer available on the device?</para>
	/// </summary>
	public static bool supportsAccelerometer => SupportsAccelerometer();

	/// <summary>
	///   <para>Is a gyroscope available on the device?</para>
	/// </summary>
	public static bool supportsGyroscope => IsGyroAvailable();

	/// <summary>
	///   <para>Is the device capable of reporting its location?</para>
	/// </summary>
	public static bool supportsLocationService => SupportsLocationService();

	/// <summary>
	///   <para>Is the device capable of providing the user haptic feedback by vibration?</para>
	/// </summary>
	public static bool supportsVibration => SupportsVibration();

	/// <summary>
	///   <para>Is there an Audio device available for playback?</para>
	/// </summary>
	public static bool supportsAudio => SupportsAudio();

	/// <summary>
	///   <para>Returns the kind of device the application is running on (Read Only).</para>
	/// </summary>
	public static DeviceType deviceType => GetDeviceType();

	/// <summary>
	///   <para>Amount of video memory present (Read Only).</para>
	/// </summary>
	public static int graphicsMemorySize => GetGraphicsMemorySize();

	/// <summary>
	///   <para>The name of the graphics device (Read Only).</para>
	/// </summary>
	public static string graphicsDeviceName => GetGraphicsDeviceName();

	/// <summary>
	///   <para>The vendor of the graphics device (Read Only).</para>
	/// </summary>
	public static string graphicsDeviceVendor => GetGraphicsDeviceVendor();

	/// <summary>
	///   <para>The identifier code of the graphics device (Read Only).</para>
	/// </summary>
	public static int graphicsDeviceID => GetGraphicsDeviceID();

	/// <summary>
	///   <para>The identifier code of the graphics device vendor (Read Only).</para>
	/// </summary>
	public static int graphicsDeviceVendorID => GetGraphicsDeviceVendorID();

	/// <summary>
	///   <para>The graphics API type used by the graphics device (Read Only).</para>
	/// </summary>
	public static GraphicsDeviceType graphicsDeviceType => GetGraphicsDeviceType();

	/// <summary>
	///   <para>Returns true if the texture UV coordinate convention for this platform has Y starting at the top of the image.</para>
	/// </summary>
	public static bool graphicsUVStartsAtTop => GetGraphicsUVStartsAtTop();

	/// <summary>
	///   <para>The graphics API type and driver version used by the graphics device (Read Only).</para>
	/// </summary>
	public static string graphicsDeviceVersion => GetGraphicsDeviceVersion();

	/// <summary>
	///   <para>Graphics device shader capability level (Read Only).</para>
	/// </summary>
	public static int graphicsShaderLevel => GetGraphicsShaderLevel();

	/// <summary>
	///   <para>Is graphics device using multi-threaded rendering (Read Only)?</para>
	/// </summary>
	public static bool graphicsMultiThreaded => GetGraphicsMultiThreaded();

	/// <summary>
	///   <para>Are built-in shadows supported? (Read Only)</para>
	/// </summary>
	public static bool supportsShadows => SupportsShadows();

	/// <summary>
	///   <para>Is sampling raw depth from shadowmaps supported? (Read Only)</para>
	/// </summary>
	public static bool supportsRawShadowDepthSampling => SupportsRawShadowDepthSampling();

	/// <summary>
	///   <para>Are render textures supported? (Read Only)</para>
	/// </summary>
	[Obsolete("supportsRenderTextures always returns true, no need to call it")]
	public static bool supportsRenderTextures => true;

	/// <summary>
	///   <para>Whether motion vectors are supported on this platform.</para>
	/// </summary>
	public static bool supportsMotionVectors => SupportsMotionVectors();

	/// <summary>
	///   <para>Are cubemap render textures supported? (Read Only)</para>
	/// </summary>
	public static bool supportsRenderToCubemap => SupportsRenderToCubemap();

	/// <summary>
	///   <para>Are image effects supported? (Read Only)</para>
	/// </summary>
	public static bool supportsImageEffects => SupportsImageEffects();

	/// <summary>
	///   <para>Are 3D (volume) textures supported? (Read Only)</para>
	/// </summary>
	public static bool supports3DTextures => Supports3DTextures();

	/// <summary>
	///   <para>Are 2D Array textures supported? (Read Only)</para>
	/// </summary>
	public static bool supports2DArrayTextures => Supports2DArrayTextures();

	/// <summary>
	///   <para>Are 3D (volume) RenderTextures supported? (Read Only)</para>
	/// </summary>
	public static bool supports3DRenderTextures => Supports3DRenderTextures();

	/// <summary>
	///   <para>Are Cubemap Array textures supported? (Read Only)</para>
	/// </summary>
	public static bool supportsCubemapArrayTextures => SupportsCubemapArrayTextures();

	/// <summary>
	///   <para>Support for various Graphics.CopyTexture cases (Read Only).</para>
	/// </summary>
	public static CopyTextureSupport copyTextureSupport => GetCopyTextureSupport();

	/// <summary>
	///   <para>Are compute shaders supported? (Read Only)</para>
	/// </summary>
	public static bool supportsComputeShaders => SupportsComputeShaders();

	/// <summary>
	///   <para>Is GPU draw call instancing supported? (Read Only)</para>
	/// </summary>
	public static bool supportsInstancing => SupportsInstancing();

	/// <summary>
	///   <para>Does the hardware support quad topology? (Read Only)</para>
	/// </summary>
	public static bool supportsHardwareQuadTopology => SupportsHardwareQuadTopology();

	/// <summary>
	///   <para>Are 32-bit index buffers supported? (Read Only)</para>
	/// </summary>
	public static bool supports32bitsIndexBuffer => Supports32bitsIndexBuffer();

	/// <summary>
	///   <para>Are sparse textures supported? (Read Only)</para>
	/// </summary>
	public static bool supportsSparseTextures => SupportsSparseTextures();

	/// <summary>
	///   <para>How many simultaneous render targets (MRTs) are supported? (Read Only)</para>
	/// </summary>
	public static int supportedRenderTargetCount => SupportedRenderTargetCount();

	/// <summary>
	///   <para>Are multisampled textures supported? (Read Only)</para>
	/// </summary>
	public static int supportsMultisampledTextures => SupportsMultisampledTextures();

	/// <summary>
	///   <para>Returns true if the 'Mirror Once' texture wrap mode is supported. (Read Only)</para>
	/// </summary>
	public static int supportsTextureWrapMirrorOnce => SupportsTextureWrapMirrorOnce();

	/// <summary>
	///   <para>This property is true if the current platform uses a reversed depth buffer (where values range from 1 at the near plane and 0 at far plane), and false if the depth buffer is normal (0 is near, 1 is far). (Read Only)</para>
	/// </summary>
	public static bool usesReversedZBuffer => UsesReversedZBuffer();

	/// <summary>
	///   <para>Is the stencil buffer supported? (Read Only)</para>
	/// </summary>
	[Obsolete("supportsStencil always returns true, no need to call it")]
	public static int supportsStencil => 1;

	/// <summary>
	///   <para>What NPOT (non-power of two size) texture support does the GPU provide? (Read Only)</para>
	/// </summary>
	public static NPOTSupport npotSupport => GetNPOTSupport();

	/// <summary>
	///   <para>Maximum texture size (Read Only).</para>
	/// </summary>
	public static int maxTextureSize => GetMaxTextureSize();

	/// <summary>
	///   <para>Maximum Cubemap texture size (Read Only).</para>
	/// </summary>
	public static int maxCubemapSize => GetMaxCubemapSize();

	internal static int maxRenderTextureSize => GetMaxRenderTextureSize();

	/// <summary>
	///   <para>Returns true when the platform supports asynchronous compute queues and false if otherwise.
	///
	/// Note that asynchronous compute queues are only supported on PS4.</para>
	/// </summary>
	public static bool supportsAsyncCompute => SupportsAsyncCompute();

	/// <summary>
	///   <para>Returns true when the platform supports GPUFences and false if otherwise.
	///
	/// Note that GPUFences are only supported on PS4.</para>
	/// </summary>
	public static bool supportsGPUFence => SupportsGPUFence();

	/// <summary>
	///   <para>Returns true if asynchronous readback of GPU data is available for this device and false otherwise.</para>
	/// </summary>
	public static bool supportsAsyncGPUReadback => SupportsAsyncGPUReadback();

	[Obsolete("graphicsPixelFillrate is no longer supported in Unity 5.0+.")]
	public static int graphicsPixelFillrate => -1;

	[Obsolete("Vertex program support is required in Unity 5.0+")]
	public static bool supportsVertexPrograms => true;

	private static bool IsValidEnumValue(Enum value)
	{
		if (!Enum.IsDefined(value.GetType(), value))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	///   <para>Is render texture format supported?</para>
	/// </summary>
	/// <param name="format">The format to look up.</param>
	/// <returns>
	///   <para>True if the format is supported.</para>
	/// </returns>
	public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
	{
		if (!IsValidEnumValue(format))
		{
			throw new ArgumentException("Failed SupportsRenderTextureFormat; format is not a valid RenderTextureFormat");
		}
		return HasRenderTextureNative(format);
	}

	/// <summary>
	///   <para>Is blending supported on render texture format?</para>
	/// </summary>
	/// <param name="format">The format to look up.</param>
	/// <returns>
	///   <para>True if blending is supported on the given format.</para>
	/// </returns>
	public static bool SupportsBlendingOnRenderTextureFormat(RenderTextureFormat format)
	{
		if (!IsValidEnumValue(format))
		{
			throw new ArgumentException("Failed SupportsBlendingOnRenderTextureFormat; format is not a valid RenderTextureFormat");
		}
		return SupportsBlendingOnRenderTextureFormatNative(format);
	}

	/// <summary>
	///   <para>Is texture format supported on this device?</para>
	/// </summary>
	/// <param name="format">The TextureFormat format to look up.</param>
	/// <returns>
	///   <para>True if the format is supported.</para>
	/// </returns>
	public static bool SupportsTextureFormat(TextureFormat format)
	{
		if (!IsValidEnumValue(format))
		{
			throw new ArgumentException("Failed SupportsTextureFormat; format is not a valid TextureFormat");
		}
		return SupportsTextureFormatNative(format);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetBatteryLevel")]
	private static extern float GetBatteryLevel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetBatteryStatus")]
	private static extern BatteryStatus GetBatteryStatus();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetOperatingSystem")]
	private static extern string GetOperatingSystem();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetOperatingSystemFamily")]
	private static extern OperatingSystemFamily GetOperatingSystemFamily();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetProcessorType")]
	private static extern string GetProcessorType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetProcessorFrequencyMHz")]
	private static extern int GetProcessorFrequencyMHz();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetProcessorCount")]
	private static extern int GetProcessorCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetPhysicalMemoryMB")]
	private static extern int GetPhysicalMemoryMB();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetDeviceUniqueIdentifier")]
	private static extern string GetDeviceUniqueIdentifier();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetDeviceName")]
	private static extern string GetDeviceName();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetDeviceModel")]
	private static extern string GetDeviceModel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::SupportsAccelerometer")]
	private static extern bool SupportsAccelerometer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern bool IsGyroAvailable();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::SupportsLocationService")]
	private static extern bool SupportsLocationService();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::SupportsVibration")]
	private static extern bool SupportsVibration();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::SupportsAudio")]
	private static extern bool SupportsAudio();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetDeviceType")]
	private static extern DeviceType GetDeviceType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsMemorySize")]
	private static extern int GetGraphicsMemorySize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceName")]
	private static extern string GetGraphicsDeviceName();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVendor")]
	private static extern string GetGraphicsDeviceVendor();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceID")]
	private static extern int GetGraphicsDeviceID();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVendorID")]
	private static extern int GetGraphicsDeviceVendorID();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceType")]
	private static extern GraphicsDeviceType GetGraphicsDeviceType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsUVStartsAtTop")]
	private static extern bool GetGraphicsUVStartsAtTop();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVersion")]
	private static extern string GetGraphicsDeviceVersion();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsShaderLevel")]
	private static extern int GetGraphicsShaderLevel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsMultiThreaded")]
	private static extern bool GetGraphicsMultiThreaded();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsShadows")]
	private static extern bool SupportsShadows();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsRawShadowDepthSampling")]
	private static extern bool SupportsRawShadowDepthSampling();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("SupportsMotionVectors")]
	private static extern bool SupportsMotionVectors();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsRenderToCubemap")]
	private static extern bool SupportsRenderToCubemap();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsImageEffects")]
	private static extern bool SupportsImageEffects();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::Supports3DTextures")]
	private static extern bool Supports3DTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::Supports2DArrayTextures")]
	private static extern bool Supports2DArrayTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::Supports3DRenderTextures")]
	private static extern bool Supports3DRenderTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsCubemapArrayTextures")]
	private static extern bool SupportsCubemapArrayTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetCopyTextureSupport")]
	private static extern CopyTextureSupport GetCopyTextureSupport();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsComputeShaders")]
	private static extern bool SupportsComputeShaders();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsInstancing")]
	private static extern bool SupportsInstancing();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsHardwareQuadTopology")]
	private static extern bool SupportsHardwareQuadTopology();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::Supports32bitsIndexBuffer")]
	private static extern bool Supports32bitsIndexBuffer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsSparseTextures")]
	private static extern bool SupportsSparseTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportedRenderTargetCount")]
	private static extern int SupportedRenderTargetCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampledTextures")]
	private static extern int SupportsMultisampledTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsTextureWrapMirrorOnce")]
	private static extern int SupportsTextureWrapMirrorOnce();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::UsesReversedZBuffer")]
	private static extern bool UsesReversedZBuffer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::HasRenderTexture")]
	private static extern bool HasRenderTextureNative(RenderTextureFormat format);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsBlendingOnRenderTextureFormat")]
	private static extern bool SupportsBlendingOnRenderTextureFormatNative(RenderTextureFormat format);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsTextureFormat")]
	private static extern bool SupportsTextureFormatNative(TextureFormat format);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetNPOTSupport")]
	private static extern NPOTSupport GetNPOTSupport();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxTextureSize")]
	private static extern int GetMaxTextureSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxCubemapSize")]
	private static extern int GetMaxCubemapSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxRenderTextureSize")]
	private static extern int GetMaxRenderTextureSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsAsyncCompute")]
	private static extern bool SupportsAsyncCompute();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsGPUFence")]
	private static extern bool SupportsGPUFence();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsAsyncGPUReadback")]
	private static extern bool SupportsAsyncGPUReadback();
}

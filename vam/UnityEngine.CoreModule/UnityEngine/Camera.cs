using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A Camera is a device through which the player views the world.</para>
/// </summary>
[NativeHeader("Runtime/Camera/Camera.h")]
[NativeHeader("Runtime/Camera/RenderManager.h")]
[NativeHeader("Runtime/GfxDevice/GfxDeviceTypes.h")]
[NativeHeader("Runtime/Graphics/RenderTexture.h")]
[NativeHeader("Runtime/Shaders/Shader.h")]
[UsedByNativeCode]
[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
[NativeHeader("Runtime/Misc/GameObjectUtility.h")]
public sealed class Camera : Behaviour
{
	/// <summary>
	///   <para>Enum used to specify either the left or the right eye of a stereoscopic camera.</para>
	/// </summary>
	public enum StereoscopicEye
	{
		/// <summary>
		///   <para>Specifies the target to be the left eye.</para>
		/// </summary>
		Left,
		/// <summary>
		///   <para>Specifies the target to be the right eye.</para>
		/// </summary>
		Right
	}

	/// <summary>
	///   <para>A Camera eye corresponding to the left or right human eye for stereoscopic rendering, or neither for non-stereoscopic rendering.
	///
	/// A single Camera can render both left and right views in a single frame. Therefore, this enum describes which eye the Camera is currently rendering when returned by Camera.stereoActiveEye during a rendering callback (such as Camera.OnRenderImage), or which eye to act on when passed into a function.
	///
	/// The default value is Camera.MonoOrStereoscopicEye.Left, so Camera.MonoOrStereoscopicEye.Left may be returned by some methods or properties when called outside of rendering if stereoscopic rendering is enabled.</para>
	/// </summary>
	public enum MonoOrStereoscopicEye
	{
		/// <summary>
		///   <para>Camera eye corresponding to stereoscopic rendering of the left eye.</para>
		/// </summary>
		Left,
		/// <summary>
		///   <para>Camera eye corresponding to stereoscopic rendering of the right eye.</para>
		/// </summary>
		Right,
		/// <summary>
		///   <para>Camera eye corresponding to non-stereoscopic rendering.</para>
		/// </summary>
		Mono
	}

	/// <summary>
	///   <para>Delegate type for camera callbacks.</para>
	/// </summary>
	/// <param name="cam"></param>
	public delegate void CameraCallback(Camera cam);

	/// <summary>
	///   <para>Event that is fired before any camera starts culling.</para>
	/// </summary>
	public static CameraCallback onPreCull;

	/// <summary>
	///   <para>Event that is fired before any camera starts rendering.</para>
	/// </summary>
	public static CameraCallback onPreRender;

	/// <summary>
	///   <para>Event that is fired after any camera finishes rendering.</para>
	/// </summary>
	public static CameraCallback onPostRender;

	internal static extern int PreviewCullingLayer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>If not null, the camera will only render the contents of the specified scene.</para>
	/// </summary>
	public Scene scene
	{
		get
		{
			INTERNAL_get_scene(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_scene(ref value);
		}
	}

	/// <summary>
	///   <para>Returns the eye that is currently rendering.
	/// If called when stereo is not enabled it will return Camera.MonoOrStereoscopicEye.Mono.
	///
	/// If called during a camera rendering callback such as OnRenderImage it will return the currently rendering eye.
	///
	/// If called outside of a rendering callback and stereo is enabled, it will return the default eye which is Camera.MonoOrStereoscopicEye.Left.</para>
	/// </summary>
	public extern MonoOrStereoscopicEye stereoActiveEye
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Set the target display for this Camera.</para>
	/// </summary>
	public extern int targetDisplay
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Returns all enabled cameras in the scene.</para>
	/// </summary>
	public static extern Camera[] allCameras
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The number of cameras in the current scene.</para>
	/// </summary>
	public static extern int allCamerasCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Per-layer culling distances.</para>
	/// </summary>
	public extern float[] layerCullDistances
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The near clipping plane distance.</para>
	/// </summary>
	[NativeProperty("Near")]
	public extern float nearClipPlane
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The far clipping plane distance.</para>
	/// </summary>
	[NativeProperty("Far")]
	public extern float farClipPlane
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The field of view of the camera in degrees.</para>
	/// </summary>
	[NativeProperty("Fov")]
	public extern float fieldOfView
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The rendering path that should be used, if possible.</para>
	/// </summary>
	public extern RenderingPath renderingPath
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The rendering path that is currently being used (Read Only).</para>
	/// </summary>
	public extern RenderingPath actualRenderingPath
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("CalculateRenderingPath")]
		get;
	}

	/// <summary>
	///   <para>High dynamic range rendering.</para>
	/// </summary>
	public extern bool allowHDR
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>MSAA rendering.</para>
	/// </summary>
	public extern bool allowMSAA
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Dynamic Resolution Scaling.</para>
	/// </summary>
	public extern bool allowDynamicResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Should camera rendering be forced into a RenderTexture.</para>
	/// </summary>
	[NativeProperty("ForceIntoRT")]
	public extern bool forceIntoRenderTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Camera's half-size when in orthographic mode.</para>
	/// </summary>
	public extern float orthographicSize
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Is the camera orthographic (true) or perspective (false)?</para>
	/// </summary>
	public extern bool orthographic
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Opaque object sorting mode.</para>
	/// </summary>
	public extern OpaqueSortMode opaqueSortMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Transparent object sorting mode.</para>
	/// </summary>
	public extern TransparencySortMode transparencySortMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>An axis that describes the direction along which the distances of objects are measured for the purpose of sorting.</para>
	/// </summary>
	public Vector3 transparencySortAxis
	{
		get
		{
			get_transparencySortAxis_Injected(out var ret);
			return ret;
		}
		set
		{
			set_transparencySortAxis_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Camera's depth in the camera rendering order.</para>
	/// </summary>
	public extern float depth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The aspect ratio (width divided by height).</para>
	/// </summary>
	public extern float aspect
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Get the world-space speed of the camera (Read Only).</para>
	/// </summary>
	public Vector3 velocity
	{
		get
		{
			get_velocity_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>This is used to render parts of the scene selectively.</para>
	/// </summary>
	public extern int cullingMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Mask to select which layers can trigger events on the camera.</para>
	/// </summary>
	public extern int eventMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>How to perform per-layer culling for a Camera.</para>
	/// </summary>
	public extern bool layerCullSpherical
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Identifies what kind of camera this is.</para>
	/// </summary>
	public extern CameraType cameraType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Whether or not the Camera will use occlusion culling during rendering.</para>
	/// </summary>
	public extern bool useOcclusionCulling
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Sets a custom matrix for the camera to use for all culling queries.</para>
	/// </summary>
	public Matrix4x4 cullingMatrix
	{
		get
		{
			get_cullingMatrix_Injected(out var ret);
			return ret;
		}
		set
		{
			set_cullingMatrix_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The color with which the screen will be cleared.</para>
	/// </summary>
	public Color backgroundColor
	{
		get
		{
			get_backgroundColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_backgroundColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>How the camera clears the background.</para>
	/// </summary>
	public extern CameraClearFlags clearFlags
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>How and if camera generates a depth texture.</para>
	/// </summary>
	public extern DepthTextureMode depthTextureMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Should the camera clear the stencil buffer after the deferred light pass?</para>
	/// </summary>
	public extern bool clearStencilAfterLightingPass
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Where on the screen is the camera rendered in normalized coordinates.</para>
	/// </summary>
	[NativeProperty("NormalizedViewportRect")]
	public Rect rect
	{
		get
		{
			get_rect_Injected(out var ret);
			return ret;
		}
		set
		{
			set_rect_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Where on the screen is the camera rendered in pixel coordinates.</para>
	/// </summary>
	[NativeProperty("ScreenViewportRect")]
	public Rect pixelRect
	{
		get
		{
			get_pixelRect_Injected(out var ret);
			return ret;
		}
		set
		{
			set_pixelRect_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>How wide is the camera in pixels (not accounting for dynamic resolution scaling) (Read Only).</para>
	/// </summary>
	public extern int pixelWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("CameraScripting::GetPixelWidth", HasExplicitThis = true)]
		get;
	}

	/// <summary>
	///   <para>How tall is the camera in pixels (not accounting for dynamic resolution scaling) (Read Only).</para>
	/// </summary>
	public extern int pixelHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("CameraScripting::GetPixelHeight", HasExplicitThis = true)]
		get;
	}

	/// <summary>
	///   <para>How wide is the camera in pixels (accounting for dynamic resolution scaling) (Read Only).</para>
	/// </summary>
	public extern int scaledPixelWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("CameraScripting::GetScaledPixelWidth", HasExplicitThis = true)]
		get;
	}

	/// <summary>
	///   <para>How tall is the camera in pixels (accounting for dynamic resolution scaling) (Read Only).</para>
	/// </summary>
	public extern int scaledPixelHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("CameraScripting::GetScaledPixelHeight", HasExplicitThis = true)]
		get;
	}

	/// <summary>
	///   <para>Destination render texture.</para>
	/// </summary>
	public extern RenderTexture targetTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Gets the temporary RenderTexture target for this Camera.</para>
	/// </summary>
	public extern RenderTexture activeTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetCurrentTargetTexture")]
		get;
	}

	/// <summary>
	///   <para>Matrix that transforms from camera space to world space (Read Only).</para>
	/// </summary>
	public Matrix4x4 cameraToWorldMatrix
	{
		get
		{
			get_cameraToWorldMatrix_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Matrix that transforms from world to camera space.</para>
	/// </summary>
	public Matrix4x4 worldToCameraMatrix
	{
		get
		{
			get_worldToCameraMatrix_Injected(out var ret);
			return ret;
		}
		set
		{
			set_worldToCameraMatrix_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Set a custom projection matrix.</para>
	/// </summary>
	public Matrix4x4 projectionMatrix
	{
		get
		{
			get_projectionMatrix_Injected(out var ret);
			return ret;
		}
		set
		{
			set_projectionMatrix_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Get or set the raw projection matrix with no camera offset (no jittering).</para>
	/// </summary>
	public Matrix4x4 nonJitteredProjectionMatrix
	{
		get
		{
			get_nonJitteredProjectionMatrix_Injected(out var ret);
			return ret;
		}
		set
		{
			set_nonJitteredProjectionMatrix_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Should the jittered matrix be used for transparency rendering?</para>
	/// </summary>
	[NativeProperty("UseJitteredProjectionMatrixForTransparent")]
	public extern bool useJitteredProjectionMatrixForTransparentRendering
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Get the view projection matrix used on the last frame.</para>
	/// </summary>
	public Matrix4x4 previousViewProjectionMatrix
	{
		get
		{
			get_previousViewProjectionMatrix_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>The first enabled camera tagged "MainCamera" (Read Only).</para>
	/// </summary>
	public static extern Camera main
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("FindMainCamera")]
		get;
	}

	/// <summary>
	///   <para>The camera we are currently rendering with, for low-level render control only (Read Only).</para>
	/// </summary>
	public static extern Camera current
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetCurrentCameraPtr")]
		get;
	}

	/// <summary>
	///   <para>Stereoscopic rendering.</para>
	/// </summary>
	public extern bool stereoEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>The distance between the virtual eyes. Use this to query or set the current eye separation. Note that most VR devices provide this value, in which case setting the value will have no effect.</para>
	/// </summary>
	public extern float stereoSeparation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Distance to a point where virtual eyes converge.</para>
	/// </summary>
	public extern float stereoConvergence
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Determines whether the stereo view matrices are suitable to allow for a single pass cull.</para>
	/// </summary>
	public extern bool areVRStereoViewMatricesWithinSingleCullTolerance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("AreVRStereoViewMatricesWithinSingleCullTolerance")]
		get;
	}

	/// <summary>
	///   <para>Defines which eye of a VR display the Camera renders into.</para>
	/// </summary>
	public extern StereoTargetEyeMask stereoTargetEye
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Number of command buffers set up on this camera (Read Only).</para>
	/// </summary>
	public extern int commandBufferCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern string[] GetCameraBufferWarnings();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_scene(out Scene value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_scene(ref Scene value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetTargetBuffersImpl(out RenderBuffer color, out RenderBuffer depth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetTargetBuffersMRTImpl(RenderBuffer[] color, out RenderBuffer depth);

	/// <summary>
	///   <para>Sets the Camera to render to the chosen buffers of one or more RenderTextures.</para>
	/// </summary>
	/// <param name="colorBuffer">The RenderBuffer(s) to which color information will be rendered.</param>
	/// <param name="depthBuffer">The RenderBuffer to which depth information will be rendered.</param>
	public void SetTargetBuffers(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
	{
		SetTargetBuffersImpl(out colorBuffer, out depthBuffer);
	}

	/// <summary>
	///   <para>Sets the Camera to render to the chosen buffers of one or more RenderTextures.</para>
	/// </summary>
	/// <param name="colorBuffer">The RenderBuffer(s) to which color information will be rendered.</param>
	/// <param name="depthBuffer">The RenderBuffer to which depth information will be rendered.</param>
	public void SetTargetBuffers(RenderBuffer[] colorBuffer, RenderBuffer depthBuffer)
	{
		SetTargetBuffersMRTImpl(colorBuffer, out depthBuffer);
	}

	/// <summary>
	///   <para>Fills an array of Camera with the current cameras in the scene, without allocating a new array.</para>
	/// </summary>
	/// <param name="cameras">An array to be filled up with cameras currently in the scene.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern int GetAllCameras(Camera[] cameras);

	/// <summary>
	///   <para>Render the camera manually.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Render();

	/// <summary>
	///   <para>Render the camera with shader replacement.</para>
	/// </summary>
	/// <param name="shader"></param>
	/// <param name="replacementTag"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RenderWithShader(Shader shader, string replacementTag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RenderDontRestore();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetupCurrent(Camera cur);

	[ExcludeFromDocs]
	public bool RenderToCubemap(Cubemap cubemap)
	{
		int faceMask = 63;
		return RenderToCubemap(cubemap, faceMask);
	}

	/// <summary>
	///   <para>Render into a static cubemap from this camera.</para>
	/// </summary>
	/// <param name="cubemap">The cube map to render to.</param>
	/// <param name="faceMask">A bitmask which determines which of the six faces are rendered to.</param>
	/// <returns>
	///   <para>False if rendering fails, else true.</para>
	/// </returns>
	public bool RenderToCubemap(Cubemap cubemap, [DefaultValue("63")] int faceMask)
	{
		return Internal_RenderToCubemapTexture(cubemap, faceMask);
	}

	[ExcludeFromDocs]
	public bool RenderToCubemap(RenderTexture cubemap)
	{
		int faceMask = 63;
		return RenderToCubemap(cubemap, faceMask);
	}

	/// <summary>
	///   <para>Render into a cubemap from this camera.</para>
	/// </summary>
	/// <param name="faceMask">A bitfield indicating which cubemap faces should be rendered into.</param>
	/// <param name="cubemap">The texture to render to.</param>
	/// <returns>
	///   <para>False if rendering fails, else true.</para>
	/// </returns>
	public bool RenderToCubemap(RenderTexture cubemap, [DefaultValue("63")] int faceMask)
	{
		return Internal_RenderToCubemapRT(cubemap, faceMask);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern bool Internal_RenderToCubemapRT(RenderTexture cubemap, int faceMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern bool Internal_RenderToCubemapTexture(Cubemap cubemap, int faceMask);

	/// <summary>
	///   <para>Makes this camera's settings match other camera.</para>
	/// </summary>
	/// <param name="other">Copy camera settings to the other camera.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void CopyFrom(Camera other);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern bool IsFiltered(GameObject go);

	/// <summary>
	///   <para>Get command buffers to be executed at a specified place.</para>
	/// </summary>
	/// <param name="evt">When to execute the command buffer during rendering.</param>
	/// <returns>
	///   <para>Array of command buffers.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern CommandBuffer[] GetCommandBuffers(CameraEvent evt);

	internal void OnlyUsedForTesting1()
	{
	}

	internal void OnlyUsedForTesting2()
	{
	}

	/// <summary>
	///   <para>Resets this Camera's transparency sort settings to the default. Default transparency settings are taken from GraphicsSettings instead of directly from this Camera.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetTransparencySortSettings();

	/// <summary>
	///   <para>Revert the aspect ratio to the screen's aspect ratio.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetAspect();

	/// <summary>
	///   <para>Make culling queries reflect the camera's built in parameters.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetCullingMatrix();

	/// <summary>
	///   <para>Make the camera render with shader replacement.</para>
	/// </summary>
	/// <param name="shader"></param>
	/// <param name="replacementTag"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetReplacementShader(Shader shader, string replacementTag);

	/// <summary>
	///   <para>Remove shader replacement from camera.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetReplacementShader();

	/// <summary>
	///   <para>Make the rendering position reflect the camera's position in the scene.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetWorldToCameraMatrix();

	/// <summary>
	///   <para>Make the projection reflect normal camera's parameters.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetProjectionMatrix();

	/// <summary>
	///   <para>Calculates and returns oblique near-plane projection matrix.</para>
	/// </summary>
	/// <param name="clipPlane">Vector4 that describes a clip plane.</param>
	/// <returns>
	///   <para>Oblique near-plane projection matrix.</para>
	/// </returns>
	[FreeFunction("CameraScripting::CalculateObliqueMatrix", HasExplicitThis = true)]
	public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane)
	{
		CalculateObliqueMatrix_Injected(ref clipPlane, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Transforms position from world space into screen space.</para>
	/// </summary>
	/// <param name="position"></param>
	public Vector3 WorldToScreenPoint(Vector3 position)
	{
		WorldToScreenPoint_Injected(ref position, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Transforms position from world space into viewport space.</para>
	/// </summary>
	/// <param name="position"></param>
	public Vector3 WorldToViewportPoint(Vector3 position)
	{
		WorldToViewportPoint_Injected(ref position, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Transforms position from viewport space into world space.</para>
	/// </summary>
	/// <param name="position">The 3d vector in Viewport space.</param>
	/// <returns>
	///   <para>The 3d vector in World space.</para>
	/// </returns>
	public Vector3 ViewportToWorldPoint(Vector3 position)
	{
		ViewportToWorldPoint_Injected(ref position, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Transforms position from screen space into world space.</para>
	/// </summary>
	/// <param name="position"></param>
	public Vector3 ScreenToWorldPoint(Vector3 position)
	{
		ScreenToWorldPoint_Injected(ref position, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Transforms position from screen space into viewport space.</para>
	/// </summary>
	/// <param name="position"></param>
	public Vector3 ScreenToViewportPoint(Vector3 position)
	{
		ScreenToViewportPoint_Injected(ref position, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Transforms position from viewport space into screen space.</para>
	/// </summary>
	/// <param name="position"></param>
	public Vector3 ViewportToScreenPoint(Vector3 position)
	{
		ViewportToScreenPoint_Injected(ref position, out var ret);
		return ret;
	}

	private Ray ViewportPointToRay(Vector2 pos)
	{
		ViewportPointToRay_Injected(ref pos, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Returns a ray going from camera through a viewport point.</para>
	/// </summary>
	/// <param name="pos"></param>
	public Ray ViewportPointToRay(Vector3 pos)
	{
		return ViewportPointToRay((Vector2)pos);
	}

	private Ray ScreenPointToRay(Vector2 pos)
	{
		ScreenPointToRay_Injected(ref pos, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Returns a ray going from camera through a screen point.</para>
	/// </summary>
	/// <param name="pos"></param>
	public Ray ScreenPointToRay(Vector3 pos)
	{
		return ScreenPointToRay((Vector2)pos);
	}

	[FreeFunction("CameraScripting::RaycastTry", HasExplicitThis = true)]
	internal GameObject RaycastTry(Ray ray, float distance, int layerMask)
	{
		return RaycastTry_Injected(ref ray, distance, layerMask);
	}

	[FreeFunction("CameraScripting::RaycastTry2D", HasExplicitThis = true)]
	internal GameObject RaycastTry2D(Ray ray, float distance, int layerMask)
	{
		return RaycastTry2D_Injected(ref ray, distance, layerMask);
	}

	[FreeFunction("CameraScripting::CalculateViewportRayVectors", HasExplicitThis = true)]
	private void CalculateFrustumCornersInternal(Rect viewport, float z, MonoOrStereoscopicEye eye, [Out] Vector3[] outCorners)
	{
		CalculateFrustumCornersInternal_Injected(ref viewport, z, eye, outCorners);
	}

	public void CalculateFrustumCorners(Rect viewport, float z, MonoOrStereoscopicEye eye, Vector3[] outCorners)
	{
		if (outCorners == null)
		{
			throw new ArgumentNullException("outCorners");
		}
		if (outCorners.Length < 4)
		{
			throw new ArgumentException("outCorners minimum size is 4", "outCorners");
		}
		CalculateFrustumCornersInternal(viewport, z, eye, outCorners);
	}

	public Matrix4x4 GetStereoNonJitteredProjectionMatrix(StereoscopicEye eye)
	{
		GetStereoNonJitteredProjectionMatrix_Injected(eye, out var ret);
		return ret;
	}

	public Matrix4x4 GetStereoViewMatrix(StereoscopicEye eye)
	{
		GetStereoViewMatrix_Injected(eye, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void CopyStereoDeviceProjectionMatrixToNonJittered(StereoscopicEye eye);

	public Matrix4x4 GetStereoProjectionMatrix(StereoscopicEye eye)
	{
		GetStereoProjectionMatrix_Injected(eye, out var ret);
		return ret;
	}

	public void SetStereoProjectionMatrix(StereoscopicEye eye, Matrix4x4 matrix)
	{
		SetStereoProjectionMatrix_Injected(eye, ref matrix);
	}

	/// <summary>
	///   <para>Reset the camera to using the Unity computed projection matrices for all stereoscopic eyes.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetStereoProjectionMatrices();

	public void SetStereoViewMatrix(StereoscopicEye eye, Matrix4x4 matrix)
	{
		SetStereoViewMatrix_Injected(eye, ref matrix);
	}

	/// <summary>
	///   <para>Reset the camera to using the Unity computed view matrices for all stereoscopic eyes.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetStereoViewMatrices();

	/// <summary>
	///   <para>Remove command buffers from execution at a specified place.</para>
	/// </summary>
	/// <param name="evt">When to execute the command buffer during rendering.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void RemoveCommandBuffers(CameraEvent evt);

	/// <summary>
	///   <para>Remove all command buffers set on this camera.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void RemoveAllCommandBuffers();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("RenderToCubemap")]
	private extern bool RenderToCubemapImpl(RenderTexture cubemap, int faceMask, MonoOrStereoscopicEye stereoEye);

	public bool RenderToCubemap(RenderTexture cubemap, int faceMask, MonoOrStereoscopicEye stereoEye)
	{
		return RenderToCubemapImpl(cubemap, faceMask, stereoEye);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("AddCommandBuffer")]
	private extern void AddCommandBufferImpl(CameraEvent evt, [NotNull] CommandBuffer buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("AddCommandBufferAsync")]
	private extern void AddCommandBufferAsyncImpl(CameraEvent evt, [NotNull] CommandBuffer buffer, ComputeQueueType queueType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("RemoveCommandBuffer")]
	private extern void RemoveCommandBufferImpl(CameraEvent evt, [NotNull] CommandBuffer buffer);

	/// <summary>
	///   <para>Add a command buffer to be executed at a specified place.</para>
	/// </summary>
	/// <param name="evt">When to execute the command buffer during rendering.</param>
	/// <param name="buffer">The buffer to execute.</param>
	public void AddCommandBuffer(CameraEvent evt, CommandBuffer buffer)
	{
		if (buffer == null)
		{
			throw new NullReferenceException("buffer is null");
		}
		AddCommandBufferImpl(evt, buffer);
	}

	/// <summary>
	///   <para>Adds a command buffer to the GPU's async compute queues and executes that command buffer when graphics processing reaches a given point.</para>
	/// </summary>
	/// <param name="evt">The point during the graphics processing at which this command buffer should commence on the GPU.</param>
	/// <param name="buffer">The buffer to execute.</param>
	/// <param name="queueType">The desired async compute queue type to execute the buffer on.</param>
	public void AddCommandBufferAsync(CameraEvent evt, CommandBuffer buffer, ComputeQueueType queueType)
	{
		if (buffer == null)
		{
			throw new NullReferenceException("buffer is null");
		}
		AddCommandBufferAsyncImpl(evt, buffer, queueType);
	}

	/// <summary>
	///   <para>Remove command buffer from execution at a specified place.</para>
	/// </summary>
	/// <param name="evt">When to execute the command buffer during rendering.</param>
	/// <param name="buffer">The buffer to execute.</param>
	public void RemoveCommandBuffer(CameraEvent evt, CommandBuffer buffer)
	{
		if (buffer == null)
		{
			throw new NullReferenceException("buffer is null");
		}
		RemoveCommandBufferImpl(evt, buffer);
	}

	[RequiredByNativeCode]
	private static void FireOnPreCull(Camera cam)
	{
		if (onPreCull != null)
		{
			onPreCull(cam);
		}
	}

	[RequiredByNativeCode]
	private static void FireOnPreRender(Camera cam)
	{
		if (onPreRender != null)
		{
			onPreRender(cam);
		}
	}

	[RequiredByNativeCode]
	private static void FireOnPostRender(Camera cam)
	{
		if (onPostRender != null)
		{
			onPostRender(cam);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_transparencySortAxis_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_transparencySortAxis_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_velocity_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_cullingMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_cullingMatrix_Injected(ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_backgroundColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_backgroundColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_rect_Injected(out Rect ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_rect_Injected(ref Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_pixelRect_Injected(out Rect ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_pixelRect_Injected(ref Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_cameraToWorldMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_worldToCameraMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_worldToCameraMatrix_Injected(ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_projectionMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_projectionMatrix_Injected(ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_nonJitteredProjectionMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_nonJitteredProjectionMatrix_Injected(ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_previousViewProjectionMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void CalculateObliqueMatrix_Injected(ref Vector4 clipPlane, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void WorldToScreenPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void WorldToViewportPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ViewportToWorldPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ScreenToWorldPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ScreenToViewportPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ViewportToScreenPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ViewportPointToRay_Injected(ref Vector2 pos, out Ray ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ScreenPointToRay_Injected(ref Vector2 pos, out Ray ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern GameObject RaycastTry_Injected(ref Ray ray, float distance, int layerMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern GameObject RaycastTry2D_Injected(ref Ray ray, float distance, int layerMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void CalculateFrustumCornersInternal_Injected(ref Rect viewport, float z, MonoOrStereoscopicEye eye, [Out] Vector3[] outCorners);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetStereoNonJitteredProjectionMatrix_Injected(StereoscopicEye eye, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetStereoViewMatrix_Injected(StereoscopicEye eye, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetStereoProjectionMatrix_Injected(StereoscopicEye eye, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetStereoProjectionMatrix_Injected(StereoscopicEye eye, ref Matrix4x4 matrix);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetStereoViewMatrix_Injected(StereoscopicEye eye, ref Matrix4x4 matrix);
}

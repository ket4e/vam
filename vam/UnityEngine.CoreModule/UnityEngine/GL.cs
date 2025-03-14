using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

/// <summary>
///   <para>Low-level graphics library.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Camera/Camera.h")]
[NativeHeader("Runtime/GfxDevice/GfxDevice.h")]
[StaticAccessor("GetGfxDevice()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Camera/CameraUtil.h")]
public sealed class GL
{
	/// <summary>
	///   <para>Mode for Begin: draw triangles.</para>
	/// </summary>
	public const int TRIANGLES = 4;

	/// <summary>
	///   <para>Mode for Begin: draw triangle strip.</para>
	/// </summary>
	public const int TRIANGLE_STRIP = 5;

	/// <summary>
	///   <para>Mode for Begin: draw quads.</para>
	/// </summary>
	public const int QUADS = 7;

	/// <summary>
	///   <para>Mode for Begin: draw lines.</para>
	/// </summary>
	public const int LINES = 1;

	/// <summary>
	///   <para>Mode for Begin: draw line strip.</para>
	/// </summary>
	public const int LINE_STRIP = 2;

	/// <summary>
	///   <para>Should rendering be done in wireframe?</para>
	/// </summary>
	public static extern bool wireframe
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Controls whether Linear-to-sRGB color conversion is performed while rendering.</para>
	/// </summary>
	public static extern bool sRGBWrite
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Select whether to invert the backface culling (true) or not (false).</para>
	/// </summary>
	[NativeProperty("UserBackfaceMode")]
	public static extern bool invertCulling
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The current modelview matrix.</para>
	/// </summary>
	public static Matrix4x4 modelview
	{
		get
		{
			return GetWorldViewMatrix();
		}
		set
		{
			SetViewMatrix(value);
		}
	}

	/// <summary>
	///   <para>Submit a vertex.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ImmediateVertex")]
	public static extern void Vertex3(float x, float y, float z);

	/// <summary>
	///   <para>Submit a vertex.</para>
	/// </summary>
	/// <param name="v"></param>
	public static void Vertex(Vector3 v)
	{
		Vertex3(v.x, v.y, v.z);
	}

	/// <summary>
	///   <para>Sets current texture coordinate (x,y,z) for all texture units.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ImmediateTexCoordAll")]
	public static extern void TexCoord3(float x, float y, float z);

	/// <summary>
	///   <para>Sets current texture coordinate (v.x,v.y,v.z) for all texture units.</para>
	/// </summary>
	/// <param name="v"></param>
	public static void TexCoord(Vector3 v)
	{
		TexCoord3(v.x, v.y, v.z);
	}

	/// <summary>
	///   <para>Sets current texture coordinate (x,y) for all texture units.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void TexCoord2(float x, float y)
	{
		TexCoord3(x, y, 0f);
	}

	/// <summary>
	///   <para>Sets current texture coordinate (x,y,z) to the actual texture unit.</para>
	/// </summary>
	/// <param name="unit"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ImmediateTexCoord")]
	public static extern void MultiTexCoord3(int unit, float x, float y, float z);

	/// <summary>
	///   <para>Sets current texture coordinate (v.x,v.y,v.z) to the actual texture unit.</para>
	/// </summary>
	/// <param name="unit"></param>
	/// <param name="v"></param>
	public static void MultiTexCoord(int unit, Vector3 v)
	{
		MultiTexCoord3(unit, v.x, v.y, v.z);
	}

	/// <summary>
	///   <para>Sets current texture coordinate (x,y) for the actual texture unit.</para>
	/// </summary>
	/// <param name="unit"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void MultiTexCoord2(int unit, float x, float y)
	{
		MultiTexCoord3(unit, x, y, 0f);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ImmediateColor")]
	private static extern void ImmediateColor(float r, float g, float b, float a);

	/// <summary>
	///   <para>Sets current vertex color.</para>
	/// </summary>
	/// <param name="c"></param>
	public static void Color(Color c)
	{
		ImmediateColor(c.r, c.g, c.b, c.a);
	}

	/// <summary>
	///   <para>Sends queued-up commands in the driver's command buffer to the GPU.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void Flush();

	/// <summary>
	///   <para>Resolves the render target for subsequent operations sampling from it.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void RenderTargetBarrier();

	private static Matrix4x4 GetWorldViewMatrix()
	{
		GetWorldViewMatrix_Injected(out var ret);
		return ret;
	}

	private static void SetViewMatrix(Matrix4x4 m)
	{
		SetViewMatrix_Injected(ref m);
	}

	/// <summary>
	///   <para>Multiplies the current modelview matrix with the one specified.</para>
	/// </summary>
	/// <param name="m"></param>
	[NativeName("SetWorldMatrix")]
	public static void MultMatrix(Matrix4x4 m)
	{
		MultMatrix_Injected(ref m);
	}

	/// <summary>
	///   <para>Send a user-defined event to a native code plugin.</para>
	/// </summary>
	/// <param name="eventID">User defined id to send to the callback.</param>
	/// <param name="callback">Native code callback to queue for Unity's renderer to invoke.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("IssuePluginEvent(eventID) is deprecated. Use IssuePluginEvent(callback, eventID) instead.", false)]
	[NativeName("InsertCustomMarker")]
	public static extern void IssuePluginEvent(int eventID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("SetRevertBackfacing(revertBackFaces) is deprecated. Use invertCulling property instead.", false)]
	[NativeName("SetUserBackfaceMode")]
	public static extern void SetRevertBackfacing(bool revertBackFaces);

	/// <summary>
	///   <para>Saves both projection and modelview matrices to the matrix stack.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GLPushMatrixScript")]
	public static extern void PushMatrix();

	/// <summary>
	///   <para>Restores both projection and modelview matrices off the top of the matrix stack.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GLPopMatrixScript")]
	public static extern void PopMatrix();

	/// <summary>
	///   <para>Load the identity matrix to the current modelview matrix.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GLLoadIdentityScript")]
	public static extern void LoadIdentity();

	/// <summary>
	///   <para>Helper function to set up an ortho perspective transform.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GLLoadOrthoScript")]
	public static extern void LoadOrtho();

	/// <summary>
	///   <para>Setup a matrix for pixel-correct rendering.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GLLoadPixelMatrixScript")]
	public static extern void LoadPixelMatrix();

	/// <summary>
	///   <para>Load an arbitrary matrix to the current projection matrix.</para>
	/// </summary>
	/// <param name="mat"></param>
	[FreeFunction("GLLoadProjectionMatrixScript")]
	public static void LoadProjectionMatrix(Matrix4x4 mat)
	{
		LoadProjectionMatrix_Injected(ref mat);
	}

	/// <summary>
	///   <para>Invalidate the internally cached render state.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GLInvalidateState")]
	public static extern void InvalidateState();

	/// <summary>
	///   <para>Compute GPU projection matrix from camera's projection matrix.</para>
	/// </summary>
	/// <param name="proj">Source projection matrix.</param>
	/// <param name="renderIntoTexture">Will this projection be used for rendering into a RenderTexture?</param>
	/// <returns>
	///   <para>Adjusted projection matrix for the current graphics API.</para>
	/// </returns>
	[FreeFunction("GLGetGPUProjectionMatrix")]
	public static Matrix4x4 GetGPUProjectionMatrix(Matrix4x4 proj, bool renderIntoTexture)
	{
		GetGPUProjectionMatrix_Injected(ref proj, renderIntoTexture, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern void GLLoadPixelMatrixScript(float left, float right, float bottom, float top);

	/// <summary>
	///   <para>Setup a matrix for pixel-correct rendering.</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="bottom"></param>
	/// <param name="top"></param>
	public static void LoadPixelMatrix(float left, float right, float bottom, float top)
	{
		GLLoadPixelMatrixScript(left, right, bottom, top);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern void GLIssuePluginEvent(IntPtr callback, int eventID);

	/// <summary>
	///   <para>Send a user-defined event to a native code plugin.</para>
	/// </summary>
	/// <param name="eventID">User defined id to send to the callback.</param>
	/// <param name="callback">Native code callback to queue for Unity's renderer to invoke.</param>
	public static void IssuePluginEvent(IntPtr callback, int eventID)
	{
		if (callback == IntPtr.Zero)
		{
			throw new ArgumentException("Null callback specified.", "callback");
		}
		GLIssuePluginEvent(callback, eventID);
	}

	/// <summary>
	///   <para>Begin drawing 3D primitives.</para>
	/// </summary>
	/// <param name="mode">Primitives to draw: can be TRIANGLES, TRIANGLE_STRIP, QUADS or LINES.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GLBegin", ThrowsException = true)]
	public static extern void Begin(int mode);

	/// <summary>
	///   <para>End drawing 3D primitives.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GLEnd")]
	public static extern void End();

	[FreeFunction]
	private static void GLClear(bool clearDepth, bool clearColor, Color backgroundColor, float depth)
	{
		GLClear_Injected(clearDepth, clearColor, ref backgroundColor, depth);
	}

	/// <summary>
	///   <para>Clear the current render buffer.</para>
	/// </summary>
	/// <param name="clearDepth">Should the depth buffer be cleared?</param>
	/// <param name="clearColor">Should the color buffer be cleared?</param>
	/// <param name="backgroundColor">The color to clear with, used only if clearColor is true.</param>
	/// <param name="depth">The depth to clear Z buffer with, used only if clearDepth is true.</param>
	public static void Clear(bool clearDepth, bool clearColor, Color backgroundColor, [DefaultValue("1.0f")] float depth)
	{
		GLClear(clearDepth, clearColor, backgroundColor, depth);
	}

	public static void Clear(bool clearDepth, bool clearColor, Color backgroundColor)
	{
		GLClear(clearDepth, clearColor, backgroundColor, 1f);
	}

	/// <summary>
	///   <para>Set the rendering viewport.</para>
	/// </summary>
	/// <param name="pixelRect"></param>
	[FreeFunction("SetGLViewport")]
	public static void Viewport(Rect pixelRect)
	{
		Viewport_Injected(ref pixelRect);
	}

	/// <summary>
	///   <para>Clear the current render buffer with camera's skybox.</para>
	/// </summary>
	/// <param name="clearDepth">Should the depth buffer be cleared?</param>
	/// <param name="camera">Camera to get projection parameters and skybox from.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ClearWithSkybox")]
	public static extern void ClearWithSkybox(bool clearDepth, Camera camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetWorldViewMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetViewMatrix_Injected(ref Matrix4x4 m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MultMatrix_Injected(ref Matrix4x4 m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void LoadProjectionMatrix_Injected(ref Matrix4x4 mat);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGPUProjectionMatrix_Injected(ref Matrix4x4 proj, bool renderIntoTexture, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GLClear_Injected(bool clearDepth, bool clearColor, ref Color backgroundColor, float depth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Viewport_Injected(ref Rect pixelRect);
}

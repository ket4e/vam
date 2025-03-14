using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>WebCam Textures are textures onto which the live video input is rendered.</para>
/// </summary>
public sealed class WebCamTexture : Texture
{
	/// <summary>
	///   <para>Returns if the camera is currently playing.</para>
	/// </summary>
	public extern bool isPlaying
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Set this to specify the name of the device to use.</para>
	/// </summary>
	public extern string deviceName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Set the requested frame rate of the camera device (in frames per second).</para>
	/// </summary>
	public extern float requestedFPS
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Set the requested width of the camera device.</para>
	/// </summary>
	public extern int requestedWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Set the requested height of the camera device.</para>
	/// </summary>
	public extern int requestedHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Return a list of available devices.</para>
	/// </summary>
	public static extern WebCamDevice[] devices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Returns an clockwise angle (in degrees), which can be used to rotate a polygon so camera contents are shown in correct orientation.</para>
	/// </summary>
	public extern int videoRotationAngle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Returns if the texture image is vertically flipped.</para>
	/// </summary>
	public extern bool videoVerticallyMirrored
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Did the video buffer update this frame?</para>
	/// </summary>
	public extern bool didUpdateThisFrame
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Create a WebCamTexture.</para>
	/// </summary>
	/// <param name="deviceName">The name of the video input device to be used.</param>
	/// <param name="requestedWidth">The requested width of the texture.</param>
	/// <param name="requestedHeight">The requested height of the texture.</param>
	/// <param name="requestedFPS">The requested frame rate of the texture.</param>
	public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight, int requestedFPS)
	{
		Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, requestedFPS);
	}

	/// <summary>
	///   <para>Create a WebCamTexture.</para>
	/// </summary>
	/// <param name="deviceName">The name of the video input device to be used.</param>
	/// <param name="requestedWidth">The requested width of the texture.</param>
	/// <param name="requestedHeight">The requested height of the texture.</param>
	/// <param name="requestedFPS">The requested frame rate of the texture.</param>
	public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight)
	{
		Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, 0);
	}

	/// <summary>
	///   <para>Create a WebCamTexture.</para>
	/// </summary>
	/// <param name="deviceName">The name of the video input device to be used.</param>
	/// <param name="requestedWidth">The requested width of the texture.</param>
	/// <param name="requestedHeight">The requested height of the texture.</param>
	/// <param name="requestedFPS">The requested frame rate of the texture.</param>
	public WebCamTexture(string deviceName)
	{
		Internal_CreateWebCamTexture(this, deviceName, 0, 0, 0);
	}

	/// <summary>
	///   <para>Create a WebCamTexture.</para>
	/// </summary>
	/// <param name="deviceName">The name of the video input device to be used.</param>
	/// <param name="requestedWidth">The requested width of the texture.</param>
	/// <param name="requestedHeight">The requested height of the texture.</param>
	/// <param name="requestedFPS">The requested frame rate of the texture.</param>
	public WebCamTexture(int requestedWidth, int requestedHeight, int requestedFPS)
	{
		Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, requestedFPS);
	}

	/// <summary>
	///   <para>Create a WebCamTexture.</para>
	/// </summary>
	/// <param name="deviceName">The name of the video input device to be used.</param>
	/// <param name="requestedWidth">The requested width of the texture.</param>
	/// <param name="requestedHeight">The requested height of the texture.</param>
	/// <param name="requestedFPS">The requested frame rate of the texture.</param>
	public WebCamTexture(int requestedWidth, int requestedHeight)
	{
		Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, 0);
	}

	/// <summary>
	///   <para>Create a WebCamTexture.</para>
	/// </summary>
	/// <param name="deviceName">The name of the video input device to be used.</param>
	/// <param name="requestedWidth">The requested width of the texture.</param>
	/// <param name="requestedHeight">The requested height of the texture.</param>
	/// <param name="requestedFPS">The requested frame rate of the texture.</param>
	public WebCamTexture()
	{
		Internal_CreateWebCamTexture(this, "", 0, 0, 0);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_CreateWebCamTexture([Writable] WebCamTexture self, string scriptingDevice, int requestedWidth, int requestedHeight, int maxFramerate);

	/// <summary>
	///   <para>Starts the camera.</para>
	/// </summary>
	public void Play()
	{
		INTERNAL_CALL_Play(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Play(WebCamTexture self);

	/// <summary>
	///   <para>Pauses the camera.</para>
	/// </summary>
	public void Pause()
	{
		INTERNAL_CALL_Pause(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Pause(WebCamTexture self);

	/// <summary>
	///   <para>Stops the camera.</para>
	/// </summary>
	public void Stop()
	{
		INTERNAL_CALL_Stop(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Stop(WebCamTexture self);

	/// <summary>
	///   <para>Returns pixel color at coordinates (x, y).</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public Color GetPixel(int x, int y)
	{
		INTERNAL_CALL_GetPixel(this, x, y, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetPixel(WebCamTexture self, int x, int y, out Color value);

	/// <summary>
	///   <para>Get a block of pixel colors.</para>
	/// </summary>
	public Color[] GetPixels()
	{
		return GetPixels(0, 0, width, height);
	}

	/// <summary>
	///   <para>Get a block of pixel colors.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="blockWidth"></param>
	/// <param name="blockHeight"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight);

	/// <summary>
	///   <para>Returns the pixels data in raw format.</para>
	/// </summary>
	/// <param name="colors">Optional array to receive pixel data.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Color32[] GetPixels32([DefaultValue("null")] Color32[] colors);

	[ExcludeFromDocs]
	public Color32[] GetPixels32()
	{
		Color32[] colors = null;
		return GetPixels32(colors);
	}
}

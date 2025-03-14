using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking;

/// <summary>
///   <para>A general-purpose UploadHandler subclass, using a native-code memory buffer.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/UnityWebRequest/Public/UploadHandler/UploadHandlerRaw.h")]
public sealed class UploadHandlerRaw : UploadHandler
{
	/// <summary>
	///   <para>General constructor. Contents of the input argument are copied into a native buffer.</para>
	/// </summary>
	/// <param name="data">Raw data to transmit to the remote server.</param>
	public UploadHandlerRaw(byte[] data)
	{
		if (data != null && data.Length == 0)
		{
			throw new ArgumentException("Cannot create a data handler without payload data");
		}
		m_Ptr = Create(this, data);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create(UploadHandlerRaw self, byte[] data);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetContentType")]
	private extern string InternalGetContentType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("SetContentType")]
	private extern void InternalSetContentType(string newContentType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern byte[] InternalGetData();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetProgress")]
	private extern float InternalGetProgress();

	internal override string GetContentType()
	{
		return InternalGetContentType();
	}

	internal override void SetContentType(string newContentType)
	{
		InternalSetContentType(newContentType);
	}

	internal override byte[] GetData()
	{
		return InternalGetData();
	}

	internal override float GetProgress()
	{
		return InternalGetProgress();
	}
}

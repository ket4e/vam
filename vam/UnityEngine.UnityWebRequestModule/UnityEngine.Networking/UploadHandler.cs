using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking;

/// <summary>
///   <para>Helper object for UnityWebRequests. Manages the buffering and transmission of body data during HTTP requests.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/UnityWebRequest/Public/UploadHandler/UploadHandler.h")]
public class UploadHandler : IDisposable
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	/// <summary>
	///   <para>The raw data which will be transmitted to the remote server as body data. (Read Only)</para>
	/// </summary>
	public byte[] data => GetData();

	/// <summary>
	///   <para>Determines the default Content-Type header which will be transmitted with the outbound HTTP request.</para>
	/// </summary>
	public string contentType
	{
		get
		{
			return GetContentType();
		}
		set
		{
			SetContentType(value);
		}
	}

	/// <summary>
	///   <para>Returns the proportion of data uploaded to the remote server compared to the total amount of data to upload. (Read Only)</para>
	/// </summary>
	public float progress => GetProgress();

	internal UploadHandler()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern void Release();

	~UploadHandler()
	{
		Dispose();
	}

	/// <summary>
	///   <para>Signals that this [UploadHandler] is no longer being used, and should clean up any resources it is using.</para>
	/// </summary>
	public void Dispose()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Release();
			m_Ptr = IntPtr.Zero;
		}
	}

	internal virtual byte[] GetData()
	{
		return null;
	}

	internal virtual string GetContentType()
	{
		return "text/plain";
	}

	internal virtual void SetContentType(string newContentType)
	{
	}

	internal virtual float GetProgress()
	{
		return 0.5f;
	}
}

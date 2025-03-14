using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Networking;

/// <summary>
///   <para>Manage and process HTTP response body data received from a remote server.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/UnityWebRequest/Public/DownloadHandler/DownloadHandler.h")]
public class DownloadHandler : IDisposable
{
	[NonSerialized]
	[VisibleToOtherModules]
	internal IntPtr m_Ptr;

	/// <summary>
	///   <para>Returns true if this DownloadHandler has been informed by its parent UnityWebRequest that all data has been received, and this DownloadHandler has completed any necessary post-download processing. (Read Only)</para>
	/// </summary>
	public bool isDone => IsDone();

	/// <summary>
	///   <para>Returns the raw bytes downloaded from the remote server, or null. (Read Only)</para>
	/// </summary>
	public byte[] data => GetData();

	/// <summary>
	///   <para>Convenience property. Returns the bytes from data interpreted as a UTF8 string. (Read Only)</para>
	/// </summary>
	public string text => GetText();

	[VisibleToOtherModules]
	internal DownloadHandler()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern void Release();

	~DownloadHandler()
	{
		Dispose();
	}

	/// <summary>
	///   <para>Signals that this [DownloadHandler] is no longer being used, and should clean up any resources it is using.</para>
	/// </summary>
	public void Dispose()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Release();
			m_Ptr = IntPtr.Zero;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool IsDone();

	/// <summary>
	///   <para>Callback, invoked when the data property is accessed.</para>
	/// </summary>
	/// <returns>
	///   <para>Byte array to return as the value of the data property.</para>
	/// </returns>
	protected virtual byte[] GetData()
	{
		return null;
	}

	/// <summary>
	///   <para>Callback, invoked when the text property is accessed.</para>
	/// </summary>
	/// <returns>
	///   <para>String to return as the return value of the text property.</para>
	/// </returns>
	protected virtual string GetText()
	{
		byte[] array = GetData();
		if (array != null && array.Length > 0)
		{
			return GetTextEncoder().GetString(array, 0, array.Length);
		}
		return "";
	}

	private Encoding GetTextEncoder()
	{
		string contentType = GetContentType();
		if (!string.IsNullOrEmpty(contentType))
		{
			int num = contentType.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
			if (num > -1)
			{
				int num2 = contentType.IndexOf('=', num);
				if (num2 > -1)
				{
					string text = contentType.Substring(num2 + 1).Trim().Trim('\'', '"')
						.Trim();
					int num3 = text.IndexOf(';');
					if (num3 > -1)
					{
						text = text.Substring(0, num3);
					}
					try
					{
						return Encoding.GetEncoding(text);
					}
					catch (ArgumentException ex)
					{
						Debug.LogWarning($"Unsupported encoding '{text}': {ex.Message}");
					}
				}
			}
		}
		return Encoding.UTF8;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern string GetContentType();

	/// <summary>
	///   <para>Callback, invoked as data is received from the remote server.</para>
	/// </summary>
	/// <param name="data">A buffer containing unprocessed data, received from the remote server.</param>
	/// <param name="dataLength">The number of bytes in data which are new.</param>
	/// <returns>
	///   <para>True if the download should continue, false to abort.</para>
	/// </returns>
	[UsedByNativeCode]
	protected virtual bool ReceiveData(byte[] data, int dataLength)
	{
		return true;
	}

	/// <summary>
	///   <para>Callback, invoked with a Content-Length header is received.</para>
	/// </summary>
	/// <param name="contentLength">The value of the received Content-Length header.</param>
	[UsedByNativeCode]
	protected virtual void ReceiveContentLength(int contentLength)
	{
	}

	/// <summary>
	///   <para>Callback, invoked when all data has been received from the remote server.</para>
	/// </summary>
	[UsedByNativeCode]
	protected virtual void CompleteContent()
	{
	}

	/// <summary>
	///   <para>Callback, invoked when UnityWebRequest.downloadProgress is accessed.</para>
	/// </summary>
	/// <returns>
	///   <para>The return value for UnityWebRequest.downloadProgress.</para>
	/// </returns>
	[UsedByNativeCode]
	protected virtual float GetProgress()
	{
		return 0f;
	}

	protected static T GetCheckedDownloader<T>(UnityWebRequest www) where T : DownloadHandler
	{
		if (www == null)
		{
			throw new NullReferenceException("Cannot get content from a null UnityWebRequest object");
		}
		if (!www.isDone)
		{
			throw new InvalidOperationException("Cannot get content from an unfinished UnityWebRequest object");
		}
		if (www.isNetworkError)
		{
			throw new InvalidOperationException(www.error);
		}
		return (T)www.downloadHandler;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	[VisibleToOtherModules]
	internal static extern byte[] InternalGetByteArray(DownloadHandler dh);
}

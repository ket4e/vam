using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking;

/// <summary>
///   <para>Download handler for saving the downloaded data to file.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/UnityWebRequest/Public/DownloadHandler/DownloadHandlerVFS.h")]
public sealed class DownloadHandlerFile : DownloadHandler
{
	/// <summary>
	///   <para>Should the created file be removed if download is aborted (manually or due to an error). Default: false.</para>
	/// </summary>
	public extern bool removeFileOnAbort
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Creates a new instance and a file on disk where downloaded data will be written to.</para>
	/// </summary>
	/// <param name="path">Path to file to be written.</param>
	public DownloadHandlerFile(string path)
	{
		InternalCreateVFS(path);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern IntPtr Create(DownloadHandlerFile obj, string path);

	private void InternalCreateVFS(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		m_Ptr = Create(this, path);
	}

	protected override byte[] GetData()
	{
		throw new NotSupportedException("Raw data access is not supported");
	}

	protected override string GetText()
	{
		throw new NotSupportedException("String access is not supported");
	}
}

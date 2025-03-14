using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking;

/// <summary>
///   <para>A specialized DownloadHandler for creating MovieTexture out of downloaded bytes.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Runtime/Video/MovieTexture.h")]
[NativeHeader("Modules/UnityWebRequestAudio/Public/DownloadHandlerMovieTexture.h")]
public sealed class DownloadHandlerMovieTexture : DownloadHandler
{
	/// <summary>
	///   <para>A MovieTexture created out of downloaded bytes.</para>
	/// </summary>
	public extern MovieTexture movieTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Create new DownloadHandlerMovieTexture.</para>
	/// </summary>
	public DownloadHandlerMovieTexture()
	{
		InternalCreateDHMovieTexture();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create(DownloadHandlerMovieTexture obj);

	private void InternalCreateDHMovieTexture()
	{
		m_Ptr = Create(this);
	}

	/// <summary>
	///   <para>Raw downloaded data.</para>
	/// </summary>
	/// <returns>
	///   <para>Raw downloaded bytes.</para>
	/// </returns>
	protected override byte[] GetData()
	{
		return DownloadHandler.InternalGetByteArray(this);
	}

	protected override string GetText()
	{
		throw new NotSupportedException("String access is not supported for movies");
	}

	/// <summary>
	///   <para>A convenience (helper) method for casting DownloadHandler to DownloadHandlerMovieTexture and accessing its movieTexture property.</para>
	/// </summary>
	/// <param name="uwr">A UnityWebRequest with attached DownloadHandlerMovieTexture.</param>
	/// <returns>
	///   <para>A MovieTexture created out of downloaded bytes.</para>
	/// </returns>
	public static MovieTexture GetContent(UnityWebRequest uwr)
	{
		return DownloadHandler.GetCheckedDownloader<DownloadHandlerMovieTexture>(uwr).movieTexture;
	}
}

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Networking;

/// <summary>
///   <para>Responsible for rejecting or accepting certificates received on https requests.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/UnityWebRequest/Public/CertificateHandler/CertificateHandlerScript.h")]
public class CertificateHandler : IDisposable
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	protected CertificateHandler()
	{
		m_Ptr = Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create(CertificateHandler obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern void Release();

	~CertificateHandler()
	{
		Dispose();
	}

	/// <summary>
	///   <para>Callback, invoked for each leaf certificate sent by the remote server.</para>
	/// </summary>
	/// <param name="certificateData">Certificate data in PEM or DER format. If certificate data contains multiple certificates, the first one is the leaf certificate.</param>
	/// <returns>
	///   <para>true if the certificate should be accepted, false if not.</para>
	/// </returns>
	protected virtual bool ValidateCertificate(byte[] certificateData)
	{
		return false;
	}

	[RequiredByNativeCode]
	internal bool ValidateCertificateNative(byte[] certificateData)
	{
		return ValidateCertificate(certificateData);
	}

	/// <summary>
	///   <para>Signals that this [CertificateHandler] is no longer being used, and should clean up any resources it is using.</para>
	/// </summary>
	public void Dispose()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Release();
			m_Ptr = IntPtr.Zero;
		}
	}
}

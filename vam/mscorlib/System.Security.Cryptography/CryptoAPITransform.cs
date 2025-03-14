using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Cryptography;

[ComVisible(true)]
public sealed class CryptoAPITransform : IDisposable, ICryptoTransform
{
	private bool m_disposed;

	public bool CanReuseTransform => true;

	public bool CanTransformMultipleBlocks => true;

	public int InputBlockSize => 0;

	public IntPtr KeyHandle
	{
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		get
		{
			return IntPtr.Zero;
		}
	}

	public int OutputBlockSize => 0;

	internal CryptoAPITransform()
	{
		m_disposed = false;
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Clear()
	{
		Dispose(disposing: false);
	}

	private void Dispose(bool disposing)
	{
		if (!m_disposed)
		{
			if (disposing)
			{
			}
			m_disposed = true;
		}
	}

	public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
	{
		return 0;
	}

	public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
	{
		return null;
	}

	[ComVisible(false)]
	public void Reset()
	{
	}
}

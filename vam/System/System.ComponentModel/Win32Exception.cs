using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.ComponentModel;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public class Win32Exception : ExternalException
{
	private int native_error_code;

	public int NativeErrorCode => native_error_code;

	public Win32Exception()
		: base(W32ErrorMessage(Marshal.GetLastWin32Error()))
	{
		native_error_code = Marshal.GetLastWin32Error();
	}

	public Win32Exception(int error)
		: base(W32ErrorMessage(error))
	{
		native_error_code = error;
	}

	public Win32Exception(int error, string message)
		: base(message)
	{
		native_error_code = error;
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public Win32Exception(string message)
		: base(message)
	{
		native_error_code = Marshal.GetLastWin32Error();
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public Win32Exception(string message, Exception innerException)
		: base(message, innerException)
	{
		native_error_code = Marshal.GetLastWin32Error();
	}

	protected Win32Exception(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		native_error_code = info.GetInt32("NativeErrorCode");
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("NativeErrorCode", native_error_code);
		base.GetObjectData(info, context);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern string W32ErrorMessage(int error_code);
}

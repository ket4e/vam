using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation;

internal struct Win32_SOCKET_ADDRESS
{
	private const int AF_INET6 = 23;

	public IntPtr Sockaddr;

	public int SockaddrLength;

	public IPAddress GetIPAddress()
	{
		System.Net.NetworkInformation.Win32_SOCKADDR win32_SOCKADDR = (System.Net.NetworkInformation.Win32_SOCKADDR)Marshal.PtrToStructure(Sockaddr, typeof(System.Net.NetworkInformation.Win32_SOCKADDR));
		byte[] array;
		if (win32_SOCKADDR.AddressFamily == 23)
		{
			array = new byte[16];
			Array.Copy(win32_SOCKADDR.AddressData, 6, array, 0, 16);
		}
		else
		{
			array = new byte[4];
			Array.Copy(win32_SOCKADDR.AddressData, 2, array, 0, 4);
		}
		return new IPAddress(array);
	}
}

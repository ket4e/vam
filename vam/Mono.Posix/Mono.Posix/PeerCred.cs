using System;
using System.Net.Sockets;

namespace Mono.Posix;

[Obsolete("Use Mono.Unix.PeerCred")]
public class PeerCred
{
	private const int so_peercred = 10001;

	private PeerCredData data;

	public int ProcessID => data.pid;

	public int UserID => data.uid;

	public int GroupID => data.gid;

	public PeerCred(Socket sock)
	{
		if (sock.AddressFamily != AddressFamily.Unix)
		{
			throw new ArgumentException("Only Unix sockets are supported", "sock");
		}
		data = (PeerCredData)sock.GetSocketOption(SocketOptionLevel.Socket, (SocketOptionName)10001);
	}
}

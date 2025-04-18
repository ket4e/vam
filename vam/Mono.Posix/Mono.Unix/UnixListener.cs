using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Mono.Unix;

public class UnixListener : MarshalByRefObject, IDisposable
{
	private bool disposed;

	private bool listening;

	private Socket server;

	private EndPoint savedEP;

	public EndPoint LocalEndpoint => savedEP;

	protected Socket Server => server;

	public UnixListener(string path)
	{
		if (!Directory.Exists(Path.GetDirectoryName(path)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
		}
		Init(new UnixEndPoint(path));
	}

	public UnixListener(UnixEndPoint localEndPoint)
	{
		if (localEndPoint == null)
		{
			throw new ArgumentNullException("localendPoint");
		}
		Init(localEndPoint);
	}

	private void Init(UnixEndPoint ep)
	{
		listening = false;
		string filename = ep.Filename;
		if (File.Exists(filename))
		{
			Socket socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
			try
			{
				socket.Connect(ep);
				socket.Close();
				throw new InvalidOperationException("There's already a server listening on " + filename);
			}
			catch (SocketException)
			{
			}
			File.Delete(filename);
		}
		server = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
		server.Bind(ep);
		savedEP = server.LocalEndPoint;
	}

	public Socket AcceptSocket()
	{
		CheckDisposed();
		if (!listening)
		{
			throw new InvalidOperationException("Socket is not listening");
		}
		return server.Accept();
	}

	public UnixClient AcceptUnixClient()
	{
		CheckDisposed();
		if (!listening)
		{
			throw new InvalidOperationException("Socket is not listening");
		}
		return new UnixClient(AcceptSocket());
	}

	~UnixListener()
	{
		Dispose(disposing: false);
	}

	public bool Pending()
	{
		CheckDisposed();
		if (!listening)
		{
			throw new InvalidOperationException("Socket is not listening");
		}
		return server.Poll(1000, SelectMode.SelectRead);
	}

	public void Start()
	{
		Start(5);
	}

	public void Start(int backlog)
	{
		CheckDisposed();
		if (!listening)
		{
			server.Listen(backlog);
			listening = true;
		}
	}

	public void Stop()
	{
		CheckDisposed();
		Dispose(disposing: true);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			if (server != null)
			{
				server.Close();
			}
			server = null;
		}
		disposed = true;
	}

	private void CheckDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
	}
}

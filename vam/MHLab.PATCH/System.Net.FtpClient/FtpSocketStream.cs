using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace System.Net.FtpClient;

public class FtpSocketStream : Stream, IDisposable
{
	private DateTime m_lastActivity = DateTime.Now;

	private Socket m_socket;

	private int m_socketPollInterval = 15000;

	private NetworkStream m_netStream;

	private SslStream m_sslStream;

	private int m_readTimeout = -1;

	private int m_connectTimeout = 30000;

	protected Socket Socket
	{
		get
		{
			return m_socket;
		}
		private set
		{
			m_socket = value;
		}
	}

	public int SocketPollInterval
	{
		get
		{
			return m_socketPollInterval;
		}
		set
		{
			m_socketPollInterval = value;
		}
	}

	internal int SocketDataAvailable
	{
		get
		{
			if (m_socket != null)
			{
				return m_socket.Available;
			}
			return 0;
		}
	}

	public bool IsConnected
	{
		get
		{
			try
			{
				if (m_socket == null)
				{
					return false;
				}
				if (!m_socket.Connected)
				{
					Close();
					return false;
				}
				if (!CanRead || !CanWrite)
				{
					Close();
					return false;
				}
				if (m_socketPollInterval > 0 && DateTime.Now.Subtract(m_lastActivity).TotalMilliseconds > (double)m_socketPollInterval)
				{
					FtpTrace.WriteLine("Testing connectivity using Socket.Poll()...");
					if (m_socket.Poll(500000, SelectMode.SelectRead) && m_socket.Available == 0)
					{
						Close();
						return false;
					}
				}
			}
			catch (SocketException ex)
			{
				Close();
				FtpTrace.WriteLine("FtpSocketStream.IsConnected: Caught and discarded SocketException while testing for connectivity: {0}", ex.ToString());
				return false;
			}
			catch (IOException ex2)
			{
				Close();
				FtpTrace.WriteLine("FtpSocketStream.IsConnected: Caught and discarded IOException while testing for connectivity: {0}", ex2.ToString());
				return false;
			}
			return true;
		}
	}

	public bool IsEncrypted => m_sslStream != null;

	private NetworkStream NetworkStream
	{
		get
		{
			return m_netStream;
		}
		set
		{
			m_netStream = value;
		}
	}

	private SslStream SslStream
	{
		get
		{
			return m_sslStream;
		}
		set
		{
			m_sslStream = value;
		}
	}

	protected Stream BaseStream
	{
		get
		{
			if (m_sslStream != null)
			{
				return m_sslStream;
			}
			if (m_netStream != null)
			{
				return m_netStream;
			}
			return null;
		}
	}

	public override bool CanRead
	{
		get
		{
			if (m_netStream != null)
			{
				return m_netStream.CanRead;
			}
			return false;
		}
	}

	public override bool CanSeek => false;

	public override bool CanWrite
	{
		get
		{
			if (m_netStream != null)
			{
				return m_netStream.CanWrite;
			}
			return false;
		}
	}

	public override long Length => 0L;

	public override long Position
	{
		get
		{
			if (BaseStream != null)
			{
				return BaseStream.Position;
			}
			return 0L;
		}
		set
		{
			throw new InvalidOperationException();
		}
	}

	public override int ReadTimeout
	{
		get
		{
			return m_readTimeout;
		}
		set
		{
			m_readTimeout = value;
		}
	}

	public int ConnectTimeout
	{
		get
		{
			return m_connectTimeout;
		}
		set
		{
			m_connectTimeout = value;
		}
	}

	public IPEndPoint LocalEndPoint
	{
		get
		{
			if (m_socket == null)
			{
				return null;
			}
			return (IPEndPoint)m_socket.LocalEndPoint;
		}
	}

	public IPEndPoint RemoteEndPoint
	{
		get
		{
			if (m_socket == null)
			{
				return null;
			}
			return (IPEndPoint)m_socket.RemoteEndPoint;
		}
	}

	private event FtpSocketStreamSslValidation m_sslvalidate;

	public event FtpSocketStreamSslValidation ValidateCertificate
	{
		add
		{
			m_sslvalidate += value;
		}
		remove
		{
			m_sslvalidate -= value;
		}
	}

	protected bool OnValidateCertificate(X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
	{
		FtpSocketStreamSslValidation sslvalidate = this.m_sslvalidate;
		if (sslvalidate != null)
		{
			FtpSslValidationEventArgs ftpSslValidationEventArgs = new FtpSslValidationEventArgs
			{
				Certificate = certificate,
				Chain = chain,
				PolicyErrors = errors,
				Accept = (errors == SslPolicyErrors.None)
			};
			sslvalidate(this, ftpSslValidationEventArgs);
			return ftpSslValidationEventArgs.Accept;
		}
		return errors == SslPolicyErrors.None;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new InvalidOperationException();
	}

	public override void SetLength(long value)
	{
		throw new InvalidOperationException();
	}

	public override void Flush()
	{
		if (!IsConnected)
		{
			throw new InvalidOperationException("The FtpSocketStream object is not connected.");
		}
		if (BaseStream == null)
		{
			throw new InvalidOperationException("The base stream of the FtpSocketStream object is null.");
		}
		BaseStream.Flush();
	}

	internal int RawSocketRead(byte[] buffer)
	{
		int result = 0;
		if (m_socket != null && m_socket.Connected)
		{
			result = m_socket.Receive(buffer, buffer.Length, SocketFlags.None);
		}
		return result;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		IAsyncResult asyncResult = null;
		if (BaseStream == null)
		{
			return 0;
		}
		m_lastActivity = DateTime.Now;
		asyncResult = BaseStream.BeginRead(buffer, offset, count, null, null);
		if (!asyncResult.AsyncWaitHandle.WaitOne(m_readTimeout, exitContext: true))
		{
			Close();
			throw new TimeoutException("Timed out trying to read data from the socket stream!");
		}
		return BaseStream.EndRead(asyncResult);
	}

	public string ReadLine(Encoding encoding)
	{
		List<byte> list = new List<byte>();
		byte[] array = new byte[1];
		string result = null;
		while (Read(array, 0, array.Length) > 0)
		{
			list.Add(array[0]);
			if (array[0] == 10)
			{
				result = encoding.GetString(list.ToArray()).Trim('\r', '\n');
				break;
			}
		}
		return result;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (BaseStream != null)
		{
			BaseStream.Write(buffer, offset, count);
			m_lastActivity = DateTime.Now;
		}
	}

	public void WriteLine(Encoding encoding, string buf)
	{
		byte[] bytes = encoding.GetBytes($"{buf}\r\n");
		Write(bytes, 0, bytes.Length);
	}

	public new void Dispose()
	{
		FtpTrace.WriteLine("Disposing FtpSocketStream...");
		Close();
	}

	public override void Close()
	{
		if (m_socket != null)
		{
			try
			{
				if (m_socket.Connected)
				{
					m_socket.Close();
				}
			}
			catch (SocketException ex)
			{
				FtpTrace.WriteLine("Caught and discarded a SocketException while cleaning up the Socket: {0}", ex.ToString());
			}
			finally
			{
				m_socket = null;
			}
		}
		if (m_netStream != null)
		{
			try
			{
				m_netStream.Dispose();
			}
			catch (IOException ex2)
			{
				FtpTrace.WriteLine("Caught and discarded an IOException while cleaning up the NetworkStream: {0}", ex2.ToString());
			}
			finally
			{
				m_netStream = null;
			}
		}
		if (m_sslStream == null)
		{
			return;
		}
		try
		{
			m_sslStream.Dispose();
		}
		catch (IOException ex3)
		{
			FtpTrace.WriteLine("Caught and discarded an IOException while cleaning up the SslStream: {0}", ex3.ToString());
		}
		finally
		{
			m_sslStream = null;
		}
	}

	public void SetSocketOption(SocketOptionLevel level, SocketOptionName name, bool value)
	{
		if (m_socket == null)
		{
			throw new InvalidOperationException("The underlying socket is null. Have you established a connection?");
		}
		m_socket.SetSocketOption(level, name, value);
	}

	public void Connect(string host, int port, FtpIpVersion ipVersions)
	{
		IAsyncResult asyncResult = null;
		IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
		if (ipVersions == (FtpIpVersion)0)
		{
			throw new ArgumentException("The ipVersions parameter must contain at least 1 flag.");
		}
		for (int i = 0; i < hostAddresses.Length; i++)
		{
			if (ipVersions != FtpIpVersion.ANY)
			{
				switch (hostAddresses[i].AddressFamily)
				{
				case AddressFamily.InterNetwork:
					if ((ipVersions & FtpIpVersion.IPv4) != FtpIpVersion.IPv4)
					{
						continue;
					}
					break;
				case AddressFamily.InterNetworkV6:
					if ((ipVersions & FtpIpVersion.IPv6) != FtpIpVersion.IPv6)
					{
						continue;
					}
					break;
				}
			}
			m_socket = new Socket(hostAddresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			asyncResult = m_socket.BeginConnect(hostAddresses[i], port, null, null);
			if (!asyncResult.AsyncWaitHandle.WaitOne(m_connectTimeout, exitContext: true))
			{
				Close();
				if (i + 1 == hostAddresses.Length)
				{
					throw new TimeoutException("Timed out trying to connect!");
				}
				continue;
			}
			m_socket.EndConnect(asyncResult);
			break;
		}
		if (m_socket == null || !m_socket.Connected)
		{
			Close();
			throw new IOException("Failed to connect to host.");
		}
		m_netStream = new NetworkStream(m_socket);
		m_lastActivity = DateTime.Now;
	}

	public void ActivateEncryption(string targethost)
	{
		ActivateEncryption(targethost, null, SslProtocols.Default);
	}

	public void ActivateEncryption(string targethost, X509CertificateCollection clientCerts)
	{
		ActivateEncryption(targethost, clientCerts, SslProtocols.Default);
	}

	public void ActivateEncryption(string targethost, X509CertificateCollection clientCerts, SslProtocols sslProtocols)
	{
		if (!IsConnected)
		{
			throw new InvalidOperationException("The FtpSocketStream object is not connected.");
		}
		if (m_netStream == null)
		{
			throw new InvalidOperationException("The base network stream is null.");
		}
		if (m_sslStream != null)
		{
			throw new InvalidOperationException("SSL Encryption has already been enabled on this stream.");
		}
		try
		{
			m_sslStream = new SslStream(NetworkStream, leaveStreamOpen: true, (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => OnValidateCertificate(certificate, chain, sslPolicyErrors));
			DateTime now = DateTime.Now;
			m_sslStream.AuthenticateAsClient(targethost, clientCerts, sslProtocols, checkCertificateRevocation: true);
			TimeSpan timeSpan = DateTime.Now.Subtract(now);
			FtpTrace.WriteLine("Time to activate encryption: {0}h {1}m {2}s, Total Seconds: {3}.", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.TotalSeconds);
		}
		catch (AuthenticationException ex)
		{
			Close();
			throw ex;
		}
	}

	public void Listen(IPAddress address, int port)
	{
		if (!IsConnected)
		{
			if (m_socket == null)
			{
				m_socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			}
			m_socket.Bind(new IPEndPoint(address, port));
			m_socket.Listen(1);
		}
	}

	public void Accept()
	{
		if (m_socket != null)
		{
			m_socket = m_socket.Accept();
		}
	}

	public IAsyncResult BeginAccept(AsyncCallback callback, object state)
	{
		if (m_socket != null)
		{
			return m_socket.BeginAccept(callback, state);
		}
		return null;
	}

	public void EndAccept(IAsyncResult ar)
	{
		if (m_socket != null)
		{
			m_socket = m_socket.EndAccept(ar);
			m_netStream = new NetworkStream(m_socket);
		}
	}
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Net.FtpClient;

public class FtpClient : IFtpClient, IDisposable
{
	private delegate FtpReply AsyncExecute(string command);

	private delegate void AsyncConnect();

	private delegate void AsyncDisconnect();

	private delegate Stream AsyncOpenRead(string path, FtpDataType type, long restart);

	private delegate Stream AsyncOpenWrite(string path, FtpDataType type);

	private delegate Stream AsyncOpenAppend(string path, FtpDataType type);

	private delegate FtpListItem AsyncDereferenceLink(FtpListItem item, int recMax);

	private delegate FtpListItem AsyncGetObjectInfo(string path);

	private delegate FtpListItem[] AsyncGetListing(string path, FtpListOption options);

	private delegate string[] AsyncGetNameListing(string path);

	private delegate void AsyncSetDataType(FtpDataType type);

	private delegate void AsyncSetWorkingDirectory(string path);

	private delegate string AsyncGetWorkingDirectory();

	private delegate long AsyncGetFileSize(string path);

	private delegate DateTime AsyncGetModifiedTime(string path);

	private delegate void AsyncDeleteFile(string path);

	private delegate void AsyncDeleteDirectory(string path, bool force, FtpListOption options);

	private delegate bool AsyncDirectoryExists(string path);

	private delegate bool AsyncFileExists(string path, FtpListOption options);

	private delegate void AsyncCreateDirectory(string path, bool force);

	private delegate void AsyncRename(string path, string dest);

	private delegate FtpHashAlgorithm AsyncGetHashAlgorithm();

	private delegate void AsyncSetHashAlgorithm(FtpHashAlgorithm type);

	private delegate FtpHash AsyncGetHash(string path);

	private sealed class FtpControlConnectionClone : Attribute
	{
	}

	private readonly object m_lock = new object();

	private readonly Dictionary<IAsyncResult, object> m_asyncmethods = new Dictionary<IAsyncResult, object>();

	private FtpSocketStream m_stream;

	private bool m_isDisposed;

	private FtpIpVersion m_ipVersions = FtpIpVersion.ANY;

	private int m_socketPollInterval = 15000;

	private bool m_staleDataTest = true;

	private bool m_threadSafeDataChannels = true;

	private bool m_isClone;

	private Encoding m_textEncoding = Encoding.ASCII;

	private string m_host;

	private int m_port;

	private NetworkCredential m_credentials;

	private int m_maxDerefCount = 20;

	private X509CertificateCollection m_clientCerts = new X509CertificateCollection();

	private FtpDataConnectionType m_dataConnectionType;

	private bool m_ungracefullDisconnect;

	private int m_connectTimeout = 15000;

	private int m_readTimeout = 15000;

	private int m_dataConnectionConnectTimeout = 15000;

	private int m_dataConnectionReadTimeout = 15000;

	private bool m_keepAlive;

	private FtpCapability m_caps;

	private FtpHashAlgorithm m_hashAlgorithms;

	private FtpEncryptionMode m_encryptionmode;

	private bool m_dataConnectionEncryption = true;

	private SslProtocols m_SslProtocols = SslProtocols.Default;

	private FtpSslValidation m_sslvalidate;

	public bool IsDisposed
	{
		get
		{
			return m_isDisposed;
		}
		private set
		{
			m_isDisposed = value;
		}
	}

	protected Stream BaseStream => m_stream;

	[FtpControlConnectionClone]
	public FtpIpVersion InternetProtocolVersions
	{
		get
		{
			return m_ipVersions;
		}
		set
		{
			m_ipVersions = value;
		}
	}

	[FtpControlConnectionClone]
	public int SocketPollInterval
	{
		get
		{
			return m_socketPollInterval;
		}
		set
		{
			m_socketPollInterval = value;
			if (m_stream != null)
			{
				m_stream.SocketPollInterval = value;
			}
		}
	}

	[FtpControlConnectionClone]
	public bool StaleDataCheck
	{
		get
		{
			return m_staleDataTest;
		}
		set
		{
			m_staleDataTest = value;
		}
	}

	public bool IsConnected
	{
		get
		{
			if (m_stream != null)
			{
				return m_stream.IsConnected;
			}
			return false;
		}
	}

	[FtpControlConnectionClone]
	public bool EnableThreadSafeDataConnections
	{
		get
		{
			return m_threadSafeDataChannels;
		}
		set
		{
			m_threadSafeDataChannels = value;
		}
	}

	internal bool IsClone
	{
		get
		{
			return m_isClone;
		}
		private set
		{
			m_isClone = value;
		}
	}

	[FtpControlConnectionClone]
	public Encoding Encoding
	{
		get
		{
			return m_textEncoding;
		}
		set
		{
			lock (m_lock)
			{
				m_textEncoding = value;
			}
		}
	}

	[FtpControlConnectionClone]
	public string Host
	{
		get
		{
			return m_host;
		}
		set
		{
			m_host = value;
		}
	}

	[FtpControlConnectionClone]
	public int Port
	{
		get
		{
			if (m_port == 0)
			{
				switch (EncryptionMode)
				{
				case FtpEncryptionMode.None:
				case FtpEncryptionMode.Explicit:
					return 21;
				case FtpEncryptionMode.Implicit:
					return 990;
				}
			}
			return m_port;
		}
		set
		{
			m_port = value;
		}
	}

	[FtpControlConnectionClone]
	public NetworkCredential Credentials
	{
		get
		{
			return m_credentials;
		}
		set
		{
			m_credentials = value;
		}
	}

	[FtpControlConnectionClone]
	public int MaximumDereferenceCount
	{
		get
		{
			return m_maxDerefCount;
		}
		set
		{
			m_maxDerefCount = value;
		}
	}

	[FtpControlConnectionClone]
	public X509CertificateCollection ClientCertificates
	{
		get
		{
			return m_clientCerts;
		}
		protected set
		{
			m_clientCerts = value;
		}
	}

	[FtpControlConnectionClone]
	public FtpDataConnectionType DataConnectionType
	{
		get
		{
			return m_dataConnectionType;
		}
		set
		{
			m_dataConnectionType = value;
		}
	}

	[FtpControlConnectionClone]
	public bool UngracefullDisconnection
	{
		get
		{
			return m_ungracefullDisconnect;
		}
		set
		{
			m_ungracefullDisconnect = value;
		}
	}

	[FtpControlConnectionClone]
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

	[FtpControlConnectionClone]
	public int ReadTimeout
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

	[FtpControlConnectionClone]
	public int DataConnectionConnectTimeout
	{
		get
		{
			return m_dataConnectionConnectTimeout;
		}
		set
		{
			m_dataConnectionConnectTimeout = value;
		}
	}

	[FtpControlConnectionClone]
	public int DataConnectionReadTimeout
	{
		get
		{
			return m_dataConnectionReadTimeout;
		}
		set
		{
			m_dataConnectionReadTimeout = value;
		}
	}

	[FtpControlConnectionClone]
	public bool SocketKeepAlive
	{
		get
		{
			return m_keepAlive;
		}
		set
		{
			m_keepAlive = value;
			if (m_stream != null)
			{
				m_stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, value);
			}
		}
	}

	[FtpControlConnectionClone]
	public FtpCapability Capabilities
	{
		get
		{
			if (m_stream == null || !m_stream.IsConnected)
			{
				Connect();
			}
			return m_caps;
		}
		protected set
		{
			m_caps = value;
		}
	}

	public FtpHashAlgorithm HashAlgorithms
	{
		get
		{
			if (m_stream == null || !m_stream.IsConnected)
			{
				Connect();
			}
			return m_hashAlgorithms;
		}
		private set
		{
			m_hashAlgorithms = value;
		}
	}

	[FtpControlConnectionClone]
	public FtpEncryptionMode EncryptionMode
	{
		get
		{
			return m_encryptionmode;
		}
		set
		{
			m_encryptionmode = value;
		}
	}

	[FtpControlConnectionClone]
	public bool DataConnectionEncryption
	{
		get
		{
			return m_dataConnectionEncryption;
		}
		set
		{
			m_dataConnectionEncryption = value;
		}
	}

	[FtpControlConnectionClone]
	public SslProtocols SslProtocols
	{
		get
		{
			return m_SslProtocols;
		}
		set
		{
			m_SslProtocols = value;
		}
	}

	public string SystemType
	{
		get
		{
			FtpReply ftpReply = Execute("SYST");
			if (ftpReply.Success)
			{
				return ftpReply.Message;
			}
			return null;
		}
	}

	public event FtpSslValidation ValidateCertificate
	{
		add
		{
			m_sslvalidate = (FtpSslValidation)Delegate.Combine(m_sslvalidate, value);
		}
		remove
		{
			m_sslvalidate = (FtpSslValidation)Delegate.Remove(m_sslvalidate, value);
		}
	}

	public bool HasFeature(FtpCapability cap)
	{
		return (Capabilities & cap) == cap;
	}

	private void OnValidateCertficate(FtpSslValidationEventArgs e)
	{
		m_sslvalidate?.Invoke(this, e);
	}

	protected T GetAsyncDelegate<T>(IAsyncResult ar)
	{
		lock (m_asyncmethods)
		{
			if (m_isDisposed)
			{
				throw new ObjectDisposedException("This connection object has already been disposed.");
			}
			if (!m_asyncmethods.ContainsKey(ar))
			{
				throw new InvalidOperationException("The specified IAsyncResult could not be located.");
			}
			if (!(m_asyncmethods[ar] is T))
			{
				StackTrace stackTrace = new StackTrace(1);
				throw new InvalidCastException("The AsyncResult cannot be matched to the specified delegate. " + $"Are you sure you meant to call {stackTrace.GetFrame(0).GetMethod().Name} and not another method?");
			}
			T result = (T)m_asyncmethods[ar];
			m_asyncmethods.Remove(ar);
			return result;
		}
	}

	protected FtpClient CloneConnection()
	{
		FtpClient ftpClient = new FtpClient();
		ftpClient.m_isClone = true;
		PropertyInfo[] properties = GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(FtpControlConnectionClone), inherit: true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				propertyInfo.SetValue(ftpClient, propertyInfo.GetValue(this, null), null);
			}
		}
		ftpClient.ValidateCertificate += delegate(FtpClient obj, FtpSslValidationEventArgs e)
		{
			e.Accept = true;
		};
		return ftpClient;
	}

	internal FtpReply GetReply()
	{
		FtpReply result = default(FtpReply);
		lock (m_lock)
		{
			if (!IsConnected)
			{
				throw new InvalidOperationException("No connection to the server has been established.");
			}
			m_stream.ReadTimeout = m_readTimeout;
			string text;
			while ((text = m_stream.ReadLine(Encoding)) != null)
			{
				FtpTrace.WriteLine(text);
				Match match;
				if ((match = Regex.Match(text, "^(?<code>[0-9]{3}) (?<message>.*)$")).Success)
				{
					result.Code = match.Groups["code"].Value;
					result.Message = match.Groups["message"].Value;
					break;
				}
				result.InfoMessages += $"{text}\n";
			}
		}
		return result;
	}

	public FtpReply Execute(string command, params object[] args)
	{
		return Execute(string.Format(command, args));
	}

	public FtpReply Execute(string command)
	{
		lock (m_lock)
		{
			if (StaleDataCheck && m_stream != null && m_stream.SocketDataAvailable > 0)
			{
				FtpTrace.WriteLine("There is stale data on the socket, maybe our connection timed out. Re-connecting.");
				if (m_stream.IsConnected && !m_stream.IsEncrypted)
				{
					byte[] array = new byte[m_stream.SocketDataAvailable];
					m_stream.RawSocketRead(array);
					FtpTrace.Write("The data was: ");
					FtpTrace.WriteLine(Encoding.GetString(array).TrimEnd('\r', '\n'));
				}
				m_stream.Close();
			}
			if (!IsConnected)
			{
				if (command == "QUIT")
				{
					FtpTrace.WriteLine("Not sending QUIT because the connection has already been closed.");
					FtpReply result = default(FtpReply);
					result.Code = "200";
					result.Message = "Connection already closed.";
					return result;
				}
				Connect();
			}
			FtpTrace.WriteLine(command.StartsWith("PASS") ? "PASS <omitted>" : command);
			m_stream.WriteLine(m_textEncoding, command);
			return GetReply();
		}
	}

	public IAsyncResult BeginExecute(string command, AsyncCallback callback, object state)
	{
		AsyncExecute value;
		IAsyncResult asyncResult = (value = Execute).BeginInvoke(command, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public FtpReply EndExecute(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncExecute>(ar).EndInvoke(ar);
	}

	public virtual void Connect()
	{
		lock (m_lock)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("This FtpClient object has been disposed. It is no longer accessible.");
			}
			if (m_stream == null)
			{
				m_stream = new FtpSocketStream();
				m_stream.ValidateCertificate += FireValidateCertficate;
			}
			else if (IsConnected)
			{
				Disconnect();
			}
			if (Host == null)
			{
				throw new FtpException("No host has been specified");
			}
			if (!IsClone)
			{
				m_caps = FtpCapability.NONE;
			}
			m_hashAlgorithms = FtpHashAlgorithm.NONE;
			m_stream.ConnectTimeout = m_connectTimeout;
			m_stream.SocketPollInterval = m_socketPollInterval;
			m_stream.Connect(Host, Port, InternetProtocolVersions);
			m_stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_keepAlive);
			if (EncryptionMode == FtpEncryptionMode.Implicit)
			{
				m_stream.ActivateEncryption(Host, (m_clientCerts.Count > 0) ? m_clientCerts : null, m_SslProtocols);
			}
			FtpReply reply;
			FtpReply ftpReply = (reply = GetReply());
			if (!ftpReply.Success)
			{
				if (reply.Code == null)
				{
					throw new IOException("The connection was terminated before a greeting could be read.");
				}
				throw new FtpCommandException(reply);
			}
			if (EncryptionMode == FtpEncryptionMode.Explicit)
			{
				ftpReply = (reply = Execute("AUTH TLS"));
				if (!ftpReply.Success)
				{
					throw new FtpSecurityNotAvailableException("AUTH TLS command failed.");
				}
				m_stream.ActivateEncryption(Host, (m_clientCerts.Count > 0) ? m_clientCerts : null, m_SslProtocols);
			}
			if (m_credentials != null)
			{
				Authenticate();
			}
			if (m_stream.IsEncrypted && DataConnectionEncryption)
			{
				ftpReply = (reply = Execute("PBSZ 0"));
				if (!ftpReply.Success)
				{
					throw new FtpCommandException(reply);
				}
				ftpReply = (reply = Execute("PROT P"));
				if (!ftpReply.Success)
				{
					throw new FtpCommandException(reply);
				}
			}
			if (!IsClone)
			{
				ftpReply = (reply = Execute("FEAT"));
				if (ftpReply.Success && reply.InfoMessages != null)
				{
					GetFeatures(reply);
				}
			}
			if (m_textEncoding == Encoding.ASCII && HasFeature(FtpCapability.UTF8))
			{
				m_textEncoding = Encoding.UTF8;
			}
			FtpTrace.WriteLine("Text encoding: " + m_textEncoding.ToString());
			if (m_textEncoding == Encoding.UTF8)
			{
				Execute("OPTS UTF8 ON");
			}
		}
	}

	protected virtual void Authenticate()
	{
		FtpReply reply;
		FtpReply ftpReply = (reply = Execute("USER {0}", Credentials.UserName));
		if (!ftpReply.Success)
		{
			throw new FtpCommandException(reply);
		}
		if (reply.Type == FtpResponseType.PositiveIntermediate)
		{
			ftpReply = (reply = Execute("PASS {0}", Credentials.Password));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
		}
	}

	protected virtual void GetFeatures(FtpReply reply)
	{
		string[] array = reply.InfoMessages.Split('\n');
		foreach (string text in array)
		{
			if (text.ToUpper().Trim().StartsWith("MLST") || text.ToUpper().Trim().StartsWith("MLSD"))
			{
				m_caps |= FtpCapability.MLSD;
			}
			else if (text.ToUpper().Trim().StartsWith("MDTM"))
			{
				m_caps |= FtpCapability.MDTM;
			}
			else if (text.ToUpper().Trim().StartsWith("REST STREAM"))
			{
				m_caps |= FtpCapability.REST;
			}
			else if (text.ToUpper().Trim().StartsWith("SIZE"))
			{
				m_caps |= FtpCapability.SIZE;
			}
			else if (text.ToUpper().Trim().StartsWith("UTF8"))
			{
				m_caps |= FtpCapability.UTF8;
			}
			else if (text.ToUpper().Trim().StartsWith("PRET"))
			{
				m_caps |= FtpCapability.PRET;
			}
			else if (text.ToUpper().Trim().StartsWith("MFMT"))
			{
				m_caps |= FtpCapability.MFMT;
			}
			else if (text.ToUpper().Trim().StartsWith("MFCT"))
			{
				m_caps |= FtpCapability.MFCT;
			}
			else if (text.ToUpper().Trim().StartsWith("MFF"))
			{
				m_caps |= FtpCapability.MFF;
			}
			else if (text.ToUpper().Trim().StartsWith("MD5"))
			{
				m_caps |= FtpCapability.MD5;
			}
			else if (text.ToUpper().Trim().StartsWith("XMD5"))
			{
				m_caps |= FtpCapability.XMD5;
			}
			else if (text.ToUpper().Trim().StartsWith("XCRC"))
			{
				m_caps |= FtpCapability.XCRC;
			}
			else if (text.ToUpper().Trim().StartsWith("XSHA1"))
			{
				m_caps |= FtpCapability.XSHA1;
			}
			else if (text.ToUpper().Trim().StartsWith("XSHA256"))
			{
				m_caps |= FtpCapability.XSHA256;
			}
			else if (text.ToUpper().Trim().StartsWith("XSHA512"))
			{
				m_caps |= FtpCapability.XSHA512;
			}
			else
			{
				if (!text.ToUpper().Trim().StartsWith("HASH"))
				{
					continue;
				}
				m_caps |= FtpCapability.HASH;
				Match match;
				if (!(match = Regex.Match(text.ToUpper().Trim(), "^HASH\\s+(?<types>.*)$")).Success)
				{
					continue;
				}
				string[] array2 = match.Groups["types"].Value.Split(';');
				for (int j = 0; j < array2.Length; j++)
				{
					switch (array2[j].ToUpper().Trim())
					{
					case "SHA-1":
					case "SHA-1*":
						m_hashAlgorithms |= FtpHashAlgorithm.SHA1;
						break;
					case "SHA-256":
					case "SHA-256*":
						m_hashAlgorithms |= FtpHashAlgorithm.SHA256;
						break;
					case "SHA-512":
					case "SHA-512*":
						m_hashAlgorithms |= FtpHashAlgorithm.SHA512;
						break;
					case "MD5":
					case "MD5*":
						m_hashAlgorithms |= FtpHashAlgorithm.MD5;
						break;
					case "CRC":
					case "CRC*":
						m_hashAlgorithms |= FtpHashAlgorithm.CRC;
						break;
					}
				}
			}
		}
	}

	public IAsyncResult BeginConnect(AsyncCallback callback, object state)
	{
		AsyncConnect value;
		IAsyncResult asyncResult = (value = Connect).BeginInvoke(callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public void EndConnect(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncConnect>(ar).EndInvoke(ar);
	}

	private void FireValidateCertficate(FtpSocketStream stream, FtpSslValidationEventArgs e)
	{
		OnValidateCertficate(e);
	}

	public virtual void Disconnect()
	{
		lock (m_lock)
		{
			if (m_stream == null || !m_stream.IsConnected)
			{
				return;
			}
			try
			{
				if (!UngracefullDisconnection)
				{
					Execute("QUIT");
				}
			}
			catch (SocketException ex)
			{
				FtpTrace.WriteLine("FtpClient.Disconnect(): SocketException caught and discarded while closing control connection: {0}", ex.ToString());
			}
			catch (IOException ex2)
			{
				FtpTrace.WriteLine("FtpClient.Disconnect(): IOException caught and discarded while closing control connection: {0}", ex2.ToString());
			}
			catch (FtpCommandException ex3)
			{
				FtpTrace.WriteLine("FtpClient.Disconnect(): FtpCommandException caught and discarded while closing control connection: {0}", ex3.ToString());
			}
			catch (FtpException ex4)
			{
				FtpTrace.WriteLine("FtpClient.Disconnect(): FtpException caught and discarded while closing control connection: {0}", ex4.ToString());
			}
			finally
			{
				m_stream.Close();
			}
		}
	}

	public IAsyncResult BeginDisconnect(AsyncCallback callback, object state)
	{
		AsyncDisconnect value;
		IAsyncResult asyncResult = (value = Disconnect).BeginInvoke(callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public void EndDisconnect(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncDisconnect>(ar).EndInvoke(ar);
	}

	private FtpDataStream OpenPassiveDataStream(FtpDataConnectionType type, string command, long restart)
	{
		FtpDataStream ftpDataStream = null;
		string text = null;
		int num = 0;
		if (m_stream == null)
		{
			throw new InvalidOperationException("The control connection stream is null! Generally this means there is no connection to the server. Cannot open a passive data stream.");
		}
		FtpReply ftpReply;
		FtpReply reply;
		if (type == FtpDataConnectionType.EPSV || type == FtpDataConnectionType.AutoPassive)
		{
			ftpReply = (reply = Execute("EPSV"));
			if (!ftpReply.Success)
			{
				if (reply.Type == FtpResponseType.PermanentNegativeCompletion && type == FtpDataConnectionType.AutoPassive && m_stream != null && m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetwork)
				{
					return OpenPassiveDataStream(FtpDataConnectionType.PASV, command, restart);
				}
				throw new FtpCommandException(reply);
			}
			Match match = Regex.Match(reply.Message, "\\(\\|\\|\\|(?<port>\\d+)\\|\\)");
			if (!match.Success)
			{
				throw new FtpException("Failed to get the EPSV port from: " + reply.Message);
			}
			text = m_host;
			num = int.Parse(match.Groups["port"].Value);
		}
		else
		{
			if (m_stream.LocalEndPoint.AddressFamily != AddressFamily.InterNetwork)
			{
				throw new FtpException("Only IPv4 is supported by the PASV command. Use EPSV instead.");
			}
			ftpReply = (reply = Execute("PASV"));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
			Match match = Regex.Match(reply.Message, "(?<quad1>\\d+),(?<quad2>\\d+),(?<quad3>\\d+),(?<quad4>\\d+),(?<port1>\\d+),(?<port2>\\d+)");
			if (!match.Success || match.Groups.Count != 7)
			{
				throw new FtpException($"Malformed PASV response: {reply.Message}");
			}
			text = ((type != FtpDataConnectionType.PASVEX) ? string.Format("{0}.{1}.{2}.{3}", match.Groups["quad1"].Value, match.Groups["quad2"].Value, match.Groups["quad3"].Value, match.Groups["quad4"].Value) : m_host);
			num = (int.Parse(match.Groups["port1"].Value) << 8) + int.Parse(match.Groups["port2"].Value);
		}
		ftpDataStream = new FtpDataStream(this);
		ftpDataStream.ConnectTimeout = DataConnectionConnectTimeout;
		ftpDataStream.ReadTimeout = DataConnectionReadTimeout;
		ftpDataStream.Connect(text, num, InternetProtocolVersions);
		ftpDataStream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_keepAlive);
		if (restart > 0)
		{
			ftpReply = (reply = Execute("REST {0}", restart));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
		}
		ftpReply = (reply = Execute(command));
		if (!ftpReply.Success)
		{
			ftpDataStream.Close();
			throw new FtpCommandException(reply);
		}
		ftpDataStream.CommandStatus = reply;
		if (m_dataConnectionEncryption && m_encryptionmode != 0)
		{
			ftpDataStream.ActivateEncryption(m_host, (ClientCertificates.Count > 0) ? ClientCertificates : null, m_SslProtocols);
		}
		return ftpDataStream;
	}

	private FtpDataStream OpenActiveDataStream(FtpDataConnectionType type, string command, long restart)
	{
		FtpDataStream ftpDataStream = new FtpDataStream(this);
		if (m_stream == null)
		{
			throw new InvalidOperationException("The control connection stream is null! Generally this means there is no connection to the server. Cannot open an active data stream.");
		}
		ftpDataStream.Listen(m_stream.LocalEndPoint.Address, 0);
		IAsyncResult asyncResult = ftpDataStream.BeginAccept(null, null);
		FtpReply ftpReply;
		FtpReply reply;
		if (type == FtpDataConnectionType.EPRT || type == FtpDataConnectionType.AutoActive)
		{
			int num = 0;
			num = ftpDataStream.LocalEndPoint.AddressFamily switch
			{
				AddressFamily.InterNetwork => 1, 
				AddressFamily.InterNetworkV6 => 2, 
				_ => throw new InvalidOperationException("The IP protocol being used is not supported."), 
			};
			ftpReply = (reply = Execute("EPRT |{0}|{1}|{2}|", num, ftpDataStream.LocalEndPoint.Address.ToString(), ftpDataStream.LocalEndPoint.Port));
			if (!ftpReply.Success)
			{
				if (reply.Type == FtpResponseType.PermanentNegativeCompletion && type == FtpDataConnectionType.AutoActive && m_stream != null && m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetwork)
				{
					ftpDataStream.ControlConnection = null;
					ftpDataStream.Close();
					return OpenActiveDataStream(FtpDataConnectionType.PORT, command, restart);
				}
				ftpDataStream.Close();
				throw new FtpCommandException(reply);
			}
		}
		else
		{
			if (m_stream.LocalEndPoint.AddressFamily != AddressFamily.InterNetwork)
			{
				throw new FtpException("Only IPv4 is supported by the PORT command. Use EPRT instead.");
			}
			ftpReply = (reply = Execute("PORT {0},{1},{2}", ftpDataStream.LocalEndPoint.Address.ToString().Replace('.', ','), ftpDataStream.LocalEndPoint.Port / 256, ftpDataStream.LocalEndPoint.Port % 256));
			if (!ftpReply.Success)
			{
				ftpDataStream.Close();
				throw new FtpCommandException(reply);
			}
		}
		if (restart > 0)
		{
			ftpReply = (reply = Execute("REST {0}", restart));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
		}
		ftpReply = (reply = Execute(command));
		if (!ftpReply.Success)
		{
			ftpDataStream.Close();
			throw new FtpCommandException(reply);
		}
		ftpDataStream.CommandStatus = reply;
		asyncResult.AsyncWaitHandle.WaitOne(m_dataConnectionConnectTimeout);
		if (!asyncResult.IsCompleted)
		{
			ftpDataStream.Close();
			throw new TimeoutException("Timed out waiting for the server to connect to the active data socket.");
		}
		ftpDataStream.EndAccept(asyncResult);
		if (m_dataConnectionEncryption && m_encryptionmode != 0)
		{
			ftpDataStream.ActivateEncryption(m_host, (ClientCertificates.Count > 0) ? ClientCertificates : null, m_SslProtocols);
		}
		ftpDataStream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_keepAlive);
		ftpDataStream.ReadTimeout = m_dataConnectionReadTimeout;
		return ftpDataStream;
	}

	private FtpDataStream OpenDataStream(string command, long restart)
	{
		FtpDataConnectionType ftpDataConnectionType = m_dataConnectionType;
		FtpDataStream ftpDataStream = null;
		lock (m_lock)
		{
			if (!IsConnected)
			{
				Connect();
			}
			if (m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
			{
				switch (ftpDataConnectionType)
				{
				case FtpDataConnectionType.PORT:
					ftpDataConnectionType = FtpDataConnectionType.EPRT;
					FtpTrace.WriteLine("Changed data connection type to EPRT because we are connected with IPv6.");
					break;
				case FtpDataConnectionType.PASV:
				case FtpDataConnectionType.PASVEX:
					ftpDataConnectionType = FtpDataConnectionType.EPSV;
					FtpTrace.WriteLine("Changed data connection type to EPSV because we are connected with IPv6.");
					break;
				}
			}
			switch (ftpDataConnectionType)
			{
			case FtpDataConnectionType.AutoPassive:
			case FtpDataConnectionType.PASV:
			case FtpDataConnectionType.PASVEX:
			case FtpDataConnectionType.EPSV:
				ftpDataStream = OpenPassiveDataStream(ftpDataConnectionType, command, restart);
				break;
			case FtpDataConnectionType.AutoActive:
			case FtpDataConnectionType.PORT:
			case FtpDataConnectionType.EPRT:
				ftpDataStream = OpenActiveDataStream(ftpDataConnectionType, command, restart);
				break;
			}
			if (ftpDataStream == null)
			{
				throw new InvalidOperationException("The specified data channel type is not implemented.");
			}
			return ftpDataStream;
		}
	}

	internal FtpReply CloseDataStream(FtpDataStream stream)
	{
		FtpReply ftpReply = default(FtpReply);
		if (stream == null)
		{
			throw new ArgumentException("The data stream parameter was null");
		}
		lock (m_lock)
		{
			try
			{
				if (IsConnected && stream.CommandStatus.Type == FtpResponseType.PositivePreliminary)
				{
					FtpReply ftpReply2 = (ftpReply = GetReply());
					if (!ftpReply2.Success)
					{
						throw new FtpCommandException(ftpReply);
					}
				}
			}
			finally
			{
				if (IsClone)
				{
					Disconnect();
					Dispose();
				}
			}
		}
		return ftpReply;
	}

	public Stream OpenRead(string path)
	{
		return OpenRead(path, FtpDataType.Binary, 0L);
	}

	public Stream OpenRead(string path, FtpDataType type)
	{
		return OpenRead(path, type, 0L);
	}

	public Stream OpenRead(string path, long restart)
	{
		return OpenRead(path, FtpDataType.Binary, restart);
	}

	public virtual Stream OpenRead(string path, FtpDataType type, long restart)
	{
		FtpClient ftpClient = null;
		FtpDataStream ftpDataStream = null;
		long num = 0L;
		lock (m_lock)
		{
			if (m_threadSafeDataChannels)
			{
				ftpClient = CloneConnection();
				ftpClient.Connect();
				ftpClient.SetWorkingDirectory(GetWorkingDirectory());
			}
			else
			{
				ftpClient = this;
			}
			ftpClient.SetDataType(type);
			num = ftpClient.GetFileSize(path);
			ftpDataStream = ftpClient.OpenDataStream($"RETR {path.GetFtpPath()}", restart);
		}
		if (ftpDataStream != null)
		{
			if (num > 0)
			{
				ftpDataStream.SetLength(num);
			}
			if (restart > 0)
			{
				ftpDataStream.SetPosition(restart);
			}
		}
		return ftpDataStream;
	}

	public IAsyncResult BeginOpenRead(string path, AsyncCallback callback, object state)
	{
		return BeginOpenRead(path, FtpDataType.Binary, 0L, callback, state);
	}

	public IAsyncResult BeginOpenRead(string path, FtpDataType type, AsyncCallback callback, object state)
	{
		return BeginOpenRead(path, type, 0L, callback, state);
	}

	public IAsyncResult BeginOpenRead(string path, long restart, AsyncCallback callback, object state)
	{
		return BeginOpenRead(path, FtpDataType.Binary, restart, callback, state);
	}

	public IAsyncResult BeginOpenRead(string path, FtpDataType type, long restart, AsyncCallback callback, object state)
	{
		AsyncOpenRead value;
		IAsyncResult asyncResult = (value = OpenRead).BeginInvoke(path, type, restart, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public Stream EndOpenRead(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncOpenRead>(ar).EndInvoke(ar);
	}

	public Stream OpenWrite(string path)
	{
		return OpenWrite(path, FtpDataType.Binary);
	}

	public virtual Stream OpenWrite(string path, FtpDataType type)
	{
		FtpClient ftpClient = null;
		FtpDataStream ftpDataStream = null;
		long num = 0L;
		lock (m_lock)
		{
			if (m_threadSafeDataChannels)
			{
				ftpClient = CloneConnection();
				ftpClient.Connect();
				ftpClient.SetWorkingDirectory(GetWorkingDirectory());
			}
			else
			{
				ftpClient = this;
			}
			ftpClient.SetDataType(type);
			num = ftpClient.GetFileSize(path);
			ftpDataStream = ftpClient.OpenDataStream($"STOR {path.GetFtpPath()}", 0L);
			if (num > 0)
			{
				ftpDataStream?.SetLength(num);
			}
		}
		return ftpDataStream;
	}

	public IAsyncResult BeginOpenWrite(string path, AsyncCallback callback, object state)
	{
		return BeginOpenWrite(path, FtpDataType.Binary, callback, state);
	}

	public IAsyncResult BeginOpenWrite(string path, FtpDataType type, AsyncCallback callback, object state)
	{
		AsyncOpenWrite value;
		IAsyncResult asyncResult = (value = OpenWrite).BeginInvoke(path, type, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public Stream EndOpenWrite(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncOpenWrite>(ar).EndInvoke(ar);
	}

	public Stream OpenAppend(string path)
	{
		return OpenAppend(path, FtpDataType.Binary);
	}

	public virtual Stream OpenAppend(string path, FtpDataType type)
	{
		FtpClient ftpClient = null;
		FtpDataStream ftpDataStream = null;
		long num = 0L;
		lock (m_lock)
		{
			if (m_threadSafeDataChannels)
			{
				ftpClient = CloneConnection();
				ftpClient.Connect();
				ftpClient.SetWorkingDirectory(GetWorkingDirectory());
			}
			else
			{
				ftpClient = this;
			}
			ftpClient.SetDataType(type);
			num = ftpClient.GetFileSize(path);
			ftpDataStream = ftpClient.OpenDataStream($"APPE {path.GetFtpPath()}", 0L);
			if (num > 0 && ftpDataStream != null)
			{
				ftpDataStream.SetLength(num);
				ftpDataStream.SetPosition(num);
			}
		}
		return ftpDataStream;
	}

	public IAsyncResult BeginOpenAppend(string path, AsyncCallback callback, object state)
	{
		return BeginOpenAppend(path, FtpDataType.Binary, callback, state);
	}

	public IAsyncResult BeginOpenAppend(string path, FtpDataType type, AsyncCallback callback, object state)
	{
		AsyncOpenAppend value;
		IAsyncResult asyncResult = (value = OpenAppend).BeginInvoke(path, type, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public Stream EndOpenAppend(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncOpenAppend>(ar).EndInvoke(ar);
	}

	public FtpListItem DereferenceLink(FtpListItem item)
	{
		return DereferenceLink(item, MaximumDereferenceCount);
	}

	public FtpListItem DereferenceLink(FtpListItem item, int recMax)
	{
		int count = 0;
		return DereferenceLink(item, recMax, ref count);
	}

	private FtpListItem DereferenceLink(FtpListItem item, int recMax, ref int count)
	{
		if (item.Type != FtpFileSystemObjectType.Link)
		{
			throw new FtpException("You can only derefernce a symbolic link. Please verify the item type is Link.");
		}
		if (item.LinkTarget == null)
		{
			throw new FtpException("The link target was null. Please check this before trying to dereference the link.");
		}
		FtpListItem[] listing = GetListing(item.LinkTarget.GetFtpDirectoryName(), FtpListOption.ForceList);
		foreach (FtpListItem ftpListItem in listing)
		{
			if (!(item.LinkTarget == ftpListItem.FullName))
			{
				continue;
			}
			if (ftpListItem.Type == FtpFileSystemObjectType.Link)
			{
				if (++count == recMax)
				{
					return null;
				}
				return DereferenceLink(ftpListItem, recMax, ref count);
			}
			if (HasFeature(FtpCapability.MDTM))
			{
				DateTime modifiedTime = GetModifiedTime(ftpListItem.FullName);
				if (modifiedTime != DateTime.MinValue)
				{
					ftpListItem.Modified = modifiedTime;
				}
			}
			if (ftpListItem.Type == FtpFileSystemObjectType.File && ftpListItem.Size < 0 && HasFeature(FtpCapability.SIZE))
			{
				ftpListItem.Size = GetFileSize(ftpListItem.FullName);
			}
			return ftpListItem;
		}
		return null;
	}

	public IAsyncResult BeginDereferenceLink(FtpListItem item, int recMax, AsyncCallback callback, object state)
	{
		AsyncDereferenceLink value;
		IAsyncResult asyncResult = (value = DereferenceLink).BeginInvoke(item, recMax, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public IAsyncResult BeginDereferenceLink(FtpListItem item, AsyncCallback callback, object state)
	{
		return BeginDereferenceLink(item, MaximumDereferenceCount, callback, state);
	}

	public FtpListItem EndDereferenceLink(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncDereferenceLink>(ar).EndInvoke(ar);
	}

	public FtpListItem GetObjectInfo(string path)
	{
		if ((Capabilities & FtpCapability.MLSD) != FtpCapability.MLSD)
		{
			throw new InvalidOperationException("The GetObjectInfo method only works on servers that support machine listings. Please check the Capabilities flags for FtpCapability.MLSD before calling this method.");
		}
		FtpReply ftpReply;
		FtpReply ftpReply2 = (ftpReply = Execute("MLST {0}", path));
		if (ftpReply2.Success)
		{
			string[] array = ftpReply.InfoMessages.Split('\n');
			if (array.Length > 1)
			{
				string text = "";
				for (int i = 1; i < array.Length; i++)
				{
					text += array[i];
				}
				return FtpListItem.Parse(null, text, m_caps);
			}
		}
		else
		{
			FtpTrace.WriteLine("Failed to get object info for path {0} with error {1}", path, ftpReply.ErrorMessage);
		}
		return null;
	}

	public IAsyncResult BeginGetObjectInfo(string path, AsyncCallback callback, object state)
	{
		AsyncGetObjectInfo value;
		IAsyncResult asyncResult = (value = GetObjectInfo).BeginInvoke(path, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public FtpListItem EndGetObjectInfo(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncGetObjectInfo>(ar).EndInvoke(ar);
	}

	public FtpListItem[] GetListing()
	{
		return GetListing(null);
	}

	public FtpListItem[] GetListing(string path)
	{
		return GetListing(path, (FtpListOption)0);
	}

	public FtpListItem[] GetListing(string path, FtpListOption options)
	{
		FtpListItem ftpListItem = null;
		List<FtpListItem> list = new List<FtpListItem>();
		List<string> list2 = new List<string>();
		string text = null;
		string workingDirectory = GetWorkingDirectory();
		string text2 = null;
		if (path == null || path.Trim().Length == 0)
		{
			workingDirectory = GetWorkingDirectory();
			path = ((workingDirectory == null || workingDirectory.Trim().Length <= 0) ? "./" : workingDirectory);
		}
		else if (!path.StartsWith("/") && workingDirectory != null && workingDirectory.Trim().Length > 0)
		{
			if (path.StartsWith("./"))
			{
				path = path.Remove(0, 2);
			}
			path = $"{workingDirectory}/{path}".GetFtpPath();
		}
		if ((options & FtpListOption.ForceList) != FtpListOption.ForceList && HasFeature(FtpCapability.MLSD))
		{
			text = "MLSD";
		}
		else if ((options & FtpListOption.UseLS) == FtpListOption.UseLS)
		{
			text = "LS";
		}
		else if ((options & FtpListOption.NameList) == FtpListOption.NameList)
		{
			text = "NLST";
		}
		else
		{
			string text3 = "";
			text = "LIST";
			if ((options & FtpListOption.AllFiles) == FtpListOption.AllFiles)
			{
				text3 += "a";
			}
			if ((options & FtpListOption.Recursive) == FtpListOption.Recursive)
			{
				text3 += "R";
			}
			if (text3.Length > 0)
			{
				text = text + " -" + text3;
			}
		}
		if ((options & FtpListOption.NoPath) != FtpListOption.NoPath)
		{
			text = $"{text} {path.GetFtpPath()}";
		}
		lock (m_lock)
		{
			Execute("TYPE I");
			using FtpDataStream ftpDataStream = OpenDataStream(text, 0L);
			try
			{
				while ((text2 = ftpDataStream.ReadLine(Encoding)) != null)
				{
					if (text2.Length > 0)
					{
						list2.Add(text2);
						FtpTrace.WriteLine(text2);
					}
				}
			}
			finally
			{
				ftpDataStream.Close();
			}
		}
		for (int i = 0; i < list2.Count; i++)
		{
			text2 = list2[i];
			if ((options & FtpListOption.NameList) == FtpListOption.NameList)
			{
				ftpListItem = new FtpListItem
				{
					FullName = text2
				};
				if (DirectoryExists(ftpListItem.FullName))
				{
					ftpListItem.Type = FtpFileSystemObjectType.Directory;
				}
				else
				{
					ftpListItem.Type = FtpFileSystemObjectType.File;
				}
				list.Add(ftpListItem);
			}
			else
			{
				if (text.StartsWith("LIST") && (options & FtpListOption.Recursive) == FtpListOption.Recursive && text2.StartsWith("/") && text2.EndsWith(":"))
				{
					path = text2.TrimEnd(':');
					continue;
				}
				if (i + 1 < list2.Count && (list2[i + 1].StartsWith("\t") || list2[i + 1].StartsWith(" ")))
				{
					text2 += list2[++i];
				}
				ftpListItem = FtpListItem.Parse(path, text2, m_caps);
				if (ftpListItem != null && ftpListItem.Name != "." && ftpListItem.Name != "..")
				{
					list.Add(ftpListItem);
				}
				else
				{
					FtpTrace.WriteLine("Failed to parse file listing: " + text2);
				}
			}
			if (ftpListItem == null)
			{
				continue;
			}
			if (ftpListItem.Type == FtpFileSystemObjectType.Link && (options & FtpListOption.DerefLinks) == FtpListOption.DerefLinks)
			{
				ftpListItem.LinkObject = DereferenceLink(ftpListItem);
			}
			if ((options & FtpListOption.Modify) == FtpListOption.Modify && HasFeature(FtpCapability.MDTM) && (ftpListItem.Modified == DateTime.MinValue || text.StartsWith("LIST")))
			{
				if (ftpListItem.Type == FtpFileSystemObjectType.Directory)
				{
					FtpTrace.WriteLine("Trying to retrieve modification time of a directory, some servers don't like this...");
				}
				DateTime modifiedTime;
				if ((modifiedTime = GetModifiedTime(ftpListItem.FullName)) != DateTime.MinValue)
				{
					ftpListItem.Modified = modifiedTime;
				}
			}
			if ((options & FtpListOption.Size) == FtpListOption.Size && HasFeature(FtpCapability.SIZE) && ftpListItem.Size == -1)
			{
				if (ftpListItem.Type != FtpFileSystemObjectType.Directory)
				{
					ftpListItem.Size = GetFileSize(ftpListItem.FullName);
				}
				else
				{
					ftpListItem.Size = 0L;
				}
			}
		}
		return list.ToArray();
	}

	public IAsyncResult BeginGetListing(AsyncCallback callback, object state)
	{
		return BeginGetListing(null, callback, state);
	}

	public IAsyncResult BeginGetListing(string path, AsyncCallback callback, object state)
	{
		return BeginGetListing(path, FtpListOption.SizeModify, callback, state);
	}

	public IAsyncResult BeginGetListing(string path, FtpListOption options, AsyncCallback callback, object state)
	{
		AsyncGetListing value;
		IAsyncResult asyncResult = (value = GetListing).BeginInvoke(path, options, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public FtpListItem[] EndGetListing(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncGetListing>(ar).EndInvoke(ar);
	}

	public string[] GetNameListing()
	{
		return GetNameListing(null);
	}

	public string[] GetNameListing(string path)
	{
		List<string> list = new List<string>();
		string workingDirectory = GetWorkingDirectory();
		path = path.GetFtpPath();
		if (path == null || path.Trim().Length == 0)
		{
			path = ((workingDirectory == null || workingDirectory.Trim().Length <= 0) ? "./" : workingDirectory);
		}
		else if (!path.StartsWith("/") && workingDirectory != null && workingDirectory.Trim().Length > 0)
		{
			if (path.StartsWith("./"))
			{
				path = path.Remove(0, 2);
			}
			path = $"{workingDirectory}/{path}".GetFtpPath();
		}
		lock (m_lock)
		{
			Execute("TYPE I");
			using FtpDataStream ftpDataStream = OpenDataStream($"NLST {path.GetFtpPath()}", 0L);
			try
			{
				string item;
				while ((item = ftpDataStream.ReadLine(Encoding)) != null)
				{
					list.Add(item);
				}
			}
			finally
			{
				ftpDataStream.Close();
			}
		}
		return list.ToArray();
	}

	public IAsyncResult BeginGetNameListing(string path, AsyncCallback callback, object state)
	{
		AsyncGetNameListing value;
		IAsyncResult asyncResult = (value = GetNameListing).BeginInvoke(path, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public IAsyncResult BeginGetNameListing(AsyncCallback callback, object state)
	{
		return BeginGetNameListing(null, callback, state);
	}

	public string[] EndGetNameListing(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncGetNameListing>(ar).EndInvoke(ar);
	}

	protected void SetDataType(FtpDataType type)
	{
		lock (m_lock)
		{
			switch (type)
			{
			case FtpDataType.ASCII:
			{
				FtpReply reply;
				FtpReply ftpReply = (reply = Execute("TYPE A"));
				if (!ftpReply.Success)
				{
					throw new FtpCommandException(reply);
				}
				break;
			}
			case FtpDataType.Binary:
			{
				FtpReply reply;
				FtpReply ftpReply = (reply = Execute("TYPE I"));
				if (!ftpReply.Success)
				{
					throw new FtpCommandException(reply);
				}
				break;
			}
			default:
				throw new FtpException("Unsupported data type: " + type);
			}
		}
	}

	protected IAsyncResult BeginSetDataType(FtpDataType type, AsyncCallback callback, object state)
	{
		AsyncSetDataType value;
		IAsyncResult asyncResult = (value = SetDataType).BeginInvoke(type, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	protected void EndSetDataType(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncSetDataType>(ar).EndInvoke(ar);
	}

	public void SetWorkingDirectory(string path)
	{
		string ftpPath = path.GetFtpPath();
		if (ftpPath == "." || ftpPath == "./")
		{
			return;
		}
		lock (m_lock)
		{
			FtpReply reply;
			FtpReply ftpReply = (reply = Execute("CWD {0}", ftpPath));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
		}
	}

	public IAsyncResult BeginSetWorkingDirectory(string path, AsyncCallback callback, object state)
	{
		AsyncSetWorkingDirectory value;
		IAsyncResult asyncResult = (value = SetWorkingDirectory).BeginInvoke(path, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public void EndSetWorkingDirectory(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncSetWorkingDirectory>(ar).EndInvoke(ar);
	}

	public string GetWorkingDirectory()
	{
		FtpReply reply;
		lock (m_lock)
		{
			FtpReply ftpReply = (reply = Execute("PWD"));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
		}
		Match match;
		if ((match = Regex.Match(reply.Message, "\"(?<pwd>.*)\"")).Success)
		{
			return match.Groups["pwd"].Value;
		}
		if ((match = Regex.Match(reply.Message, "PWD = (?<pwd>.*)")).Success)
		{
			return match.Groups["pwd"].Value;
		}
		FtpTrace.WriteLine("Failed to parse working directory from: " + reply.Message);
		return "./";
	}

	public IAsyncResult BeginGetWorkingDirectory(AsyncCallback callback, object state)
	{
		AsyncGetWorkingDirectory value;
		IAsyncResult asyncResult = (value = GetWorkingDirectory).BeginInvoke(callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public string EndGetWorkingDirectory(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncGetWorkingDirectory>(ar).EndInvoke(ar);
	}

	public virtual long GetFileSize(string path)
	{
		long result = 0L;
		lock (m_lock)
		{
			FtpReply ftpReply;
			FtpReply ftpReply2 = (ftpReply = Execute("SIZE {0}", path.GetFtpPath()));
			if (!ftpReply2.Success)
			{
				return -1L;
			}
			if (!long.TryParse(ftpReply.Message, out result))
			{
				return -1L;
			}
			return result;
		}
	}

	public IAsyncResult BeginGetFileSize(string path, AsyncCallback callback, object state)
	{
		AsyncGetFileSize value;
		IAsyncResult asyncResult = (value = GetFileSize).BeginInvoke(path, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public long EndGetFileSize(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncGetFileSize>(ar).EndInvoke(ar);
	}

	public virtual DateTime GetModifiedTime(string path)
	{
		DateTime result = DateTime.MinValue;
		lock (m_lock)
		{
			FtpReply ftpReply;
			FtpReply ftpReply2 = (ftpReply = Execute("MDTM {0}", path.GetFtpPath()));
			if (ftpReply2.Success)
			{
				result = ftpReply.Message.GetFtpDate(DateTimeStyles.AssumeUniversal);
			}
		}
		return result;
	}

	public IAsyncResult BeginGetModifiedTime(string path, AsyncCallback callback, object state)
	{
		AsyncGetModifiedTime value;
		IAsyncResult asyncResult = (value = GetModifiedTime).BeginInvoke(path, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public DateTime EndGetModifiedTime(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncGetModifiedTime>(ar).EndInvoke(ar);
	}

	public void DeleteFile(string path)
	{
		lock (m_lock)
		{
			FtpReply reply;
			FtpReply ftpReply = (reply = Execute("DELE {0}", path.GetFtpPath()));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
		}
	}

	public IAsyncResult BeginDeleteFile(string path, AsyncCallback callback, object state)
	{
		AsyncDeleteFile value;
		IAsyncResult asyncResult = (value = DeleteFile).BeginInvoke(path, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public void EndDeleteFile(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncDeleteFile>(ar).EndInvoke(ar);
	}

	public void DeleteDirectory(string path)
	{
		DeleteDirectory(path, force: false);
	}

	public void DeleteDirectory(string path, bool force)
	{
		DeleteDirectory(path, force, (FtpListOption)0);
	}

	public void DeleteDirectory(string path, bool force, FtpListOption options)
	{
		string ftpPath = path.GetFtpPath();
		lock (m_lock)
		{
			if (force)
			{
				FtpListItem[] listing = GetListing(path, options);
				foreach (FtpListItem ftpListItem in listing)
				{
					switch (ftpListItem.Type)
					{
					case FtpFileSystemObjectType.File:
						DeleteFile(ftpListItem.FullName);
						break;
					case FtpFileSystemObjectType.Directory:
						DeleteDirectory(ftpListItem.FullName, force: true, options);
						break;
					default:
						throw new FtpException("Don't know how to delete object type: " + ftpListItem.Type);
					}
				}
			}
			switch (ftpPath)
			{
			case "./":
				return;
			case "/":
				return;
			}
			FtpReply reply;
			FtpReply ftpReply = (reply = Execute("RMD {0}", ftpPath));
			if (ftpReply.Success)
			{
				return;
			}
			throw new FtpCommandException(reply);
		}
	}

	public IAsyncResult BeginDeleteDirectory(string path, AsyncCallback callback, object state)
	{
		return BeginDeleteDirectory(path, force: true, (FtpListOption)0, callback, state);
	}

	public IAsyncResult BeginDeleteDirectory(string path, bool force, AsyncCallback callback, object state)
	{
		return BeginDeleteDirectory(path, force, (FtpListOption)0, callback, state);
	}

	public IAsyncResult BeginDeleteDirectory(string path, bool force, FtpListOption options, AsyncCallback callback, object state)
	{
		AsyncDeleteDirectory value;
		IAsyncResult asyncResult = (value = DeleteDirectory).BeginInvoke(path, force, options, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public void EndDeleteDirectory(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncDeleteDirectory>(ar).EndInvoke(ar);
	}

	public bool DirectoryExists(string path)
	{
		string ftpPath = path.GetFtpPath();
		switch (ftpPath)
		{
		case ".":
		case "./":
		case "/":
			return true;
		default:
			lock (m_lock)
			{
				string workingDirectory = GetWorkingDirectory();
				if (Execute("CWD {0}", ftpPath).Success)
				{
					if (!Execute("CWD {0}", workingDirectory.GetFtpPath()).Success)
					{
						throw new FtpException("DirectoryExists(): Failed to restore the working directory.");
					}
					return true;
				}
			}
			return false;
		}
	}

	public IAsyncResult BeginDirectoryExists(string path, AsyncCallback callback, object state)
	{
		AsyncDirectoryExists value;
		IAsyncResult asyncResult = (value = DirectoryExists).BeginInvoke(path, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public bool EndDirectoryExists(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncDirectoryExists>(ar).EndInvoke(ar);
	}

	public bool FileExists(string path)
	{
		return FileExists(path, (FtpListOption)0);
	}

	public bool FileExists(string path, FtpListOption options)
	{
		string ftpDirectoryName = path.GetFtpDirectoryName();
		lock (m_lock)
		{
			if (!DirectoryExists(ftpDirectoryName))
			{
				return false;
			}
			FtpListItem[] listing = GetListing(ftpDirectoryName, options);
			foreach (FtpListItem ftpListItem in listing)
			{
				if (ftpListItem.Type == FtpFileSystemObjectType.File && ftpListItem.Name == path.GetFtpFileName())
				{
					return true;
				}
			}
		}
		return false;
	}

	public IAsyncResult BeginFileExists(string path, AsyncCallback callback, object state)
	{
		return BeginFileExists(path, (FtpListOption)0, callback, state);
	}

	public IAsyncResult BeginFileExists(string path, FtpListOption options, AsyncCallback callback, object state)
	{
		AsyncFileExists value;
		IAsyncResult asyncResult = (value = FileExists).BeginInvoke(path, options, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public bool EndFileExists(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncFileExists>(ar).EndInvoke(ar);
	}

	public void CreateDirectory(string path)
	{
		CreateDirectory(path, force: true);
	}

	public void CreateDirectory(string path, bool force)
	{
		string ftpPath = path.GetFtpPath();
		switch (ftpPath)
		{
		case "./":
			return;
		case "/":
			return;
		}
		lock (m_lock)
		{
			path = path.GetFtpPath().TrimEnd('/');
			if (force && !DirectoryExists(path.GetFtpDirectoryName()))
			{
				FtpTrace.WriteLine($"CreateDirectory(\"{path}\", {force}): Create non-existent parent: {path.GetFtpDirectoryName()}");
				CreateDirectory(path.GetFtpDirectoryName(), force: true);
			}
			else if (DirectoryExists(path))
			{
				return;
			}
			FtpTrace.WriteLine($"CreateDirectory(\"{ftpPath}\", {force})");
			FtpReply reply;
			FtpReply ftpReply = (reply = Execute("MKD {0}", ftpPath));
			if (ftpReply.Success)
			{
				return;
			}
			throw new FtpCommandException(reply);
		}
	}

	public IAsyncResult BeginCreateDirectory(string path, AsyncCallback callback, object state)
	{
		return BeginCreateDirectory(path, force: true, callback, state);
	}

	public IAsyncResult BeginCreateDirectory(string path, bool force, AsyncCallback callback, object state)
	{
		AsyncCreateDirectory value;
		IAsyncResult asyncResult = (value = CreateDirectory).BeginInvoke(path, force, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public void EndCreateDirectory(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncCreateDirectory>(ar).EndInvoke(ar);
	}

	public void Rename(string path, string dest)
	{
		lock (m_lock)
		{
			FtpReply reply;
			FtpReply ftpReply = (reply = Execute("RNFR {0}", path.GetFtpPath()));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
			ftpReply = (reply = Execute("RNTO {0}", dest.GetFtpPath()));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
		}
	}

	public IAsyncResult BeginRename(string path, string dest, AsyncCallback callback, object state)
	{
		AsyncRename value;
		IAsyncResult asyncResult = (value = Rename).BeginInvoke(path, dest, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public void EndRename(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncRename>(ar).EndInvoke(ar);
	}

	public FtpHashAlgorithm GetHashAlgorithm()
	{
		FtpHashAlgorithm result = FtpHashAlgorithm.NONE;
		lock (m_lock)
		{
			FtpReply ftpReply;
			FtpReply ftpReply2 = (ftpReply = Execute("OPTS HASH"));
			if (ftpReply2.Success)
			{
				switch (ftpReply.Message)
				{
				case "SHA-1":
					result = FtpHashAlgorithm.SHA1;
					break;
				case "SHA-256":
					result = FtpHashAlgorithm.SHA256;
					break;
				case "SHA-512":
					result = FtpHashAlgorithm.SHA512;
					break;
				case "MD5":
					result = FtpHashAlgorithm.MD5;
					break;
				}
			}
		}
		return result;
	}

	public IAsyncResult BeginGetHashAlgorithm(AsyncCallback callback, object state)
	{
		AsyncGetHashAlgorithm value;
		IAsyncResult asyncResult = (value = GetHashAlgorithm).BeginInvoke(callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public FtpHashAlgorithm EndGetHashAlgorithm(IAsyncResult ar)
	{
		return GetAsyncDelegate<AsyncGetHashAlgorithm>(ar).EndInvoke(ar);
	}

	public void SetHashAlgorithm(FtpHashAlgorithm type)
	{
		lock (m_lock)
		{
			if ((HashAlgorithms & type) != type)
			{
				throw new NotImplementedException($"The hash algorithm {type.ToString()} was not advertised by the server.");
			}
			string text = type switch
			{
				FtpHashAlgorithm.SHA1 => "SHA-1", 
				FtpHashAlgorithm.SHA256 => "SHA-256", 
				FtpHashAlgorithm.SHA512 => "SHA-512", 
				FtpHashAlgorithm.MD5 => "MD5", 
				_ => type.ToString(), 
			};
			FtpReply reply;
			FtpReply ftpReply = (reply = Execute("OPTS HASH {0}", text));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
		}
	}

	public IAsyncResult BeginSetHashAlgorithm(FtpHashAlgorithm type, AsyncCallback callback, object state)
	{
		AsyncSetHashAlgorithm value;
		IAsyncResult asyncResult = (value = SetHashAlgorithm).BeginInvoke(type, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public void EndSetHashAlgorithm(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncSetHashAlgorithm>(ar).EndInvoke(ar);
	}

	public FtpHash GetHash(string path)
	{
		FtpHash ftpHash = new FtpHash();
		if (path == null)
		{
			throw new ArgumentException("GetHash(path) argument can't be null");
		}
		FtpReply reply;
		lock (m_lock)
		{
			FtpReply ftpReply = (reply = Execute("HASH {0}", path.GetFtpPath()));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
		}
		Match match;
		if (!(match = Regex.Match(reply.Message, "(?<algorithm>.+)\\s(?<bytestart>\\d+)-(?<byteend>\\d+)\\s(?<hash>.+)\\s(?<filename>.+)")).Success)
		{
			match = Regex.Match(reply.Message, "(?<algorithm>.+)\\s(?<hash>.+)\\s");
		}
		if (match != null && match.Success)
		{
			switch (match.Groups["algorithm"].Value)
			{
			case "SHA-1":
				ftpHash.Algorithm = FtpHashAlgorithm.SHA1;
				break;
			case "SHA-256":
				ftpHash.Algorithm = FtpHashAlgorithm.SHA256;
				break;
			case "SHA-512":
				ftpHash.Algorithm = FtpHashAlgorithm.SHA512;
				break;
			case "MD5":
				ftpHash.Algorithm = FtpHashAlgorithm.MD5;
				break;
			default:
				throw new NotImplementedException("Unknown hash algorithm: " + match.Groups["algorithm"].Value);
			}
			ftpHash.Value = match.Groups["hash"].Value;
		}
		else
		{
			FtpTrace.WriteLine("Failed to parse hash from: {0}", reply.Message);
		}
		return ftpHash;
	}

	public IAsyncResult BeginGetHash(string path, AsyncCallback callback, object state)
	{
		AsyncGetHash value;
		IAsyncResult asyncResult = (value = GetHash).BeginInvoke(path, callback, state);
		lock (m_asyncmethods)
		{
			m_asyncmethods.Add(asyncResult, value);
			return asyncResult;
		}
	}

	public void EndGetHash(IAsyncResult ar)
	{
		GetAsyncDelegate<AsyncGetHash>(ar).EndInvoke(ar);
	}

	public void DisableUTF8()
	{
		lock (m_lock)
		{
			FtpReply reply;
			FtpReply ftpReply = (reply = Execute("OPTS UTF8 OFF"));
			if (!ftpReply.Success)
			{
				throw new FtpCommandException(reply);
			}
			m_textEncoding = Encoding.ASCII;
		}
	}

	public void Dispose()
	{
		lock (m_lock)
		{
			if (IsDisposed)
			{
				return;
			}
			FtpTrace.WriteLine("Disposing FtpClient object...");
			try
			{
				if (IsConnected)
				{
					Disconnect();
				}
			}
			catch (Exception ex)
			{
				FtpTrace.WriteLine("FtpClient.Dispose(): Caught and discarded an exception while disconnecting from host: {0}", ex.ToString());
			}
			if (m_stream != null)
			{
				try
				{
					m_stream.Dispose();
				}
				catch (Exception ex2)
				{
					FtpTrace.WriteLine("FtpClient.Dispose(): Caught and discarded an exception while disposing FtpStream object: {0}", ex2.ToString());
				}
				finally
				{
					m_stream = null;
				}
			}
			m_credentials = null;
			m_textEncoding = null;
			m_host = null;
			m_asyncmethods.Clear();
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	~FtpClient()
	{
		Dispose();
	}

	public static FtpClient Connect(Uri uri, bool checkcertificate)
	{
		FtpClient ftpClient = new FtpClient();
		if (uri == null)
		{
			throw new ArgumentException("Invalid URI object");
		}
		string text = uri.Scheme.ToLower();
		if (!(text == "ftp") && !(text == "ftps"))
		{
			throw new UriFormatException("The specified URI scheme is not supported. Please use ftp:// or ftps://");
		}
		ftpClient.Host = uri.Host;
		ftpClient.Port = uri.Port;
		if (uri.UserInfo != null && uri.UserInfo.Length > 0)
		{
			if (uri.UserInfo.Contains(":"))
			{
				string[] array = uri.UserInfo.Split(':');
				if (array.Length != 2)
				{
					throw new UriFormatException("The user info portion of the URI contains more than 1 colon. The username and password portion of the URI should be URL encoded.");
				}
				ftpClient.Credentials = new NetworkCredential(array[0], array[1]);
			}
			else
			{
				ftpClient.Credentials = new NetworkCredential(uri.UserInfo, "");
			}
		}
		else
		{
			ftpClient.Credentials = new NetworkCredential("ftp", "ftp");
		}
		ftpClient.ValidateCertificate += delegate(FtpClient control, FtpSslValidationEventArgs e)
		{
			if (e.PolicyErrors != SslPolicyErrors.None && checkcertificate)
			{
				e.Accept = false;
			}
			else
			{
				e.Accept = true;
			}
		};
		ftpClient.Connect();
		if (uri.PathAndQuery != null && uri.PathAndQuery.EndsWith("/"))
		{
			ftpClient.SetWorkingDirectory(uri.PathAndQuery);
		}
		return ftpClient;
	}

	public static FtpClient Connect(Uri uri)
	{
		return Connect(uri, checkcertificate: true);
	}

	public static Stream OpenRead(Uri uri, bool checkcertificate, FtpDataType datatype, long restart)
	{
		if (uri.PathAndQuery == null || uri.PathAndQuery.Length == 0)
		{
			throw new UriFormatException("The supplied URI does not contain a valid path.");
		}
		if (uri.PathAndQuery.EndsWith("/"))
		{
			throw new UriFormatException("The supplied URI points at a directory.");
		}
		FtpClient ftpClient = Connect(uri, checkcertificate);
		ftpClient.EnableThreadSafeDataConnections = false;
		return ftpClient.OpenRead(uri.PathAndQuery, datatype, restart);
	}

	public static Stream OpenRead(Uri uri, bool checkcertificate, FtpDataType datatype)
	{
		return OpenRead(uri, checkcertificate, datatype, 0L);
	}

	public static Stream OpenRead(Uri uri, bool checkcertificate)
	{
		return OpenRead(uri, checkcertificate, FtpDataType.Binary, 0L);
	}

	public static Stream OpenRead(Uri uri)
	{
		return OpenRead(uri, checkcertificate: true, FtpDataType.Binary, 0L);
	}

	public static Stream OpenWrite(Uri uri, bool checkcertificate, FtpDataType datatype)
	{
		if (uri.PathAndQuery == null || uri.PathAndQuery.Length == 0)
		{
			throw new UriFormatException("The supplied URI does not contain a valid path.");
		}
		if (uri.PathAndQuery.EndsWith("/"))
		{
			throw new UriFormatException("The supplied URI points at a directory.");
		}
		FtpClient ftpClient = Connect(uri, checkcertificate);
		ftpClient.EnableThreadSafeDataConnections = false;
		return ftpClient.OpenWrite(uri.PathAndQuery, datatype);
	}

	public static Stream OpenWrite(Uri uri, bool checkcertificate)
	{
		return OpenWrite(uri, checkcertificate, FtpDataType.Binary);
	}

	public static Stream OpenWrite(Uri uri)
	{
		return OpenWrite(uri, checkcertificate: true, FtpDataType.Binary);
	}

	public static Stream OpenAppend(Uri uri, bool checkcertificate, FtpDataType datatype)
	{
		if (uri.PathAndQuery == null || uri.PathAndQuery.Length == 0)
		{
			throw new UriFormatException("The supplied URI does not contain a valid path.");
		}
		if (uri.PathAndQuery.EndsWith("/"))
		{
			throw new UriFormatException("The supplied URI points at a directory.");
		}
		FtpClient ftpClient = Connect(uri, checkcertificate);
		ftpClient.EnableThreadSafeDataConnections = false;
		return ftpClient.OpenAppend(uri.PathAndQuery, datatype);
	}

	public static Stream OpenAppend(Uri uri, bool checkcertificate)
	{
		return OpenAppend(uri, checkcertificate, FtpDataType.Binary);
	}

	public static Stream OpenAppend(Uri uri)
	{
		return OpenAppend(uri, checkcertificate: true, FtpDataType.Binary);
	}
}

using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Protocol.Tls;

namespace System.Net.Security;

[System.MonoTODO("Non-X509Certificate2 certificate is not supported")]
public class SslStream : AuthenticatedStream
{
	private SslStreamBase ssl_stream;

	private RemoteCertificateValidationCallback validation_callback;

	private LocalCertificateSelectionCallback selection_callback;

	public override bool CanRead => base.InnerStream.CanRead;

	public override bool CanSeek => base.InnerStream.CanSeek;

	public override bool CanTimeout => base.InnerStream.CanTimeout;

	public override bool CanWrite => base.InnerStream.CanWrite;

	public override long Length => base.InnerStream.Length;

	public override long Position
	{
		get
		{
			return base.InnerStream.Position;
		}
		set
		{
			throw new NotSupportedException("This stream does not support seek operations");
		}
	}

	public override bool IsAuthenticated => ssl_stream != null;

	public override bool IsEncrypted => IsAuthenticated;

	public override bool IsMutuallyAuthenticated => IsAuthenticated && ((!IsServer) ? (LocalCertificate != null) : (RemoteCertificate != null));

	public override bool IsServer => ssl_stream is SslServerStream;

	public override bool IsSigned => IsAuthenticated;

	public override int ReadTimeout
	{
		get
		{
			return base.InnerStream.ReadTimeout;
		}
		set
		{
			base.InnerStream.ReadTimeout = value;
		}
	}

	public override int WriteTimeout
	{
		get
		{
			return base.InnerStream.WriteTimeout;
		}
		set
		{
			base.InnerStream.WriteTimeout = value;
		}
	}

	public virtual bool CheckCertRevocationStatus
	{
		get
		{
			if (!IsAuthenticated)
			{
				return false;
			}
			return ssl_stream.CheckCertRevocationStatus;
		}
	}

	public virtual System.Security.Authentication.CipherAlgorithmType CipherAlgorithm
	{
		get
		{
			CheckConnectionAuthenticated();
			switch (ssl_stream.CipherAlgorithm)
			{
			case Mono.Security.Protocol.Tls.CipherAlgorithmType.Des:
				return System.Security.Authentication.CipherAlgorithmType.Des;
			case Mono.Security.Protocol.Tls.CipherAlgorithmType.None:
				return System.Security.Authentication.CipherAlgorithmType.None;
			case Mono.Security.Protocol.Tls.CipherAlgorithmType.Rc2:
				return System.Security.Authentication.CipherAlgorithmType.Rc2;
			case Mono.Security.Protocol.Tls.CipherAlgorithmType.Rc4:
				return System.Security.Authentication.CipherAlgorithmType.Rc4;
			case Mono.Security.Protocol.Tls.CipherAlgorithmType.TripleDes:
				return System.Security.Authentication.CipherAlgorithmType.TripleDes;
			case Mono.Security.Protocol.Tls.CipherAlgorithmType.Rijndael:
				switch (ssl_stream.CipherStrength)
				{
				case 128:
					return System.Security.Authentication.CipherAlgorithmType.Aes128;
				case 192:
					return System.Security.Authentication.CipherAlgorithmType.Aes192;
				case 256:
					return System.Security.Authentication.CipherAlgorithmType.Aes256;
				}
				break;
			}
			throw new InvalidOperationException("Not supported cipher algorithm is in use. It is likely a bug in SslStream.");
		}
	}

	public virtual int CipherStrength
	{
		get
		{
			CheckConnectionAuthenticated();
			return ssl_stream.CipherStrength;
		}
	}

	public virtual System.Security.Authentication.HashAlgorithmType HashAlgorithm
	{
		get
		{
			CheckConnectionAuthenticated();
			return ssl_stream.HashAlgorithm switch
			{
				Mono.Security.Protocol.Tls.HashAlgorithmType.Md5 => System.Security.Authentication.HashAlgorithmType.Md5, 
				Mono.Security.Protocol.Tls.HashAlgorithmType.None => System.Security.Authentication.HashAlgorithmType.None, 
				Mono.Security.Protocol.Tls.HashAlgorithmType.Sha1 => System.Security.Authentication.HashAlgorithmType.Sha1, 
				_ => throw new InvalidOperationException("Not supported hash algorithm is in use. It is likely a bug in SslStream."), 
			};
		}
	}

	public virtual int HashStrength
	{
		get
		{
			CheckConnectionAuthenticated();
			return ssl_stream.HashStrength;
		}
	}

	public virtual System.Security.Authentication.ExchangeAlgorithmType KeyExchangeAlgorithm
	{
		get
		{
			CheckConnectionAuthenticated();
			return ssl_stream.KeyExchangeAlgorithm switch
			{
				Mono.Security.Protocol.Tls.ExchangeAlgorithmType.DiffieHellman => System.Security.Authentication.ExchangeAlgorithmType.DiffieHellman, 
				Mono.Security.Protocol.Tls.ExchangeAlgorithmType.None => System.Security.Authentication.ExchangeAlgorithmType.None, 
				Mono.Security.Protocol.Tls.ExchangeAlgorithmType.RsaKeyX => System.Security.Authentication.ExchangeAlgorithmType.RsaKeyX, 
				Mono.Security.Protocol.Tls.ExchangeAlgorithmType.RsaSign => System.Security.Authentication.ExchangeAlgorithmType.RsaSign, 
				_ => throw new InvalidOperationException("Not supported exchange algorithm is in use. It is likely a bug in SslStream."), 
			};
		}
	}

	public virtual int KeyExchangeStrength
	{
		get
		{
			CheckConnectionAuthenticated();
			return ssl_stream.KeyExchangeStrength;
		}
	}

	public virtual X509Certificate LocalCertificate
	{
		get
		{
			CheckConnectionAuthenticated();
			return (!IsServer) ? ((SslClientStream)ssl_stream).SelectedClientCertificate : ssl_stream.ServerCertificate;
		}
	}

	public virtual X509Certificate RemoteCertificate
	{
		get
		{
			CheckConnectionAuthenticated();
			return IsServer ? ((SslServerStream)ssl_stream).ClientCertificate : ssl_stream.ServerCertificate;
		}
	}

	public virtual SslProtocols SslProtocol
	{
		get
		{
			CheckConnectionAuthenticated();
			return ssl_stream.SecurityProtocol switch
			{
				Mono.Security.Protocol.Tls.SecurityProtocolType.Default => SslProtocols.Default, 
				Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl2 => SslProtocols.Ssl2, 
				Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl3 => SslProtocols.Ssl3, 
				Mono.Security.Protocol.Tls.SecurityProtocolType.Tls => SslProtocols.Tls, 
				_ => throw new InvalidOperationException("Not supported SSL/TLS protocol is in use. It is likely a bug in SslStream."), 
			};
		}
	}

	public SslStream(Stream innerStream)
		: this(innerStream, leaveStreamOpen: false)
	{
	}

	public SslStream(Stream innerStream, bool leaveStreamOpen)
		: base(innerStream, leaveStreamOpen)
	{
	}

	[System.MonoTODO("certValidationCallback is not passed X509Chain and SslPolicyErrors correctly")]
	public SslStream(Stream innerStream, bool leaveStreamOpen, RemoteCertificateValidationCallback certValidationCallback)
		: this(innerStream, leaveStreamOpen, certValidationCallback, null)
	{
	}

	[System.MonoTODO("certValidationCallback is not passed X509Chain and SslPolicyErrors correctly")]
	public SslStream(Stream innerStream, bool leaveStreamOpen, RemoteCertificateValidationCallback certValidationCallback, LocalCertificateSelectionCallback certSelectionCallback)
		: base(innerStream, leaveStreamOpen)
	{
		validation_callback = certValidationCallback;
		selection_callback = certSelectionCallback;
	}

	private X509Certificate OnCertificateSelection(X509CertificateCollection clientCerts, X509Certificate serverCert, string targetHost, X509CertificateCollection serverRequestedCerts)
	{
		string[] array = new string[serverRequestedCerts?.Count ?? 0];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = serverRequestedCerts[i].GetIssuerName();
		}
		return selection_callback(this, targetHost, clientCerts, serverCert, array);
	}

	public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, AsyncCallback asyncCallback, object asyncState)
	{
		return BeginAuthenticateAsClient(targetHost, new X509CertificateCollection(), SslProtocols.Tls, checkCertificateRevocation: false, asyncCallback, asyncState);
	}

	public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols sslProtocolType, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
	{
		if (IsAuthenticated)
		{
			throw new InvalidOperationException("This SslStream is already authenticated");
		}
		SslClientStream sslClientStream = new SslClientStream(base.InnerStream, targetHost, !base.LeaveInnerStreamOpen, GetMonoSslProtocol(sslProtocolType), clientCertificates);
		sslClientStream.CheckCertRevocationStatus = checkCertificateRevocation;
		sslClientStream.PrivateKeyCertSelectionDelegate = delegate(X509Certificate cert, string host)
		{
			string certHashString = cert.GetCertHashString();
			foreach (X509Certificate clientCertificate in clientCertificates)
			{
				if (!(clientCertificate.GetCertHashString() != certHashString))
				{
					X509Certificate2 x509Certificate2 = clientCertificate as X509Certificate2;
					x509Certificate2 = x509Certificate2 ?? new X509Certificate2(clientCertificate);
					return x509Certificate2.PrivateKey;
				}
			}
			return null;
		};
		if (validation_callback != null)
		{
			sslClientStream.ServerCertValidationDelegate = delegate(X509Certificate cert, int[] certErrors)
			{
				X509Chain x509Chain = new X509Chain();
				X509Certificate2 x509Certificate = cert as X509Certificate2;
				if (x509Certificate == null)
				{
					x509Certificate = new X509Certificate2(cert);
				}
				if (!ServicePointManager.CheckCertificateRevocationList)
				{
					x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
				}
				SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
				for (int i = 0; i < certErrors.Length; i++)
				{
					sslPolicyErrors = certErrors[i] switch
					{
						-2146762490 => sslPolicyErrors | SslPolicyErrors.RemoteCertificateNotAvailable, 
						-2146762481 => sslPolicyErrors | SslPolicyErrors.RemoteCertificateNameMismatch, 
						_ => sslPolicyErrors | SslPolicyErrors.RemoteCertificateChainErrors, 
					};
				}
				x509Chain.Build(x509Certificate);
				X509ChainStatus[] chainStatus = x509Chain.ChainStatus;
				for (int j = 0; j < chainStatus.Length; j++)
				{
					X509ChainStatus x509ChainStatus = chainStatus[j];
					if (x509ChainStatus.Status != 0)
					{
						sslPolicyErrors = (((x509ChainStatus.Status & X509ChainStatusFlags.PartialChain) == 0) ? (sslPolicyErrors | SslPolicyErrors.RemoteCertificateChainErrors) : (sslPolicyErrors | SslPolicyErrors.RemoteCertificateNotAvailable));
					}
				}
				return validation_callback(this, cert, x509Chain, sslPolicyErrors);
			};
		}
		if (selection_callback != null)
		{
			sslClientStream.ClientCertSelectionDelegate = OnCertificateSelection;
		}
		ssl_stream = sslClientStream;
		return BeginWrite(new byte[0], 0, 0, asyncCallback, asyncState);
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
	{
		CheckConnectionAuthenticated();
		return ssl_stream.BeginRead(buffer, offset, count, asyncCallback, asyncState);
	}

	public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, AsyncCallback callback, object asyncState)
	{
		return BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired: false, SslProtocols.Tls, checkCertificateRevocation: false, callback, asyncState);
	}

	public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols sslProtocolType, bool checkCertificateRevocation, AsyncCallback callback, object asyncState)
	{
		if (IsAuthenticated)
		{
			throw new InvalidOperationException("This SslStream is already authenticated");
		}
		SslServerStream sslServerStream = new SslServerStream(base.InnerStream, serverCertificate, clientCertificateRequired, !base.LeaveInnerStreamOpen, GetMonoSslProtocol(sslProtocolType));
		sslServerStream.CheckCertRevocationStatus = checkCertificateRevocation;
		sslServerStream.PrivateKeyCertSelectionDelegate = (X509Certificate cert, string targetHost) => ((serverCertificate as X509Certificate2) ?? new X509Certificate2(serverCertificate))?.PrivateKey;
		if (validation_callback != null)
		{
			sslServerStream.ClientCertValidationDelegate = delegate(X509Certificate cert, int[] certErrors)
			{
				X509Chain x509Chain = null;
				if (cert is X509Certificate2)
				{
					x509Chain = new X509Chain();
					x509Chain.Build((X509Certificate2)cert);
				}
				SslPolicyErrors sslPolicyErrors = ((certErrors.Length > 0) ? SslPolicyErrors.RemoteCertificateChainErrors : SslPolicyErrors.None);
				return validation_callback(this, cert, x509Chain, sslPolicyErrors);
			};
		}
		ssl_stream = sslServerStream;
		return BeginRead(new byte[0], 0, 0, callback, asyncState);
	}

	private Mono.Security.Protocol.Tls.SecurityProtocolType GetMonoSslProtocol(SslProtocols ms)
	{
		return ms switch
		{
			SslProtocols.Ssl2 => Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl2, 
			SslProtocols.Ssl3 => Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl3, 
			SslProtocols.Tls => Mono.Security.Protocol.Tls.SecurityProtocolType.Tls, 
			_ => Mono.Security.Protocol.Tls.SecurityProtocolType.Default, 
		};
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
	{
		CheckConnectionAuthenticated();
		return ssl_stream.BeginWrite(buffer, offset, count, asyncCallback, asyncState);
	}

	public virtual void AuthenticateAsClient(string targetHost)
	{
		AuthenticateAsClient(targetHost, new X509CertificateCollection(), SslProtocols.Tls, checkCertificateRevocation: false);
	}

	public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols sslProtocolType, bool checkCertificateRevocation)
	{
		EndAuthenticateAsClient(BeginAuthenticateAsClient(targetHost, clientCertificates, sslProtocolType, checkCertificateRevocation, null, null));
	}

	public virtual void AuthenticateAsServer(X509Certificate serverCertificate)
	{
		AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, SslProtocols.Tls, checkCertificateRevocation: false);
	}

	public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols sslProtocolType, bool checkCertificateRevocation)
	{
		EndAuthenticateAsServer(BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired, sslProtocolType, checkCertificateRevocation, null, null));
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (ssl_stream != null)
			{
				ssl_stream.Dispose();
			}
			ssl_stream = null;
		}
		base.Dispose(disposing);
	}

	public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
	{
		CheckConnectionAuthenticated();
		if (CanRead)
		{
			ssl_stream.EndRead(asyncResult);
		}
		else
		{
			ssl_stream.EndWrite(asyncResult);
		}
	}

	public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
	{
		CheckConnectionAuthenticated();
		if (CanRead)
		{
			ssl_stream.EndRead(asyncResult);
		}
		else
		{
			ssl_stream.EndWrite(asyncResult);
		}
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		CheckConnectionAuthenticated();
		return ssl_stream.EndRead(asyncResult);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		CheckConnectionAuthenticated();
		ssl_stream.EndWrite(asyncResult);
	}

	public override void Flush()
	{
		CheckConnectionAuthenticated();
		base.InnerStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return EndRead(BeginRead(buffer, offset, count, null, null));
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException("This stream does not support seek operations");
	}

	public override void SetLength(long value)
	{
		base.InnerStream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		EndWrite(BeginWrite(buffer, offset, count, null, null));
	}

	public void Write(byte[] buffer)
	{
		Write(buffer, 0, buffer.Length);
	}

	private void CheckConnectionAuthenticated()
	{
		if (!IsAuthenticated)
		{
			throw new InvalidOperationException("This operation is invalid until it is successfully authenticated");
		}
	}
}

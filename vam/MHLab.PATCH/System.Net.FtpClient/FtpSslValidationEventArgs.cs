using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.FtpClient;

public class FtpSslValidationEventArgs : EventArgs
{
	private X509Certificate m_certificate;

	private X509Chain m_chain;

	private SslPolicyErrors m_policyErrors;

	private bool m_accept;

	public X509Certificate Certificate
	{
		get
		{
			return m_certificate;
		}
		set
		{
			m_certificate = value;
		}
	}

	public X509Chain Chain
	{
		get
		{
			return m_chain;
		}
		set
		{
			m_chain = value;
		}
	}

	public SslPolicyErrors PolicyErrors
	{
		get
		{
			return m_policyErrors;
		}
		set
		{
			m_policyErrors = value;
		}
	}

	public bool Accept
	{
		get
		{
			return m_accept;
		}
		set
		{
			m_accept = value;
		}
	}
}

using System.Security.Principal;
using System.Text;

namespace System.Net;

public sealed class HttpListenerContext
{
	private HttpListenerRequest request;

	private HttpListenerResponse response;

	private IPrincipal user;

	private System.Net.HttpConnection cnc;

	private string error;

	private int err_status = 400;

	internal HttpListener Listener;

	internal int ErrorStatus
	{
		get
		{
			return err_status;
		}
		set
		{
			err_status = value;
		}
	}

	internal string ErrorMessage
	{
		get
		{
			return error;
		}
		set
		{
			error = value;
		}
	}

	internal bool HaveError => error != null;

	internal System.Net.HttpConnection Connection => cnc;

	public HttpListenerRequest Request => request;

	public HttpListenerResponse Response => response;

	public IPrincipal User => user;

	internal HttpListenerContext(System.Net.HttpConnection cnc)
	{
		this.cnc = cnc;
		request = new HttpListenerRequest(this);
		response = new HttpListenerResponse(this);
	}

	internal void ParseAuthentication(AuthenticationSchemes expectedSchemes)
	{
		if (expectedSchemes == AuthenticationSchemes.Anonymous)
		{
			return;
		}
		string text = request.Headers["Authorization"];
		if (text != null && text.Length >= 2)
		{
			string[] array = text.Split(new char[1] { ' ' }, 2);
			if (string.Compare(array[0], "basic", ignoreCase: true) == 0)
			{
				user = ParseBasicAuthentication(array[1]);
			}
		}
	}

	internal IPrincipal ParseBasicAuthentication(string authData)
	{
		try
		{
			string text = null;
			string text2 = null;
			int num = -1;
			string @string = Encoding.Default.GetString(Convert.FromBase64String(authData));
			num = @string.IndexOf(':');
			text2 = @string.Substring(num + 1);
			@string = @string.Substring(0, num);
			num = @string.IndexOf('\\');
			text = ((num <= 0) ? @string : @string.Substring(num));
			HttpListenerBasicIdentity identity = new HttpListenerBasicIdentity(text, text2);
			return new GenericPrincipal(identity, new string[0]);
		}
		catch (Exception)
		{
			return null;
		}
	}
}

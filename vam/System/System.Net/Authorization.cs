namespace System.Net;

public class Authorization
{
	private string token;

	private bool complete;

	private string connectionGroupId;

	private string[] protectionRealm;

	private IAuthenticationModule module;

	public string Message => token;

	public bool Complete => complete;

	public string ConnectionGroupId => connectionGroupId;

	public string[] ProtectionRealm
	{
		get
		{
			return protectionRealm;
		}
		set
		{
			protectionRealm = value;
		}
	}

	internal IAuthenticationModule Module
	{
		get
		{
			return module;
		}
		set
		{
			module = value;
		}
	}

	[System.MonoTODO]
	public bool MutuallyAuthenticated
	{
		get
		{
			throw GetMustImplement();
		}
		set
		{
			throw GetMustImplement();
		}
	}

	public Authorization(string token)
		: this(token, complete: true)
	{
	}

	public Authorization(string token, bool complete)
		: this(token, complete, null)
	{
	}

	public Authorization(string token, bool complete, string connectionGroupId)
	{
		this.token = token;
		this.complete = complete;
		this.connectionGroupId = connectionGroupId;
	}

	private static Exception GetMustImplement()
	{
		return new NotImplementedException();
	}
}

using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel;

[Serializable]
public class WarningException : SystemException
{
	private string helpUrl;

	private string helpTopic;

	public string HelpTopic => helpTopic;

	public string HelpUrl => helpUrl;

	public WarningException(string message)
		: base(message)
	{
	}

	public WarningException(string message, string helpUrl)
		: base(message)
	{
		this.helpUrl = helpUrl;
	}

	public WarningException(string message, string helpUrl, string helpTopic)
		: base(message)
	{
		this.helpUrl = helpUrl;
		this.helpTopic = helpTopic;
	}

	public WarningException()
		: base(global::Locale.GetText("Warning"))
	{
	}

	public WarningException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected WarningException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		try
		{
			helpTopic = info.GetString("helpTopic");
			helpUrl = info.GetString("helpUrl");
		}
		catch (SerializationException)
		{
			helpTopic = info.GetString("HelpTopic");
			helpUrl = info.GetString("HelpUrl");
		}
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		base.GetObjectData(info, context);
		info.AddValue("helpTopic", helpTopic);
		info.AddValue("helpUrl", helpUrl);
	}
}

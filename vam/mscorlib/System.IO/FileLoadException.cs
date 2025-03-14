using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace System.IO;

[Serializable]
[ComVisible(true)]
public class FileLoadException : IOException
{
	private const int Result = -2147024894;

	private string msg;

	private string fileName;

	private string fusionLog;

	public override string Message => msg;

	public string FileName => fileName;

	public string FusionLog
	{
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
		get
		{
			return fusionLog;
		}
	}

	public FileLoadException()
		: base(Locale.GetText("I/O Error"))
	{
		base.HResult = -2147024894;
		msg = Locale.GetText("I/O Error");
	}

	public FileLoadException(string message)
		: base(message)
	{
		base.HResult = -2147024894;
		msg = message;
	}

	public FileLoadException(string message, string fileName)
		: base(message)
	{
		base.HResult = -2147024894;
		msg = message;
		this.fileName = fileName;
	}

	public FileLoadException(string message, Exception inner)
		: base(message, inner)
	{
		base.HResult = -2147024894;
		msg = message;
	}

	public FileLoadException(string message, string fileName, Exception inner)
		: base(message, inner)
	{
		base.HResult = -2147024894;
		msg = message;
		this.fileName = fileName;
	}

	protected FileLoadException(SerializationInfo info, StreamingContext context)
	{
		fileName = info.GetString("FileLoad_FileName");
		fusionLog = info.GetString("FileLoad_FusionLog");
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("FileLoad_FileName", fileName);
		info.AddValue("FileLoad_FusionLog", fusionLog);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(GetType().FullName);
		stringBuilder.AppendFormat(": {0}", msg);
		if (fileName != null)
		{
			stringBuilder.AppendFormat(" : {0}", fileName);
		}
		if (InnerException != null)
		{
			stringBuilder.AppendFormat(" ----> {0}", InnerException);
		}
		if (StackTrace != null)
		{
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(StackTrace);
		}
		return stringBuilder.ToString();
	}
}

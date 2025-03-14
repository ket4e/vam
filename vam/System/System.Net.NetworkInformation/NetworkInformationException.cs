using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Net.NetworkInformation;

[Serializable]
public class NetworkInformationException : Win32Exception
{
	private int error_code;

	public override int ErrorCode => error_code;

	public NetworkInformationException()
	{
	}

	public NetworkInformationException(int errorCode)
		: base(errorCode)
	{
	}

	protected NetworkInformationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		error_code = info.GetInt32("ErrorCode");
	}
}

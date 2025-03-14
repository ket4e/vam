using System.Runtime.Serialization;

namespace System.EnterpriseServices;

[Serializable]
public sealed class RegistrationException : SystemException
{
	private RegistrationErrorInfo[] errorInfo;

	public RegistrationErrorInfo[] ErrorInfo => errorInfo;

	[System.MonoTODO]
	public RegistrationException(string msg)
		: base(msg)
	{
	}

	public RegistrationException()
		: this("Registration error")
	{
	}

	public RegistrationException(string msg, Exception innerException)
		: base(msg, innerException)
	{
	}

	[System.MonoTODO]
	public override void GetObjectData(SerializationInfo info, StreamingContext ctx)
	{
		throw new NotImplementedException();
	}
}

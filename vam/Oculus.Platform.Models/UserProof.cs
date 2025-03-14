using System;

namespace Oculus.Platform.Models;

public class UserProof
{
	public readonly string Value;

	public UserProof(IntPtr o)
	{
		Value = CAPI.ovr_UserProof_GetNonce(o);
	}
}

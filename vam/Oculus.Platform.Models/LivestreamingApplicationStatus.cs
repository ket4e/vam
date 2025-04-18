using System;

namespace Oculus.Platform.Models;

public class LivestreamingApplicationStatus
{
	public readonly bool StreamingEnabled;

	public LivestreamingApplicationStatus(IntPtr o)
	{
		StreamingEnabled = CAPI.ovr_LivestreamingApplicationStatus_GetStreamingEnabled(o);
	}
}

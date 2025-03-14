using System;

namespace Un4seen.BassWasapi;

[Serializable]
public class BassWasapiHandlerEventArgs : EventArgs
{
	private readonly BassWasapiHandlerSyncType _syncType = BassWasapiHandlerSyncType.SourceResumed;

	private readonly int _data;

	public BassWasapiHandlerSyncType SyncType => _syncType;

	public int Data => _data;

	public BassWasapiHandlerEventArgs(BassWasapiHandlerSyncType syncType, int data)
	{
		_syncType = syncType;
	}
}

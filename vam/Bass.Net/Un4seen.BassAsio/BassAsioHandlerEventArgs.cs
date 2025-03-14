using System;

namespace Un4seen.BassAsio;

[Serializable]
public class BassAsioHandlerEventArgs : EventArgs
{
	private readonly BassAsioHandlerSyncType _syncType = BassAsioHandlerSyncType.SourceResumed;

	private readonly int _data;

	public BassAsioHandlerSyncType SyncType => _syncType;

	public int Data => _data;

	public BassAsioHandlerEventArgs(BassAsioHandlerSyncType syncType, int data)
	{
		_syncType = syncType;
	}
}

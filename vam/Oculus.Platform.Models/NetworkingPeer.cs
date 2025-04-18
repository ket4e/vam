namespace Oculus.Platform.Models;

public class NetworkingPeer
{
	public ulong ID { get; private set; }

	public PeerConnectionState State { get; private set; }

	public NetworkingPeer(ulong id, PeerConnectionState state)
	{
		ID = id;
		State = state;
	}
}

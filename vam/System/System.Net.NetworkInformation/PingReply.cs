namespace System.Net.NetworkInformation;

public class PingReply
{
	private IPAddress address;

	private byte[] buffer;

	private PingOptions options;

	private long rtt;

	private IPStatus status;

	public IPAddress Address => address;

	public byte[] Buffer => buffer;

	public PingOptions Options => options;

	public long RoundtripTime => rtt;

	public IPStatus Status => status;

	internal PingReply(IPAddress address, byte[] buffer, PingOptions options, long roundtripTime, IPStatus status)
	{
		this.address = address;
		this.buffer = buffer;
		this.options = options;
		rtt = roundtripTime;
		this.status = status;
	}
}

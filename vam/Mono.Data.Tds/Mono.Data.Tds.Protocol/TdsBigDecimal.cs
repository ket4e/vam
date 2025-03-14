namespace Mono.Data.Tds.Protocol;

public class TdsBigDecimal
{
	private bool isNegative;

	private byte precision;

	private byte scale;

	private int[] data;

	public int[] Data => data;

	public byte Precision => precision;

	public byte Scale => scale;

	public bool IsNegative => isNegative;

	public TdsBigDecimal(byte precision, byte scale, bool isNegative, int[] data)
	{
		this.isNegative = isNegative;
		this.precision = precision;
		this.scale = scale;
		this.data = data;
	}
}

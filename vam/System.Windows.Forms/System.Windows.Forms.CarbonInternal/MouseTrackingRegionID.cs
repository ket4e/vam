namespace System.Windows.Forms.CarbonInternal;

internal struct MouseTrackingRegionID
{
	public uint signature;

	public uint id;

	public MouseTrackingRegionID(uint signature, uint id)
	{
		this.signature = signature;
		this.id = id;
	}
}

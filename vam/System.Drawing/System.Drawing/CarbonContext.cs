namespace System.Drawing;

internal struct CarbonContext
{
	public IntPtr port;

	public IntPtr ctx;

	public int width;

	public int height;

	public CarbonContext(IntPtr port, IntPtr ctx, int width, int height)
	{
		this.port = port;
		this.ctx = ctx;
		this.width = width;
		this.height = height;
	}
}

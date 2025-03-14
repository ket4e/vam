namespace System.Drawing;

public sealed class BufferedGraphicsManager
{
	private static BufferedGraphicsContext graphics_context;

	public static BufferedGraphicsContext Current => graphics_context;

	private BufferedGraphicsManager()
	{
	}

	static BufferedGraphicsManager()
	{
		graphics_context = new BufferedGraphicsContext();
	}
}

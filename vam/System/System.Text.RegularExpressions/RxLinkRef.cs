namespace System.Text.RegularExpressions;

internal class RxLinkRef : System.Text.RegularExpressions.LinkRef
{
	public int[] offsets;

	public int current;

	public RxLinkRef()
	{
		offsets = new int[8];
	}

	public void PushInstructionBase(int offset)
	{
		if (((uint)current & (true ? 1u : 0u)) != 0)
		{
			throw new Exception();
		}
		if (current == offsets.Length)
		{
			int[] destinationArray = new int[offsets.Length * 2];
			Array.Copy(offsets, destinationArray, offsets.Length);
			offsets = destinationArray;
		}
		offsets[current++] = offset;
	}

	public void PushOffsetPosition(int offset)
	{
		if ((current & 1) == 0)
		{
			throw new Exception();
		}
		offsets[current++] = offset;
	}
}

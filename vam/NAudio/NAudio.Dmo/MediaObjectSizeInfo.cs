namespace NAudio.Dmo;

public class MediaObjectSizeInfo
{
	public int Size { get; private set; }

	public int MaxLookahead { get; }

	public int Alignment { get; }

	public MediaObjectSizeInfo(int size, int maxLookahead, int alignment)
	{
		Size = size;
		MaxLookahead = maxLookahead;
		Alignment = alignment;
	}

	public override string ToString()
	{
		return $"Size: {Size}, Alignment {Alignment}, MaxLookahead {MaxLookahead}";
	}
}

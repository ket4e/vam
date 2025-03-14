namespace System.Runtime.InteropServices;

public sealed class HandleCollector
{
	private int count;

	private readonly int init;

	private readonly int max;

	private readonly string name;

	private DateTime previous_collection = DateTime.MinValue;

	public int Count => count;

	public int InitialThreshold => init;

	public int MaximumThreshold => max;

	public string Name => name;

	public HandleCollector(string name, int initialThreshold)
		: this(name, initialThreshold, int.MaxValue)
	{
	}

	public HandleCollector(string name, int initialThreshold, int maximumThreshold)
	{
		if (initialThreshold < 0)
		{
			throw new ArgumentOutOfRangeException("initialThreshold", "initialThreshold must not be less than zero");
		}
		if (maximumThreshold < 0)
		{
			throw new ArgumentOutOfRangeException("maximumThreshold", "maximumThreshold must not be less than zero");
		}
		if (maximumThreshold < initialThreshold)
		{
			throw new ArgumentException("maximumThreshold must not be less than initialThreshold");
		}
		this.name = name;
		init = initialThreshold;
		max = maximumThreshold;
	}

	public void Add()
	{
		if (++count >= max)
		{
			GC.Collect(GC.MaxGeneration);
		}
		else if (count >= init && DateTime.Now - previous_collection > TimeSpan.FromSeconds(5.0))
		{
			GC.Collect(GC.MaxGeneration);
			previous_collection = DateTime.Now;
		}
	}

	public void Remove()
	{
		if (count == 0)
		{
			throw new InvalidOperationException("Cannot call Remove method when Count is 0");
		}
		count--;
	}
}

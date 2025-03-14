namespace Leap.Unity;

public class SlidingMax
{
	private struct IndexValuePair
	{
		public int index;

		public float value;

		public IndexValuePair(int index, float value)
		{
			this.index = index;
			this.value = value;
		}
	}

	private int _history;

	private int _count;

	private Deque<IndexValuePair> _buffer = new Deque<IndexValuePair>();

	public float Max => _buffer.Back.value;

	public SlidingMax(int history)
	{
		_history = history;
		_count = 0;
	}

	public void AddValue(float value)
	{
		while (_buffer.Count != 0 && _buffer.Front.value <= value)
		{
			_buffer.PopFront();
		}
		_buffer.PushFront(new IndexValuePair(_count, value));
		_count++;
		while (_buffer.Back.index < _count - _history)
		{
			_buffer.PopBack();
		}
	}
}

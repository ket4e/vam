namespace System.Xml.XPath;

internal class PrecedingIterator : SimpleIterator
{
	private bool finished;

	private bool started;

	private XPathNavigator startPosition;

	public override bool ReverseAxis => true;

	public PrecedingIterator(BaseIterator iter)
		: base(iter)
	{
		startPosition = iter.Current.Clone();
	}

	private PrecedingIterator(PrecedingIterator other)
		: base(other, clone: true)
	{
		startPosition = other.startPosition;
		started = other.started;
		finished = other.finished;
	}

	public override XPathNodeIterator Clone()
	{
		return new PrecedingIterator(this);
	}

	public override bool MoveNextCore()
	{
		if (finished)
		{
			return false;
		}
		if (!started)
		{
			started = true;
			_nav.MoveToRoot();
		}
		bool flag = true;
		while (flag)
		{
			if (!_nav.MoveToFirstChild())
			{
				while (!_nav.MoveToNext())
				{
					if (!_nav.MoveToParent())
					{
						finished = true;
						return false;
					}
				}
			}
			if (_nav.IsDescendant(startPosition))
			{
				continue;
			}
			flag = false;
			break;
		}
		if (_nav.ComparePosition(startPosition) != 0)
		{
			finished = true;
			return false;
		}
		return true;
	}
}

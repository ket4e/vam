using System;

namespace Valve.Newtonsoft.Json.Linq;

public class JsonLoadSettings
{
	private CommentHandling _commentHandling;

	private LineInfoHandling _lineInfoHandling;

	public CommentHandling CommentHandling
	{
		get
		{
			return _commentHandling;
		}
		set
		{
			if (value < CommentHandling.Ignore || value > CommentHandling.Load)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_commentHandling = value;
		}
	}

	public LineInfoHandling LineInfoHandling
	{
		get
		{
			return _lineInfoHandling;
		}
		set
		{
			if (value < LineInfoHandling.Ignore || value > LineInfoHandling.Load)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_lineInfoHandling = value;
		}
	}
}

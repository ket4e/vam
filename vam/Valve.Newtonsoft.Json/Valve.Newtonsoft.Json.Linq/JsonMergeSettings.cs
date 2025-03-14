using System;

namespace Valve.Newtonsoft.Json.Linq;

public class JsonMergeSettings
{
	private MergeArrayHandling _mergeArrayHandling;

	private MergeNullValueHandling _mergeNullValueHandling;

	public MergeArrayHandling MergeArrayHandling
	{
		get
		{
			return _mergeArrayHandling;
		}
		set
		{
			if (value < MergeArrayHandling.Concat || value > MergeArrayHandling.Merge)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_mergeArrayHandling = value;
		}
	}

	public MergeNullValueHandling MergeNullValueHandling
	{
		get
		{
			return _mergeNullValueHandling;
		}
		set
		{
			if (value < MergeNullValueHandling.Ignore || value > MergeNullValueHandling.Merge)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_mergeNullValueHandling = value;
		}
	}
}

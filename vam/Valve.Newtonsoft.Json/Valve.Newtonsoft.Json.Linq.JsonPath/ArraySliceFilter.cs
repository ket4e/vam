using System;
using System.Collections.Generic;
using System.Globalization;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Linq.JsonPath;

internal class ArraySliceFilter : PathFilter
{
	public int? Start { get; set; }

	public int? End { get; set; }

	public int? Step { get; set; }

	public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
	{
		if (Step == 0)
		{
			throw new JsonException("Step cannot be zero.");
		}
		foreach (JToken t in current)
		{
			if (t is JArray a)
			{
				int stepCount = Step ?? 1;
				int num = Start ?? ((stepCount <= 0) ? (a.Count - 1) : 0);
				int stopIndex = End ?? ((stepCount > 0) ? a.Count : (-1));
				if (Start < 0)
				{
					num = a.Count + num;
				}
				if (End < 0)
				{
					stopIndex = a.Count + stopIndex;
				}
				num = Math.Max(num, (stepCount <= 0) ? int.MinValue : 0);
				num = Math.Min(num, (stepCount > 0) ? a.Count : (a.Count - 1));
				stopIndex = Math.Max(stopIndex, -1);
				stopIndex = Math.Min(stopIndex, a.Count);
				bool positiveStep = stepCount > 0;
				if (IsValid(num, stopIndex, positiveStep))
				{
					for (int i = num; IsValid(i, stopIndex, positiveStep); i += stepCount)
					{
						yield return a[i];
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Array slice of {0} to {1} returned no results.".FormatWith(CultureInfo.InvariantCulture, Start.HasValue ? Start.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) : "*", End.HasValue ? End.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) : "*"));
				}
			}
			else if (errorWhenNoMatch)
			{
				throw new JsonException("Array slice is not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, t.GetType().Name));
			}
		}
	}

	private bool IsValid(int index, int stopIndex, bool positiveStep)
	{
		if (positiveStep)
		{
			return index < stopIndex;
		}
		return index > stopIndex;
	}
}

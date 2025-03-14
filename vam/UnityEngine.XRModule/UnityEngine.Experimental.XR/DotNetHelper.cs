using System.Collections.Generic;

namespace UnityEngine.Experimental.XR;

internal static class DotNetHelper
{
	public static bool TryCopyFixedArrayToList<T>(T[] fixedArrayIn, List<T> listOut)
	{
		if (fixedArrayIn == null)
		{
			return false;
		}
		int num = fixedArrayIn.Length;
		listOut.Clear();
		if (listOut.Capacity < num)
		{
			listOut.Capacity = num;
		}
		listOut.AddRange(fixedArrayIn);
		return true;
	}
}

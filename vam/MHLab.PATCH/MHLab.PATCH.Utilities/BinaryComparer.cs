namespace MHLab.PATCH.Utilities;

internal class BinaryComparer
{
	public static bool CompareArray(byte[] strA, byte[] strB)
	{
		int num = strA.Length;
		if (num != strB.Length)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (strA[i] != strB[i])
			{
				return false;
			}
		}
		return true;
	}
}

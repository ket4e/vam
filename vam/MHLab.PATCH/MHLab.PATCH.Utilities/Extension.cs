using System.IO;

namespace MHLab.PATCH.Utilities;

internal static class Extension
{
	public static void CopyTo(this Stream input, Stream output)
	{
		byte[] array = new byte[16384];
		int count;
		while ((count = input.Read(array, 0, array.Length)) > 0)
		{
			output.Write(array, 0, count);
		}
	}
}

using System.Drawing;
using System.IO;
using System.Reflection;

namespace System.Windows.Forms;

internal class ResourceImageLoader
{
	private static Assembly assembly = typeof(ResourceImageLoader).Assembly;

	internal static Bitmap Get(string name)
	{
		Stream manifestResourceStream = assembly.GetManifestResourceStream(name);
		if (manifestResourceStream == null)
		{
			Console.WriteLine("Failed to read {0}", name);
			return null;
		}
		return new Bitmap(manifestResourceStream);
	}

	internal static Icon GetIcon(string name)
	{
		Stream manifestResourceStream = assembly.GetManifestResourceStream(name);
		if (manifestResourceStream == null)
		{
			Console.WriteLine("Failed to read {0}", name);
			return null;
		}
		return new Icon(manifestResourceStream);
	}
}

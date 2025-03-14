using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Design;

public class DesigntimeLicenseContextSerializer
{
	private DesigntimeLicenseContextSerializer()
	{
	}

	public static void Serialize(Stream o, string cryptoKey, DesigntimeLicenseContext context)
	{
		object[] array = new object[2] { cryptoKey, null };
		Hashtable hashtable = new Hashtable();
		foreach (DictionaryEntry key in context.keys)
		{
			hashtable.Add(((Type)key.Key).AssemblyQualifiedName, key.Value);
		}
		array[1] = hashtable;
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(o, array);
	}
}

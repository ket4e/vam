using System.Xml.Serialization.Advanced;

namespace System.Xml.Serialization;

public abstract class SchemaImporter
{
	private SchemaImporterExtensionCollection extensions;

	public SchemaImporterExtensionCollection Extensions
	{
		get
		{
			if (extensions == null)
			{
				extensions = new SchemaImporterExtensionCollection();
			}
			return extensions;
		}
	}

	internal SchemaImporter()
	{
	}
}

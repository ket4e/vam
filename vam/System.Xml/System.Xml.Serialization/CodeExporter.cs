using System.CodeDom;

namespace System.Xml.Serialization;

public abstract class CodeExporter
{
	internal MapCodeGenerator codeGenerator;

	public CodeAttributeDeclarationCollection IncludeMetadata => codeGenerator.IncludeMetadata;

	internal CodeExporter()
	{
	}
}

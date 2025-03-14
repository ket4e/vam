namespace System.Configuration;

internal class SectionData
{
	public readonly string SectionName;

	public readonly string TypeName;

	public readonly bool AllowLocation;

	public readonly System.Configuration.AllowDefinition AllowDefinition;

	public string FileName;

	public readonly bool RequirePermission;

	public SectionData(string sectionName, string typeName, bool allowLocation, System.Configuration.AllowDefinition allowDefinition, bool requirePermission)
	{
		SectionName = sectionName;
		TypeName = typeName;
		AllowLocation = allowLocation;
		AllowDefinition = allowDefinition;
		RequirePermission = requirePermission;
	}
}

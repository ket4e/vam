namespace System.Configuration;

public class ConfigurationSectionGroup
{
	private bool require_declaration;

	private bool declared;

	private string name;

	private string type_name;

	private ConfigurationSectionCollection sections;

	private ConfigurationSectionGroupCollection groups;

	private Configuration config;

	private SectionGroupInfo group;

	private bool initialized;

	private Configuration Config
	{
		get
		{
			if (config == null)
			{
				throw new InvalidOperationException("ConfigurationSectionGroup cannot be edited until it is added to a Configuration instance as its descendant");
			}
			return config;
		}
	}

	public bool IsDeclared => declared;

	[System.MonoTODO]
	public bool IsDeclarationRequired => require_declaration;

	public string Name => name;

	[System.MonoInternalNote("Check if this is correct")]
	public string SectionGroupName => group.XPath;

	public ConfigurationSectionGroupCollection SectionGroups
	{
		get
		{
			if (groups == null)
			{
				groups = new ConfigurationSectionGroupCollection(Config, group);
			}
			return groups;
		}
	}

	public ConfigurationSectionCollection Sections
	{
		get
		{
			if (sections == null)
			{
				sections = new ConfigurationSectionCollection(Config, group);
			}
			return sections;
		}
	}

	public string Type
	{
		get
		{
			return type_name;
		}
		set
		{
			type_name = value;
		}
	}

	internal void Initialize(Configuration config, SectionGroupInfo group)
	{
		if (initialized)
		{
			throw new SystemException("INTERNAL ERROR: this configuration section is being initialized twice: " + GetType());
		}
		initialized = true;
		this.config = config;
		this.group = group;
	}

	internal void SetName(string name)
	{
		this.name = name;
	}

	[System.MonoTODO]
	public void ForceDeclaration(bool require)
	{
		require_declaration = require;
	}

	public void ForceDeclaration()
	{
		ForceDeclaration(require: true);
	}
}

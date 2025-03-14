namespace System.Configuration;

public sealed class SectionInformation
{
	private ConfigurationSection parent;

	private ConfigurationAllowDefinition allow_definition = ConfigurationAllowDefinition.Everywhere;

	private ConfigurationAllowExeDefinition allow_exe_definition = ConfigurationAllowExeDefinition.MachineToApplication;

	private bool allow_location;

	private bool allow_override;

	private bool inherit_on_child_apps;

	private bool restart_on_external_changes;

	private bool require_permission;

	private string config_source;

	private bool force_update;

	private bool is_declared;

	private bool is_locked;

	private string name;

	private string type_name;

	private string raw_xml;

	private ProtectedConfigurationProvider protection_provider;

	internal string ConfigFilePath { get; set; }

	public ConfigurationAllowDefinition AllowDefinition
	{
		get
		{
			return allow_definition;
		}
		set
		{
			allow_definition = value;
		}
	}

	public ConfigurationAllowExeDefinition AllowExeDefinition
	{
		get
		{
			return allow_exe_definition;
		}
		set
		{
			allow_exe_definition = value;
		}
	}

	public bool AllowLocation
	{
		get
		{
			return allow_location;
		}
		set
		{
			allow_location = value;
		}
	}

	public bool AllowOverride
	{
		get
		{
			return allow_override;
		}
		set
		{
			allow_override = value;
		}
	}

	public string ConfigSource
	{
		get
		{
			return config_source;
		}
		set
		{
			config_source = value;
		}
	}

	public bool ForceSave
	{
		get
		{
			return force_update;
		}
		set
		{
			force_update = value;
		}
	}

	public bool InheritInChildApplications
	{
		get
		{
			return inherit_on_child_apps;
		}
		set
		{
			inherit_on_child_apps = value;
		}
	}

	[System.MonoTODO]
	public bool IsDeclarationRequired
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	[System.MonoTODO]
	public bool IsDeclared => is_declared;

	[System.MonoTODO]
	public bool IsLocked => is_locked;

	public bool IsProtected => protection_provider != null;

	public string Name => name;

	public ProtectedConfigurationProvider ProtectionProvider => protection_provider;

	[System.MonoTODO]
	public bool RequirePermission
	{
		get
		{
			return require_permission;
		}
		set
		{
			require_permission = value;
		}
	}

	[System.MonoTODO]
	public bool RestartOnExternalChanges
	{
		get
		{
			return restart_on_external_changes;
		}
		set
		{
			restart_on_external_changes = value;
		}
	}

	[System.MonoTODO]
	public string SectionName => name;

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

	[System.MonoTODO("default value for require_permission")]
	internal SectionInformation()
	{
		allow_definition = ConfigurationAllowDefinition.Everywhere;
		allow_location = true;
		allow_override = true;
		inherit_on_child_apps = true;
		restart_on_external_changes = true;
	}

	public ConfigurationSection GetParentSection()
	{
		return parent;
	}

	internal void SetParentSection(ConfigurationSection parent)
	{
		this.parent = parent;
	}

	public string GetRawXml()
	{
		return raw_xml;
	}

	public void ProtectSection(string provider)
	{
		protection_provider = ProtectedConfiguration.GetProvider(provider, throwOnError: true);
	}

	[System.MonoTODO]
	public void ForceDeclaration(bool require)
	{
	}

	public void ForceDeclaration()
	{
		ForceDeclaration(require: true);
	}

	[System.MonoTODO]
	public void RevertToParent()
	{
		throw new NotImplementedException();
	}

	public void UnprotectSection()
	{
		protection_provider = null;
	}

	public void SetRawXml(string xml)
	{
		raw_xml = xml;
	}

	[System.MonoTODO]
	internal void SetName(string name)
	{
		this.name = name;
	}
}

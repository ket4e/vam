using System.Configuration;

namespace System.Windows.Forms;

public sealed class WindowsFormsSection : ConfigurationSection
{
	private ConfigurationPropertyCollection properties;

	private ConfigurationProperty jit_debugging;

	[ConfigurationProperty("jitDebugging", DefaultValue = "False")]
	public bool JitDebugging
	{
		get
		{
			return (bool)base[jit_debugging];
		}
		set
		{
			base[jit_debugging] = value;
		}
	}

	protected override ConfigurationPropertyCollection Properties => properties;

	public WindowsFormsSection()
	{
		properties = new ConfigurationPropertyCollection();
		jit_debugging = new ConfigurationProperty("jitDebugging", typeof(bool), false);
		properties.Add(jit_debugging);
	}
}

using System.Configuration;

namespace System.CodeDom.Compiler;

internal sealed class CodeDomConfigurationHandler : ConfigurationSection
{
	private static ConfigurationPropertyCollection properties;

	private static ConfigurationProperty compilersProp;

	private static System.CodeDom.Compiler.CompilerCollection default_compilers;

	[ConfigurationProperty("compilers")]
	public System.CodeDom.Compiler.CompilerCollection Compilers => (System.CodeDom.Compiler.CompilerCollection)base[compilersProp];

	public CompilerInfo[] CompilerInfos => ((System.CodeDom.Compiler.CompilerCollection)base[compilersProp])?.CompilerInfos;

	protected override ConfigurationPropertyCollection Properties => properties;

	static CodeDomConfigurationHandler()
	{
		default_compilers = new System.CodeDom.Compiler.CompilerCollection();
		compilersProp = new ConfigurationProperty("compilers", typeof(System.CodeDom.Compiler.CompilerCollection), default_compilers);
		properties = new ConfigurationPropertyCollection();
		properties.Add(compilersProp);
	}

	protected override void InitializeDefault()
	{
		compilersProp = new ConfigurationProperty("compilers", typeof(System.CodeDom.Compiler.CompilerCollection), default_compilers);
	}

	[System.MonoTODO]
	protected override void PostDeserialize()
	{
		base.PostDeserialize();
	}

	protected override object GetRuntimeObject()
	{
		return this;
	}
}

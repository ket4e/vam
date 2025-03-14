namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public class RunInstallerAttribute : Attribute
{
	public static readonly RunInstallerAttribute Yes = new RunInstallerAttribute(runInstaller: true);

	public static readonly RunInstallerAttribute No = new RunInstallerAttribute(runInstaller: false);

	public static readonly RunInstallerAttribute Default = new RunInstallerAttribute(runInstaller: false);

	private bool runInstaller;

	public bool RunInstaller => runInstaller;

	public RunInstallerAttribute(bool runInstaller)
	{
		this.runInstaller = runInstaller;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is RunInstallerAttribute))
		{
			return false;
		}
		return ((RunInstallerAttribute)obj).RunInstaller.Equals(runInstaller);
	}

	public override int GetHashCode()
	{
		return runInstaller.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return Equals(Default);
	}
}

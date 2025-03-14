using System.Security.Permissions;

namespace System.Data.SqlClient;

public sealed class SqlDependency
{
	private string uniqueId = Guid.NewGuid().ToString();

	public string Id => uniqueId;

	[System.MonoTODO]
	public bool HasChanges => true;

	public event OnChangeEventHandler OnChange;

	[System.MonoTODO]
	public SqlDependency()
	{
	}

	[System.MonoTODO]
	public SqlDependency(SqlCommand command)
	{
	}

	[System.MonoTODO]
	public SqlDependency(SqlCommand command, string options, int timeout)
	{
	}

	[System.MonoTODO]
	public void AddCommandDependency(SqlCommand command)
	{
	}

	[System.MonoTODO]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public static bool Start(string connectionString)
	{
		return true;
	}

	[System.MonoTODO]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public static bool Start(string connectionString, string queue)
	{
		return true;
	}

	[System.MonoTODO]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public static bool Stop(string connectionString)
	{
		return true;
	}

	[System.MonoTODO]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public static bool Stop(string connectionString, string queue)
	{
		return true;
	}
}

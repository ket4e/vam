using System.ComponentModel;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb;

[Serializable]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class OleDbPermissionAttribute : DBDataPermissionAttribute
{
	private string _provider;

	[Obsolete]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public string Provider
	{
		get
		{
			if (_provider == null)
			{
				return string.Empty;
			}
			return _provider;
		}
		set
		{
			_provider = value;
		}
	}

	public OleDbPermissionAttribute(SecurityAction action)
		: base(action)
	{
	}

	public override IPermission CreatePermission()
	{
		return new OleDbPermission(this);
	}
}

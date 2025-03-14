using System.ComponentModel;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb;

[Serializable]
public sealed class OleDbPermission : DBDataPermission
{
	private string _provider;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[Obsolete]
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

	[Obsolete("use OleDbPermission(PermissionState.None)", true)]
	public OleDbPermission()
		: base(PermissionState.None)
	{
	}

	public OleDbPermission(PermissionState state)
		: base(state)
	{
	}

	[Obsolete("use OleDbPermission(PermissionState.None)", true)]
	public OleDbPermission(PermissionState state, bool allowBlankPassword)
		: base(state)
	{
		base.AllowBlankPassword = allowBlankPassword;
	}

	internal OleDbPermission(DBDataPermission permission)
		: base(permission)
	{
	}

	internal OleDbPermission(DBDataPermissionAttribute attribute)
		: base(attribute)
	{
	}

	public override IPermission Copy()
	{
		return new OleDbPermission(this);
	}
}

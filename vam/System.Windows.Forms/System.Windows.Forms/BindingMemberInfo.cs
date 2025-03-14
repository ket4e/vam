namespace System.Windows.Forms;

public struct BindingMemberInfo
{
	private string data_member;

	private string data_field;

	private string data_path;

	public string BindingField => data_field;

	public string BindingMember => data_member;

	public string BindingPath => data_path;

	public BindingMemberInfo(string dataMember)
	{
		if (dataMember != null)
		{
			data_member = dataMember;
		}
		else
		{
			data_member = string.Empty;
		}
		int num = data_member.LastIndexOf('.');
		if (num != -1)
		{
			data_field = data_member.Substring(num + 1);
			data_path = data_member.Substring(0, num);
		}
		else
		{
			data_field = data_member;
			data_path = string.Empty;
		}
	}

	public override bool Equals(object otherObject)
	{
		if (otherObject is BindingMemberInfo)
		{
			return data_field == ((BindingMemberInfo)otherObject).data_field && data_path == ((BindingMemberInfo)otherObject).data_path && data_member == ((BindingMemberInfo)otherObject).data_member;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return data_member.GetHashCode();
	}

	public static bool operator ==(BindingMemberInfo a, BindingMemberInfo b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(BindingMemberInfo a, BindingMemberInfo b)
	{
		return !a.Equals(b);
	}
}

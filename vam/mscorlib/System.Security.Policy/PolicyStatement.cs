using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public sealed class PolicyStatement : ISecurityEncodable, ISecurityPolicyEncodable
{
	private PermissionSet perms;

	private PolicyStatementAttribute attrs;

	public PermissionSet PermissionSet
	{
		get
		{
			if (perms == null)
			{
				perms = new PermissionSet(PermissionState.None);
				perms.SetReadOnly(value: true);
			}
			return perms;
		}
		set
		{
			perms = value;
		}
	}

	public PolicyStatementAttribute Attributes
	{
		get
		{
			return attrs;
		}
		set
		{
			switch (value)
			{
			case PolicyStatementAttribute.Nothing:
			case PolicyStatementAttribute.Exclusive:
			case PolicyStatementAttribute.LevelFinal:
			case PolicyStatementAttribute.All:
				attrs = value;
				break;
			default:
			{
				string text = Locale.GetText("Invalid value for {0}.");
				throw new ArgumentException(string.Format(text, "PolicyStatementAttribute"));
			}
			}
		}
	}

	public string AttributeString => attrs switch
	{
		PolicyStatementAttribute.Exclusive => "Exclusive", 
		PolicyStatementAttribute.LevelFinal => "LevelFinal", 
		PolicyStatementAttribute.All => "Exclusive LevelFinal", 
		_ => string.Empty, 
	};

	public PolicyStatement(PermissionSet permSet)
		: this(permSet, PolicyStatementAttribute.Nothing)
	{
	}

	public PolicyStatement(PermissionSet permSet, PolicyStatementAttribute attributes)
	{
		if (permSet != null)
		{
			perms = permSet.Copy();
			perms.SetReadOnly(value: true);
		}
		attrs = attributes;
	}

	public PolicyStatement Copy()
	{
		return new PolicyStatement(perms, attrs);
	}

	public void FromXml(SecurityElement et)
	{
		FromXml(et, null);
	}

	public void FromXml(SecurityElement et, PolicyLevel level)
	{
		if (et == null)
		{
			throw new ArgumentNullException("et");
		}
		if (et.Tag != "PolicyStatement")
		{
			throw new ArgumentException(Locale.GetText("Invalid tag."));
		}
		string text = et.Attribute("Attributes");
		if (text != null)
		{
			attrs = (PolicyStatementAttribute)(int)Enum.Parse(typeof(PolicyStatementAttribute), text);
		}
		SecurityElement et2 = et.SearchForChildByTag("PermissionSet");
		PermissionSet.FromXml(et2);
	}

	public SecurityElement ToXml()
	{
		return ToXml(null);
	}

	public SecurityElement ToXml(PolicyLevel level)
	{
		SecurityElement securityElement = new SecurityElement("PolicyStatement");
		securityElement.AddAttribute("version", "1");
		if (attrs != 0)
		{
			securityElement.AddAttribute("Attributes", attrs.ToString());
		}
		securityElement.AddChild(PermissionSet.ToXml());
		return securityElement;
	}

	[ComVisible(false)]
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is PolicyStatement policyStatement))
		{
			return false;
		}
		return PermissionSet.Equals(obj) && attrs == policyStatement.attrs;
	}

	[ComVisible(false)]
	public override int GetHashCode()
	{
		return PermissionSet.GetHashCode() ^ (int)attrs;
	}

	internal static PolicyStatement Empty()
	{
		return new PolicyStatement(new PermissionSet(PermissionState.None));
	}
}

namespace System.Runtime.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class CollectionDataContractAttribute : Attribute
{
	private string name;

	private string ns;

	private string item_name;

	private string key_name;

	private string value_name;

	private bool is_reference;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public string Namespace
	{
		get
		{
			return ns;
		}
		set
		{
			ns = value;
		}
	}

	public string ItemName
	{
		get
		{
			return item_name;
		}
		set
		{
			item_name = value;
		}
	}

	public string KeyName
	{
		get
		{
			return key_name;
		}
		set
		{
			key_name = value;
		}
	}

	public string ValueName
	{
		get
		{
			return value_name;
		}
		set
		{
			value_name = value;
		}
	}

	public bool IsReference { get; set; }
}

using System.Configuration;
using System.Net.Mail;

namespace System.Net.Configuration;

public sealed class SmtpSection : ConfigurationSection
{
	[ConfigurationProperty("deliveryMethod", DefaultValue = "Network")]
	public SmtpDeliveryMethod DeliveryMethod
	{
		get
		{
			return (SmtpDeliveryMethod)(int)base["deliveryMethod"];
		}
		set
		{
			base["deliveryMethod"] = value;
		}
	}

	[ConfigurationProperty("from")]
	public string From
	{
		get
		{
			return (string)base["from"];
		}
		set
		{
			base["from"] = value;
		}
	}

	[ConfigurationProperty("network")]
	public SmtpNetworkElement Network => (SmtpNetworkElement)base["network"];

	[ConfigurationProperty("specifiedPickupDirectory")]
	public SmtpSpecifiedPickupDirectoryElement SpecifiedPickupDirectory => (SmtpSpecifiedPickupDirectoryElement)base["specifiedPickupDirectory"];

	protected override ConfigurationPropertyCollection Properties => base.Properties;
}

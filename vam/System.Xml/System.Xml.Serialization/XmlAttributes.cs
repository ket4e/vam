using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace System.Xml.Serialization;

public class XmlAttributes
{
	private XmlAnyAttributeAttribute xmlAnyAttribute;

	private XmlAnyElementAttributes xmlAnyElements = new XmlAnyElementAttributes();

	private XmlArrayAttribute xmlArray;

	private XmlArrayItemAttributes xmlArrayItems = new XmlArrayItemAttributes();

	private XmlAttributeAttribute xmlAttribute;

	private XmlChoiceIdentifierAttribute xmlChoiceIdentifier;

	private object xmlDefaultValue = DBNull.Value;

	private XmlElementAttributes xmlElements = new XmlElementAttributes();

	private XmlEnumAttribute xmlEnum;

	private bool xmlIgnore;

	private bool xmlns;

	private XmlRootAttribute xmlRoot;

	private XmlTextAttribute xmlText;

	private XmlTypeAttribute xmlType;

	public XmlAnyAttributeAttribute XmlAnyAttribute
	{
		get
		{
			return xmlAnyAttribute;
		}
		set
		{
			xmlAnyAttribute = value;
		}
	}

	public XmlAnyElementAttributes XmlAnyElements => xmlAnyElements;

	public XmlArrayAttribute XmlArray
	{
		get
		{
			return xmlArray;
		}
		set
		{
			xmlArray = value;
		}
	}

	public XmlArrayItemAttributes XmlArrayItems => xmlArrayItems;

	public XmlAttributeAttribute XmlAttribute
	{
		get
		{
			return xmlAttribute;
		}
		set
		{
			xmlAttribute = value;
		}
	}

	public XmlChoiceIdentifierAttribute XmlChoiceIdentifier => xmlChoiceIdentifier;

	public object XmlDefaultValue
	{
		get
		{
			return xmlDefaultValue;
		}
		set
		{
			xmlDefaultValue = value;
		}
	}

	public XmlElementAttributes XmlElements => xmlElements;

	public XmlEnumAttribute XmlEnum
	{
		get
		{
			return xmlEnum;
		}
		set
		{
			xmlEnum = value;
		}
	}

	public bool XmlIgnore
	{
		get
		{
			return xmlIgnore;
		}
		set
		{
			xmlIgnore = value;
		}
	}

	public bool Xmlns
	{
		get
		{
			return xmlns;
		}
		set
		{
			xmlns = value;
		}
	}

	public XmlRootAttribute XmlRoot
	{
		get
		{
			return xmlRoot;
		}
		set
		{
			xmlRoot = value;
		}
	}

	public XmlTextAttribute XmlText
	{
		get
		{
			return xmlText;
		}
		set
		{
			xmlText = value;
		}
	}

	public XmlTypeAttribute XmlType
	{
		get
		{
			return xmlType;
		}
		set
		{
			xmlType = value;
		}
	}

	public XmlAttributes()
	{
	}

	public XmlAttributes(ICustomAttributeProvider provider)
	{
		object[] customAttributes = provider.GetCustomAttributes(inherit: false);
		object[] array = customAttributes;
		foreach (object obj in array)
		{
			if (obj is XmlAnyAttributeAttribute)
			{
				xmlAnyAttribute = (XmlAnyAttributeAttribute)obj;
			}
			else if (obj is XmlAnyElementAttribute)
			{
				xmlAnyElements.Add((XmlAnyElementAttribute)obj);
			}
			else if (obj is XmlArrayAttribute)
			{
				xmlArray = (XmlArrayAttribute)obj;
			}
			else if (obj is XmlArrayItemAttribute)
			{
				xmlArrayItems.Add((XmlArrayItemAttribute)obj);
			}
			else if (obj is XmlAttributeAttribute)
			{
				xmlAttribute = (XmlAttributeAttribute)obj;
			}
			else if (obj is XmlChoiceIdentifierAttribute)
			{
				xmlChoiceIdentifier = (XmlChoiceIdentifierAttribute)obj;
			}
			else if (obj is DefaultValueAttribute)
			{
				xmlDefaultValue = ((DefaultValueAttribute)obj).Value;
			}
			else if (obj is XmlElementAttribute)
			{
				xmlElements.Add((XmlElementAttribute)obj);
			}
			else if (obj is XmlEnumAttribute)
			{
				xmlEnum = (XmlEnumAttribute)obj;
			}
			else if (obj is XmlIgnoreAttribute)
			{
				xmlIgnore = true;
			}
			else if (obj is XmlNamespaceDeclarationsAttribute)
			{
				xmlns = true;
			}
			else if (obj is XmlRootAttribute)
			{
				xmlRoot = (XmlRootAttribute)obj;
			}
			else if (obj is XmlTextAttribute)
			{
				xmlText = (XmlTextAttribute)obj;
			}
			else if (obj is XmlTypeAttribute)
			{
				xmlType = (XmlTypeAttribute)obj;
			}
		}
	}

	internal void AddKeyHash(StringBuilder sb)
	{
		sb.Append("XA ");
		KeyHelper.AddField(sb, 1, xmlIgnore);
		KeyHelper.AddField(sb, 2, xmlns);
		KeyHelper.AddField(sb, 3, xmlAnyAttribute != null);
		xmlAnyElements.AddKeyHash(sb);
		xmlArrayItems.AddKeyHash(sb);
		xmlElements.AddKeyHash(sb);
		if (xmlArray != null)
		{
			xmlArray.AddKeyHash(sb);
		}
		if (xmlAttribute != null)
		{
			xmlAttribute.AddKeyHash(sb);
		}
		if (xmlDefaultValue == null)
		{
			sb.Append("n");
		}
		else if (!(xmlDefaultValue is DBNull))
		{
			string text = XmlCustomFormatter.ToXmlString(TypeTranslator.GetTypeData(xmlDefaultValue.GetType()), xmlDefaultValue);
			sb.Append("v" + text);
		}
		if (xmlEnum != null)
		{
			xmlEnum.AddKeyHash(sb);
		}
		if (xmlRoot != null)
		{
			xmlRoot.AddKeyHash(sb);
		}
		if (xmlText != null)
		{
			xmlText.AddKeyHash(sb);
		}
		if (xmlType != null)
		{
			xmlType.AddKeyHash(sb);
		}
		if (xmlChoiceIdentifier != null)
		{
			xmlChoiceIdentifier.AddKeyHash(sb);
		}
		sb.Append("|");
	}
}

using System.Configuration;
using System.Xml;

namespace System.Data.Common;

public class DbProviderFactoriesConfigurationHandler : IConfigurationSectionHandler
{
	public virtual object Create(object parent, object configContext, XmlNode section)
	{
		DataSet dataSet = (parent as DataSet) ?? CreateDataSet();
		FillDataTables(dataSet, section);
		return dataSet;
	}

	private DataSet CreateDataSet()
	{
		DataSet dataSet = new DataSet("system.data");
		DataTable dataTable = dataSet.Tables.Add("DbProviderFactories");
		DataColumn[] array = new DataColumn[4]
		{
			new DataColumn("Name", typeof(string)),
			new DataColumn("Description", typeof(string)),
			new DataColumn("InvariantName", typeof(string)),
			new DataColumn("AssemblyQualifiedName", typeof(string))
		};
		dataTable.Columns.AddRange(array);
		dataTable.PrimaryKey = new DataColumn[1] { array[2] };
		return dataSet;
	}

	private void FillDataTables(DataSet ds, XmlNode section)
	{
		DataTable dataTable = ds.Tables[0];
		foreach (XmlNode childNode in section.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element || !(childNode.Name == "DbProviderFactories"))
			{
				continue;
			}
			foreach (XmlNode childNode2 in childNode.ChildNodes)
			{
				if (childNode2.NodeType == XmlNodeType.Element)
				{
					switch (childNode2.Name)
					{
					case "add":
						AddRow(dataTable, childNode2);
						break;
					case "clear":
						dataTable.Rows.Clear();
						break;
					case "remove":
						RemoveRow(dataTable, childNode2);
						break;
					default:
						throw new ConfigurationErrorsException("Unrecognized element.", childNode2);
					}
				}
			}
		}
	}

	private string GetAttributeValue(XmlNode node, string name, bool required)
	{
		XmlAttribute xmlAttribute = node.Attributes[name];
		if (xmlAttribute == null)
		{
			if (!required)
			{
				return null;
			}
			throw new ConfigurationErrorsException("Required Attribute '" + name + "' is  missing!", node);
		}
		string value = xmlAttribute.Value;
		if (value == string.Empty)
		{
			throw new ConfigurationException("Attribute '" + name + "' cannot be empty!", node);
		}
		return value;
	}

	private void AddRow(DataTable dt, XmlNode addNode)
	{
		string attributeValue = GetAttributeValue(addNode, "name", required: true);
		string attributeValue2 = GetAttributeValue(addNode, "description", required: true);
		string attributeValue3 = GetAttributeValue(addNode, "invariant", required: true);
		string attributeValue4 = GetAttributeValue(addNode, "type", required: true);
		DataRow dataRow = dt.NewRow();
		dataRow[0] = attributeValue;
		dataRow[1] = attributeValue2;
		dataRow[2] = attributeValue3;
		dataRow[3] = attributeValue4;
		dt.Rows.Add(dataRow);
	}

	private void RemoveRow(DataTable dt, XmlNode removeNode)
	{
		string attributeValue = GetAttributeValue(removeNode, "invariant", required: true);
		dt.Rows.Find(attributeValue)?.Delete();
	}
}

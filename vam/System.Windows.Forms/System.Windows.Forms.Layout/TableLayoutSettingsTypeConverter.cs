using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;

namespace System.Windows.Forms.Layout;

public class TableLayoutSettingsTypeConverter : TypeConverter
{
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			return true;
		}
		return base.CanConvertTo(context, destinationType);
	}

	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}
		return base.CanConvertFrom(context, sourceType);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (!(value is TableLayoutSettings) || destinationType != typeof(string))
		{
			return base.ConvertTo(context, culture, value, destinationType);
		}
		TableLayoutSettings tableLayoutSettings = value as TableLayoutSettings;
		StringWriter stringWriter = new StringWriter();
		XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
		xmlTextWriter.WriteStartDocument();
		List<ControlInfo> controls = tableLayoutSettings.GetControls();
		xmlTextWriter.WriteStartElement("TableLayoutSettings");
		xmlTextWriter.WriteStartElement("Controls");
		foreach (ControlInfo item in controls)
		{
			xmlTextWriter.WriteStartElement("Control");
			xmlTextWriter.WriteAttributeString("Name", item.Control.ToString());
			xmlTextWriter.WriteAttributeString("Row", item.Row.ToString());
			xmlTextWriter.WriteAttributeString("RowSpan", item.RowSpan.ToString());
			xmlTextWriter.WriteAttributeString("Column", item.Col.ToString());
			xmlTextWriter.WriteAttributeString("ColumnSpan", item.ColSpan.ToString());
			xmlTextWriter.WriteEndElement();
		}
		xmlTextWriter.WriteEndElement();
		List<string> list = new List<string>();
		foreach (ColumnStyle item2 in (IEnumerable)tableLayoutSettings.ColumnStyles)
		{
			list.Add(item2.SizeType.ToString());
			list.Add(item2.Width.ToString(CultureInfo.InvariantCulture));
		}
		xmlTextWriter.WriteStartElement("Columns");
		xmlTextWriter.WriteAttributeString("Styles", string.Join(",", list.ToArray()));
		xmlTextWriter.WriteEndElement();
		list.Clear();
		foreach (RowStyle item3 in (IEnumerable)tableLayoutSettings.RowStyles)
		{
			list.Add(item3.SizeType.ToString());
			list.Add(item3.Height.ToString(CultureInfo.InvariantCulture));
		}
		xmlTextWriter.WriteStartElement("Rows");
		xmlTextWriter.WriteAttributeString("Styles", string.Join(",", list.ToArray()));
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndDocument();
		xmlTextWriter.Close();
		return stringWriter.ToString();
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (!(value is string))
		{
			return base.ConvertFrom(context, culture, value);
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(value as string);
		TableLayoutSettings tableLayoutSettings = new TableLayoutSettings(null);
		int rowCount = ParseControl(xmlDocument, tableLayoutSettings);
		ParseColumnStyle(xmlDocument, tableLayoutSettings);
		ParseRowStyle(xmlDocument, tableLayoutSettings);
		tableLayoutSettings.RowCount = rowCount;
		return tableLayoutSettings;
	}

	private int ParseControl(XmlDocument xmldoc, TableLayoutSettings settings)
	{
		int num = 0;
		foreach (XmlNode item in xmldoc.GetElementsByTagName("Control"))
		{
			if (item.Attributes["Name"] != null && !string.IsNullOrEmpty(item.Attributes["Name"].Value))
			{
				if (item.Attributes["Row"] != null)
				{
					settings.SetRow(item.Attributes["Name"].Value, GetValue(item.Attributes["Row"].Value));
					num++;
				}
				if (item.Attributes["RowSpan"] != null)
				{
					settings.SetRowSpan(item.Attributes["Name"].Value, GetValue(item.Attributes["RowSpan"].Value));
				}
				if (item.Attributes["Column"] != null)
				{
					settings.SetColumn(item.Attributes["Name"].Value, GetValue(item.Attributes["Column"].Value));
				}
				if (item.Attributes["ColumnSpan"] != null)
				{
					settings.SetColumnSpan(item.Attributes["Name"].Value, GetValue(item.Attributes["ColumnSpan"].Value));
				}
			}
		}
		return num;
	}

	private void ParseColumnStyle(XmlDocument xmldoc, TableLayoutSettings settings)
	{
		foreach (XmlNode item in xmldoc.GetElementsByTagName("Columns"))
		{
			if (item.Attributes["Styles"] == null)
			{
				continue;
			}
			string value = item.Attributes["Styles"].Value;
			if (!string.IsNullOrEmpty(value))
			{
				string[] array = BuggySplit(value);
				for (int i = 0; i < array.Length; i += 2)
				{
					float result = 0f;
					SizeType sizeType = (SizeType)(int)Enum.Parse(typeof(SizeType), array[i]);
					float.TryParse(array[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out result);
					settings.ColumnStyles.Add(new ColumnStyle(sizeType, result));
				}
			}
		}
	}

	private void ParseRowStyle(XmlDocument xmldoc, TableLayoutSettings settings)
	{
		foreach (XmlNode item in xmldoc.GetElementsByTagName("Rows"))
		{
			if (item.Attributes["Styles"] == null)
			{
				continue;
			}
			string value = item.Attributes["Styles"].Value;
			if (!string.IsNullOrEmpty(value))
			{
				string[] array = BuggySplit(value);
				for (int i = 0; i < array.Length; i += 2)
				{
					float result = 0f;
					SizeType sizeType = (SizeType)(int)Enum.Parse(typeof(SizeType), array[i]);
					float.TryParse(array[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out result);
					settings.RowStyles.Add(new RowStyle(sizeType, result));
				}
			}
		}
	}

	private int GetValue(string attValue)
	{
		int result = -1;
		if (!string.IsNullOrEmpty(attValue))
		{
			int.TryParse(attValue, out result);
		}
		return result;
	}

	private string[] BuggySplit(string s)
	{
		List<string> list = new List<string>();
		string[] array = s.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			switch (array[i].ToLowerInvariant())
			{
			case "autosize":
			case "absolute":
			case "percent":
				list.Add(array[i]);
				continue;
			}
			if (i + 1 < array.Length)
			{
				if (float.TryParse(array[i + 1], out var _))
				{
					list.Add($"{array[i]}.{array[i + 1]}");
					i++;
				}
				else
				{
					list.Add(array[i]);
				}
			}
			else
			{
				list.Add(array[i]);
			}
		}
		return list.ToArray();
	}
}

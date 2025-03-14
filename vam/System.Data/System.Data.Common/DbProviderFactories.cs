using System.Configuration;
using System.Reflection;
using System.Threading;

namespace System.Data.Common;

public static class DbProviderFactories
{
	internal const string CONFIG_SECTION_NAME = "system.data";

	internal const string CONFIG_SEC_TABLE_NAME = "DbProviderFactories";

	private static object configEntries;

	public static DbProviderFactory GetFactory(DataRow providerRow)
	{
		string typeName = (string)providerRow["AssemblyQualifiedName"];
		Type type = Type.GetType(typeName, throwOnError: false, ignoreCase: true);
		if (type != null && type.IsSubclassOf(typeof(DbProviderFactory)))
		{
			FieldInfo field = type.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
			if (field != null)
			{
				return field.GetValue(null) as DbProviderFactory;
			}
		}
		throw new ConfigurationErrorsException("Failed to find or load the registered .Net Framework Data Provider.");
	}

	public static DbProviderFactory GetFactory(string providerInvariantName)
	{
		DataTable factoryClasses = GetFactoryClasses();
		if (factoryClasses != null)
		{
			DataRow dataRow = factoryClasses.Rows.Find(providerInvariantName);
			if (dataRow != null)
			{
				return GetFactory(dataRow);
			}
		}
		throw new ConfigurationErrorsException($"Failed to find or load the registered .Net Framework Data Provider '{providerInvariantName}'.");
	}

	public static DataTable GetFactoryClasses()
	{
		DataTable dataTable = GetConfigEntries()?.Tables["DbProviderFactories"];
		if (dataTable != null)
		{
			dataTable = dataTable.Copy();
		}
		return dataTable;
	}

	internal static DataSet GetConfigEntries()
	{
		if (configEntries != null)
		{
			return configEntries as DataSet;
		}
		DataSet value = (DataSet)ConfigurationManager.GetSection("system.data");
		Interlocked.CompareExchange(ref configEntries, value, null);
		return configEntries as DataSet;
	}
}

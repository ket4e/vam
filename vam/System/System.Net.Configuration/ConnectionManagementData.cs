using System.Collections;

namespace System.Net.Configuration;

internal class ConnectionManagementData
{
	private const int defaultMaxConnections = 2;

	private Hashtable data;

	public Hashtable Data => data;

	public ConnectionManagementData(object parent)
	{
		data = new Hashtable(CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
		if (parent == null || !(parent is System.Net.Configuration.ConnectionManagementData))
		{
			return;
		}
		System.Net.Configuration.ConnectionManagementData connectionManagementData = (System.Net.Configuration.ConnectionManagementData)parent;
		foreach (string key in connectionManagementData.data.Keys)
		{
			data[key] = connectionManagementData.data[key];
		}
	}

	public void Add(string address, string nconns)
	{
		if (nconns == null || nconns == string.Empty)
		{
			nconns = "2";
		}
		data[address] = uint.Parse(nconns);
	}

	public void Add(string address, int nconns)
	{
		data[address] = (uint)nconns;
	}

	public void Remove(string address)
	{
		data.Remove(address);
	}

	public void Clear()
	{
		data.Clear();
	}

	public uint GetMaxConnections(string hostOrIP)
	{
		object obj = data[hostOrIP];
		if (obj == null)
		{
			obj = data["*"];
		}
		if (obj == null)
		{
			return 2u;
		}
		return (uint)obj;
	}
}

using System;
using System.Collections;

namespace Mono.Data.Tds.Protocol;

public class TdsConnectionPoolManager
{
	private Hashtable pools = Hashtable.Synchronized(new Hashtable());

	private TdsVersion version;

	public TdsConnectionPoolManager(TdsVersion version)
	{
		this.version = version;
	}

	public TdsConnectionPool GetConnectionPool(string connectionString, TdsConnectionInfo info)
	{
		TdsConnectionPool tdsConnectionPool = (TdsConnectionPool)pools[connectionString];
		if (tdsConnectionPool == null)
		{
			pools[connectionString] = new TdsConnectionPool(this, info);
			tdsConnectionPool = (TdsConnectionPool)pools[connectionString];
		}
		return tdsConnectionPool;
	}

	public TdsConnectionPool GetConnectionPool(string connectionString)
	{
		return (TdsConnectionPool)pools[connectionString];
	}

	public virtual Tds CreateConnection(TdsConnectionInfo info)
	{
		return version switch
		{
			TdsVersion.tds42 => new Tds42(info.DataSource, info.Port, info.PacketSize, info.Timeout), 
			TdsVersion.tds50 => new Tds50(info.DataSource, info.Port, info.PacketSize, info.Timeout), 
			TdsVersion.tds70 => new Tds70(info.DataSource, info.Port, info.PacketSize, info.Timeout), 
			TdsVersion.tds80 => new Tds80(info.DataSource, info.Port, info.PacketSize, info.Timeout), 
			_ => throw new NotSupportedException(), 
		};
	}

	public IDictionary GetConnectionPool()
	{
		return pools;
	}
}

using System.Collections;
using System.Configuration;
using System.Threading;

namespace System.Diagnostics;

internal sealed class DiagnosticsConfiguration
{
	private static object settings;

	public static IDictionary Settings
	{
		get
		{
			if (settings == null)
			{
				object config = ConfigurationSettings.GetConfig("system.diagnostics");
				if (config == null)
				{
					throw new Exception("INTERNAL configuration error: failed to get configuration 'system.diagnostics'");
				}
				Thread.MemoryBarrier();
				while (Interlocked.CompareExchange(ref settings, config, null) == null)
				{
				}
				Thread.MemoryBarrier();
			}
			return (IDictionary)settings;
		}
	}
}

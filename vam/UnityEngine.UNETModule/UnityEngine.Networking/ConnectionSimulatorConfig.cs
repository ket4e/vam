using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Networking;

/// <summary>
///   <para>Create configuration for network simulator; You can use this class in editor and developer build only.</para>
/// </summary>
public sealed class ConnectionSimulatorConfig : IDisposable
{
	internal IntPtr m_Ptr;

	/// <summary>
	///   <para>Will create object describing network simulation parameters.</para>
	/// </summary>
	/// <param name="outMinDelay">Minimal simulation delay for outgoing traffic in ms.</param>
	/// <param name="outAvgDelay">Average simulation delay for outgoing traffic in ms.</param>
	/// <param name="inMinDelay">Minimal  simulation delay for incoming traffic in ms.</param>
	/// <param name="inAvgDelay">Average  simulation delay for incoming traffic in ms.</param>
	/// <param name="packetLossPercentage">Probability of packet loss  0 &lt;= p &lt;= 1.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern ConnectionSimulatorConfig(int outMinDelay, int outAvgDelay, int inMinDelay, int inAvgDelay, float packetLossPercentage);

	/// <summary>
	///   <para>Destructor.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public extern void Dispose();

	~ConnectionSimulatorConfig()
	{
		Dispose();
	}
}

namespace UnityEngine;

/// <summary>
///   <para>Describes different levels of log information the network layer supports.</para>
/// </summary>
public enum NetworkLogLevel
{
	/// <summary>
	///   <para>Only report errors, otherwise silent.</para>
	/// </summary>
	Off = 0,
	/// <summary>
	///   <para>Report informational messages like connectivity events.</para>
	/// </summary>
	Informational = 1,
	/// <summary>
	///   <para>Full debug level logging down to each individual message being reported.</para>
	/// </summary>
	Full = 3
}

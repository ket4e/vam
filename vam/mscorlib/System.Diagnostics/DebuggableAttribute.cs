using System.Runtime.InteropServices;

namespace System.Diagnostics;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
[ComVisible(true)]
public sealed class DebuggableAttribute : Attribute
{
	[Flags]
	[ComVisible(true)]
	public enum DebuggingModes
	{
		None = 0,
		Default = 1,
		IgnoreSymbolStoreSequencePoints = 2,
		EnableEditAndContinue = 4,
		DisableOptimizations = 0x100
	}

	private bool JITTrackingEnabledFlag;

	private bool JITOptimizerDisabledFlag;

	private DebuggingModes debuggingModes;

	public DebuggingModes DebuggingFlags => debuggingModes;

	public bool IsJITTrackingEnabled => JITTrackingEnabledFlag;

	public bool IsJITOptimizerDisabled => JITOptimizerDisabledFlag;

	public DebuggableAttribute(bool isJITTrackingEnabled, bool isJITOptimizerDisabled)
	{
		JITTrackingEnabledFlag = isJITTrackingEnabled;
		JITOptimizerDisabledFlag = isJITOptimizerDisabled;
		if (isJITTrackingEnabled)
		{
			debuggingModes |= DebuggingModes.Default;
		}
		if (isJITOptimizerDisabled)
		{
			debuggingModes |= DebuggingModes.DisableOptimizations;
		}
	}

	public DebuggableAttribute(DebuggingModes modes)
	{
		debuggingModes = modes;
		JITTrackingEnabledFlag = (debuggingModes & DebuggingModes.Default) != 0;
		JITOptimizerDisabledFlag = (debuggingModes & DebuggingModes.DisableOptimizations) != 0;
	}
}

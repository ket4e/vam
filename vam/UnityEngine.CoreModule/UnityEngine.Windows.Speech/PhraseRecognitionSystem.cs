using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech;

/// <summary>
///   <para>Phrase recognition system is responsible for managing phrase recognizers and dispatching recognition events to them.</para>
/// </summary>
public static class PhraseRecognitionSystem
{
	/// <summary>
	///   <para>Delegate for OnError event.</para>
	/// </summary>
	/// <param name="errorCode">Error code for the error that occurred.</param>
	public delegate void ErrorDelegate(SpeechError errorCode);

	/// <summary>
	///   <para>Delegate for OnStatusChanged event.</para>
	/// </summary>
	/// <param name="status">The new status of the phrase recognition system.</param>
	public delegate void StatusDelegate(SpeechSystemStatus status);

	/// <summary>
	///   <para>Returns whether speech recognition is supported on the machine that the application is running on.</para>
	/// </summary>
	[ThreadAndSerializationSafe]
	public static extern bool isSupported
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Returns the current status of the phrase recognition system.</para>
	/// </summary>
	public static extern SpeechSystemStatus Status
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static event ErrorDelegate OnError;

	public static event StatusDelegate OnStatusChanged;

	/// <summary>
	///   <para>Attempts to restart the phrase recognition system.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void Restart();

	/// <summary>
	///   <para>Shuts phrase recognition system down.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void Shutdown();

	[RequiredByNativeCode]
	private static void PhraseRecognitionSystem_InvokeErrorEvent(SpeechError errorCode)
	{
		PhraseRecognitionSystem.OnError?.Invoke(errorCode);
	}

	[RequiredByNativeCode]
	private static void PhraseRecognitionSystem_InvokeStatusChangedEvent(SpeechSystemStatus status)
	{
		PhraseRecognitionSystem.OnStatusChanged?.Invoke(status);
	}
}

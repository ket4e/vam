using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech;

/// <summary>
///   <para>DictationRecognizer listens to speech input and attempts to determine what phrase was uttered.</para>
/// </summary>
public sealed class DictationRecognizer : IDisposable
{
	/// <summary>
	///   <para>Callback indicating a hypothesis change event. You should register with DictationHypothesis event.</para>
	/// </summary>
	/// <param name="text">The text that the recognizer believes may have been recognized.</param>
	public delegate void DictationHypothesisDelegate(string text);

	/// <summary>
	///   <para>Callback indicating a phrase has been recognized with the specified confidence level. You should register with DictationResult event.</para>
	/// </summary>
	/// <param name="text">The recognized text.</param>
	/// <param name="confidence">The confidence level at which the text was recognized.</param>
	public delegate void DictationResultDelegate(string text, ConfidenceLevel confidence);

	/// <summary>
	///   <para>Delegate for DictationComplete event.</para>
	/// </summary>
	/// <param name="cause">The cause of dictation session completion.</param>
	public delegate void DictationCompletedDelegate(DictationCompletionCause cause);

	/// <summary>
	///   <para>Delegate for DictationError event.</para>
	/// </summary>
	/// <param name="error">The error mesage.</param>
	/// <param name="hresult">HRESULT code that corresponds to the error.</param>
	public delegate void DictationErrorHandler(string error, int hresult);

	private IntPtr m_Recognizer;

	/// <summary>
	///   <para>Indicates the status of dictation recognizer.</para>
	/// </summary>
	public SpeechSystemStatus Status => (m_Recognizer != IntPtr.Zero) ? GetStatus(m_Recognizer) : SpeechSystemStatus.Stopped;

	/// <summary>
	///   <para>The time length in seconds before dictation recognizer session ends due to lack of audio input.</para>
	/// </summary>
	public float AutoSilenceTimeoutSeconds
	{
		get
		{
			if (m_Recognizer == IntPtr.Zero)
			{
				return 0f;
			}
			return GetAutoSilenceTimeoutSeconds(m_Recognizer);
		}
		set
		{
			if (!(m_Recognizer == IntPtr.Zero))
			{
				SetAutoSilenceTimeoutSeconds(m_Recognizer, value);
			}
		}
	}

	/// <summary>
	///   <para>The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.</para>
	/// </summary>
	public float InitialSilenceTimeoutSeconds
	{
		get
		{
			if (m_Recognizer == IntPtr.Zero)
			{
				return 0f;
			}
			return GetInitialSilenceTimeoutSeconds(m_Recognizer);
		}
		set
		{
			if (!(m_Recognizer == IntPtr.Zero))
			{
				SetInitialSilenceTimeoutSeconds(m_Recognizer, value);
			}
		}
	}

	public event DictationHypothesisDelegate DictationHypothesis;

	public event DictationResultDelegate DictationResult;

	public event DictationCompletedDelegate DictationComplete;

	public event DictationErrorHandler DictationError;

	/// <summary>
	///   <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
	/// </summary>
	/// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
	/// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
	/// <param name="confidenceLevel"></param>
	public DictationRecognizer()
		: this(ConfidenceLevel.Medium, DictationTopicConstraint.Dictation)
	{
	}

	/// <summary>
	///   <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
	/// </summary>
	/// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
	/// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
	/// <param name="confidenceLevel"></param>
	public DictationRecognizer(ConfidenceLevel confidenceLevel)
		: this(confidenceLevel, DictationTopicConstraint.Dictation)
	{
	}

	/// <summary>
	///   <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
	/// </summary>
	/// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
	/// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
	/// <param name="confidenceLevel"></param>
	public DictationRecognizer(DictationTopicConstraint topic)
		: this(ConfidenceLevel.Medium, topic)
	{
	}

	/// <summary>
	///   <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
	/// </summary>
	/// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
	/// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
	/// <param name="confidenceLevel"></param>
	public DictationRecognizer(ConfidenceLevel minimumConfidence, DictationTopicConstraint topic)
	{
		m_Recognizer = Create(minimumConfidence, topic);
	}

	private IntPtr Create(ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint)
	{
		INTERNAL_CALL_Create(this, minimumConfidence, topicConstraint, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Create(DictationRecognizer self, ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Start(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Stop(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Destroy(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private static extern void DestroyThreaded(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern SpeechSystemStatus GetStatus(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern float GetAutoSilenceTimeoutSeconds(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SetAutoSilenceTimeoutSeconds(IntPtr self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern float GetInitialSilenceTimeoutSeconds(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SetInitialSilenceTimeoutSeconds(IntPtr self, float value);

	~DictationRecognizer()
	{
		if (m_Recognizer != IntPtr.Zero)
		{
			DestroyThreaded(m_Recognizer);
			m_Recognizer = IntPtr.Zero;
			GC.SuppressFinalize(this);
		}
	}

	/// <summary>
	///   <para>Starts the dictation recognization session. Dictation recognizer can only be started if PhraseRecognitionSystem is not running.</para>
	/// </summary>
	public void Start()
	{
		if (!(m_Recognizer == IntPtr.Zero))
		{
			Start(m_Recognizer);
		}
	}

	/// <summary>
	///   <para>Stops the dictation recognization session.</para>
	/// </summary>
	public void Stop()
	{
		if (!(m_Recognizer == IntPtr.Zero))
		{
			Stop(m_Recognizer);
		}
	}

	/// <summary>
	///   <para>Disposes the resources this dictation recognizer uses.</para>
	/// </summary>
	public void Dispose()
	{
		if (m_Recognizer != IntPtr.Zero)
		{
			Destroy(m_Recognizer);
			m_Recognizer = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	[RequiredByNativeCode]
	private void DictationRecognizer_InvokeHypothesisGeneratedEvent(string keyword)
	{
		this.DictationHypothesis?.Invoke(keyword);
	}

	[RequiredByNativeCode]
	private void DictationRecognizer_InvokeResultGeneratedEvent(string keyword, ConfidenceLevel minimumConfidence)
	{
		this.DictationResult?.Invoke(keyword, minimumConfidence);
	}

	[RequiredByNativeCode]
	private void DictationRecognizer_InvokeCompletedEvent(DictationCompletionCause cause)
	{
		this.DictationComplete?.Invoke(cause);
	}

	[RequiredByNativeCode]
	private void DictationRecognizer_InvokeErrorEvent(string error, int hresult)
	{
		this.DictationError?.Invoke(error, hresult);
	}
}

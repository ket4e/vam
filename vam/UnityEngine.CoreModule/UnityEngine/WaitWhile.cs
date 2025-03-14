using System;

namespace UnityEngine;

/// <summary>
///   <para>Suspends the coroutine execution until the supplied delegate evaluates to false.</para>
/// </summary>
public sealed class WaitWhile : CustomYieldInstruction
{
	private Func<bool> m_Predicate;

	public override bool keepWaiting => m_Predicate();

	public WaitWhile(Func<bool> predicate)
	{
		m_Predicate = predicate;
	}
}

using System;
using UnityEngine;

namespace Leap.Unity;

[ExecuteInEditMode]
public abstract class HandModelBase : MonoBehaviour
{
	private bool isTracked;

	[NonSerialized]
	public HandModelManager.ModelGroup group;

	public bool IsTracked => isTracked;

	public abstract Chirality Handedness { get; set; }

	public abstract ModelType HandModelType { get; }

	public event Action OnBegin;

	public event Action OnFinish;

	public virtual void InitHand()
	{
	}

	public virtual void BeginHand()
	{
		if (this.OnBegin != null)
		{
			this.OnBegin();
		}
		isTracked = true;
	}

	public abstract void UpdateHand();

	public virtual void FinishHand()
	{
		if (this.OnFinish != null)
		{
			this.OnFinish();
		}
		isTracked = false;
	}

	public abstract Hand GetLeapHand();

	public abstract void SetLeapHand(Hand hand);

	public virtual bool SupportsEditorPersistence()
	{
		return false;
	}
}

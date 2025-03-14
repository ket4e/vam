using UnityEngine;

namespace GPUTools.Physics.Scripts.Behaviours;

[ExecuteInEditMode]
public class GpuCutCapsule : GpuEditCapsule
{
	private void OnEnable()
	{
		if (capsuleCollider == null)
		{
			capsuleCollider = GetComponent<CapsuleCollider>();
		}
		if (Application.isPlaying)
		{
			GPUCollidersManager.RegisterCutCapsule(this);
		}
		UpdateData();
	}

	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			GPUCollidersManager.DeregisterCutCapsule(this);
		}
	}
}

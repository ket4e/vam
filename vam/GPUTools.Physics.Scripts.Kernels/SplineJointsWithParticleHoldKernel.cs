using GPUTools.Common.Scripts.PL.Abstract;
using GPUTools.Common.Scripts.PL.Attributes;
using GPUTools.Common.Scripts.PL.Tools;
using GPUTools.Physics.Scripts.Types.Dynamic;
using UnityEngine;

namespace GPUTools.Physics.Scripts.Kernels;

public class SplineJointsWithParticleHoldKernel : KernelBase
{
	[GpuData("segments")]
	public GpuValue<int> Segments { get; set; }

	[GpuData("particles")]
	public GpuBuffer<GPParticle> Particles { get; set; }

	[GpuData("pointToPreviousPointDistances")]
	public GpuBuffer<float> PointToPreviousPointDistances { get; set; }

	[GpuData("splineJointPower")]
	public GpuValue<float> SplineJointPower { get; set; }

	public SplineJointsWithParticleHoldKernel()
		: base("Compute/SplineJointsWithParticleHold", "CSSplineJointsWithParticleHold")
	{
	}

	public override int GetGroupsNumX()
	{
		if (Particles != null)
		{
			return Mathf.CeilToInt((float)(Particles.Count / Segments.Value) / 256f);
		}
		return 0;
	}
}

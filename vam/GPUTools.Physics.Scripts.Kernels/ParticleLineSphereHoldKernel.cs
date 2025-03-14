using GPUTools.Common.Scripts.PL.Abstract;
using GPUTools.Common.Scripts.PL.Attributes;
using GPUTools.Common.Scripts.PL.Tools;
using GPUTools.Physics.Scripts.Types.Dynamic;
using GPUTools.Physics.Scripts.Types.Shapes;
using UnityEngine;

namespace GPUTools.Physics.Scripts.Kernels;

public class ParticleLineSphereHoldKernel : KernelBase
{
	[GpuData("segments")]
	public GpuValue<int> Segments { get; set; }

	[GpuData("particles")]
	public GpuBuffer<GPParticle> Particles { get; set; }

	[GpuData("holdLineSpheres")]
	public GpuBuffer<GPLineSphere> HoldLineSpheres { get; set; }

	public ParticleLineSphereHoldKernel()
		: base("Compute/ParticleLineSphereHold", "CSParticleLineSphereHold")
	{
	}

	public override int GetGroupsNumX()
	{
		if (Particles != null)
		{
			return Mathf.CeilToInt((float)Particles.Count / 256f);
		}
		return 0;
	}
}

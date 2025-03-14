using GPUTools.Common.Scripts.PL.Abstract;
using GPUTools.Common.Scripts.PL.Attributes;
using GPUTools.Common.Scripts.PL.Tools;
using GPUTools.Physics.Scripts.Types.Dynamic;
using GPUTools.Physics.Scripts.Types.Joints;
using GPUTools.Physics.Scripts.Types.Shapes;
using UnityEngine;

namespace GPUTools.Physics.Scripts.Kernels;

public class ParticleLineSphereRigidityIncreaseKernel : KernelBase
{
	[GpuData("particles")]
	public GpuBuffer<GPParticle> Particles { get; set; }

	[GpuData("pointJoints")]
	public GpuBuffer<GPPointJoint> PointJoints { get; set; }

	[GpuData("rigidityIncreaseLineSpheres")]
	public GpuBuffer<GPLineSphere> RigidityIncreaseLineSpheres { get; set; }

	public ParticleLineSphereRigidityIncreaseKernel()
		: base("Compute/ParticleLineSphereRigidityIncrease", "CSParticleLineSphereRigidityIncrease")
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

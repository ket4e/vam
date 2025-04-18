using GPUTools.Common.Scripts.PL.Abstract;
using GPUTools.Common.Scripts.PL.Attributes;
using GPUTools.Common.Scripts.PL.Tools;
using GPUTools.Physics.Scripts.Types.Dynamic;
using GPUTools.Physics.Scripts.Types.Shapes;
using UnityEngine;

namespace GPUTools.Physics.Scripts.Kernels;

public class GrabJointsKernel : KernelBase
{
	[GpuData("grabSpheres")]
	public GpuBuffer<GPGrabSphere> GrabSpheres { get; set; }

	[GpuData("particles")]
	public GpuBuffer<GPParticle> Particles { get; set; }

	public GrabJointsKernel()
		: base("Compute/GrabJoints", "CSGrabJoints")
	{
	}

	public override int GetGroupsNumX()
	{
		if (Particles != null)
		{
			return Mathf.CeilToInt((float)Particles.ComputeBuffer.count / 256f);
		}
		return 0;
	}
}

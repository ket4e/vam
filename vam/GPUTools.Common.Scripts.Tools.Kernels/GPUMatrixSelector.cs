using GPUTools.Common.Scripts.PL.Abstract;
using GPUTools.Common.Scripts.PL.Attributes;
using GPUTools.Common.Scripts.PL.Tools;
using UnityEngine;

namespace GPUTools.Common.Scripts.Tools.Kernels;

public class GPUMatrixSelector : KernelBase
{
	[GpuData("indices")]
	public GpuBuffer<int> Indices { get; set; }

	[GpuData("matrices")]
	public GpuBuffer<Matrix4x4> Matrices { get; set; }

	[GpuData("selectedMatrices")]
	public GpuBuffer<Matrix4x4> SelectedMatrices { get; set; }

	public GPUMatrixSelector()
		: base("Compute/Selector", "CSMatrixSelector")
	{
	}

	public override int GetGroupsNumX()
	{
		return Mathf.CeilToInt((float)Indices.Count / 256f);
	}
}

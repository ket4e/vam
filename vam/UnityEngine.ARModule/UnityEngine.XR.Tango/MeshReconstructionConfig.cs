using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Tango;

[NativeHeader("ARScriptingClasses.h")]
[UsedByNativeCode]
internal struct MeshReconstructionConfig
{
	public double resolution;

	public double minDepth;

	public double maxDepth;

	public int minNumVertices;

	public bool useParallelIntegration;

	public bool generateColor;

	public bool useSpaceClearing;

	public UpdateMethod updateMethod;

	public static MeshReconstructionConfig GetDefault()
	{
		MeshReconstructionConfig result = default(MeshReconstructionConfig);
		result.resolution = 0.03;
		result.minDepth = 0.6;
		result.maxDepth = 3.5;
		result.useParallelIntegration = false;
		result.generateColor = true;
		result.useSpaceClearing = false;
		result.minNumVertices = 1;
		result.updateMethod = UpdateMethod.Traversal;
		return result;
	}
}

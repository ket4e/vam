using System;

namespace ZenFulcrum.VR.OpenVRBinding;

public struct RenderModel_t
{
	public IntPtr rVertexData;

	public uint unVertexCount;

	public IntPtr rIndexData;

	public uint unTriangleCount;

	public int diffuseTextureId;
}

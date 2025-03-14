using System;

namespace UnityEngine.Rendering;

/// <summary>
///   <para>Identifies a RenderTexture for a Rendering.CommandBuffer.</para>
/// </summary>
public struct RenderTargetIdentifier
{
	private BuiltinRenderTextureType m_Type;

	private int m_NameID;

	private int m_InstanceID;

	private IntPtr m_BufferPointer;

	private int m_MipLevel;

	private CubemapFace m_CubeFace;

	private int m_DepthSlice;

	/// <summary>
	///   <para>Creates a render target identifier.</para>
	/// </summary>
	/// <param name="type">Built-in temporary render texture type.</param>
	/// <param name="name">Temporary render texture name.</param>
	/// <param name="nameID">Temporary render texture name (as integer, see Shader.PropertyToID).</param>
	/// <param name="tex">RenderTexture or Texture object to use.</param>
	public RenderTargetIdentifier(BuiltinRenderTextureType type)
	{
		m_Type = type;
		m_NameID = -1;
		m_InstanceID = 0;
		m_BufferPointer = IntPtr.Zero;
		m_MipLevel = 0;
		m_CubeFace = CubemapFace.Unknown;
		m_DepthSlice = 0;
	}

	/// <summary>
	///   <para>Creates a render target identifier.</para>
	/// </summary>
	/// <param name="type">Built-in temporary render texture type.</param>
	/// <param name="name">Temporary render texture name.</param>
	/// <param name="nameID">Temporary render texture name (as integer, see Shader.PropertyToID).</param>
	/// <param name="tex">RenderTexture or Texture object to use.</param>
	public RenderTargetIdentifier(string name)
	{
		m_Type = BuiltinRenderTextureType.PropertyName;
		m_NameID = Shader.PropertyToID(name);
		m_InstanceID = 0;
		m_BufferPointer = IntPtr.Zero;
		m_MipLevel = 0;
		m_CubeFace = CubemapFace.Unknown;
		m_DepthSlice = 0;
	}

	public RenderTargetIdentifier(string name, int mipLevel = 0, CubemapFace cubeFace = CubemapFace.Unknown, int depthSlice = 0)
	{
		m_Type = BuiltinRenderTextureType.PropertyName;
		m_NameID = Shader.PropertyToID(name);
		m_InstanceID = 0;
		m_BufferPointer = IntPtr.Zero;
		m_MipLevel = mipLevel;
		m_CubeFace = cubeFace;
		m_DepthSlice = depthSlice;
	}

	/// <summary>
	///   <para>Creates a render target identifier.</para>
	/// </summary>
	/// <param name="type">Built-in temporary render texture type.</param>
	/// <param name="name">Temporary render texture name.</param>
	/// <param name="nameID">Temporary render texture name (as integer, see Shader.PropertyToID).</param>
	/// <param name="tex">RenderTexture or Texture object to use.</param>
	public RenderTargetIdentifier(int nameID)
	{
		m_Type = BuiltinRenderTextureType.PropertyName;
		m_NameID = nameID;
		m_InstanceID = 0;
		m_BufferPointer = IntPtr.Zero;
		m_MipLevel = 0;
		m_CubeFace = CubemapFace.Unknown;
		m_DepthSlice = 0;
	}

	public RenderTargetIdentifier(int nameID, int mipLevel = 0, CubemapFace cubeFace = CubemapFace.Unknown, int depthSlice = 0)
	{
		m_Type = BuiltinRenderTextureType.PropertyName;
		m_NameID = nameID;
		m_InstanceID = 0;
		m_BufferPointer = IntPtr.Zero;
		m_MipLevel = mipLevel;
		m_CubeFace = cubeFace;
		m_DepthSlice = depthSlice;
	}

	/// <summary>
	///   <para>Creates a render target identifier.</para>
	/// </summary>
	/// <param name="type">Built-in temporary render texture type.</param>
	/// <param name="name">Temporary render texture name.</param>
	/// <param name="nameID">Temporary render texture name (as integer, see Shader.PropertyToID).</param>
	/// <param name="tex">RenderTexture or Texture object to use.</param>
	public RenderTargetIdentifier(Texture tex)
	{
		if (tex == null)
		{
			m_Type = BuiltinRenderTextureType.None;
		}
		else if (tex is RenderTexture)
		{
			m_Type = BuiltinRenderTextureType.RenderTexture;
		}
		else
		{
			m_Type = BuiltinRenderTextureType.BindableTexture;
		}
		m_BufferPointer = IntPtr.Zero;
		m_NameID = -1;
		m_InstanceID = (tex ? tex.GetInstanceID() : 0);
		m_MipLevel = 0;
		m_CubeFace = CubemapFace.Unknown;
		m_DepthSlice = 0;
	}

	public RenderTargetIdentifier(Texture tex, int mipLevel = 0, CubemapFace cubeFace = CubemapFace.Unknown, int depthSlice = 0)
	{
		if (tex == null)
		{
			m_Type = BuiltinRenderTextureType.None;
		}
		else if (tex is RenderTexture)
		{
			m_Type = BuiltinRenderTextureType.RenderTexture;
		}
		else
		{
			m_Type = BuiltinRenderTextureType.BindableTexture;
		}
		m_BufferPointer = IntPtr.Zero;
		m_NameID = -1;
		m_InstanceID = (tex ? tex.GetInstanceID() : 0);
		m_MipLevel = mipLevel;
		m_CubeFace = cubeFace;
		m_DepthSlice = depthSlice;
	}

	public RenderTargetIdentifier(RenderBuffer buf, int mipLevel = 0, CubemapFace cubeFace = CubemapFace.Unknown, int depthSlice = 0)
	{
		m_Type = BuiltinRenderTextureType.BufferPtr;
		m_NameID = -1;
		m_InstanceID = buf.m_RenderTextureInstanceID;
		m_BufferPointer = buf.m_BufferPtr;
		m_MipLevel = mipLevel;
		m_CubeFace = cubeFace;
		m_DepthSlice = depthSlice;
	}

	public static implicit operator RenderTargetIdentifier(BuiltinRenderTextureType type)
	{
		return new RenderTargetIdentifier(type);
	}

	public static implicit operator RenderTargetIdentifier(string name)
	{
		return new RenderTargetIdentifier(name);
	}

	public static implicit operator RenderTargetIdentifier(int nameID)
	{
		return new RenderTargetIdentifier(nameID);
	}

	public static implicit operator RenderTargetIdentifier(Texture tex)
	{
		return new RenderTargetIdentifier(tex);
	}

	public static implicit operator RenderTargetIdentifier(RenderBuffer buf)
	{
		return new RenderTargetIdentifier(buf);
	}

	public override string ToString()
	{
		return UnityString.Format("Type {0} NameID {1} InstanceID {2}", m_Type, m_NameID, m_InstanceID);
	}

	public override int GetHashCode()
	{
		return (m_Type.GetHashCode() * 23 + m_NameID.GetHashCode()) * 23 + m_InstanceID.GetHashCode();
	}

	public bool Equals(RenderTargetIdentifier rhs)
	{
		return m_Type == rhs.m_Type && m_NameID == rhs.m_NameID && m_InstanceID == rhs.m_InstanceID && m_BufferPointer == rhs.m_BufferPointer && m_MipLevel == rhs.m_MipLevel && m_CubeFace == rhs.m_CubeFace && m_DepthSlice == rhs.m_DepthSlice;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is RenderTargetIdentifier rhs))
		{
			return false;
		}
		return Equals(rhs);
	}

	public static bool operator ==(RenderTargetIdentifier lhs, RenderTargetIdentifier rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(RenderTargetIdentifier lhs, RenderTargetIdentifier rhs)
	{
		return !lhs.Equals(rhs);
	}
}

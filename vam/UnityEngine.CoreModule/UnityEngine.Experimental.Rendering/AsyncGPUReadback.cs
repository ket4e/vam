using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Allows the asynchronous read back of GPU resources.</para>
/// </summary>
[StaticAccessor("AsyncGPUReadbackManager::GetInstance()", StaticAccessorType.Dot)]
public static class AsyncGPUReadback
{
	/// <summary>
	///   <para>Triggers a request to asynchronously fetch the data from a GPU resource.</para>
	/// </summary>
	/// <param name="src">The resource to read the data from.</param>
	/// <param name="size">Size in bytes of the data to be retrieved from the ComputeBuffer.</param>
	/// <param name="offset">Offset in bytes in the ComputeBuffer.</param>
	/// <param name="mipIndex">The index of the mipmap to be fetched.</param>
	/// <param name="dstFormat">The target TextureFormat of the data. Conversion will happen automatically if format is different from the format stored on GPU.</param>
	/// <param name="x">Starting X coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="width">Width in pixels of the Texture data to be fetched.</param>
	/// <param name="y">Starting Y coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="height">Height in pixels of the Texture data to be fetched.</param>
	/// <param name="z">Start Z coordinate in pixels for the Texture3D being fetched. Index of Start layer for TextureCube, Texture2DArray and TextureCubeArray being fetched.</param>
	/// <param name="depth">Depth in pixels for Texture3D being fetched. Number of layers for TextureCube, TextureArray and TextureCubeArray.</param>
	/// <returns>
	///   <para>An AsyncGPUReadbackRequest that can be used to both access the data and check whether it is available.</para>
	/// </returns>
	public static AsyncGPUReadbackRequest Request(ComputeBuffer src)
	{
		if (src == null)
		{
			throw new ArgumentNullException();
		}
		return Request_Internal_ComputeBuffer_1(src);
	}

	/// <summary>
	///   <para>Triggers a request to asynchronously fetch the data from a GPU resource.</para>
	/// </summary>
	/// <param name="src">The resource to read the data from.</param>
	/// <param name="size">Size in bytes of the data to be retrieved from the ComputeBuffer.</param>
	/// <param name="offset">Offset in bytes in the ComputeBuffer.</param>
	/// <param name="mipIndex">The index of the mipmap to be fetched.</param>
	/// <param name="dstFormat">The target TextureFormat of the data. Conversion will happen automatically if format is different from the format stored on GPU.</param>
	/// <param name="x">Starting X coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="width">Width in pixels of the Texture data to be fetched.</param>
	/// <param name="y">Starting Y coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="height">Height in pixels of the Texture data to be fetched.</param>
	/// <param name="z">Start Z coordinate in pixels for the Texture3D being fetched. Index of Start layer for TextureCube, Texture2DArray and TextureCubeArray being fetched.</param>
	/// <param name="depth">Depth in pixels for Texture3D being fetched. Number of layers for TextureCube, TextureArray and TextureCubeArray.</param>
	/// <returns>
	///   <para>An AsyncGPUReadbackRequest that can be used to both access the data and check whether it is available.</para>
	/// </returns>
	public static AsyncGPUReadbackRequest Request(ComputeBuffer src, int size, int offset)
	{
		if (src == null)
		{
			throw new ArgumentNullException();
		}
		return Request_Internal_ComputeBuffer_2(src, size, offset);
	}

	/// <summary>
	///   <para>Triggers a request to asynchronously fetch the data from a GPU resource.</para>
	/// </summary>
	/// <param name="src">The resource to read the data from.</param>
	/// <param name="size">Size in bytes of the data to be retrieved from the ComputeBuffer.</param>
	/// <param name="offset">Offset in bytes in the ComputeBuffer.</param>
	/// <param name="mipIndex">The index of the mipmap to be fetched.</param>
	/// <param name="dstFormat">The target TextureFormat of the data. Conversion will happen automatically if format is different from the format stored on GPU.</param>
	/// <param name="x">Starting X coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="width">Width in pixels of the Texture data to be fetched.</param>
	/// <param name="y">Starting Y coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="height">Height in pixels of the Texture data to be fetched.</param>
	/// <param name="z">Start Z coordinate in pixels for the Texture3D being fetched. Index of Start layer for TextureCube, Texture2DArray and TextureCubeArray being fetched.</param>
	/// <param name="depth">Depth in pixels for Texture3D being fetched. Number of layers for TextureCube, TextureArray and TextureCubeArray.</param>
	/// <returns>
	///   <para>An AsyncGPUReadbackRequest that can be used to both access the data and check whether it is available.</para>
	/// </returns>
	public static AsyncGPUReadbackRequest Request(Texture src, int mipIndex = 0)
	{
		if (src == null)
		{
			throw new ArgumentNullException();
		}
		return Request_Internal_Texture_1(src, mipIndex);
	}

	/// <summary>
	///   <para>Triggers a request to asynchronously fetch the data from a GPU resource.</para>
	/// </summary>
	/// <param name="src">The resource to read the data from.</param>
	/// <param name="size">Size in bytes of the data to be retrieved from the ComputeBuffer.</param>
	/// <param name="offset">Offset in bytes in the ComputeBuffer.</param>
	/// <param name="mipIndex">The index of the mipmap to be fetched.</param>
	/// <param name="dstFormat">The target TextureFormat of the data. Conversion will happen automatically if format is different from the format stored on GPU.</param>
	/// <param name="x">Starting X coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="width">Width in pixels of the Texture data to be fetched.</param>
	/// <param name="y">Starting Y coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="height">Height in pixels of the Texture data to be fetched.</param>
	/// <param name="z">Start Z coordinate in pixels for the Texture3D being fetched. Index of Start layer for TextureCube, Texture2DArray and TextureCubeArray being fetched.</param>
	/// <param name="depth">Depth in pixels for Texture3D being fetched. Number of layers for TextureCube, TextureArray and TextureCubeArray.</param>
	/// <returns>
	///   <para>An AsyncGPUReadbackRequest that can be used to both access the data and check whether it is available.</para>
	/// </returns>
	public static AsyncGPUReadbackRequest Request(Texture src, int mipIndex, TextureFormat dstFormat)
	{
		if (src == null)
		{
			throw new ArgumentNullException();
		}
		return Request_Internal_Texture_2(src, mipIndex, dstFormat);
	}

	/// <summary>
	///   <para>Triggers a request to asynchronously fetch the data from a GPU resource.</para>
	/// </summary>
	/// <param name="src">The resource to read the data from.</param>
	/// <param name="size">Size in bytes of the data to be retrieved from the ComputeBuffer.</param>
	/// <param name="offset">Offset in bytes in the ComputeBuffer.</param>
	/// <param name="mipIndex">The index of the mipmap to be fetched.</param>
	/// <param name="dstFormat">The target TextureFormat of the data. Conversion will happen automatically if format is different from the format stored on GPU.</param>
	/// <param name="x">Starting X coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="width">Width in pixels of the Texture data to be fetched.</param>
	/// <param name="y">Starting Y coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="height">Height in pixels of the Texture data to be fetched.</param>
	/// <param name="z">Start Z coordinate in pixels for the Texture3D being fetched. Index of Start layer for TextureCube, Texture2DArray and TextureCubeArray being fetched.</param>
	/// <param name="depth">Depth in pixels for Texture3D being fetched. Number of layers for TextureCube, TextureArray and TextureCubeArray.</param>
	/// <returns>
	///   <para>An AsyncGPUReadbackRequest that can be used to both access the data and check whether it is available.</para>
	/// </returns>
	public static AsyncGPUReadbackRequest Request(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth)
	{
		if (src == null)
		{
			throw new ArgumentNullException();
		}
		return Request_Internal_Texture_3(src, mipIndex, x, width, y, height, z, depth);
	}

	/// <summary>
	///   <para>Triggers a request to asynchronously fetch the data from a GPU resource.</para>
	/// </summary>
	/// <param name="src">The resource to read the data from.</param>
	/// <param name="size">Size in bytes of the data to be retrieved from the ComputeBuffer.</param>
	/// <param name="offset">Offset in bytes in the ComputeBuffer.</param>
	/// <param name="mipIndex">The index of the mipmap to be fetched.</param>
	/// <param name="dstFormat">The target TextureFormat of the data. Conversion will happen automatically if format is different from the format stored on GPU.</param>
	/// <param name="x">Starting X coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="width">Width in pixels of the Texture data to be fetched.</param>
	/// <param name="y">Starting Y coordinate in pixels of the Texture data to be fetched.</param>
	/// <param name="height">Height in pixels of the Texture data to be fetched.</param>
	/// <param name="z">Start Z coordinate in pixels for the Texture3D being fetched. Index of Start layer for TextureCube, Texture2DArray and TextureCubeArray being fetched.</param>
	/// <param name="depth">Depth in pixels for Texture3D being fetched. Number of layers for TextureCube, TextureArray and TextureCubeArray.</param>
	/// <returns>
	///   <para>An AsyncGPUReadbackRequest that can be used to both access the data and check whether it is available.</para>
	/// </returns>
	public static AsyncGPUReadbackRequest Request(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat)
	{
		if (src == null)
		{
			throw new ArgumentNullException();
		}
		return Request_Internal_Texture_4(src, mipIndex, x, width, y, height, z, depth, dstFormat);
	}

	[NativeMethod("Request")]
	private static AsyncGPUReadbackRequest Request_Internal_ComputeBuffer_1(ComputeBuffer buffer)
	{
		Request_Internal_ComputeBuffer_1_Injected(buffer, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private static AsyncGPUReadbackRequest Request_Internal_ComputeBuffer_2(ComputeBuffer src, int size, int offset)
	{
		Request_Internal_ComputeBuffer_2_Injected(src, size, offset, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private static AsyncGPUReadbackRequest Request_Internal_Texture_1(Texture src, int mipIndex)
	{
		Request_Internal_Texture_1_Injected(src, mipIndex, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private static AsyncGPUReadbackRequest Request_Internal_Texture_2(Texture src, int mipIndex, TextureFormat dstFormat)
	{
		Request_Internal_Texture_2_Injected(src, mipIndex, dstFormat, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private static AsyncGPUReadbackRequest Request_Internal_Texture_3(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth)
	{
		Request_Internal_Texture_3_Injected(src, mipIndex, x, width, y, height, z, depth, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private static AsyncGPUReadbackRequest Request_Internal_Texture_4(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat)
	{
		Request_Internal_Texture_4_Injected(src, mipIndex, x, width, y, height, z, depth, dstFormat, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Request_Internal_ComputeBuffer_1_Injected(ComputeBuffer buffer, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Request_Internal_ComputeBuffer_2_Injected(ComputeBuffer src, int size, int offset, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Request_Internal_Texture_1_Injected(Texture src, int mipIndex, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Request_Internal_Texture_2_Injected(Texture src, int mipIndex, TextureFormat dstFormat, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Request_Internal_Texture_3_Injected(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Request_Internal_Texture_4_Injected(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, out AsyncGPUReadbackRequest ret);
}

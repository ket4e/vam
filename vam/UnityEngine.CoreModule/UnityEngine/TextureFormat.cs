namespace UnityEngine;

/// <summary>
///   <para>Format used when creating textures from scripts.</para>
/// </summary>
public enum TextureFormat
{
	/// <summary>
	///   <para>Alpha-only texture format.</para>
	/// </summary>
	Alpha8 = 1,
	/// <summary>
	///   <para>A 16 bits/pixel texture format. Texture stores color with an alpha channel.</para>
	/// </summary>
	ARGB4444 = 2,
	/// <summary>
	///   <para>Color texture format, 8-bits per channel.</para>
	/// </summary>
	RGB24 = 3,
	/// <summary>
	///   <para>Color with alpha texture format, 8-bits per channel.</para>
	/// </summary>
	RGBA32 = 4,
	/// <summary>
	///   <para>Color with alpha texture format, 8-bits per channel.</para>
	/// </summary>
	ARGB32 = 5,
	/// <summary>
	///   <para>A 16 bit color texture format.</para>
	/// </summary>
	RGB565 = 7,
	/// <summary>
	///   <para>A 16 bit color texture format that only has a red channel.</para>
	/// </summary>
	R16 = 9,
	/// <summary>
	///   <para>Compressed color texture format.</para>
	/// </summary>
	DXT1 = 10,
	/// <summary>
	///   <para>Compressed color with alpha channel texture format.</para>
	/// </summary>
	DXT5 = 12,
	/// <summary>
	///   <para>Color and alpha  texture format, 4 bit per channel.</para>
	/// </summary>
	RGBA4444 = 13,
	/// <summary>
	///   <para>Color with alpha texture format, 8-bits per channel.</para>
	/// </summary>
	BGRA32 = 14,
	/// <summary>
	///   <para>Scalar (R)  texture format, 16 bit floating point.</para>
	/// </summary>
	RHalf = 15,
	/// <summary>
	///   <para>Two color (RG)  texture format, 16 bit floating point per channel.</para>
	/// </summary>
	RGHalf = 16,
	/// <summary>
	///   <para>RGB color and alpha texture format, 16 bit floating point per channel.</para>
	/// </summary>
	RGBAHalf = 17,
	/// <summary>
	///   <para>Scalar (R) texture format, 32 bit floating point.</para>
	/// </summary>
	RFloat = 18,
	/// <summary>
	///   <para>Two color (RG)  texture format, 32 bit floating point per channel.</para>
	/// </summary>
	RGFloat = 19,
	/// <summary>
	///   <para>RGB color and alpha texture format,  32-bit floats per channel.</para>
	/// </summary>
	RGBAFloat = 20,
	/// <summary>
	///   <para>A format that uses the YUV color space and is often used for video encoding or playback.</para>
	/// </summary>
	YUY2 = 21,
	/// <summary>
	///   <para>RGB HDR format, with 9 bit mantissa per channel and a 5 bit shared exponent.</para>
	/// </summary>
	RGB9e5Float = 22,
	/// <summary>
	///   <para>Compressed one channel (R) texture format.</para>
	/// </summary>
	BC4 = 26,
	/// <summary>
	///   <para>Compressed two-channel (RG) texture format.</para>
	/// </summary>
	BC5 = 27,
	/// <summary>
	///   <para>HDR compressed color texture format.</para>
	/// </summary>
	BC6H = 24,
	/// <summary>
	///   <para>High quality compressed color texture format.</para>
	/// </summary>
	BC7 = 25,
	/// <summary>
	///   <para>Compressed color texture format with Crunch compression for smaller storage sizes.</para>
	/// </summary>
	DXT1Crunched = 28,
	/// <summary>
	///   <para>Compressed color with alpha channel texture format with Crunch compression for smaller storage sizes.</para>
	/// </summary>
	DXT5Crunched = 29,
	/// <summary>
	///   <para>PowerVR (iOS) 2 bits/pixel compressed color texture format.</para>
	/// </summary>
	PVRTC_RGB2 = 30,
	/// <summary>
	///   <para>PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format.</para>
	/// </summary>
	PVRTC_RGBA2 = 31,
	/// <summary>
	///   <para>PowerVR (iOS) 4 bits/pixel compressed color texture format.</para>
	/// </summary>
	PVRTC_RGB4 = 32,
	/// <summary>
	///   <para>PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format.</para>
	/// </summary>
	PVRTC_RGBA4 = 33,
	/// <summary>
	///   <para>ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.</para>
	/// </summary>
	ETC_RGB4 = 34,
	/// <summary>
	///   <para>ETC2  EAC (GL ES 3.0) 4 bitspixel compressed unsigned single-channel texture format.</para>
	/// </summary>
	EAC_R = 41,
	/// <summary>
	///   <para>ETC2  EAC (GL ES 3.0) 4 bitspixel compressed signed single-channel texture format.</para>
	/// </summary>
	EAC_R_SIGNED = 42,
	/// <summary>
	///   <para>ETC2  EAC (GL ES 3.0) 8 bitspixel compressed unsigned dual-channel (RG) texture format.</para>
	/// </summary>
	EAC_RG = 43,
	/// <summary>
	///   <para>ETC2  EAC (GL ES 3.0) 8 bitspixel compressed signed dual-channel (RG) texture format.</para>
	/// </summary>
	EAC_RG_SIGNED = 44,
	/// <summary>
	///   <para>ETC2 (GL ES 3.0) 4 bits/pixel compressed RGB texture format.</para>
	/// </summary>
	ETC2_RGB = 45,
	/// <summary>
	///   <para>ETC2 (GL ES 3.0) 4 bits/pixel RGB+1-bit alpha texture format.</para>
	/// </summary>
	ETC2_RGBA1 = 46,
	/// <summary>
	///   <para>ETC2 (GL ES 3.0) 8 bits/pixel compressed RGBA texture format.</para>
	/// </summary>
	ETC2_RGBA8 = 47,
	/// <summary>
	///   <para>ASTC (4x4 pixel block in 128 bits) compressed RGB texture format.</para>
	/// </summary>
	ASTC_RGB_4x4 = 48,
	/// <summary>
	///   <para>ASTC (5x5 pixel block in 128 bits) compressed RGB texture format.</para>
	/// </summary>
	ASTC_RGB_5x5 = 49,
	/// <summary>
	///   <para>ASTC (6x6 pixel block in 128 bits) compressed RGB texture format.</para>
	/// </summary>
	ASTC_RGB_6x6 = 50,
	/// <summary>
	///   <para>ASTC (8x8 pixel block in 128 bits) compressed RGB texture format.</para>
	/// </summary>
	ASTC_RGB_8x8 = 51,
	/// <summary>
	///   <para>ASTC (10x10 pixel block in 128 bits) compressed RGB texture format.</para>
	/// </summary>
	ASTC_RGB_10x10 = 52,
	/// <summary>
	///   <para>ASTC (12x12 pixel block in 128 bits) compressed RGB texture format.</para>
	/// </summary>
	ASTC_RGB_12x12 = 53,
	/// <summary>
	///   <para>ASTC (4x4 pixel block in 128 bits) compressed RGBA texture format.</para>
	/// </summary>
	ASTC_RGBA_4x4 = 54,
	/// <summary>
	///   <para>ASTC (5x5 pixel block in 128 bits) compressed RGBA texture format.</para>
	/// </summary>
	ASTC_RGBA_5x5 = 55,
	/// <summary>
	///   <para>ASTC (6x6 pixel block in 128 bits) compressed RGBA texture format.</para>
	/// </summary>
	ASTC_RGBA_6x6 = 56,
	/// <summary>
	///   <para>ASTC (8x8 pixel block in 128 bits) compressed RGBA texture format.</para>
	/// </summary>
	ASTC_RGBA_8x8 = 57,
	/// <summary>
	///   <para>ASTC (10x10 pixel block in 128 bits) compressed RGBA texture format.</para>
	/// </summary>
	ASTC_RGBA_10x10 = 58,
	/// <summary>
	///   <para>ASTC (12x12 pixel block in 128 bits) compressed RGBA texture format.</para>
	/// </summary>
	ASTC_RGBA_12x12 = 59,
	/// <summary>
	///   <para>ETC 4 bits/pixel compressed RGB texture format.</para>
	/// </summary>
	ETC_RGB4_3DS = 60,
	/// <summary>
	///   <para>ETC 4 bitspixel RGB + 4 bitspixel Alpha compressed texture format.</para>
	/// </summary>
	ETC_RGBA8_3DS = 61,
	/// <summary>
	///   <para>Two color (RG) texture format, 8-bits per channel.</para>
	/// </summary>
	RG16 = 62,
	/// <summary>
	///   <para>Scalar (R) render texture format, 8 bit fixed point.</para>
	/// </summary>
	R8 = 63,
	/// <summary>
	///   <para>Compressed color texture format with Crunch compression for smaller storage sizes.</para>
	/// </summary>
	ETC_RGB4Crunched = 64,
	/// <summary>
	///   <para>Compressed color with alpha channel texture format with Crunch compression for smaller storage sizes.</para>
	/// </summary>
	ETC2_RGBA8Crunched = 65
}

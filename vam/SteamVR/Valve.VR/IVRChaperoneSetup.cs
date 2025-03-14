using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public struct IVRChaperoneSetup
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _CommitWorkingCopy(EChaperoneConfigFile configFile);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _RevertWorkingCopy();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _GetWorkingPlayAreaSize(ref float pSizeX, ref float pSizeZ);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _GetWorkingPlayAreaRect(ref HmdQuad_t rect);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _GetWorkingCollisionBoundsInfo([In][Out] HmdQuad_t[] pQuadsBuffer, ref uint punQuadsCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _GetLiveCollisionBoundsInfo([In][Out] HmdQuad_t[] pQuadsBuffer, ref uint punQuadsCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _GetWorkingSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatSeatedZeroPoseToRawTrackingPose);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _GetWorkingStandingZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatStandingZeroPoseToRawTrackingPose);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetWorkingPlayAreaSize(float sizeX, float sizeZ);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetWorkingCollisionBoundsInfo([In][Out] HmdQuad_t[] pQuadsBuffer, uint unQuadsCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetWorkingPerimeter([In][Out] HmdVector2_t[] pPointBuffer, uint unPointCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetWorkingSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pMatSeatedZeroPoseToRawTrackingPose);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetWorkingStandingZeroPoseToRawTrackingPose(ref HmdMatrix34_t pMatStandingZeroPoseToRawTrackingPose);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _ReloadFromDisk(EChaperoneConfigFile configFile);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _GetLiveSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatSeatedZeroPoseToRawTrackingPose);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _ExportLiveToBuffer(StringBuilder pBuffer, ref uint pnBufferLength);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _ImportFromBufferToWorking(string pBuffer, uint nImportFlags);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _ShowWorkingSetPreview();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _HideWorkingSetPreview();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _RoomSetupStarting();

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _CommitWorkingCopy CommitWorkingCopy;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _RevertWorkingCopy RevertWorkingCopy;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetWorkingPlayAreaSize GetWorkingPlayAreaSize;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetWorkingPlayAreaRect GetWorkingPlayAreaRect;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetWorkingCollisionBoundsInfo GetWorkingCollisionBoundsInfo;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetLiveCollisionBoundsInfo GetLiveCollisionBoundsInfo;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetWorkingSeatedZeroPoseToRawTrackingPose GetWorkingSeatedZeroPoseToRawTrackingPose;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetWorkingStandingZeroPoseToRawTrackingPose GetWorkingStandingZeroPoseToRawTrackingPose;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetWorkingPlayAreaSize SetWorkingPlayAreaSize;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetWorkingCollisionBoundsInfo SetWorkingCollisionBoundsInfo;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetWorkingPerimeter SetWorkingPerimeter;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetWorkingSeatedZeroPoseToRawTrackingPose SetWorkingSeatedZeroPoseToRawTrackingPose;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetWorkingStandingZeroPoseToRawTrackingPose SetWorkingStandingZeroPoseToRawTrackingPose;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ReloadFromDisk ReloadFromDisk;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetLiveSeatedZeroPoseToRawTrackingPose GetLiveSeatedZeroPoseToRawTrackingPose;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ExportLiveToBuffer ExportLiveToBuffer;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ImportFromBufferToWorking ImportFromBufferToWorking;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ShowWorkingSetPreview ShowWorkingSetPreview;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _HideWorkingSetPreview HideWorkingSetPreview;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _RoomSetupStarting RoomSetupStarting;
}

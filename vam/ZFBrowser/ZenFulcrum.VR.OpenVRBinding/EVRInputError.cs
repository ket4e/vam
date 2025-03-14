namespace ZenFulcrum.VR.OpenVRBinding;

public enum EVRInputError
{
	None,
	NameNotFound,
	WrongType,
	InvalidHandle,
	InvalidParam,
	NoSteam,
	MaxCapacityReached,
	IPCError,
	NoActiveActionSet,
	InvalidDevice,
	InvalidSkeleton,
	InvalidBoneCount,
	InvalidCompressedData,
	NoData,
	BufferTooSmall,
	MismatchedActionManifest,
	MissingSkeletonData,
	InvalidBoneIndex
}

namespace UnityEngine;

/// <summary>
///   <para>Human Body Bones.</para>
/// </summary>
public enum HumanBodyBones
{
	/// <summary>
	///   <para>This is the Hips bone.</para>
	/// </summary>
	Hips = 0,
	/// <summary>
	///   <para>This is the Left Upper Leg bone.</para>
	/// </summary>
	LeftUpperLeg = 1,
	/// <summary>
	///   <para>This is the Right Upper Leg bone.</para>
	/// </summary>
	RightUpperLeg = 2,
	/// <summary>
	///   <para>This is the Left Knee bone.</para>
	/// </summary>
	LeftLowerLeg = 3,
	/// <summary>
	///   <para>This is the Right Knee bone.</para>
	/// </summary>
	RightLowerLeg = 4,
	/// <summary>
	///   <para>This is the Left Ankle bone.</para>
	/// </summary>
	LeftFoot = 5,
	/// <summary>
	///   <para>This is the Right Ankle bone.</para>
	/// </summary>
	RightFoot = 6,
	/// <summary>
	///   <para>This is the first Spine bone.</para>
	/// </summary>
	Spine = 7,
	/// <summary>
	///   <para>This is the Chest bone.</para>
	/// </summary>
	Chest = 8,
	/// <summary>
	///   <para>This is the Upper Chest bone.</para>
	/// </summary>
	UpperChest = 54,
	/// <summary>
	///   <para>This is the Neck bone.</para>
	/// </summary>
	Neck = 9,
	/// <summary>
	///   <para>This is the Head bone.</para>
	/// </summary>
	Head = 10,
	/// <summary>
	///   <para>This is the Left Shoulder bone.</para>
	/// </summary>
	LeftShoulder = 11,
	/// <summary>
	///   <para>This is the Right Shoulder bone.</para>
	/// </summary>
	RightShoulder = 12,
	/// <summary>
	///   <para>This is the Left Upper Arm bone.</para>
	/// </summary>
	LeftUpperArm = 13,
	/// <summary>
	///   <para>This is the Right Upper Arm bone.</para>
	/// </summary>
	RightUpperArm = 14,
	/// <summary>
	///   <para>This is the Left Elbow bone.</para>
	/// </summary>
	LeftLowerArm = 15,
	/// <summary>
	///   <para>This is the Right Elbow bone.</para>
	/// </summary>
	RightLowerArm = 16,
	/// <summary>
	///   <para>This is the Left Wrist bone.</para>
	/// </summary>
	LeftHand = 17,
	/// <summary>
	///   <para>This is the Right Wrist bone.</para>
	/// </summary>
	RightHand = 18,
	/// <summary>
	///   <para>This is the Left Toes bone.</para>
	/// </summary>
	LeftToes = 19,
	/// <summary>
	///   <para>This is the Right Toes bone.</para>
	/// </summary>
	RightToes = 20,
	/// <summary>
	///   <para>This is the Left Eye bone.</para>
	/// </summary>
	LeftEye = 21,
	/// <summary>
	///   <para>This is the Right Eye bone.</para>
	/// </summary>
	RightEye = 22,
	/// <summary>
	///   <para>This is the Jaw bone.</para>
	/// </summary>
	Jaw = 23,
	/// <summary>
	///   <para>This is the left thumb 1st phalange.</para>
	/// </summary>
	LeftThumbProximal = 24,
	/// <summary>
	///   <para>This is the left thumb 2nd phalange.</para>
	/// </summary>
	LeftThumbIntermediate = 25,
	/// <summary>
	///   <para>This is the left thumb 3rd phalange.</para>
	/// </summary>
	LeftThumbDistal = 26,
	/// <summary>
	///   <para>This is the left index 1st phalange.</para>
	/// </summary>
	LeftIndexProximal = 27,
	/// <summary>
	///   <para>This is the left index 2nd phalange.</para>
	/// </summary>
	LeftIndexIntermediate = 28,
	/// <summary>
	///   <para>This is the left index 3rd phalange.</para>
	/// </summary>
	LeftIndexDistal = 29,
	/// <summary>
	///   <para>This is the left middle 1st phalange.</para>
	/// </summary>
	LeftMiddleProximal = 30,
	/// <summary>
	///   <para>This is the left middle 2nd phalange.</para>
	/// </summary>
	LeftMiddleIntermediate = 31,
	/// <summary>
	///   <para>This is the left middle 3rd phalange.</para>
	/// </summary>
	LeftMiddleDistal = 32,
	/// <summary>
	///   <para>This is the left ring 1st phalange.</para>
	/// </summary>
	LeftRingProximal = 33,
	/// <summary>
	///   <para>This is the left ring 2nd phalange.</para>
	/// </summary>
	LeftRingIntermediate = 34,
	/// <summary>
	///   <para>This is the left ring 3rd phalange.</para>
	/// </summary>
	LeftRingDistal = 35,
	/// <summary>
	///   <para>This is the left little 1st phalange.</para>
	/// </summary>
	LeftLittleProximal = 36,
	/// <summary>
	///   <para>This is the left little 2nd phalange.</para>
	/// </summary>
	LeftLittleIntermediate = 37,
	/// <summary>
	///   <para>This is the left little 3rd phalange.</para>
	/// </summary>
	LeftLittleDistal = 38,
	/// <summary>
	///   <para>This is the right thumb 1st phalange.</para>
	/// </summary>
	RightThumbProximal = 39,
	/// <summary>
	///   <para>This is the right thumb 2nd phalange.</para>
	/// </summary>
	RightThumbIntermediate = 40,
	/// <summary>
	///   <para>This is the right thumb 3rd phalange.</para>
	/// </summary>
	RightThumbDistal = 41,
	/// <summary>
	///   <para>This is the right index 1st phalange.</para>
	/// </summary>
	RightIndexProximal = 42,
	/// <summary>
	///   <para>This is the right index 2nd phalange.</para>
	/// </summary>
	RightIndexIntermediate = 43,
	/// <summary>
	///   <para>This is the right index 3rd phalange.</para>
	/// </summary>
	RightIndexDistal = 44,
	/// <summary>
	///   <para>This is the right middle 1st phalange.</para>
	/// </summary>
	RightMiddleProximal = 45,
	/// <summary>
	///   <para>This is the right middle 2nd phalange.</para>
	/// </summary>
	RightMiddleIntermediate = 46,
	/// <summary>
	///   <para>This is the right middle 3rd phalange.</para>
	/// </summary>
	RightMiddleDistal = 47,
	/// <summary>
	///   <para>This is the right ring 1st phalange.</para>
	/// </summary>
	RightRingProximal = 48,
	/// <summary>
	///   <para>This is the right ring 2nd phalange.</para>
	/// </summary>
	RightRingIntermediate = 49,
	/// <summary>
	///   <para>This is the right ring 3rd phalange.</para>
	/// </summary>
	RightRingDistal = 50,
	/// <summary>
	///   <para>This is the right little 1st phalange.</para>
	/// </summary>
	RightLittleProximal = 51,
	/// <summary>
	///   <para>This is the right little 2nd phalange.</para>
	/// </summary>
	RightLittleIntermediate = 52,
	/// <summary>
	///   <para>This is the right little 3rd phalange.</para>
	/// </summary>
	RightLittleDistal = 53,
	/// <summary>
	///   <para>This is the Last bone index delimiter.</para>
	/// </summary>
	LastBone = 55
}

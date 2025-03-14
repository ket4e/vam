namespace UnityEngine;

/// <summary>
///   <para>Use ForceMode to specify how to apply a force using Rigidbody.AddForce.</para>
/// </summary>
public enum ForceMode
{
	/// <summary>
	///   <para>Add a continuous force to the rigidbody, using its mass.</para>
	/// </summary>
	Force = 0,
	/// <summary>
	///   <para>Add a continuous acceleration to the rigidbody, ignoring its mass.</para>
	/// </summary>
	Acceleration = 5,
	/// <summary>
	///   <para>Add an instant force impulse to the rigidbody, using its mass.</para>
	/// </summary>
	Impulse = 1,
	/// <summary>
	///   <para>Add an instant velocity change to the rigidbody, ignoring its mass.</para>
	/// </summary>
	VelocityChange = 2
}

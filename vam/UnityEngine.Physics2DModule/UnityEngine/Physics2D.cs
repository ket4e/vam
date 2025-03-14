using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Global settings and helpers for 2D physics.</para>
/// </summary>
[NativeHeader("Physics2DScriptingClasses.h")]
[NativeHeader("Physics2DScriptingClasses.h")]
[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
[NativeHeader("Modules/Physics2D/PhysicsManager2D.h")]
public class Physics2D
{
	/// <summary>
	///   <para>Layer mask constant for the default layer that ignores raycasts.</para>
	/// </summary>
	public const int IgnoreRaycastLayer = 4;

	/// <summary>
	///   <para>Layer mask constant that includes all layers participating in raycasts by default.</para>
	/// </summary>
	public const int DefaultRaycastLayers = -5;

	/// <summary>
	///   <para>Layer mask constant that includes all layers.</para>
	/// </summary>
	public const int AllLayers = -1;

	private static List<Rigidbody2D> m_LastDisabledRigidbody2D = new List<Rigidbody2D>();

	/// <summary>
	///   <para>The number of iterations of the physics solver when considering objects' velocities.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern int velocityIterations
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The number of iterations of the physics solver when considering objects' positions.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern int positionIterations
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Acceleration due to gravity.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static Vector2 gravity
	{
		get
		{
			get_gravity_Injected(out var ret);
			return ret;
		}
		set
		{
			set_gravity_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Do raycasts detect Colliders configured as triggers?</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool queriesHitTriggers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Sets the raycasts or linecasts that start inside Colliders to detect or not detect those Colliders.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool queriesStartInColliders
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Use this to control whether or not the appropriate OnCollisionExit2D or OnTriggerExit2D callbacks should be called when a Collider2D is disabled.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool callbacksOnDisable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Whether or not to automatically sync transform changes with the physics system whenever a Transform component changes.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool autoSyncTransforms
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Sets whether the physics should be simulated automatically or not.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool autoSimulation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>A set of options that control how physics operates when using the job system to multithread the physics simulation.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static PhysicsJobOptions2D jobOptions
	{
		get
		{
			get_jobOptions_Injected(out var ret);
			return ret;
		}
		set
		{
			set_jobOptions_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Any collisions with a relative linear velocity below this threshold will be treated as inelastic.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float velocityThreshold
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The maximum linear position correction used when solving constraints.  This helps to prevent overshoot.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float maxLinearCorrection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The maximum angular position correction used when solving constraints.  This helps to prevent overshoot.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float maxAngularCorrection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The maximum linear speed of a rigid-body per physics update.  Increasing this can cause numerical problems.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float maxTranslationSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The maximum angular speed of a rigid-body per physics update.  Increasing this can cause numerical problems.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float maxRotationSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The default contact offset of the newly created colliders.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float defaultContactOffset
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The scale factor that controls how fast overlaps are resolved.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float baumgarteScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The scale factor that controls how fast TOI overlaps are resolved.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float baumgarteTOIScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The time in seconds that a rigid-body must be still before it will go to sleep.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float timeToSleep
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>A rigid-body cannot sleep if its linear velocity is above this tolerance.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float linearSleepTolerance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>A rigid-body cannot sleep if its angular velocity is above this tolerance.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float angularSleepTolerance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Should the collider gizmos always be shown even when they are not selected?</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool alwaysShowColliders
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Should the collider gizmos show the sleep-state for each collider?</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool showColliderSleep
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Should the collider gizmos show current contacts for each collider?</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool showColliderContacts
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Should the collider gizmos show the AABBs for each collider?</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool showColliderAABB
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The scale of the contact arrow used by the collider gizmos.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float contactArrowScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The color used by the gizmos to show all awake colliders (collider is awake when the body is awake).</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static Color colliderAwakeColor
	{
		get
		{
			get_colliderAwakeColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_colliderAwakeColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The color used by the gizmos to show all asleep colliders (collider is asleep when the body is asleep).</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static Color colliderAsleepColor
	{
		get
		{
			get_colliderAsleepColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_colliderAsleepColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The color used by the gizmos to show all collider contacts.</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static Color colliderContactColor
	{
		get
		{
			get_colliderContactColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_colliderContactColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Sets the color used by the gizmos to show all Collider axis-aligned bounding boxes (AABBs).</para>
	/// </summary>
	[StaticAccessor("GetPhysics2DSettings()")]
	public static Color colliderAABBColor
	{
		get
		{
			get_colliderAABBColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_colliderAABBColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Simulate physics in the scene.</para>
	/// </summary>
	/// <param name="step">The time to advance physics by.</param>
	/// <returns>
	///   <para>Whether the simulation was run or not.  Running the simulation during physics callbacks will always fail.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("Simulate_Binding")]
	public static extern bool Simulate(float step);

	/// <summary>
	///   <para>Synchronizes.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void SyncTransforms();

	[ExcludeFromDocs]
	public static void IgnoreCollision([Writable] Collider2D collider1, [Writable] Collider2D collider2)
	{
		IgnoreCollision(collider1, collider2, ignore: true);
	}

	/// <summary>
	///   <para>Makes the collision detection system ignore all collisionstriggers between collider1 and collider2/.</para>
	/// </summary>
	/// <param name="collider1">The first collider to compare to collider2.</param>
	/// <param name="collider2">The second collider to compare to collider1.</param>
	/// <param name="ignore">Whether collisionstriggers between collider1 and collider2/ should be ignored or not.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void IgnoreCollision([Writable][NotNull] Collider2D collider1, [Writable][NotNull] Collider2D collider2, [DefaultValue("true")] bool ignore);

	/// <summary>
	///   <para>Checks whether the collision detection system will ignore all collisionstriggers between collider1 and collider2/ or not.</para>
	/// </summary>
	/// <param name="collider1">The first collider to compare to collider2.</param>
	/// <param name="collider2">The second collider to compare to collider1.</param>
	/// <returns>
	///   <para>Whether the collision detection system will ignore all collisionstriggers between collider1 and collider2/ or not.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool GetIgnoreCollision([Writable] Collider2D collider1, [Writable] Collider2D collider2);

	[ExcludeFromDocs]
	public static void IgnoreLayerCollision(int layer1, int layer2)
	{
		IgnoreLayerCollision(layer1, layer2, ignore: true);
	}

	/// <summary>
	///   <para>Choose whether to detect or ignore collisions between a specified pair of layers.</para>
	/// </summary>
	/// <param name="layer1">ID of the first layer.</param>
	/// <param name="layer2">ID of the second layer.</param>
	/// <param name="ignore">Should collisions between these layers be ignored?</param>
	public static void IgnoreLayerCollision(int layer1, int layer2, bool ignore)
	{
		if (layer1 < 0 || layer1 > 31)
		{
			throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		if (layer2 < 0 || layer2 > 31)
		{
			throw new ArgumentOutOfRangeException("layer2 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		IgnoreLayerCollision_Internal(layer1, layer2, ignore);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysics2DSettings()")]
	[NativeMethod("IgnoreLayerCollision")]
	private static extern void IgnoreLayerCollision_Internal(int layer1, int layer2, bool ignore);

	/// <summary>
	///   <para>Checks whether collisions between the specified layers be ignored or not.</para>
	/// </summary>
	/// <param name="layer1">ID of first layer.</param>
	/// <param name="layer2">ID of second layer.</param>
	/// <returns>
	///   <para>Whether collisions between the specified layers be ignored or not.</para>
	/// </returns>
	public static bool GetIgnoreLayerCollision(int layer1, int layer2)
	{
		if (layer1 < 0 || layer1 > 31)
		{
			throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		if (layer2 < 0 || layer2 > 31)
		{
			throw new ArgumentOutOfRangeException("layer2 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		return GetIgnoreLayerCollision_Internal(layer1, layer2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysics2DSettings()")]
	[NativeMethod("GetIgnoreLayerCollision")]
	private static extern bool GetIgnoreLayerCollision_Internal(int layer1, int layer2);

	/// <summary>
	///   <para>Set the collision layer mask that indicates which layer(s) the specified layer can collide with.</para>
	/// </summary>
	/// <param name="layer">The layer to set the collision layer mask for.</param>
	/// <param name="layerMask">A mask where each bit indicates a layer and whether it can collide with layer or not.</param>
	public static void SetLayerCollisionMask(int layer, int layerMask)
	{
		if (layer < 0 || layer > 31)
		{
			throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		SetLayerCollisionMask_Internal(layer, layerMask);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysics2DSettings()")]
	[NativeMethod("SetLayerCollisionMask")]
	private static extern void SetLayerCollisionMask_Internal(int layer, int layerMask);

	/// <summary>
	///   <para>Get the collision layer mask that indicates which layer(s) the specified layer can collide with.</para>
	/// </summary>
	/// <param name="layer">The layer to retrieve the collision layer mask for.</param>
	/// <returns>
	///   <para>A mask where each bit indicates a layer and whether it can collide with layer or not.</para>
	/// </returns>
	public static int GetLayerCollisionMask(int layer)
	{
		if (layer < 0 || layer > 31)
		{
			throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		return GetLayerCollisionMask_Internal(layer);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysics2DSettings()")]
	[NativeMethod("GetLayerCollisionMask")]
	private static extern int GetLayerCollisionMask_Internal(int layer);

	/// <summary>
	///   <para>Checks whether the passed colliders are in contact or not.</para>
	/// </summary>
	/// <param name="collider1">The collider to check if it is touching collider2.</param>
	/// <param name="collider2">The collider to check if it is touching collider1.</param>
	/// <returns>
	///   <para>Whether collider1 is touching collider2 or not.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool IsTouching([NotNull][Writable] Collider2D collider1, [NotNull][Writable] Collider2D collider2);

	/// <summary>
	///   <para>Checks whether the passed colliders are in contact or not.</para>
	/// </summary>
	/// <param name="collider1">The collider to check if it is touching collider2.</param>
	/// <param name="collider2">The collider to check if it is touching collider1.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <returns>
	///   <para>Whether collider1 is touching collider2 or not.</para>
	/// </returns>
	public static bool IsTouching([Writable] Collider2D collider1, [Writable] Collider2D collider2, ContactFilter2D contactFilter)
	{
		return IsTouching_TwoCollidersWithFilter(collider1, collider2, contactFilter);
	}

	[NativeMethod("IsTouching")]
	private static bool IsTouching_TwoCollidersWithFilter([Writable][NotNull] Collider2D collider1, [Writable][NotNull] Collider2D collider2, ContactFilter2D contactFilter)
	{
		return IsTouching_TwoCollidersWithFilter_Injected(collider1, collider2, ref contactFilter);
	}

	/// <summary>
	///   <para>Checks whether the passed colliders are in contact or not.</para>
	/// </summary>
	/// <param name="collider">The collider to check if it is touching any other collider filtered by the contactFilter.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <returns>
	///   <para>Whether the collider is touching any other collider filtered by the contactFilter or not.</para>
	/// </returns>
	public static bool IsTouching([Writable] Collider2D collider, ContactFilter2D contactFilter)
	{
		return IsTouching_SingleColliderWithFilter(collider, contactFilter);
	}

	[NativeMethod("IsTouching")]
	private static bool IsTouching_SingleColliderWithFilter([NotNull][Writable] Collider2D collider, ContactFilter2D contactFilter)
	{
		return IsTouching_SingleColliderWithFilter_Injected(collider, ref contactFilter);
	}

	[ExcludeFromDocs]
	public static bool IsTouchingLayers([Writable] Collider2D collider)
	{
		return IsTouchingLayers(collider, -1);
	}

	/// <summary>
	///   <para>Checks whether the collider is touching any colliders on the specified layerMask or not.</para>
	/// </summary>
	/// <param name="collider">The collider to check if it is touching colliders on the layerMask.</param>
	/// <param name="layerMask">Any colliders on any of these layers count as touching.</param>
	/// <returns>
	///   <para>Whether the collider is touching any colliders on the specified layerMask or not.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool IsTouchingLayers([NotNull][Writable] Collider2D collider, [DefaultValue("Physics2D.AllLayers")] int layerMask);

	/// <summary>
	///   <para>Calculates the minimum distance between two colliders.</para>
	/// </summary>
	/// <param name="colliderA">A collider used to calculate the minimum distance against colliderB.</param>
	/// <param name="colliderB">A collider used to calculate the minimum distance against colliderA.</param>
	/// <returns>
	///   <para>The minimum distance between colliderA and colliderB.</para>
	/// </returns>
	public static ColliderDistance2D Distance([Writable] Collider2D colliderA, [Writable] Collider2D colliderB)
	{
		if (colliderA == null)
		{
			throw new ArgumentNullException("ColliderA cannot be NULL.");
		}
		if (colliderB == null)
		{
			throw new ArgumentNullException("ColliderB cannot be NULL.");
		}
		if (colliderA == colliderB)
		{
			throw new ArgumentException("Cannot calculate the distance between the same collider.");
		}
		return Distance_Internal(colliderA, colliderB);
	}

	[NativeMethod("Distance")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static ColliderDistance2D Distance_Internal([NotNull][Writable] Collider2D colliderA, [NotNull][Writable] Collider2D colliderB)
	{
		Distance_Internal_Injected(colliderA, colliderB, out var ret);
		return ret;
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Linecast(Vector2 start, Vector2 end)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return Linecast_Internal(start, end, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return Linecast_Internal(start, end, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return Linecast_Internal(start, end, contactFilter);
	}

	/// <summary>
	///   <para>Casts a line segment against colliders in the Scene.</para>
	/// </summary>
	/// <param name="start">The start point of the line in world space.</param>
	/// <param name="end">The end point of the line in world space.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D Linecast(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return Linecast_Internal(start, end, contactFilter);
	}

	/// <summary>
	///   <para>Casts a line segment against colliders in the Scene with results filtered by ContactFilter2D.</para>
	/// </summary>
	/// <param name="start">The start point of the line in world space.</param>
	/// <param name="end">The end point of the line in world space.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return LinecastNonAlloc_Internal(start, end, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return LinecastAll_Internal(start, end, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return LinecastAll_Internal(start, end, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return LinecastAll_Internal(start, end, contactFilter);
	}

	/// <summary>
	///   <para>Casts a line against colliders in the scene.</para>
	/// </summary>
	/// <param name="start">The start point of the line in world space.</param>
	/// <param name="end">The end point of the line in world space.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return LinecastAll_Internal(start, end, contactFilter);
	}

	[ExcludeFromDocs]
	public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return LinecastNonAlloc_Internal(start, end, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return LinecastNonAlloc_Internal(start, end, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return LinecastNonAlloc_Internal(start, end, contactFilter, results);
	}

	/// <summary>
	///   <para>Casts a line against colliders in the scene.</para>
	/// </summary>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <param name="start">The start point of the line in world space.</param>
	/// <param name="end">The end point of the line in world space.</param>
	/// <param name="results">Returned array of objects that intersect the line.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return LinecastNonAlloc_Internal(start, end, contactFilter, results);
	}

	[NativeMethod("Linecast_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static RaycastHit2D Linecast_Internal(Vector2 start, Vector2 end, ContactFilter2D contactFilter)
	{
		Linecast_Internal_Injected(ref start, ref end, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("LinecastAll_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static RaycastHit2D[] LinecastAll_Internal(Vector2 start, Vector2 end, ContactFilter2D contactFilter)
	{
		return LinecastAll_Internal_Injected(ref start, ref end, ref contactFilter);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("LinecastNonAlloc_Binding")]
	private static int LinecastNonAlloc_Internal(Vector2 start, Vector2 end, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
	{
		return LinecastNonAlloc_Internal_Injected(ref start, ref end, ref contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return Raycast_Internal(origin, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return Raycast_Internal(origin, direction, distance, contactFilter);
	}

	[RequiredByNativeCode]
	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return Raycast_Internal(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return Raycast_Internal(origin, direction, distance, contactFilter);
	}

	/// <summary>
	///   <para>Casts a ray against colliders in the scene.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the ray originates.</param>
	/// <param name="direction">The vector representing the direction of the ray.</param>
	/// <param name="distance">Maximum distance over which to cast the ray.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return Raycast_Internal(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return RaycastNonAlloc_Internal(origin, direction, float.PositiveInfinity, contactFilter, results);
	}

	/// <summary>
	///   <para>Casts a ray against colliders in the Scene.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the ray originates.</param>
	/// <param name="direction">The vector representing the direction of the ray.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <param name="distance">Maximum distance over which to cast the ray.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
	{
		return RaycastNonAlloc_Internal(origin, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastAll_Internal(origin, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastAll_Internal(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastAll_Internal(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return RaycastAll_Internal(origin, direction, distance, contactFilter);
	}

	/// <summary>
	///   <para>Casts a ray against colliders in the scene, returning all colliders that contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the ray originates.</param>
	/// <param name="direction">The vector representing the direction of the ray.</param>
	/// <param name="distance">Maximum distance over which to cast the ray.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return RaycastAll_Internal(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastNonAlloc_Internal(origin, direction, float.PositiveInfinity, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastNonAlloc_Internal(origin, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastNonAlloc_Internal(origin, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return RaycastNonAlloc_Internal(origin, direction, distance, contactFilter, results);
	}

	/// <summary>
	///   <para>Casts a ray into the scene.</para>
	/// </summary>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <param name="origin">The point in 2D space where the ray originates.</param>
	/// <param name="direction">The vector representing the direction of the ray.</param>
	/// <param name="results">Array to receive results.</param>
	/// <param name="distance">Maximum distance over which to cast the ray.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return RaycastNonAlloc_Internal(origin, direction, distance, contactFilter, results);
	}

	[NativeMethod("Raycast_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static RaycastHit2D Raycast_Internal(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		Raycast_Internal_Injected(ref origin, ref direction, distance, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("RaycastAll_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static RaycastHit2D[] RaycastAll_Internal(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		return RaycastAll_Internal_Injected(ref origin, ref direction, distance, ref contactFilter);
	}

	[NativeMethod("RaycastNonAlloc_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static int RaycastNonAlloc_Internal(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
	{
		return RaycastNonAlloc_Internal_Injected(ref origin, ref direction, distance, ref contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCast_Internal(origin, radius, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCast_Internal(origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCast_Internal(origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return CircleCast_Internal(origin, radius, direction, distance, contactFilter);
	}

	/// <summary>
	///   <para>Casts a circle against colliders in the scene, returning the first collider to contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the circle originates.</param>
	/// <param name="radius">The radius of the circle.</param>
	/// <param name="direction">Vector representing the direction of the circle.</param>
	/// <param name="distance">Maximum distance over which to cast the circle.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return CircleCast_Internal(origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return CircleCastNonAlloc_Internal(origin, radius, direction, float.PositiveInfinity, contactFilter, results);
	}

	/// <summary>
	///   <para>Casts a circle against colliders in the Scene, returning all colliders that contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the circle originates.</param>
	/// <param name="radius">The radius of the circle.</param>
	/// <param name="direction">Vector representing the direction of the circle.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <param name="distance">Maximum distance over which to cast the circle.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
	{
		return CircleCastNonAlloc_Internal(origin, radius, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastAll_Internal(origin, radius, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastAll_Internal(origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastAll_Internal(origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return CircleCastAll_Internal(origin, radius, direction, distance, contactFilter);
	}

	/// <summary>
	///   <para>Casts a circle against colliders in the scene, returning all colliders that contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the circle originates.</param>
	/// <param name="radius">The radius of the circle.</param>
	/// <param name="direction">Vector representing the direction of the circle.</param>
	/// <param name="distance">Maximum distance over which to cast the circle.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return CircleCastAll_Internal(origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastNonAlloc_Internal(origin, radius, direction, float.PositiveInfinity, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastNonAlloc_Internal(origin, radius, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastNonAlloc_Internal(origin, radius, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return CircleCastNonAlloc_Internal(origin, radius, direction, distance, contactFilter, results);
	}

	/// <summary>
	///   <para>Casts a circle into the scene, returning colliders that contact with it into the provided results array.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the circle originates.</param>
	/// <param name="radius">The radius of the circle.</param>
	/// <param name="direction">Vector representing the direction of the circle.</param>
	/// <param name="results">Array to receive results.</param>
	/// <param name="distance">Maximum distance over which to cast the circle.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return CircleCastNonAlloc_Internal(origin, radius, direction, distance, contactFilter, results);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("CircleCast_Binding")]
	private static RaycastHit2D CircleCast_Internal(Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		CircleCast_Internal_Injected(ref origin, radius, ref direction, distance, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("CircleCastAll_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static RaycastHit2D[] CircleCastAll_Internal(Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		return CircleCastAll_Internal_Injected(ref origin, radius, ref direction, distance, ref contactFilter);
	}

	[NativeMethod("CircleCastNonAlloc_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static int CircleCastNonAlloc_Internal(Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
	{
		return CircleCastNonAlloc_Internal_Injected(ref origin, radius, ref direction, distance, ref contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCast_Internal(origin, size, angle, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCast_Internal(origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCast_Internal(origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return BoxCast_Internal(origin, size, angle, direction, distance, contactFilter);
	}

	/// <summary>
	///   <para>Casts a box against colliders in the scene, returning the first collider to contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the box originates.</param>
	/// <param name="size">The size of the box.</param>
	/// <param name="angle">The angle of the box (in degrees).</param>
	/// <param name="direction">Vector representing the direction of the box.</param>
	/// <param name="distance">Maximum distance over which to cast the box.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("Physics2D.AllLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return BoxCast_Internal(origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return BoxCastNonAlloc_Internal(origin, size, angle, direction, float.PositiveInfinity, contactFilter, results);
	}

	/// <summary>
	///   <para>Casts a box against the colliders in the Scene and returns all colliders that are in contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the box originates.</param>
	/// <param name="size">The size of the box.</param>
	/// <param name="angle">The angle of the box (in degrees).</param>
	/// <param name="direction">Vector representing the direction of the box.</param>
	/// <param name="distance">Maximum distance over which to cast the box.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
	{
		return BoxCastNonAlloc_Internal(origin, size, angle, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastAll_Internal(origin, size, angle, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastAll_Internal(origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastAll_Internal(origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return BoxCastAll_Internal(origin, size, angle, direction, distance, contactFilter);
	}

	/// <summary>
	///   <para>Casts a box against colliders in the scene, returning all colliders that contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the box originates.</param>
	/// <param name="size">The size of the box.</param>
	/// <param name="angle">The angle of the box (in degrees).</param>
	/// <param name="direction">Vector representing the direction of the box.</param>
	/// <param name="distance">Maximum distance over which to cast the box.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return BoxCastAll_Internal(origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastNonAlloc_Internal(origin, size, angle, direction, float.PositiveInfinity, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastNonAlloc_Internal(origin, size, angle, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastNonAlloc_Internal(origin, size, angle, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return BoxCastNonAlloc_Internal(origin, size, angle, direction, distance, contactFilter, results);
	}

	/// <summary>
	///   <para>Casts a box into the scene, returning colliders that contact with it into the provided results array.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the box originates.</param>
	/// <param name="size">The size of the box.</param>
	/// <param name="angle">The angle of the box (in degrees).</param>
	/// <param name="direction">Vector representing the direction of the box.</param>
	/// <param name="results">Array to receive results.</param>
	/// <param name="distance">Maximum distance over which to cast the box.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return BoxCastNonAlloc_Internal(origin, size, angle, direction, distance, contactFilter, results);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("BoxCast_Binding")]
	private static RaycastHit2D BoxCast_Internal(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		BoxCast_Internal_Injected(ref origin, ref size, angle, ref direction, distance, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("BoxCastAll_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static RaycastHit2D[] BoxCastAll_Internal(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		return BoxCastAll_Internal_Injected(ref origin, ref size, angle, ref direction, distance, ref contactFilter);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("BoxCastNonAlloc_Binding")]
	private static int BoxCastNonAlloc_Internal(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
	{
		return BoxCastNonAlloc_Internal_Injected(ref origin, ref size, angle, ref direction, distance, ref contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCast_Internal(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCast_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCast_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return CapsuleCast_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	/// <summary>
	///   <para>Casts a capsule against colliders in the scene, returning the first collider to contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the capsule originates.</param>
	/// <param name="size">The size of the capsule.</param>
	/// <param name="capsuleDirection">The direction of the capsule.</param>
	/// <param name="angle">The angle of the capsule (in degrees).</param>
	/// <param name="direction">Vector representing the direction to cast the capsule.</param>
	/// <param name="distance">Maximum distance over which to cast the capsule.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return CapsuleCast_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return CapsuleCastNonAlloc_Internal(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, contactFilter, results);
	}

	/// <summary>
	///   <para>Casts a capsule against the colliders in the Scene and returns all colliders that are in contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the capsule originates.</param>
	/// <param name="size">The size of the capsule.</param>
	/// <param name="capsuleDirection">The direction of the capsule.</param>
	/// <param name="angle">The angle of the capsule (in degrees).</param>
	/// <param name="direction">Vector representing the direction to cast the capsule.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <param name="distance">Maximum distance over which to cast the capsule.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
	{
		return CapsuleCastNonAlloc_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastAll_Internal(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastAll_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastAll_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return CapsuleCastAll_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	/// <summary>
	///   <para>Casts a capsule against colliders in the scene, returning all colliders that contact with it.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the capsule originates.</param>
	/// <param name="size">The size of the capsule.</param>
	/// <param name="capsuleDirection">The direction of the capsule.</param>
	/// <param name="angle">The angle of the capsule (in degrees).</param>
	/// <param name="direction">Vector representing the direction to cast the capsule.</param>
	/// <param name="distance">Maximum distance over which to cast the capsule.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return CapsuleCastAll_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastNonAlloc_Internal(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastNonAlloc_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastNonAlloc_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return CapsuleCastNonAlloc_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	/// <summary>
	///   <para>Casts a capsule into the scene, returning colliders that contact with it into the provided results array.</para>
	/// </summary>
	/// <param name="origin">The point in 2D space where the capsule originates.</param>
	/// <param name="size">The size of the capsule.</param>
	/// <param name="capsuleDirection">The direction of the capsule.</param>
	/// <param name="angle">The angle of the capsule (in degrees).</param>
	/// <param name="direction">Vector representing the direction to cast the capsule.</param>
	/// <param name="results">Array to receive results.</param>
	/// <param name="distance">Maximum distance over which to cast the capsule.</param>
	/// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return CapsuleCastNonAlloc_Internal(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("CapsuleCast_Binding")]
	private static RaycastHit2D CapsuleCast_Internal(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		CapsuleCast_Internal_Injected(ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter, out var ret);
		return ret;
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("CapsuleCastAll_Binding")]
	private static RaycastHit2D[] CapsuleCastAll_Internal(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		return CapsuleCastAll_Internal_Injected(ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("CapsuleCastNonAlloc_Binding")]
	private static int CapsuleCastNonAlloc_Internal(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
	{
		return CapsuleCastNonAlloc_Internal_Injected(ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D GetRayIntersection(Ray ray)
	{
		return GetRayIntersection_Internal(ray.origin, ray.direction, float.PositiveInfinity, -5);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D GetRayIntersection(Ray ray, float distance)
	{
		return GetRayIntersection_Internal(ray.origin, ray.direction, distance, -5);
	}

	/// <summary>
	///   <para>Cast a 3D ray against the colliders in the scene returning the first collider along the ray.</para>
	/// </summary>
	/// <param name="ray">The 3D ray defining origin and direction to test.</param>
	/// <param name="distance">Maximum distance over which to cast the ray.</param>
	/// <param name="layerMask">Filter to detect colliders only on certain layers.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static RaycastHit2D GetRayIntersection(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
	{
		return GetRayIntersection_Internal(ray.origin, ray.direction, distance, layerMask);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] GetRayIntersectionAll(Ray ray)
	{
		return GetRayIntersectionAll_Internal(ray.origin, ray.direction, float.PositiveInfinity, -5);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, float distance)
	{
		return GetRayIntersectionAll_Internal(ray.origin, ray.direction, distance, -5);
	}

	/// <summary>
	///   <para>Cast a 3D ray against the colliders in the scene returning all the colliders along the ray.</para>
	/// </summary>
	/// <param name="ray">The 3D ray defining origin and direction to test.</param>
	/// <param name="distance">Maximum distance over which to cast the ray.</param>
	/// <param name="layerMask">Filter to detect colliders only on certain layers.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	[RequiredByNativeCode]
	public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
	{
		return GetRayIntersectionAll_Internal(ray.origin, ray.direction, distance, layerMask);
	}

	[ExcludeFromDocs]
	public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results)
	{
		return GetRayIntersectionNonAlloc_Internal(ray.origin, ray.direction, float.PositiveInfinity, -5, results);
	}

	[ExcludeFromDocs]
	public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance)
	{
		return GetRayIntersectionNonAlloc_Internal(ray.origin, ray.direction, distance, -5, results);
	}

	/// <summary>
	///   <para>Cast a 3D ray against the colliders in the scene returning the colliders along the ray.</para>
	/// </summary>
	/// <param name="ray">The 3D ray defining origin and direction to test.</param>
	/// <param name="distance">Maximum distance over which to cast the ray.</param>
	/// <param name="layerMask">Filter to detect colliders only on certain layers.</param>
	/// <param name="results">Array to receive results.</param>
	/// <returns>
	///   <para>The number of results returned.</para>
	/// </returns>
	[RequiredByNativeCode]
	public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
	{
		return GetRayIntersectionNonAlloc_Internal(ray.origin, ray.direction, distance, layerMask, results);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("GetRayIntersection_Binding")]
	private static RaycastHit2D GetRayIntersection_Internal(Vector3 origin, Vector3 direction, float distance, int layerMask)
	{
		GetRayIntersection_Internal_Injected(ref origin, ref direction, distance, layerMask, out var ret);
		return ret;
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("GetRayIntersectionAll_Binding")]
	private static RaycastHit2D[] GetRayIntersectionAll_Internal(Vector3 origin, Vector3 direction, float distance, int layerMask)
	{
		return GetRayIntersectionAll_Internal_Injected(ref origin, ref direction, distance, layerMask);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("GetRayIntersectionNonAlloc_Binding")]
	private static int GetRayIntersectionNonAlloc_Internal(Vector3 origin, Vector3 direction, float distance, int layerMask, [Out] RaycastHit2D[] results)
	{
		return GetRayIntersectionNonAlloc_Internal_Injected(ref origin, ref direction, distance, layerMask, results);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapPoint(Vector2 point)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPoint_Internal(point, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapPoint(Vector2 point, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPoint_Internal(point, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapPoint(Vector2 point, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapPoint_Internal(point, contactFilter);
	}

	/// <summary>
	///   <para>Checks if a collider overlaps a point in space.</para>
	/// </summary>
	/// <param name="point">A point in world space.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The collider overlapping the point.</para>
	/// </returns>
	public static Collider2D OverlapPoint(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapPoint_Internal(point, contactFilter);
	}

	/// <summary>
	///   <para>Checks if a collider overlaps a point in world space.</para>
	/// </summary>
	/// <param name="point">A point in world space.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapPointNonAlloc_Internal(point, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapPointAll(Vector2 point)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPointAll_Internal(point, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPointAll_Internal(point, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapPointAll_Internal(point, contactFilter);
	}

	/// <summary>
	///   <para>Get a list of all colliders that overlap a point in space.</para>
	/// </summary>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <param name="point">A point in space.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static Collider2D[] OverlapPointAll(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapPointAll_Internal(point, contactFilter);
	}

	[ExcludeFromDocs]
	public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPointNonAlloc_Internal(point, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPointNonAlloc_Internal(point, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapPointNonAlloc_Internal(point, contactFilter, results);
	}

	/// <summary>
	///   <para>Get a list of all colliders that overlap a point in space.</para>
	/// </summary>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <param name="point">A point in space.</param>
	/// <param name="results">Array to receive results.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapPointNonAlloc_Internal(point, contactFilter, results);
	}

	[NativeMethod("OverlapPoint_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static Collider2D OverlapPoint_Internal(Vector2 point, ContactFilter2D contactFilter)
	{
		return OverlapPoint_Internal_Injected(ref point, ref contactFilter);
	}

	[NativeMethod("OverlapPointAll_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static Collider2D[] OverlapPointAll_Internal(Vector2 point, ContactFilter2D contactFilter)
	{
		return OverlapPointAll_Internal_Injected(ref point, ref contactFilter);
	}

	[NativeMethod("OverlapPointNonAlloc_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static int OverlapPointNonAlloc_Internal(Vector2 point, ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return OverlapPointNonAlloc_Internal_Injected(ref point, ref contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCircle(Vector2 point, float radius)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircle_Internal(point, radius, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircle_Internal(point, radius, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapCircle_Internal(point, radius, contactFilter);
	}

	/// <summary>
	///   <para>Checks if a collider falls within a circular area.</para>
	/// </summary>
	/// <param name="point">Centre of the circle.</param>
	/// <param name="radius">Radius of the circle.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The collider overlapping the circle.</para>
	/// </returns>
	public static Collider2D OverlapCircle(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapCircle_Internal(point, radius, contactFilter);
	}

	/// <summary>
	///   <para>Checks if a collider is within a circular area.</para>
	/// </summary>
	/// <param name="point">Centre of the circle.</param>
	/// <param name="radius">Radius of the circle.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapCircleNonAlloc_Internal(point, radius, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCircleAll(Vector2 point, float radius)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircleAll_Internal(point, radius, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircleAll_Internal(point, radius, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapCircleAll_Internal(point, radius, contactFilter);
	}

	/// <summary>
	///   <para>Get a list of all colliders that fall within a circular area.</para>
	/// </summary>
	/// <param name="point">Center of the circle.</param>
	/// <param name="radius">Radius of the circle.</param>
	/// <param name="layerMask">Filter to check objects only on specified layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results.</para>
	/// </returns>
	public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapCircleAll_Internal(point, radius, contactFilter);
	}

	[ExcludeFromDocs]
	public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircleNonAlloc_Internal(point, radius, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircleNonAlloc_Internal(point, radius, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapCircleNonAlloc_Internal(point, radius, contactFilter, results);
	}

	/// <summary>
	///   <para>Get a list of all colliders that fall within a circular area.</para>
	/// </summary>
	/// <param name="point">Center of the circle.</param>
	/// <param name="radius">Radius of the circle.</param>
	/// <param name="results">Array to receive results.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapCircleNonAlloc_Internal(point, radius, contactFilter, results);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("OverlapCircle_Binding")]
	private static Collider2D OverlapCircle_Internal(Vector2 point, float radius, ContactFilter2D contactFilter)
	{
		return OverlapCircle_Internal_Injected(ref point, radius, ref contactFilter);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("OverlapCircleAll_Binding")]
	private static Collider2D[] OverlapCircleAll_Internal(Vector2 point, float radius, ContactFilter2D contactFilter)
	{
		return OverlapCircleAll_Internal_Injected(ref point, radius, ref contactFilter);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("OverlapCircleNonAlloc_Binding")]
	private static int OverlapCircleNonAlloc_Internal(Vector2 point, float radius, ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return OverlapCircleNonAlloc_Internal_Injected(ref point, radius, ref contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBox_Internal(point, size, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBox_Internal(point, size, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapBox_Internal(point, size, angle, contactFilter);
	}

	/// <summary>
	///   <para>Checks if a collider falls within a box area.</para>
	/// </summary>
	/// <param name="point">Center of the box.</param>
	/// <param name="size">Size of the box.</param>
	/// <param name="angle">Angle of the box.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
	/// <returns>
	///   <para>The collider overlapping the box.</para>
	/// </returns>
	public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapBox_Internal(point, size, angle, contactFilter);
	}

	/// <summary>
	///   <para>Checks if a collider falls within a box area.</para>
	/// </summary>
	/// <param name="point">Center of the box.</param>
	/// <param name="size">Size of the box.</param>
	/// <param name="angle">Angle of the box.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapBoxNonAlloc_Internal(point, size, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBoxAll_Internal(point, size, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBoxAll_Internal(point, size, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapBoxAll_Internal(point, size, angle, contactFilter);
	}

	/// <summary>
	///   <para>Get a list of all colliders that fall within a box area.</para>
	/// </summary>
	/// <param name="point">Center of the box.</param>
	/// <param name="size">Size of the box.</param>
	/// <param name="angle">Angle of the box.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapBoxAll_Internal(point, size, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBoxNonAlloc_Internal(point, size, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBoxNonAlloc_Internal(point, size, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapBoxNonAlloc_Internal(point, size, angle, contactFilter, results);
	}

	/// <summary>
	///   <para>Get a list of all colliders that fall within a box area.</para>
	/// </summary>
	/// <param name="point">Center of the box.</param>
	/// <param name="size">Size of the box.</param>
	/// <param name="angle">Angle of the box.</param>
	/// <param name="results">Array to receive results.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapBoxNonAlloc_Internal(point, size, angle, contactFilter, results);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("OverlapBox_Binding")]
	private static Collider2D OverlapBox_Internal(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
	{
		return OverlapBox_Internal_Injected(ref point, ref size, angle, ref contactFilter);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("OverlapBoxAll_Binding")]
	private static Collider2D[] OverlapBoxAll_Internal(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
	{
		return OverlapBoxAll_Internal_Injected(ref point, ref size, angle, ref contactFilter);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("OverlapBoxNonAlloc_Binding")]
	private static int OverlapBoxNonAlloc_Internal(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return OverlapBoxNonAlloc_Internal_Injected(ref point, ref size, angle, ref contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB)
	{
		return OverlapAreaToBox_Internal(pointA, pointB, -5, float.NegativeInfinity, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask)
	{
		return OverlapAreaToBox_Internal(pointA, pointB, layerMask, float.NegativeInfinity, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
	{
		return OverlapAreaToBox_Internal(pointA, pointB, layerMask, minDepth, float.PositiveInfinity);
	}

	/// <summary>
	///   <para>Checks if a collider falls within a rectangular area.</para>
	/// </summary>
	/// <param name="pointA">One corner of the rectangle.</param>
	/// <param name="pointB">Diagonally opposite the point A corner of the rectangle.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The collider overlapping the area.</para>
	/// </returns>
	public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		return OverlapAreaToBox_Internal(pointA, pointB, layerMask, minDepth, maxDepth);
	}

	private static Collider2D OverlapAreaToBox_Internal(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth, float maxDepth)
	{
		Vector2 point = (pointA + pointB) * 0.5f;
		Vector2 size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
		return OverlapBox(point, size, 0f, layerMask, minDepth, maxDepth);
	}

	/// <summary>
	///   <para>Checks if a collider falls within a rectangular area.</para>
	/// </summary>
	/// <param name="pointA">One corner of the rectangle.</param>
	/// <param name="pointB">Diagonally opposite the point A corner of the rectangle.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, Collider2D[] results)
	{
		Vector2 point = (pointA + pointB) * 0.5f;
		Vector2 size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
		return OverlapBox(point, size, 0f, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB)
	{
		return OverlapAreaAllToBox_Internal(pointA, pointB, -5, float.NegativeInfinity, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask)
	{
		return OverlapAreaAllToBox_Internal(pointA, pointB, layerMask, float.NegativeInfinity, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
	{
		return OverlapAreaAllToBox_Internal(pointA, pointB, layerMask, minDepth, float.PositiveInfinity);
	}

	/// <summary>
	///   <para>Get a list of all colliders that fall within a rectangular area.</para>
	/// </summary>
	/// <param name="pointA">One corner of the rectangle.</param>
	/// <param name="pointB">Diagonally opposite the point A corner of the rectangle.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		return OverlapAreaAllToBox_Internal(pointA, pointB, layerMask, minDepth, maxDepth);
	}

	private static Collider2D[] OverlapAreaAllToBox_Internal(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth, float maxDepth)
	{
		Vector2 point = (pointA + pointB) * 0.5f;
		Vector2 size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
		return OverlapBoxAll(point, size, 0f, layerMask, minDepth, maxDepth);
	}

	[ExcludeFromDocs]
	public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results)
	{
		return OverlapAreaNonAllocToBox_Internal(pointA, pointB, results, -5, float.NegativeInfinity, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask)
	{
		return OverlapAreaNonAllocToBox_Internal(pointA, pointB, results, layerMask, float.NegativeInfinity, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth)
	{
		return OverlapAreaNonAllocToBox_Internal(pointA, pointB, results, layerMask, minDepth, float.PositiveInfinity);
	}

	/// <summary>
	///   <para>Get a list of all colliders that fall within a specified area.</para>
	/// </summary>
	/// <param name="pointA">One corner of the rectangle.</param>
	/// <param name="pointB">Diagonally opposite the point A corner of the rectangle.</param>
	/// <param name="results">Array to receive results.</param>
	/// <param name="layerMask">Filter to check objects only on specified layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		return OverlapAreaNonAllocToBox_Internal(pointA, pointB, results, layerMask, minDepth, maxDepth);
	}

	private static int OverlapAreaNonAllocToBox_Internal(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth, float maxDepth)
	{
		Vector2 point = (pointA + pointB) * 0.5f;
		Vector2 size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
		return OverlapBoxNonAlloc(point, size, 0f, results, layerMask, minDepth, maxDepth);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsule_Internal(point, size, direction, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsule_Internal(point, size, direction, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapCapsule_Internal(point, size, direction, angle, contactFilter);
	}

	/// <summary>
	///   <para>Checks if a collider falls within a capsule area.</para>
	/// </summary>
	/// <param name="point">Center of the capsule.</param>
	/// <param name="size">Size of the capsule.</param>
	/// <param name="direction">The direction of the capsule.</param>
	/// <param name="angle">Angle of the capsule.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
	/// <returns>
	///   <para>The collider overlapping the capsule.</para>
	/// </returns>
	public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapCapsule_Internal(point, size, direction, angle, contactFilter);
	}

	/// <summary>
	///   <para>Checks if a collider falls within a capsule area.</para>
	/// </summary>
	/// <param name="point">Center of the capsule.</param>
	/// <param name="size">Size of the capsule.</param>
	/// <param name="direction">The direction of the capsule.</param>
	/// <param name="angle">Angle of the capsule.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapCapsuleNonAlloc_Internal(point, size, direction, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsuleAll_Internal(point, size, direction, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsuleAll_Internal(point, size, direction, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapCapsuleAll_Internal(point, size, direction, angle, contactFilter);
	}

	/// <summary>
	///   <para>Get a list of all colliders that fall within a capsule area.</para>
	/// </summary>
	/// <param name="point">Center of the capsule.</param>
	/// <param name="size">Size of the capsule.</param>
	/// <param name="direction">The direction of the capsule.</param>
	/// <param name="angle">Angle of the capsule.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
	/// <returns>
	///   <para>The cast results returned.</para>
	/// </returns>
	public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapCapsuleAll_Internal(point, size, direction, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsuleNonAlloc_Internal(point, size, direction, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsuleNonAlloc_Internal(point, size, direction, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapCapsuleNonAlloc_Internal(point, size, direction, angle, contactFilter, results);
	}

	/// <summary>
	///   <para>Get a list of all colliders that fall within a capsule area.</para>
	/// </summary>
	/// <param name="point">Center of the capsule.</param>
	/// <param name="size">Size of the capsule.</param>
	/// <param name="direction">The direction of the capsule.</param>
	/// <param name="angle">Angle of the capsule.</param>
	/// <param name="results">Array to receive results.</param>
	/// <param name="layerMask">Filter to check objects only on specific layers.</param>
	/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
	/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapCapsuleNonAlloc_Internal(point, size, direction, angle, contactFilter, results);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("OverlapCapsule_Binding")]
	private static Collider2D OverlapCapsule_Internal(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
	{
		return OverlapCapsule_Internal_Injected(ref point, ref size, direction, angle, ref contactFilter);
	}

	[NativeMethod("OverlapCapsuleAll_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static Collider2D[] OverlapCapsuleAll_Internal(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
	{
		return OverlapCapsuleAll_Internal_Injected(ref point, ref size, direction, angle, ref contactFilter);
	}

	[NativeMethod("OverlapCapsuleNonAlloc_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static int OverlapCapsuleNonAlloc_Internal(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return OverlapCapsuleNonAlloc_Internal_Injected(ref point, ref size, direction, angle, ref contactFilter, results);
	}

	/// <summary>
	///   <para>Get a list of all colliders that overlap collider.</para>
	/// </summary>
	/// <param name="collider">The collider that defines the area used to query for other collider overlaps.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
	/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
	/// <returns>
	///   <para>Returns the number of results placed in the results array.</para>
	/// </returns>
	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	[NativeMethod("OverlapCollider_Binding")]
	public static int OverlapCollider([NotNull] Collider2D collider, ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return OverlapCollider_Injected(collider, ref contactFilter, results);
	}

	/// <summary>
	///   <para>Retrieves all contact points in for contacts between with the collider1 and collider2, with the results filtered by the ContactFilter2D.</para>
	/// </summary>
	/// <param name="collider1">The collider to check if it has contacts against collider2.</param>
	/// <param name="collider2">The collider to check if it has contacts against collider1.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of contacts placed in the contacts array.</para>
	/// </returns>
	public static int GetContacts(Collider2D collider1, Collider2D collider2, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
	{
		return GetColliderColliderContacts(collider1, collider2, contactFilter, contacts);
	}

	/// <summary>
	///   <para>Retrieves all contact points in contact with the collider.</para>
	/// </summary>
	/// <param name="collider">The collider to retrieve contacts for.</param>
	/// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of contacts placed in the contacts array.</para>
	/// </returns>
	public static int GetContacts(Collider2D collider, ContactPoint2D[] contacts)
	{
		return GetColliderContacts(collider, default(ContactFilter2D).NoFilter(), contacts);
	}

	/// <summary>
	///   <para>Retrieves all contact points in contact with the collider, with the results filtered by the ContactFilter2D.</para>
	/// </summary>
	/// <param name="collider">The collider to retrieve contacts for.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of contacts placed in the contacts array.</para>
	/// </returns>
	public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
	{
		return GetColliderContacts(collider, contactFilter, contacts);
	}

	/// <summary>
	///   <para>Retrieves all colliders in contact with the collider.</para>
	/// </summary>
	/// <param name="collider">The collider to retrieve contacts for.</param>
	/// <param name="colliders">An array of Collider2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of colliders placed in the colliders array.</para>
	/// </returns>
	public static int GetContacts(Collider2D collider, Collider2D[] colliders)
	{
		return GetColliderContactsCollidersOnly(collider, default(ContactFilter2D).NoFilter(), colliders);
	}

	/// <summary>
	///   <para>Retrieves all colliders in contact with the collider, with the results filtered by the ContactFilter2D.</para>
	/// </summary>
	/// <param name="collider">The collider to retrieve contacts for.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <param name="colliders">An array of Collider2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of colliders placed in the colliders array.</para>
	/// </returns>
	public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, Collider2D[] colliders)
	{
		return GetColliderContactsCollidersOnly(collider, contactFilter, colliders);
	}

	/// <summary>
	///   <para>Retrieves all contact points in contact with any of the collider(s) attached to this rigidbody.</para>
	/// </summary>
	/// <param name="rigidbody">The rigidbody to retrieve contacts for.  All colliders attached to this rigidbody will be checked.</param>
	/// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of contacts placed in the contacts array.</para>
	/// </returns>
	public static int GetContacts(Rigidbody2D rigidbody, ContactPoint2D[] contacts)
	{
		return GetRigidbodyContacts(rigidbody, default(ContactFilter2D).NoFilter(), contacts);
	}

	/// <summary>
	///   <para>Retrieves all contact points in contact with any of the collider(s) attached to this rigidbody, with the results filtered by the ContactFilter2D.</para>
	/// </summary>
	/// <param name="rigidbody">The rigidbody to retrieve contacts for.  All colliders attached to this rigidbody will be checked.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of contacts placed in the contacts array.</para>
	/// </returns>
	public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
	{
		return GetRigidbodyContacts(rigidbody, contactFilter, contacts);
	}

	/// <summary>
	///   <para>Retrieves all colliders in contact with any of the collider(s) attached to this rigidbody.</para>
	/// </summary>
	/// <param name="rigidbody">The rigidbody to retrieve contacts for.  All colliders attached to this rigidbody will be checked.</param>
	/// <param name="colliders">An array of Collider2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of colliders placed in the colliders array.</para>
	/// </returns>
	public static int GetContacts(Rigidbody2D rigidbody, Collider2D[] colliders)
	{
		return GetRigidbodyContactsCollidersOnly(rigidbody, default(ContactFilter2D).NoFilter(), colliders);
	}

	/// <summary>
	///   <para>Retrieves all colliders in contact with any of the collider(s) attached to this rigidbody, with the results filtered by the ContactFilter2D.</para>
	/// </summary>
	/// <param name="rigidbody">The rigidbody to retrieve contacts for.  All colliders attached to this rigidbody will be checked.</param>
	/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
	/// <param name="colliders">An array of Collider2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of colliders placed in the colliders array.</para>
	/// </returns>
	public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, Collider2D[] colliders)
	{
		return GetRigidbodyContactsCollidersOnly(rigidbody, contactFilter, colliders);
	}

	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	[NativeMethod("GetColliderContacts_Binding")]
	private static int GetColliderContacts([NotNull] Collider2D collider, ContactFilter2D contactFilter, [Out] ContactPoint2D[] results)
	{
		return GetColliderContacts_Injected(collider, ref contactFilter, results);
	}

	[NativeMethod("GetColliderColliderContacts_Binding")]
	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	private static int GetColliderColliderContacts([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, ContactFilter2D contactFilter, [Out] ContactPoint2D[] results)
	{
		return GetColliderColliderContacts_Injected(collider1, collider2, ref contactFilter, results);
	}

	[NativeMethod("GetRigidbodyContacts_Binding")]
	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	private static int GetRigidbodyContacts([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [Out] ContactPoint2D[] results)
	{
		return GetRigidbodyContacts_Injected(rigidbody, ref contactFilter, results);
	}

	[NativeMethod("GetColliderContactsCollidersOnly_Binding")]
	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	private static int GetColliderContactsCollidersOnly([NotNull] Collider2D collider, ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return GetColliderContactsCollidersOnly_Injected(collider, ref contactFilter, results);
	}

	[NativeMethod("GetRigidbodyContactsCollidersOnly_Binding")]
	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	private static int GetRigidbodyContactsCollidersOnly([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return GetRigidbodyContactsCollidersOnly_Injected(rigidbody, ref contactFilter, results);
	}

	internal static void SetEditorDragMovement(bool dragging, GameObject[] objs)
	{
		foreach (Rigidbody2D item in m_LastDisabledRigidbody2D)
		{
			if (item != null)
			{
				item.SetDragBehaviour(dragged: false);
			}
		}
		m_LastDisabledRigidbody2D.Clear();
		if (!dragging)
		{
			return;
		}
		foreach (GameObject gameObject in objs)
		{
			Rigidbody2D[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody2D>(includeInactive: false);
			Rigidbody2D[] array = componentsInChildren;
			foreach (Rigidbody2D rigidbody2D in array)
			{
				m_LastDisabledRigidbody2D.Add(rigidbody2D);
				rigidbody2D.SetDragBehaviour(dragged: true);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_gravity_Injected(out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_gravity_Injected(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_jobOptions_Injected(out PhysicsJobOptions2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_jobOptions_Injected(ref PhysicsJobOptions2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_colliderAwakeColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_colliderAwakeColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_colliderAsleepColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_colliderAsleepColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_colliderContactColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_colliderContactColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_colliderAABBColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_colliderAABBColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_TwoCollidersWithFilter_Injected([Writable] Collider2D collider1, [Writable] Collider2D collider2, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_SingleColliderWithFilter_Injected([Writable] Collider2D collider, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Distance_Internal_Injected([Writable] Collider2D colliderA, [Writable] Collider2D colliderB, out ColliderDistance2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Linecast_Internal_Injected(ref Vector2 start, ref Vector2 end, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RaycastHit2D[] LinecastAll_Internal_Injected(ref Vector2 start, ref Vector2 end, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int LinecastNonAlloc_Internal_Injected(ref Vector2 start, ref Vector2 end, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Raycast_Internal_Injected(ref Vector2 origin, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RaycastHit2D[] RaycastAll_Internal_Injected(ref Vector2 origin, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int RaycastNonAlloc_Internal_Injected(ref Vector2 origin, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CircleCast_Internal_Injected(ref Vector2 origin, float radius, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RaycastHit2D[] CircleCastAll_Internal_Injected(ref Vector2 origin, float radius, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CircleCastNonAlloc_Internal_Injected(ref Vector2 origin, float radius, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BoxCast_Internal_Injected(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RaycastHit2D[] BoxCastAll_Internal_Injected(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int BoxCastNonAlloc_Internal_Injected(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapsuleCast_Internal_Injected(ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RaycastHit2D[] CapsuleCastAll_Internal_Injected(ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CapsuleCastNonAlloc_Internal_Injected(ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRayIntersection_Internal_Injected(ref Vector3 origin, ref Vector3 direction, float distance, int layerMask, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RaycastHit2D[] GetRayIntersectionAll_Internal_Injected(ref Vector3 origin, ref Vector3 direction, float distance, int layerMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRayIntersectionNonAlloc_Internal_Injected(ref Vector3 origin, ref Vector3 direction, float distance, int layerMask, [Out] RaycastHit2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D OverlapPoint_Internal_Injected(ref Vector2 point, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D[] OverlapPointAll_Internal_Injected(ref Vector2 point, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapPointNonAlloc_Internal_Injected(ref Vector2 point, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D OverlapCircle_Internal_Injected(ref Vector2 point, float radius, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D[] OverlapCircleAll_Internal_Injected(ref Vector2 point, float radius, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapCircleNonAlloc_Internal_Injected(ref Vector2 point, float radius, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D OverlapBox_Internal_Injected(ref Vector2 point, ref Vector2 size, float angle, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D[] OverlapBoxAll_Internal_Injected(ref Vector2 point, ref Vector2 size, float angle, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapBoxNonAlloc_Internal_Injected(ref Vector2 point, ref Vector2 size, float angle, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D OverlapCapsule_Internal_Injected(ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D[] OverlapCapsuleAll_Internal_Injected(ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapCapsuleNonAlloc_Internal_Injected(ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapCollider_Injected(Collider2D collider, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColliderContacts_Injected(Collider2D collider, ref ContactFilter2D contactFilter, [Out] ContactPoint2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColliderColliderContacts_Injected(Collider2D collider1, Collider2D collider2, ref ContactFilter2D contactFilter, [Out] ContactPoint2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRigidbodyContacts_Injected(Rigidbody2D rigidbody, ref ContactFilter2D contactFilter, [Out] ContactPoint2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColliderContactsCollidersOnly_Injected(Collider2D collider, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRigidbodyContactsCollidersOnly_Injected(Rigidbody2D rigidbody, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);
}

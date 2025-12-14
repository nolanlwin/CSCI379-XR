using UnityEngine;

namespace SimpleFPS
{
	/// <summary>
	/// Provides cached animator parameter hash IDs to improve performance when setting animator parameters.
	/// These IDs are pre-computed at startup instead of converting strings to hashes at runtime.
	/// </summary>
	public static class AnimatorId
	{
		// Movement related parameters
		public static readonly int MoveSpeed      = Animator.StringToHash("MoveSpeed");
		public static readonly int MoveX          = Animator.StringToHash("MoveX");  // Left/right movement
		public static readonly int MoveZ          = Animator.StringToHash("MoveZ");  // Forward/backward movement
		public static readonly int LocomotionTime = Animator.StringToHash("LocomotionTime");

		// Character state parameters
		public static readonly int IsGrounded     = Animator.StringToHash("IsGrounded");
		public static readonly int IsAlive        = Animator.StringToHash("IsAlive");

		// Combat related parameters
		public static readonly int Look           = Animator.StringToHash("Look");
		public static readonly int Fire           = Animator.StringToHash("Fire");
		public static readonly int IsReloading    = Animator.StringToHash("IsReloading");
		public static readonly int Show           = Animator.StringToHash("Show");
		public static readonly int Hide           = Animator.StringToHash("Hide");
		public static readonly int WeaponId       = Animator.StringToHash("WeaponID");
	}
}

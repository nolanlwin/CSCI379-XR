using System;
using Fusion;
using UnityEngine;

namespace SimpleFPS
{
	/// <summary>
	/// Weapons component hold references to all player weapons
	/// and allows for weapon actions such as Fire or Reload.
	/// </summary>
	public class Weapons : NetworkBehaviour
	{
		[Header("Setup")]
		public Transform FireTransform;
		public Setup FirstPersonSetup;
		public Setup ThirdPersonSetup;
		public float WeaponSwitchTime = 1f;

		[Header("Sounds")]
		public AudioSource SwitchSound;

		[HideInInspector]
		public Weapon[] AllWeapons;

		public bool IsSwitching => _switchTimer.ExpiredOrNotRunning(Runner) == false;

		[Networked, HideInInspector]
		public Weapon CurrentWeapon { get; set; }

		[Networked]
		private TickTimer _switchTimer { get; set; }
		[Networked]
		private Weapon _pendingWeapon { get; set; }

		private Weapon _visibleWeapon;
		private bool _firstPersonActive;
		private Setup _activeSetup;

		public void SetFirstPersonVisuals(bool firstPerson)
		{
			if (firstPerson == _firstPersonActive)
				return;

			_firstPersonActive = firstPerson;
			_activeSetup = firstPerson ? FirstPersonSetup : ThirdPersonSetup;

			for (int i = 0; i < AllWeapons.Length; i++)
			{
				// First person weapons are rendered with a different (overlay) camera
				// to prevent clipping through geometry.
				AllWeapons[i].gameObject.SetLayer(_activeSetup.WeaponLayer, true);
			}
		}

		public void Fire(bool justPressed)
		{
			if (CurrentWeapon == null || IsSwitching)
				return;

			if (CurrentWeapon.Fire(FireTransform.position, FireTransform.forward, justPressed) == false)
				return;

			// For local player play fire animation but only
			// in forward tick as starting animation multiple times
			// during resimulations is not desired.
			if (_firstPersonActive && Runner.IsForward)
			{
				FirstPersonSetup.Animator.SetTrigger(AnimatorId.Fire);
			}
		}

		public void Reload()
		{
			if (CurrentWeapon == null || IsSwitching)
				return;

			CurrentWeapon.Reload();
		}

		public void SwitchWeapon(EWeaponType weaponType)
		{
			var newWeapon = GetWeapon(weaponType);

			if (newWeapon == null || newWeapon.IsCollected == false)
				return;

			if (newWeapon == CurrentWeapon && _pendingWeapon == null)
				return;

			if (newWeapon == _pendingWeapon)
				return;

			if (CurrentWeapon.IsReloading)
				return;

			_pendingWeapon = newWeapon;
			_switchTimer = TickTimer.CreateFromSeconds(Runner, WeaponSwitchTime);

			// For local player start with switch animation but only
			// in forward tick as starting animation multiple times
			// during resimulations is not desired.
			if (_firstPersonActive && Runner.IsForward)
			{
				FirstPersonSetup.Animator.SetTrigger(AnimatorId.Hide);
				SwitchSound.Play();
			}
		}

		public bool PickupWeapon(EWeaponType weaponType)
		{
			if (CurrentWeapon.IsReloading)
				return false;

			var weapon = GetWeapon(weaponType);
			if (weapon == null)
				return false;

			if (weapon.IsCollected)
			{
				// If the weapon is already collected at least refill the ammo.
				weapon.AddAmmo(weapon.StartAmmo - weapon.RemainingAmmo);
			}
			else
			{
				// Weapon is already present inside Player prefab,
				// marking it as IsCollected is all that is needed.
				weapon.IsCollected = true;
			}

			SwitchWeapon(weaponType);

			return true;
		}

		public Weapon GetWeapon(EWeaponType weaponType)
		{
			for (int i = 0; i < AllWeapons.Length; ++i)
			{
				if (AllWeapons[i].Type == weaponType)
					return AllWeapons[i];
			}

			return default;
		}

		private void Awake()
		{
			// All weapons are already present inside Player prefab.
			// This is the simplest solution when only few weapons are available in the game.
			AllWeapons = GetComponentsInChildren<Weapon>();

			_activeSetup = ThirdPersonSetup;
		}

		private void LateUpdate()
		{
			if (Object == null)
				return; // Not valid

			if (_visibleWeapon != null)
			{
				var weaponTransform = _visibleWeapon.transform;
				var weaponPivot = _firstPersonActive ? _visibleWeapon.FirstPersonPivot : _visibleWeapon.ThirdPersonPivot;

				// Snap visible weapon to weapon handle transform, use weapon pivot to adjust offset and rotation per weapon
				weaponTransform.rotation = _activeSetup.WeaponHandle.rotation * weaponPivot.localRotation;
				weaponTransform.position = _activeSetup.WeaponHandle.position + weaponTransform.rotation * weaponPivot.localPosition;
			}
		}

		public override void Spawned()
		{
			if (HasStateAuthority)
			{
				CurrentWeapon = AllWeapons[0];
				CurrentWeapon.IsCollected = true;
			}
		}

		public override void FixedUpdateNetwork()
		{
			TryActivatePendingWeapon();
		}

		public override void Render()
		{
			UpdateVisibleWeapon();

			if (_firstPersonActive && CurrentWeapon != null)
			{
				FirstPersonSetup.Animator.SetBool(AnimatorId.IsReloading, CurrentWeapon.IsReloading);
			}
		}

		private void UpdateVisibleWeapon()
		{
			if (_visibleWeapon == CurrentWeapon)
				return;

			_visibleWeapon = CurrentWeapon;

			// Update weapon visibility
			for (int i = 0; i < AllWeapons.Length; i++)
			{
				var weapon = AllWeapons[i];
				weapon.ToggleVisibility(weapon == CurrentWeapon);
			}

			FirstPersonSetup.LeftHandSnap.Handle = _visibleWeapon.LeftHandHandle;

			FirstPersonSetup.Animator.runtimeAnimatorController = _visibleWeapon.HandsAnimatorController;
			ThirdPersonSetup.Animator.SetFloat(AnimatorId.WeaponId, Array.IndexOf(AllWeapons, CurrentWeapon));

			// Hide and show animations are played only for local player
			if (_firstPersonActive)
			{
				FirstPersonSetup.Animator.SetTrigger(AnimatorId.Show);
			}
		}

		private void TryActivatePendingWeapon()
		{
			if (IsSwitching == false || _pendingWeapon == null)
				return;

			if (_switchTimer.RemainingTime(Runner) > WeaponSwitchTime * 0.5f)
				return; // Too soon.

			CurrentWeapon = _pendingWeapon;
			_pendingWeapon = null;

			// Make the weapon immediately active (previous weapon will be deactivated in Render)
			CurrentWeapon.ToggleVisibility(true);
		}

		// DATA STRUCTURES

		[Serializable]
		public class Setup
		{
			public Transform WeaponHandle;
			[Layer]
			public int       WeaponLayer;
			public Animator  Animator;
			public HandSnap  LeftHandSnap;
		}
	}
}

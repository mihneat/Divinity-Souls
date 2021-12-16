using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Attributes;

namespace RPG.Combat
{
	[CreateAssetMenu(fileName = "Weapon", menuName = "RPG/Weapons/Make New Weapon", order = 0)]
	public class Weapon : ScriptableObject
	{
		public float GetWeaponRange { get { return weaponRange; } }
		public float GetTimeBetweenAttacks { get { return timeBetweenAttacks; } }
		public float GetDamage { get { return damage; } }
		public bool GetHasProjectile { get { return projectile != null; } }

		[Header("Stats")]
		[SerializeField] float weaponRange = 2.0f;
		[SerializeField] float timeBetweenAttacks = 1.0f;
		[SerializeField] float damage = 20.0f;
		[SerializeField] bool isRightHanded = true;

		[Header("References")]
		[SerializeField] GameObject equippedPrefab = null;
		[SerializeField] Projectile projectile = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;

		const string weaponName = "Weapon";

		public void Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
		{
			DestroyOldWeapon(rightHandTransform, leftHandTransform);

			if (equippedPrefab) {
				Transform handTransform = GetHandTransform(rightHandTransform, leftHandTransform);
				GameObject weapon = Instantiate(equippedPrefab, handTransform);
				weapon.name = weaponName;
			}

			var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
			if (animatorOverrideController) {
				animator.runtimeAnimatorController = animatorOverrideController;
			} else if(overrideController) {
				animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
			}
		}

		private void DestroyOldWeapon(Transform rightHandTransform, Transform leftHandTransform)
		{
			Transform oldWeapon = rightHandTransform.Find(weaponName);
			if (!oldWeapon)
				oldWeapon = leftHandTransform.Find(weaponName);

			if (!oldWeapon)
				return;

			oldWeapon.name = " == Destroying == ";
			Destroy(oldWeapon.gameObject);
		}

		private Transform GetHandTransform(Transform rightHandTransform, Transform leftHandTransform)
		{
			// Choose between right hand and left hand
			return isRightHanded ? rightHandTransform : leftHandTransform;
		}

		public void LaunchProjectile(GameObject instigator, Transform rightHandTransform,
			Transform leftHandTransform, Health target, float calculatedDamage)
		{
			Transform handTransform = GetHandTransform(rightHandTransform, leftHandTransform);
			Projectile projectileInstance = Instantiate(projectile,
				handTransform.position, Quaternion.identity);

			projectileInstance.SetTarget(instigator, target, calculatedDamage);
		}
	}
}

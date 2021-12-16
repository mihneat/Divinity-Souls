using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat
{
	public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [Header("Weapon")]
        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;

        public Health GetTarget {
            get {
                return target;
			}
		}

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;

        private Mover mover;
        private ActionScheduler actionScheduler;
        private Animator animator;
        private BaseStats baseStats;

        const string defaultWeaponName = "Unarmed Player";

        private void Awake()
        {
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            if (!currentWeapon)
                EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            // If there is no Mover component, do nothing
            if (!mover)
                return;

            // Increase the time from the last attack
            timeSinceLastAttack += Time.deltaTime;

            // If there is no target, return
            if (!target)
                return;

            // If the target is dead, return
            if (target.GetIsDead)
                return;

            // Decide if it is in range
            bool isInRange = GetIsInRange();

            if (!isInRange) {
                // If it isn't, move to the target
                mover.MoveTo(target.transform.position, 1.0f);
            } else {
                // Otherwise, cancel movement and start attacking
                mover.Cancel();
                AttackBehaviour();
            }
        }

        #region Attack

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);

            if (timeSinceLastAttack >= currentWeapon.GetTimeBetweenAttacks) {
                // The triggered animation will call the Hit() function
                TriggerAttack();
                timeSinceLastAttack = 0.0f;
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.GetWeaponRange;
        }

        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);

            target = combatTarget.GetComponent<Health>();
        }

        // Sword attack animation event
        void Hit()
        {
            if (!target)
                return;

            float damage = baseStats.GetStat(Stat.Damage);
            if (currentWeapon.GetHasProjectile) {
                currentWeapon.LaunchProjectile(gameObject, rightHandTransform,
                    leftHandTransform, target.GetComponent<Health>(), damage);
                return;
            }

            // if (GetIsInRange())
            target.TakeDamage(gameObject, damage);
        }

        // Bow attack animation event
        void Shoot()
        {
            Hit();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            // If there is no target, return false
            if (!combatTarget)
                return false;

            // If the target is dead, return false
            if (combatTarget.GetComponent<Health>().GetIsDead)
                return false;

            // Otherwise, the player can attack the enemy
            return true;
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("cancelAttack");
            animator.SetTrigger("attack");
        }

        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("cancelAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage) {
                yield return currentWeapon.GetDamage;
			}
        }

        #endregion

        public void EquipWeapon(Weapon newWeapon)
        {
            if (!newWeapon)
                return;

            currentWeapon = newWeapon;

            Animator animator = GetComponent<Animator>();
            currentWeapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

		public object CaptureState()
		{
            if (!currentWeapon)
                return defaultWeaponName;

            return currentWeapon.name;
		}

		public void RestoreState(object state)
		{
            string loadedWeaponName = (string)state;
            Weapon loadedWeapon = Resources.Load<Weapon>(loadedWeaponName);

            EquipWeapon(loadedWeapon);
		}
	}
}

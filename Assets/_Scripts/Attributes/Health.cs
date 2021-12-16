using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes
{
	public class Health : MonoBehaviour, ISaveable
	{
		[SerializeField] float maxHealth = 100.0f;
		[SerializeField] TMP_Text healthText;

		private ActionScheduler actionScheduler;
		private BaseStats baseStats;

		public float GetCurrentHealth {
			get {
				return currentHealth;
			}
		}

		public float GetCurrentHealthPercentage {
			get {
				float percentage = 100.0f * (currentHealth / maxHealth);
				return Mathf.Ceil(10.0f * percentage) / 10.0f;
			}
		}

		float currentHealth = -1.0f;

		public bool GetIsDead {
			get { return isDead; }
		}

		bool isDead = false;

		private Animator animator;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			actionScheduler = GetComponent<ActionScheduler>();
			baseStats = GetComponent<BaseStats>();
		}

		private void Start()
		{
			maxHealth = baseStats.GetStat(Stat.Health);

			if (currentHealth < 0.0f)
				currentHealth = maxHealth;

			baseStats.onLevelUp += UpdateHealthOnLevelUp;
		}

		private void Update()
		{
			healthText.text = currentHealth.ToString();
		}

		public void TakeDamage(GameObject instigator, float damage)
		{
			currentHealth = Mathf.Clamp(currentHealth - damage, 0.0f, maxHealth);

			if (currentHealth <= Mathf.Epsilon) {
				Die();
				AwardExperience(instigator);
			}
		}

		private void Die()
		{
			if (isDead)
				return;

			isDead = true;
			animator.SetTrigger("die");

			actionScheduler.CancelCurrentAction();
		}

		private void AwardExperience(GameObject instigator)
		{
			Experience instigatorExperience = instigator.GetComponent<Experience>();
			if (!instigatorExperience)
				return;

			float experiencePoints = baseStats.GetStat(Stat.ExperienceReward);
			instigatorExperience.GainExperience(experiencePoints);
		}

		private void UpdateHealthOnLevelUp()
		{
			maxHealth = baseStats.GetStat(Stat.Health);
			currentHealth = maxHealth;
		}

		public object CaptureState()
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			data["currentHealth"] = GetCurrentHealth;
			data["isDead"] = GetIsDead;

			return data;
		}

		public void RestoreState(object state)
		{
			Dictionary<string, object> data = (Dictionary<string, object>)state;
			currentHealth = (float)data["currentHealth"];
			isDead = (bool)data["isDead"];

			if (isDead == true)
				isDead = false;

			if (currentHealth <= Mathf.Epsilon)
				Die();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using System;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] [Range(1, 300)] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
		[SerializeField] GameObject levelUpFX = null;

		public event Action onLevelUp;

		int currentLevel = 0;

        private Experience experience;

		private void Awake()
		{
			experience = GetComponent<Experience>();
		}

		private void Start()
		{
			currentLevel = CalculateLevel();

			if (experience) {
				experience.onExperienceGained += UpdateLevel;
				onLevelUp += LevelUpEffect;
			}
		}

		private void UpdateLevel()
		{
			int newLevel = CalculateLevel();
			if (newLevel > currentLevel) {
				currentLevel = newLevel;
				onLevelUp();
			}
		}

		private void LevelUpEffect()
		{
			if (levelUpFX)
				Instantiate(levelUpFX, transform.position, Quaternion.identity);
		}

		public float GetStat(Stat stat)
		{
            return progression.GetStat(stat, characterClass, GetLevel()) + GetAdditiveModifiers(stat);
		}

		public int GetLevel()
		{
			if (currentLevel < 1) {
				currentLevel = CalculateLevel();
			}

			return currentLevel;
		}

        private int CalculateLevel()
		{
			if (!experience)
				return startingLevel;

			float currentXP = experience.GetExperiencePoints;
			int finalLevelUp = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

			for (int currentLevel = 1; currentLevel <= finalLevelUp; currentLevel++) {
				float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, currentLevel);
				if (currentXP < xpToLevelUp)
					return currentLevel;
			}

			return finalLevelUp + 1;
		}

		private float GetAdditiveModifiers(Stat stat)
		{
			float sum = 0.0f;
			foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
				foreach (float modifier in provider.GetAdditiveModifiers(stat)) {
					sum += modifier;
				}
			}

			return sum;
		}
	}
}

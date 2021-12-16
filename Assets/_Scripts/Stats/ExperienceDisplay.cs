using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
	{
		private BaseStats baseStats;
		private Experience experience;
		private TMP_Text experienceText;

		private void Awake()
		{
			experienceText = GetComponent<TMP_Text>();

			GameObject player = GameObject.FindWithTag("Player");
			experience = player.GetComponent<Experience>();
			baseStats = player.GetComponent<BaseStats>();
		}

		private void Update()
		{
			float currentXP = experience.GetExperiencePoints;
			float levelUpXP = baseStats.GetStat(Stat.ExperienceToLevelUp);
			experienceText.text = String.Format("{0:0} / {1:0} XP", currentXP, levelUpXP);
		}
	}
}

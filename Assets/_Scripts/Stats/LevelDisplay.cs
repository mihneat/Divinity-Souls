using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
		private BaseStats baseStats;
		private TMP_Text levelText;

		private void Awake()
		{
			levelText = GetComponent<TMP_Text>();
			baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
		}

		private void Update()
		{
			levelText.text = String.Format("Level {0:0}", baseStats.GetLevel());
		}
	}
}

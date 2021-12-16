using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Attributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
		private Fighter fighter;
		private TMP_Text healthText;

		private void Awake()
		{
			healthText = GetComponent<TMP_Text>();
			fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
		}

		private void Update()
		{
			Health target = fighter.GetTarget;
			string enemyHealthValue = (target != null) ? String.Format("{0}%", target.GetCurrentHealthPercentage.ToString()) : "-";

			healthText.text = String.Format("Enemy Health: {0:0.0}", enemyHealthValue);
		}
	}
}
